using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NiqonNO.Core
{
    public static class NOContainer
    {
        private static readonly Dictionary<Type, List<object>> Registrations = new();
        private static readonly Dictionary<Type, List<object>> Observers = new();
        private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();

        public static void RegisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                AddToList(iface);
                NotifyObservers(iface);
            }
            
            return;
            void AddToList(Type type)
            {
                if (!Registrations.TryGetValue(type, out var list))
                {
                    list = new List<object>();
                    Registrations[type] = list;
                }
                list.Add(manager);
            }
        }
        public static void UnregisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                RemoveFromList(iface);
                NotifyObservers(iface);
            }

            return;
            void RemoveFromList(Type type)
            {
                if (!Registrations.TryGetValue(type, out var list)) return;
                if (!list.Remove(manager)) return;
                if (list.Count != 0) return;
                Registrations.Remove(type);
            }
        }

        public static void Inject(object target)
        {
            var fields = GetInjectableFields(target.GetType());
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<NOInjectAttribute>() == null) continue;
                var service = Resolve(field.FieldType);
                field.SetValue(target, service);

                SubscribeTo(field.FieldType, target);
            }
        }
        public static void Release(object target)
        {
            var fields = GetInjectableFields(target.GetType());
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<NOInjectAttribute>() == null) continue;
                UnsubscribeFrom(field.FieldType, target);
            }
        }
        
        private static void SubscribeTo<T>(object target) where T : INOService => SubscribeTo(typeof(T), target);

        private static void SubscribeTo(Type type, object target)
        {
            if (!Observers.TryGetValue(type, out var list))
            {
                list = new List<object>();
                Observers[type] = list;
            }
            list.Add(target);
        }

        private static void UnsubscribeFrom<T>(object target) where T : INOService => UnsubscribeFrom(typeof(T), target);

        private static void UnsubscribeFrom(Type type, object target)
        {
            if (!Observers.TryGetValue(type, out var list)) return;
            int idx = list.IndexOf(target);
            if (idx < 0) return;
            int last = list.Count - 1;
            list[idx] = list[last];
            list.RemoveAt(last);
            
            if (list.Count != 0) return;
            Observers.Remove(type);
        }

        private static void NotifyObservers(Type type)
        {
            if (!Observers.TryGetValue(type, out var targets)) return;
            foreach (var target in targets)
            {
                var field = GetSingleFieldOfType(target.GetType(), type);
                if (field != null) field.SetValue(target, Resolve(type));
            }
        }

        private static T Resolve<T>() where T : INOService => (T)Resolve(typeof(T));
        private static object Resolve(Type serviceType)
        {
            if (serviceType == null || 
                !Registrations.TryGetValue(serviceType, out var list) || 
                list.Count == 0) return null;
            return list[^1];
        }

        private static FieldInfo[] GetInjectableFields(Type type)
        {
            if(FieldCache.TryGetValue(type, out var info)) return info;
            info = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<NOInjectAttribute>() != null)
                .ToArray();
            FieldCache.Add(type, info);
            return info;
        }

        private static FieldInfo GetSingleFieldOfType(Type targetType, Type fieldType)
        {
            if (FieldCache.TryGetValue(targetType, out var info))
            {
                return info.First(f => f.FieldType == fieldType);
            }
            return null;
        }
    }
}
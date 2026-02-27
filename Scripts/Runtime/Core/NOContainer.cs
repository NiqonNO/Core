using System;
using System.Collections.Generic;
using System.Reflection;
namespace NiqonNO.Core
{
    public static class NOContainer
    {
        private static readonly Dictionary<Type, List<object>> Registrations = new();
        private static readonly Dictionary<Type, HashSet<object>> Observers = new();

        public static void RegisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            bool anyChange = false;
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                AddToList(iface);
                anyChange = true;
            }

            if(anyChange) NotifyObservers(interfaces);
            
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
            bool anyChange = false;
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                RemoveFromList(iface);
                anyChange = true;
            }
            
            if(anyChange) NotifyObservers(interfaces);

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
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = target.GetType().GetFields(flags);

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
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = target.GetType().GetFields(flags);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<NOInjectAttribute>() == null) continue;
                UnsubscribeFrom(field.FieldType, target);
            }
        }
        
        private static void SubscribeTo<T>(object target) where T : INOService => SubscribeTo(typeof(T), target);

        private static void SubscribeTo(Type type, object target)
        {
            if (!Observers.TryGetValue(type, out var set))
            {
                set = new HashSet<object>();
                Observers[type] = set;
            }
            set.Add(target);
        }

        private static void UnsubscribeFrom<T>(object target) where T : INOService => SubscribeTo(typeof(T), target);
        private static void UnsubscribeFrom(Type type, object target)
        {
            if (!Observers.TryGetValue(type, out var list)) return;
            if (!list.Remove(target)) return;
            if (list.Count != 0) return;
            Observers.Remove(type);
        }
        
        private static void NotifyObservers(IEnumerable<Type> changedTypes)
        {
            foreach (var type in changedTypes)
            {
                if (!Observers.TryGetValue(type, out var targets)) continue;
                foreach (var target in targets)
                {
                    Inject(target);
                }
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NiqonNO.Core
{
    public class NOContainer
    {
        private readonly Dictionary<Type, List<object>> Registrations = new();
        private readonly Dictionary<Type, FieldInfo[]> FieldCache = new();

        public NOContainer()
        {
            
        }
        public NOContainer(INOContext parent)
        {
            
        }

        public void RegisterContext(INOContext service)
        {
            RegisterService(typeof(INOContext), service);
        }

        public void RegisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                RegisterService(iface, manager);
            }
        }
        private void RegisterService(Type type, object service)
        {
            if (!Registrations.TryGetValue(type, out var list))
            {
                list = new List<object>();
                Registrations[type] = list;
            }

            list.Add(service);
        }

        public void UnregisterService(INOManager manager)
        {
            var interfaces = manager.GetType().GetInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface == typeof(INOService) ||
                    !typeof(INOService).IsAssignableFrom(iface)) continue;
                UnregisterService(iface, manager);
            }
        }
        private void UnregisterService(Type type, object service)
        {
            if (!Registrations.TryGetValue(type, out var list)) return;
            if (!list.Remove(service)) return;
            if (list.Count != 0) return;
            Registrations.Remove(type);
        }

        public void Inject(object target)
        {
            var fields = GetInjectableFields(target.GetType());
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<NOInjectAttribute>() == null) continue;
                var service = Resolve(field.FieldType);
                field.SetValue(target, service);
            }
        }

        private T Resolve<T>() where T : INOService => (T)Resolve(typeof(T));
        private object Resolve(Type serviceType)
        {
            if (serviceType == null || 
                !Registrations.TryGetValue(serviceType, out var list) || 
                list.Count == 0) return null;
            return list[^1];
        }

        private FieldInfo[] GetInjectableFields(Type type)
        {
            if(FieldCache.TryGetValue(type, out var info)) return info;
            info = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<NOInjectAttribute>() != null)
                .ToArray();
            FieldCache.Add(type, info);
            return info;
        }
    }
}
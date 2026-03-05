using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NiqonNO.Core
{
	public partial class NOContainer
	{
		private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();

		private static Dictionary<string, NOContainer> SceneContext;
		private static Dictionary<string, List<IInitializable>> WaitingForInitialization;

		private static FieldInfo[] GetInjectableFields(Type type)
		{
			if(FieldCache.TryGetValue(type, out var info)) return info;
			info = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(f => f.GetCustomAttribute<NOInjectAttribute>() != null)
				.ToArray();
			FieldCache.Add(type, info);
			return info;
		}
		
		public static void EnqueueForInitialization(string scope, IInitializable initializable)
		{
			if (SceneContext.TryGetValue(scope, out var context))
			{
				InitializeInitializable(context, initializable);
				return;
			}

			if (!WaitingForInitialization.TryGetValue(scope, out var list))
			{
				list = new List<IInitializable>();
				WaitingForInitialization[scope] = list;
			}
			list.Add(initializable);
		}
		public static void RegisterContext(NOContainer container)
		{
			SceneContext[container.MyScope] = container;
			if (!WaitingForInitialization.TryGetValue(container.MyScope, out var initializables)) return;
			
			foreach (var initializable in initializables)
			{
				InitializeInitializable(container, initializable);
			}
			WaitingForInitialization.Remove(container.MyScope);
		}
		private static void InitializeInitializable(NOContainer container, IInitializable initializable)
		{
			container.Inject(initializable);
			initializable.Initialize();
		}

		public static void UnregisterContext(NOContainer container)
		{
			SceneContext.Remove(container.MyScope);
		}
		
	}
}
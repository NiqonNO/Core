using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NiqonNO.Core
{
	public partial class NOContainer
	{
		private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();

		private static readonly Dictionary<string, NOContainer> SceneContext = new();
		private static readonly Dictionary<string, List<IInitializable>> WaitingForInitialization = new();

		public static void ResetStaticState()
		{
			SceneContext.Clear();
			WaitingForInitialization.Clear();
			FieldCache.Clear();
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
		
		public static void EnqueueForInitialization(string scope, IInitializable initializable)
		{
			if (initializable == null) return;
			scope ??= string.Empty;
			
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
		private static void InitializeInitializable(NOContainer container, IInitializable initializable)
		{
			container.Inject(initializable);
			initializable.Initialize();
		}
		
		public static void RegisterContainer(NOContainer container)
		{
			if (container == null)
				throw new ArgumentNullException(nameof(container));
			
			SceneContext[container.MyScope] = container;
			if (!WaitingForInitialization.TryGetValue(container.MyScope, out var initializables)) return;
			
			foreach (var initializable in initializables)
			{
				InitializeInitializable(container, initializable);
			}
			WaitingForInitialization.Remove(container.MyScope);
		}

		public static void UnregisterContainer(NOContainer container)
		{
			if (container == null) return;
			SceneContext.Remove(container.MyScope);
		}

		public static void SetupSceneContext(UnityEngine.SceneManagement.Scene scene)
		{
			NOSceneContext context = null;
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				if (!rootObject.TryGetComponent<NOSceneContext>(out context)) continue;
                context.InitializeContext();
				break;
			}
		}

		public static void DisposeSceneContext(UnityEngine.SceneManagement.Scene scene)
		{
			if (!SceneContext.TryGetValue(scene.name, out var container)) return;
			container.ResolveContext().DisposeContext();
		}
	}
}
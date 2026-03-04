using System.Collections.Generic;

namespace NiqonNO.Core
{
	public static class NOContextLocator
	{
		private static Dictionary<UnityEngine.SceneManagement.Scene, INOContext> SceneContext;
		private static Dictionary<UnityEngine.SceneManagement.Scene, List<IInitializable>> WaitingForInitialization;

		public static void EnqueueForInitialization(UnityEngine.SceneManagement.Scene scene, IInitializable initializable)
		{
			if (SceneContext.TryGetValue(scene, out var context))
			{
				InitializeInitializable(context, initializable);
				return;
			}

			if (!WaitingForInitialization.TryGetValue(scene, out var list))
			{
				list = new List<IInitializable>();
				WaitingForInitialization[scene] = list;
			}
			list.Add(initializable);
		}
		public static void RegisterContext(UnityEngine.SceneManagement.Scene scene, INOContext context)
		{
			SceneContext[scene] = context;
			if (!WaitingForInitialization.TryGetValue(scene, out var initializables)) return;
			
			foreach (var initializable in initializables)
			{
				InitializeInitializable(context, initializable);
			}
			WaitingForInitialization.Remove(scene);
		}
		private static void InitializeInitializable(INOContext context, IInitializable initializable)
		{
			context.Container.Inject(initializable);
			initializable.Initialize();
		}

		public static void UnregisterContext(UnityEngine.SceneManagement.Scene scene)
		{
			SceneContext.Remove(scene);
		}
	}
}
using System;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
	public class NOBootStrapper
	{
		private const string ResourcesCorePath = "Core";
		
		private static NOBootStrapper Instance;
		
		private NOProjectContext ProjectContext { get; set; }
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Initialize()
		{
			Application.quitting += Dispose;
			Instance = new NOBootStrapper();
			Instance.LoadProjectContext();
		}
		private static void Dispose()
		{
			Application.quitting -= Dispose;
			Instance.DisposeProjectContext();
			Instance = null;
		}

		private void LoadProjectContext()
		{
			var projectContexts = Resources.LoadAll<NOProjectContext>(ResourcesCorePath);
			if (projectContexts.IsNullOrEmpty())
			{
				Debug.LogError($"Could not find object of type {nameof(NOProjectContext)} in Resources \"{ResourcesCorePath}\" folder. Project will not be initialized.");
				return;
			}
			if (projectContexts.Length > 1)
			{
				Debug.LogWarning($"More than one objects of type {nameof(NOProjectContext)} have been found in Resources \"{ResourcesCorePath}\" folder. First result will be used.");
			}

			ProjectContext = projectContexts[0];
			ProjectContext.InitializeContext();
		}
		
		private void DisposeProjectContext()
		{
			ProjectContext.DisposeContext();
		}
	}
}
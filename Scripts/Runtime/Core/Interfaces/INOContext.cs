using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
	public interface INOContext<T> : INOContext where T : INOManager
	{
		IEnumerable<T> Managers { get; }
		
		void RegisterServices() 
		{
			foreach (var manager in Managers)
			{
				if (CheckNull(manager)) continue;
				Container.RegisterService(manager);
			}
		}
		void UnregisterServices()
		{
			foreach (var manager in Managers)
			{
				if (CheckNull(manager)) continue;
				Container.UnregisterService(manager);
				manager.Dispose();
			}
		}

		void InitializeServices()
		{
			foreach (var manager in Managers)
			{
				Container.Inject(manager);
				manager.Initialize();
			}
		}
        
		private bool CheckNull(INOManager manager)
		{
			if (manager != null) return false;
			if(this is UnityEngine.Object context)
				Debug.LogWarning($"A null manager reference was detected in the Managers collection for context '{context.name}'.", context);
			else
				Debug.LogWarning($"A null manager reference was detected in the Managers collection for context");
			return true;
		}
	}
	public interface INOContext
	{
		NOContainer Container { get; }
		NOFactory Factory { get; }

		void InitializeContext();
		void DisposeContext();
	}
}
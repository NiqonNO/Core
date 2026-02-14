using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core
{
	public abstract class NOManagerWithSingletonMonoBehaviour<T> : NOManagerMonoBehaviour where T : NOManagerMonoBehaviour
	{
		[ReadOnly, ShowInInspector, PropertyOrder(float.MinValue)]
		protected static T Instance { get; private set; }

		public override void Initialize()
		{
			if (Instance)
			{
				Debug.LogError($"More than one manager of type {GetType().Name} is initialized or previous session have not been cleared correctly.", this);
				return;
			}
			Instance = this as T;
		}
        
		public override void Dispose()
		{
			Instance = null;
		}
	}
}
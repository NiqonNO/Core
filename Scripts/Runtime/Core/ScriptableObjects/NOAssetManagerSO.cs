using UnityEngine;

namespace NiqonNO.Core
{
	public abstract class NOAssetManagerSO<TData, TDataState, TRuntimeState> :  NOManagerWithStateSO<TRuntimeState>
		where TData : NOData
		where TDataState : NODataState<TData> 
		where TRuntimeState : NOAssetManagerState<TData, TDataState>
	{

		public virtual TDataState GetState(TData data)
		{
			if (RuntimeState.States.TryGetValue(data, out var clipState)) return clipState;
			
			clipState = CreateAsset<TDataState>();
			clipState.Initialize(data);
			RuntimeState.States.Add(data, clipState);
			return clipState;
		}
		
		public override void Initialize()
		{
			base.Initialize();
			
		}
        
		public override void Dispose()
		{
			foreach (var states in RuntimeState.States.Values)
			{
				DestroyAsset(states);
			}

			RuntimeState.States.Clear();
			base.Dispose();
		}
	}
}
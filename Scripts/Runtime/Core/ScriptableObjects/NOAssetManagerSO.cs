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
			return CreateDataState(data);
		}
        
		public override void Dispose()
		{
			foreach (var state in RuntimeState.States.Values)
			{
				Destroy(state);
			}

			RuntimeState.States.Clear();
			base.Dispose();
		}

		protected TDataState CreateDataState(TData data)
		{
			TDataState clipState = Context.Factory.CreateAsset<TDataState>();
			clipState.Initialize(data);
			RuntimeState.States.Add(data, clipState);
			return clipState;
		}
	}
}
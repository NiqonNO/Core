namespace NiqonNO.Core
{
	public interface INOAssetSetManager<TData, TDataState, TRuntimeState>
		where TData : NOData
		where TDataState : NODataState<TData> 
		where TRuntimeState : INOAssetSetState<TData, TDataState>
	{
		protected INOContext Context { get; }
		protected TRuntimeState RuntimeState { get; }

		public TDataState GetState(TData data)
		{
			if (RuntimeState.States.TryGetValue(data, out var state)) return state;
			return CreateDataState(data);
		}

		public TDataState CreateDataState(TData data)
		{
			TDataState clipState = Context.Factory.CreateAsset<TDataState>();
			clipState.Initialize(data);
			RuntimeState.States.Add(data, clipState);
			return clipState;
		}
	}
}
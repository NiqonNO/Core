using System.Collections.Generic;

namespace NiqonNO.Core
{
	public interface INOAssetSetManager<TData, TDataState>
		where TData : NOData
		where TDataState : NODataState<TData>
	{
		protected INOContext Context { get; }
		public Dictionary<TData, TDataState> States { get; }

		public bool TryGetState(TData data, out TDataState state)
		{
			return States.TryGetValue(data, out state);
		}
		
		public TDataState GetOrCreateState(TData data)
		{
			if (States.TryGetValue(data, out var state)) return state;
			return CreateDataState(data);
		}

		public TDataState CreateDataState(TData data)
		{
			TDataState clipState = Context.Factory.CreateAsset<TDataState>();
			clipState.Initialize(data);
			States.Add(data, clipState);
			return clipState;
		}
	}
}
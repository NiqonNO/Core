using System.Collections.Generic;

namespace NiqonNO.Core
{
	public interface INOAssetSetState<TData, TDataState>
		where TData : NOData
		where TDataState : NODataState<TData> 
	{
		public Dictionary<TData, TDataState> States { get; }
	}
}
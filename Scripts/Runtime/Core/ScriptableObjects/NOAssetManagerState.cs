using System.Collections.Generic;

namespace NiqonNO.Core
{
	public class NOAssetManagerState<TData, TDataState> : NOManagerState
		where TData : NOData
		where TDataState : NODataState<TData> 
	{
		public readonly Dictionary<TData, TDataState> States = new ();

	}
}
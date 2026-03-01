using Sirenix.OdinInspector;

namespace NiqonNO.Core
{
	public class NOAssetSetManagerSO<TData, TDataSet, TDataState, TRuntimeState> : NOAssetManagerSO<TData, TDataState, TRuntimeState>
		where TData : NOData
		where TDataSet : NODataSet<TData>
		where TDataState : NODataState<TData> 
		where TRuntimeState : NOAssetManagerState<TData, TDataState>
	{
		[ReadOnly, ShowInInspector, PropertyOrder(float.MinValue)]
		protected TDataSet AssetSet { get; private set; }
		
		public override void Initialize()
		{
			base.Initialize();

			foreach (var data in AssetSet.Assets)
			{
				CreateDataState(data);
			}
			
		}
	}
}
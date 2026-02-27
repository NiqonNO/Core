namespace NiqonNO.Core
{
	public class NODataState<TData> : NOScriptableObject 
		where TData : NOData
	{
		public TData Asset { get; private set; }
		
		public virtual void Initialize(TData asset)
		{
			Asset = asset;
			name = Asset.ID;
		}
	}
}
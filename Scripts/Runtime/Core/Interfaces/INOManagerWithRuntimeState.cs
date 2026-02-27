namespace NiqonNO.Core
{
	public interface INOManagerWithRuntimeState<T> : INOManager
	{
		T RuntimeState { get; }
	}
}
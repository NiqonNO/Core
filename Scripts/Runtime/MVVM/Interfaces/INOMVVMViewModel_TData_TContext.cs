namespace NiqonNO.Core.MVVM
{
    public interface INOMVVMViewModel<TData, TContext> : INOMVVMViewModel<TData>  where TContext : class
    {
        TContext Context { get; set; }
        bool IsContextSet { get; }

        void SetContext(TContext context);
    }
}
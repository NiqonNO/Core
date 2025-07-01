namespace NiqonNO.Core.MVVM
{
    public abstract class NOMVVMBaseViewModel<TData, TContext> : NOMVVBaseViewModel<TData>, INOMVVMViewModel<TData, TContext>  where TContext : class
    {
        public TContext Context { get; set; }
        public bool IsContextSet => Context != null;

        public void SetContext(TContext context)
        {
            if (context.Equals(Context)) return;
            Context = context;
            OnViewModelChange();
        }
    }
}
namespace NiqonNO.Core
{
    public interface INOVariable<T> : INOValue<T>
    {
        public new T Value { set; }
    }
}
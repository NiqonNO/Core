namespace NiqonNO.Core
{
    public interface INOValue<out T>
    {
        public T Value { get; }
    }
}
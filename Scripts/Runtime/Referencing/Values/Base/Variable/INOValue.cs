using System;

namespace NiqonNO.Core
{
    public interface INOValue<T>
    {
        public T Value { get; set; }
    }
}
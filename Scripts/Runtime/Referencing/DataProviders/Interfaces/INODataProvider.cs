using UnityEngine;

namespace NiqonNO.Core
{
    public interface INODataProvider
    {
        string ItemName { get; }
        Sprite ItemIcon { get; }
        Color ItemColor { get; }
    }
}
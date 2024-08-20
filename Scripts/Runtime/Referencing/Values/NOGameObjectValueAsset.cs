using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOGameObjectValueAsset : NOValueAsset<GameObject> {}
    [Serializable] public class NOGameObjectValue : NOValue<GameObject, NOValueAsset<GameObject>> {}
}

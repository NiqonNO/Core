using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOVector3ValueAsset : NOValueAsset<Vector3> {}
    [Serializable] public class NOVector3Value : NOValue<Vector3, NOValueAsset<Vector3>> {}
}

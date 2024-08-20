using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable] public class NOTransformVariableAsset : NOVariableAsset<Transform> {}
    [Serializable] public class NOTransformVariable : NOVariable<Transform, NOVariableAsset<Transform>> {}
}

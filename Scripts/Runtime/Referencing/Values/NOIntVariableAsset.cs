using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOIntVariableAsset : NOVariableAsset<int> {}
    [Serializable] public class NOIntVariable : NOVariable<int, NOVariableAsset<int>> {}
}

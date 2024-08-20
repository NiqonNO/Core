using System;

namespace NiqonNO.Core
{
    [Serializable] public class NOStringVariableAsset : NOVariableAsset<string> {}
    [Serializable] public class NOStringVariable : NOVariable<string, NOVariableAsset<string>> {}
}

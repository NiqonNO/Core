using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOProjectContext : NOScriptableObject, INOContext<NOManagerSO>
    {
        [field: SerializeField]
        public NOManagerSO[] ScriptableObjectManagers { get; private set; }
        IEnumerable<NOManagerSO> INOContext<NOManagerSO>.Managers => ScriptableObjectManagers;

        public NOContainer Container { get; set;  }
        public NOFactory Factory { get; set;  }

        public void InitializeContext()
        {
            Container = new NOContainer();
            Factory = new NOFactory(Container);
            
            Container.RegisterContext(this);
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).RegisterServices();
            NOContainer.RegisterContainer(Container);
        }

        public void DisposeContext()
        {
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).UnregisterServices();
            NOContainer.UnregisterContainer(Container);
        }
    }
}
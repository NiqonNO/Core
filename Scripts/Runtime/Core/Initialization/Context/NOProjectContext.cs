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

        public NOContainer Container { get; private set;  }
        public NOFactory Factory { get; private set;  }

        public void InitializeContext()
        {
            Container = new NOContainer();
            Factory = new NOFactory(Container);

            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).RegisterServices();

            Container.ActivateScope(this);
        }

        public void DisposeContext()
        {
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).UnregisterServices();
            
            Container.DeactivateScope();

            Container = null;
            Factory = null;
        }
    }
}
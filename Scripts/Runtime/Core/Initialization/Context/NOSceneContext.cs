using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOSceneContext : NOMonoBehaviour, INOContext<NOManagerSO>, INOContext<NOManagerMonoBehaviour>
    {
        [field: SerializeField] 
        public NOManagerMonoBehaviour[] MonoBehaviourManagers { get; private set; }
        IEnumerable<NOManagerMonoBehaviour> INOContext<NOManagerMonoBehaviour>.Managers => MonoBehaviourManagers;
        
        [field: SerializeField] 
        public NOManagerSO[] ScriptableObjectManagers { get; private set; }
        IEnumerable<NOManagerSO>  INOContext<NOManagerSO>.Managers => ScriptableObjectManagers;

        public NOContainer Container { get; set; }
        public NOFactory Factory { get; set;  }
        
        public void InitializeContext()
        {
            Container = new NOContainer(gameObject.scene.name);
            Factory = new NOFactory(Container);

            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).RegisterServices();
            if (!MonoBehaviourManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerMonoBehaviour>).RegisterServices();
            Container.ActivateScope(this);
        }

        public void DisposeContext()
        {
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerSO>).UnregisterServices();
            if (!MonoBehaviourManagers.IsNullOrEmpty())
                (this as INOContext<NOManagerMonoBehaviour>).UnregisterServices();
            Container.DeactivateScope();
            
            Container = null;
            Factory = null;
        }
    }
}
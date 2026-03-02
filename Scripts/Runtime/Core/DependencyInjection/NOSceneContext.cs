using Sirenix.Utilities;
using UnityEngine;

namespace NiqonNO.Core
{
    public class NOSceneContext : NOMonoBehaviour
    {
        [field: SerializeField] 
        public bool MainScene { get; private set; }
        
        [field: SerializeField] 
        public NOManagerMonoBehaviour[] MonoBehaviourManagers { get; private set; }
        [field: SerializeField] 
        public NOManagerSO[] ScriptableObjectManagers { get; private set; }

        public void SetupSceneContext()
        {
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                RegisterSO();
            if (!MonoBehaviourManagers.IsNullOrEmpty())
                RegisterMono();

            if (!ScriptableObjectManagers.IsNullOrEmpty())
                InitializeSO();
            if (!MonoBehaviourManagers.IsNullOrEmpty())
                InitializeMono();
        }

        public void DisposeSceneContext()
        {
            if (!MonoBehaviourManagers.IsNullOrEmpty())
                DisposeMono();
            if (!ScriptableObjectManagers.IsNullOrEmpty())
                DisposeSO();
        }
        
        private void RegisterSO()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.RegisterService(manager);
            }
        }
        private void RegisterMono()
        {
            foreach (var manager in MonoBehaviourManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.RegisterService(manager);
            }
        }
        
        private void InitializeSO()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                manager.Initialize();
            }
        }
        private void InitializeMono()
        {
            foreach (var manager in MonoBehaviourManagers)
            {
                if (CheckNull(manager)) continue;
                manager.Initialize();
            }
        }
        
        private void DisposeSO()
        {
            foreach (var manager in ScriptableObjectManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.UnregisterService(manager);
                manager.Dispose();
            }
        }

        private void DisposeMono()
        {
            foreach (var manager in MonoBehaviourManagers)
            {
                if (CheckNull(manager)) continue;
                NOContainer.UnregisterService(manager);
                manager.Dispose();
            }
        }

        bool CheckNull(INOManager manager)
        {
            if (manager != null) return false;
            Debug.LogWarning($"Null manager in ScriptableObjectManagers array in Project context", this);
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
    public class NOSoundPoolManager : NOManagerMonoBehaviour, INOSoundEmitterPool
    {
        private const int InitialPoolSize = 32;
        private readonly Queue<NOSoundEmitter> EmitterPool = new (InitialPoolSize);
        private readonly HashSet<NOSoundEmitter> ActiveEmitters = new (InitialPoolSize);
        
        [SerializeField, SceneObjectsOnly] 
        private NOSoundEmitter EmitterTemplate;
        
        private Predicate<NOSoundEmitter> RemoveStoppedPredicate;
        
        public override void Initialize()
        {
            NOAudioManager.RegisterSoundPool(this);
            RemoveStoppedPredicate = RemoveStopped;
            InitializePool();
            enabled = false;
        }

        public override void Dispose()
        {
            foreach (var emitter in ActiveEmitters)
            {
                emitter.ForceStop();
            }
            ActiveEmitters.RemoveWhere(RemoveStoppedPredicate);
            NOAudioManager.RegisterSoundPool(null);
        }

        private void Update()
        {
            ActiveEmitters.RemoveWhere(RemoveStoppedPredicate);
        }            
        private bool RemoveStopped(NOSoundEmitter emitter)
        {
            if (emitter.IsPlaying)
                return false;

            emitter.Stop();
            return true;
        }

        private void InitializePool()
        {
            for (int i = 0; i < InitialPoolSize; i++)
                EmitterPool.Enqueue(CreateEmitter());
        }
        
        private NOSoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(EmitterTemplate, transform);
            emitter.Initialize();
            return emitter;
        }
        
        public NOSoundEmitter Acquire()
        {
            if(!EmitterPool.TryDequeue(out var emitter))
                emitter = CreateEmitter();

            ActiveEmitters.Add(emitter);
            enabled = true;
            return emitter;
        }
        public void Release(NOSoundEmitter emitter)
        {
            ActiveEmitters.Remove(emitter);
            EmitterPool.Enqueue(emitter);
            
            if(ActiveEmitters.Count == 0)
                enabled = false;
        }
    }
}
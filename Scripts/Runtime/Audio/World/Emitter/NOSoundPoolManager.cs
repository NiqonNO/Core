using System.Collections.Generic;
using NiqonNO.Core.Audio.Logic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
    public class NOSoundPoolManager : NOManagerMonoBehaviour, INOSoundEmitterPoolService
    {
        private const int InitialPoolSize = 32;
        private readonly Queue<NOSoundEmitter> EmitterPool = new(InitialPoolSize);
        private readonly Dictionary<INOSoundInstance, NOSoundEmitter> ActiveEmitters = new(InitialPoolSize);

        [SerializeField, SceneObjectsOnly]
        private NOSoundEmitter EmitterTemplate;
        
        public bool Initialized { get; private set; }

        public override void Initialize()
        {
            InitializePool();
            Initialized = true;
        }

        public override void Dispose()
        {
            Initialized = false;
            List<NOSoundEmitter> emitters = new List<NOSoundEmitter>(ActiveEmitters.Values);
            foreach(var emitter in emitters)
            {
                emitter.ForceStop();
            }
            EmitterPool.Clear();
        }

        private void InitializePool()
        {
            for (var i = 1; i < InitialPoolSize; i++)
                EmitterPool.Enqueue(CreateEmitter());

            EmitterTemplate.Initialize();
            EmitterTemplate.gameObject.SetActive(false);
            EmitterPool.Enqueue(EmitterTemplate);
        }

        private NOSoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(EmitterTemplate, transform);
            emitter.Initialize();
            emitter.gameObject.SetActive(false);
            return emitter;
        }

        public INOSoundInstance AssignToEmitter(NOSoundClip clip)
        {
            NOSoundEmitter emitter;
            INOSoundInstance instance;
            if (clip.MaxPlaying)
            {
                instance = clip.GetOldestInstance();
                ActiveEmitters.TryGetValue(instance, out emitter);
                if (emitter == null)
                    return null;
                emitter.Reset();
                return instance;
            }
            
            EmitterPool.TryDequeue(out emitter);
            if (emitter == null)
                return null;
            instance = emitter.AssignClip(clip);
            ActiveEmitters.Add(instance, emitter);
            emitter.gameObject.SetActive(true);
            return instance;
        }

        public void Return(INOSoundInstance instance)
        {
            if (!ActiveEmitters.TryGetValue(instance, out var emitter)) return;

            emitter.gameObject.SetActive(false);
            ActiveEmitters.Remove(instance);
            EmitterPool.Enqueue(emitter);
        }
    }
}
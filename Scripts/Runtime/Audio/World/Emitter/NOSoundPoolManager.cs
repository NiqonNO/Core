using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
    public class NOSoundPoolManager : NOManagerMonoBehaviour, INOSoundEmitterPoolService
    {
        private const int InitialPoolSize = 32;
        private readonly Queue<NOSoundEmitter> EmitterPool = new(InitialPoolSize);
        private readonly HashSet<NOSoundEmitter> ActiveEmitters = new(InitialPoolSize);

        [SerializeField, Min(1)]
        private int MaxEmitters = InitialPoolSize;

        [SerializeField, SceneObjectsOnly]
        private NOSoundEmitter EmitterTemplate;

        public override void Initialize()
        {
            InitializePool();
        }

        public override void Dispose()
        {
            while (ActiveEmitters.Count > 0)
            {
                var emitter = ActiveEmitters.First();
                emitter.ForceStop();
            }
            EmitterPool.Clear();
        }

        private void InitializePool()
        {
            var poolSize = Mathf.Min(InitialPoolSize, MaxEmitters);
            for (var i = 1; i < poolSize; i++)
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

        public NOSoundEmitter AcquireAvailable()
        {
            NOSoundEmitter emitter = null;
            if (EmitterPool.TryDequeue(out var pooledEmitter))
                emitter = pooledEmitter;
            else if (ActiveEmitters.Count < MaxEmitters)
                emitter = CreateEmitter();

            if (emitter == null)
                return null;

            emitter.gameObject.SetActive(true);
            ActiveEmitters.Add(emitter);
            return emitter;
        }

        public void Return(NOSoundEmitter emitter)
        {
            if (!ActiveEmitters.Remove(emitter))
                return;

            emitter.gameObject.SetActive(false);
            EmitterPool.Enqueue(emitter);
        }
    }
}
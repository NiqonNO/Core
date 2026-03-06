using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Audio.Logic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
    public class NOSoundPoolManager : NOManagerMonoBehaviour, INOSoundEmitterPoolService
    {
        private const int InitialPoolSize = 32;
        private readonly Queue<NOSoundEmitter> EmitterPool = new(InitialPoolSize);
        private readonly HashSet<NOSoundEmitter> ActiveEmitters = new(InitialPoolSize);

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
            for (int i = 1; i < InitialPoolSize; i++)
                EmitterPool.Enqueue(CreateEmitter());
            EmitterTemplate.Initialize();
            EmitterPool.Enqueue(EmitterTemplate);
        }

        private NOSoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(EmitterTemplate, transform);
            emitter.Initialize();
            return emitter;
        }

        public NOSoundEmitter Acquire()
        {
            if (!EmitterPool.TryDequeue(out var emitter))
                emitter = CreateEmitter();

            ActiveEmitters.Add(emitter);
            return emitter;
        }

        public void Return(NOSoundEmitter emitter)
        {
            if (ActiveEmitters.Remove(emitter))
                EmitterPool.Enqueue(emitter);
        }
    }
}
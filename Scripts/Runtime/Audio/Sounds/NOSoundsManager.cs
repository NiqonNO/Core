using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
    public class NOSoundsManager : NOManagerWithSingletonMonoBehaviour<NOSoundsManager>
    {
        private const int InitialPoolSize = 32;
        
        private readonly Queue<NOSoundEmitter> EmitterPool = new (InitialPoolSize);
        private readonly Dictionary<NOSoundClip, NOSoundClipState> Clips = new ();
        
        [SerializeField, SceneObjectsOnly] 
        private NOSoundEmitter EmitterTemplate;
        
        public override void Initialize()
        {
            base.Initialize();
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < InitialPoolSize; i++)
                EmitterPool.Enqueue(CreateEmitter());
        }

        public static void PlayClip(NOSoundClip clip) => Instance.Play(clip);
        private void Play(NOSoundClip clip)
        {
            var state = GetOrCreateState(clip);

            if (!state.CanPlay())
                return;

            var emitter = AcquireEmitter();

            emitter.Play(state);
        }

        public static void ForceStopClip(NOSoundClip clip) => Instance.StopAllClips(clip);
        private void StopAllClips(NOSoundClip clip)
        {
            if (!Clips.TryGetValue(clip, out var state)) return;
            state.StopAll();
        }
        
        private NOSoundClipState GetOrCreateState(NOSoundClip clip)
        {
            if(!Clips.TryGetValue(clip, out var clipState))
                clipState = CreateClipState(clip);
            return clipState;
        }
        private NOSoundClipState CreateClipState(NOSoundClip clip)
        {
            var clipState = new NOSoundClipState(clip);
            Clips.Add(clip, clipState);
            return clipState;
        }
        
        private NOSoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(EmitterTemplate, transform);
            emitter.gameObject.SetActive(false);
            emitter.Initialize(ReturnEmitter);
            return emitter;
        }
        private NOSoundEmitter AcquireEmitter()
        {
            if(!EmitterPool.TryDequeue(out var emitter))
                emitter = CreateEmitter();

            emitter.gameObject.SetActive(true);
            return emitter;
        }
        private void ReturnEmitter(NOSoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            EmitterPool.Enqueue(emitter);
        }
        
        public override void Dispose()
        {
        }
    }
}
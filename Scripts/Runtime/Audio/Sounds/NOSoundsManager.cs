using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
    public class NOSoundsManager : NOManagerWithSingletonMonoBehaviour<NOSoundsManager>
    {
        private const int InitialPoolSize = 32;
        
        private readonly Dictionary<NOSoundClip, NOSoundClipState> Clips = new ();
        private readonly Stack<NOSoundEmitter> Pool = new ();
        
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
                Pool.Push(CreateEmitter());
        }

        private NOSoundEmitter AcquireEmitter()
        {
            if(!Pool.TryPop(out var emitter))
                emitter = CreateEmitter();

            emitter.gameObject.SetActive(true);
            return emitter;
        }

        private void ReleaseEmitter(NOSoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            Pool.Push(emitter);
        }


        public static void PlayClip(NOSoundClip clip) => Instance.Play(clip);
        private void Play(NOSoundClip clip)
        {
            if(!Clips.TryGetValue(clip, out var clipState))
                clipState = CreateClipState(clip);
            
            clipState.Play();
        }

        
        public static void ForceStopClip(NOSoundClip clip) => Instance.ForceStop(clip);
        private void ForceStop(NOSoundClip clip)
        {
            if (!Clips.TryGetValue(clip, out var clipState)) return;
            
            clipState.ForceStop();
        }
        public static void ForceStopAllClips(NOSoundClip clip) => Instance.ForceStopAll(clip);
        private void ForceStopAll(NOSoundClip clip)
        {
            if (!Clips.TryGetValue(clip, out var clipState)) return;
            
            clipState.ForceStopAll();
        }
        
        private NOSoundClipState CreateClipState(NOSoundClip clip)
        {
            var clipState = new NOSoundClipState(clip, AcquireEmitter, ReleaseEmitter);
            Clips.Add(clip, clipState);
            return clipState;
        }
        private NOSoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(EmitterTemplate, transform);
            emitter.gameObject.SetActive(false);
            emitter.Initialize();
            return emitter;
        }
        
        public override void Dispose()
        {
        }
    }
}
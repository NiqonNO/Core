using UnityEngine;

namespace NiqonNO.Core.Audio
{
    public class NOSoundsManager : NOManagerWithSingletonMonoBehaviour<NOSoundsManager>
    {
        [SerializeField] private AudioSource source;
        public override void Initialize()
        {
            base.Initialize();
            InitializePool();
        }

        private void InitializePool()
        {
            
        }

        public override void Dispose()
        {
        }

        public void Play(NOSoundClip clip)
        {
            source.volume = clip.Volume;
            source.pitch = clip.Pitch;
            source.PlayOneShot(clip.GetClip());
        }

        public void Stop(NOSoundClip clip)
        {
        }
    }
}
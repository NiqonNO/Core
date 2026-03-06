using NiqonNO.Core.Audio.Logic;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
	public class NOAudioPlayer : NOInitializableMonoBehaviour
	{
		[NOInject] private INOAudioService AudioManager;
		[NOInject] private INOSoundEmitterPoolService EmitterPool;
		
		[SerializeField]
		private NOSoundPlaybackOptions Options;
		
		private INOSoundInstance TrackedInstance;
		
		public override void Initialize()
		{
		}

		public void Play(NOSoundClipData clipData) => Play(clipData, false);
		public void PlayTracked(NOSoundClipData clipData) => Play(clipData, true);
		private void Play(NOSoundClipData clipData, bool track)
		{
			if (clipData == null)
				return;
			
			var clip = AudioManager.GetClip(clipData);
			var emitter = clip.MaxPlaying ? clip.StealOldestInstance() as NOSoundEmitter : EmitterPool.AcquireAvailable();
			if (emitter == null)
				return;
			
			clip.Register(emitter);
			emitter.Play(clipData, Options, ReleaseTracked);

			if (!track)
				return;

			if (TrackedInstance != null && TrackedInstance != emitter)
				TrackedInstance.Stop();
			TrackedInstance = emitter;

		}

		void ReleaseTracked()
		{
			TrackedInstance = null;
		}

		public void StopTracked()
		{
			if (TrackedInstance == null)
				return;

			TrackedInstance.Stop();
			TrackedInstance = null;
		}

		public void StopAll(NOSoundClipData clipData)
		{
			AudioManager.StopAllInstances(clipData);
		}

		public void SetTrackedPitch(float pitch)
		{
			TrackedInstance?.SetPitch(pitch);
		}

		public void SetTrackedVolume(float volume)
		{
			TrackedInstance?.SetVolume(volume);
		}
	}
}
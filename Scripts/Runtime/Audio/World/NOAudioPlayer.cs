using NiqonNO.Core.Audio.Logic;

namespace NiqonNO.Core.Audio.World
{
	public class NOAudioPlayer : NOInitializableMonoBehaviour
	{
		[NOInject] private INOAudioService AudioManager;
		[NOInject] private INOSoundEmitterPoolService EmitterPool;
		
		/*[SerializeField]
		private NOSoundPlaybackOptions Options;*/
		
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
			var soundInstance = EmitterPool.AssignToEmitter(clip);

			if (!track)
			{
				soundInstance.Play();
				return;
			}

			if (TrackedInstance != null && TrackedInstance != soundInstance)
				TrackedInstance.Stop();
			TrackedInstance = soundInstance;
			soundInstance.Play(ReleaseTracked);
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
			ReleaseTracked();
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
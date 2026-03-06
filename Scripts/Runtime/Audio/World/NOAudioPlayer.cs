using NiqonNO.Core.Audio.Logic;

namespace NiqonNO.Core.Audio.World
{
	public class NOAudioPlayer : NOInitializableMonoBehaviour
	{
		[NOInject] private INOAudioService AudioService;
		[NOInject] private INOSoundPlaybackService PlaybackService;

		private NOSoundClip TrackedClip;
		private NOSoundInstanceHandle TrackedInstance = NOSoundInstanceHandle.Invalid;

		public override void Initialize()
		{
		}

		public void Play(NOSoundClipData clipData)
		{
			if (clipData == null)
				return;

			var clip = AudioService.GetClip(clipData);
			var handle = clip.Play(PlaybackService);
		}

		public void PlayTracked(NOSoundClipData clipData)
		{
			if (clipData == null)
				return;

			TrackedClip = AudioService.GetClip(clipData);
			TrackedInstance = TrackedClip.Play(PlaybackService);
		}

		public void StopTracked() => StopTracked(0, true);
		public void StopTracked(float fadeOutDuration, bool graceful = true)
		{
			if (!TrackedInstance.IsValid)
				return;

			PlaybackService.Stop(TrackedInstance, fadeOutDuration, graceful);
			TrackedClip.Unregister(TrackedInstance);
			TrackedInstance = NOSoundInstanceHandle.Invalid;
			TrackedClip = null;
		}

		public void StopAll(NOSoundClipData clipData) => StopAll(clipData, 0, true);
		public void StopAll(NOSoundClipData clipData, float fadeOutDuration, bool graceful = true)
		{
			if (clipData == null)
				return;

			var clip = AudioService.GetClip(clipData);
			clip.StopAll(PlaybackService, fadeOutDuration, graceful);
		}

		public void SetTrackedPitch(float pitch)
		{
			if (!TrackedInstance.IsValid)
				return;
			PlaybackService.SetPitch(TrackedInstance, pitch);
		}

		public void SetTrackedVolume(float volume)
		{
			if (PlaybackService == null || !TrackedInstance.IsValid)
				return;
			PlaybackService.SetVolume(TrackedInstance, volume);
		}

	}
}
using System;

namespace NiqonNO.Core.Audio.Logic
{
	public interface INOSoundPlaybackService : INOService
	{
		event Action<NOSoundInstanceHandle, NOSoundClipData> OnInstanceFinished;
		
		NOSoundInstanceHandle Play(NOSoundClipData data, NOSoundPlaybackOptions options = null);
		void StopAll(NOSoundClipData data, float fadeOutDuration = 0f, bool graceful = true);
		void Stop(NOSoundInstanceHandle instanceHandle, float fadeOutDuration = 0f, bool graceful = true);
		void SetPitch(NOSoundInstanceHandle instanceHandle, float pitch);
		void SetVolume(NOSoundInstanceHandle instanceHandle, float volume);
		bool IsPlaying(NOSoundInstanceHandle instanceHandle);
	}

}
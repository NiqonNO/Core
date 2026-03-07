using System;

namespace NiqonNO.Core.Audio.Logic
{
	public interface INOSoundInstance
	{
		void Play(Action<INOSoundInstance> onFinished = null);
		void Stop();
		void ForceStop();
		void SetPitch(float pitch);
		void SetVolume(float volume);
	}

}
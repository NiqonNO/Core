using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public interface INOSoundInstance
	{
		LinkedListNode<INOSoundInstance> Node { get; }
		void Setup(NOSoundClip owner, LinkedListNode<INOSoundInstance> node);
		void ClearRegistration();
		void Stop(float fadeOutDuration = 0f, bool graceful = true);
		void ForceStop();
		void SetPitch(float pitch);
		void SetVolume(float volume);
		bool IsActive { get; }
	}

}
using System.Collections.Generic;

namespace NiqonNO.Core.Audio
{
	public class NOAudioManagerState : NOManagerState<NOAudioManager>
	{
		public INOSoundEmitterPool EmitterPool;
		public readonly Dictionary<NOSoundClipData, NOSoundClip> Clips = new ();
	}
}
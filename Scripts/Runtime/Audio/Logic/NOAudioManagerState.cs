using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOAudioManagerState : NOManagerState
	{
		public Dictionary<NOSoundClipData, NOSoundClip> States { get; } = new ();
	}
}
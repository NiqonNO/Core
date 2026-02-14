namespace NiqonNO.Core.Audio
{
	public class NOAudioManager : NOManagerWithStateScriptableObject<NOAudioManager, NOAudioManagerState>
	{
		public void PlayClip(NOSoundClip clip) => NOSoundsManager.PlayClip(clip);
		public void PlayStopClip(NOSoundClip clip) => NOSoundsManager.ForceStopClip(clip);
	}
}
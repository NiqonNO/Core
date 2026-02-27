namespace NiqonNO.Core.Audio
{
	public class NOAudioManager : NOAssetManagerSO<NOSoundClipData, NOSoundClip, NOAudioManagerState>, INOAudioService
	{
		public void PlayClip(NOSoundClipData data) => GetState(data).Play();
		public void StopClip(NOSoundClipData data) => GetState(data).StopAll();
	}
}
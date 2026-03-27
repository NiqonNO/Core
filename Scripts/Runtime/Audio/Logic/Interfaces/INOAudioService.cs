namespace NiqonNO.Core.Audio.Logic
{
	public interface INOAudioService : INOService
	{
		NOSoundClip GetSoundClip(NOSoundClipData data);
		void StopAllInstances(NOSoundClipData data);
	}
}
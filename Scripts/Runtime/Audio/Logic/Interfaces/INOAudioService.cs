namespace NiqonNO.Core.Audio.Logic
{
	public interface INOAudioService : INOService
	{
		NOSoundClip GetState(NOSoundClipData data);
		void StopAllInstances(NOSoundClipData data);

	}
}
namespace NiqonNO.Core.Audio.Logic
{
	public interface INOAudioService : INOService
	{
		NOSoundClip GetClip(NOSoundClipData data);
	}
}
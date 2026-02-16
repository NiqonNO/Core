using System.Collections.Generic;

namespace NiqonNO.Core.Audio
{
	public class NOAudioManager : NOManagerWithStateScriptableObject<NOAudioManager, NOAudioManagerState>
	{
		private Dictionary<NOSoundClipData, NOSoundClip>  Clips => RuntimeState.Clips;
		private INOSoundEmitterPool  EmitterPool => RuntimeState.EmitterPool;
		
		private NOSoundClip GetClip(NOSoundClipData data)
		{
			if (Clips.TryGetValue(data, out var clipState)) return clipState;
			
			clipState = new NOSoundClip(data, EmitterPool);
			Clips.Add(data, clipState);
			return clipState;
		}
		
		public void PlayClip(NOSoundClipData data) => GetClip(data).Play();
		public void StopClip(NOSoundClipData data) => GetClip(data).StopAll();

		public static void RegisterSoundPool(INOSoundEmitterPool emitterPool) => RuntimeState.EmitterPool = emitterPool;
	}
}
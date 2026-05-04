using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOAudioManager : NOManagerWithStateSO<NOAudioManagerState>, INOAudioService, INOAssetSetManager<NOSoundClipData, NOSoundClip>
	{
		public Dictionary<NOSoundClipData, NOSoundClip> States => RuntimeState.States;
		
		public override void Dispose()
		{
			foreach (var state in RuntimeState.States.Values)
			{
				state.Dispose();
				Destroy(state);
			}
			RuntimeState.States.Clear();
			base.Dispose();
		}

		public NOSoundClip GetSoundClip(NOSoundClipData data) => SoundSetManager.GetOrCreateState(data);
		public void StopAllInstances(NOSoundClipData data)
		{
			GetSoundClip(data).Dispose();
		}

		INOAssetSetManager<NOSoundClipData, NOSoundClip> SoundSetManager => this;
		INOContext INOAssetSetManager<NOSoundClipData, NOSoundClip>.Context => base.Context;
	}
}
namespace NiqonNO.Core.Audio.Logic
{
	public class NOAudioManager : NOManagerWithStateSO<NOAudioManagerState>, INOAudioService, INOAssetSetManager<NOSoundClipData, NOSoundClip, NOAudioManagerState>
	{
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

		public NOSoundClip GetSoundClip(NOSoundClipData data) => SoundSetManager.GetState(data);
		public void StopAllInstances(NOSoundClipData data)
		{
			GetSoundClip(data).Dispose();
		}

		INOAssetSetManager<NOSoundClipData, NOSoundClip, NOAudioManagerState> SoundSetManager => this;
		INOContext INOAssetSetManager<NOSoundClipData, NOSoundClip, NOAudioManagerState>.Context => base.Context;
		NOAudioManagerState INOAssetSetManager<NOSoundClipData, NOSoundClip, NOAudioManagerState>.RuntimeState => base.RuntimeState;
	}
}
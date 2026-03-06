namespace NiqonNO.Core.Audio.Logic
{
	public class NOAudioManager : NOAssetManagerSO<NOSoundClipData, NOSoundClip, NOAudioManagerState>, INOAudioService
	{
		public NOSoundClip GetClip(NOSoundClipData data) => GetState(data);
		
		public override void Dispose()
		{
			foreach (var state in RuntimeState.States.Values)
				state.Dispose();
			base.Dispose();
		}

		public void StopAllInstances(NOSoundClipData data)
		{
			GetClip(data).Dispose();
		}
	}
}
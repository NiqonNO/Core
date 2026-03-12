namespace NiqonNO.Core.Audio.Logic
{
	public class NOAudioManager : NOAssetManagerSO<NOSoundClipData, NOSoundClip, NOAudioManagerState>, INOAudioService
	{
		public override void Dispose()
		{
			foreach (var state in RuntimeState.States.Values)
				state.Dispose();
			base.Dispose();
		}

		public void StopAllInstances(NOSoundClipData data)
		{
			GetState(data).Dispose();
		}
	}
}
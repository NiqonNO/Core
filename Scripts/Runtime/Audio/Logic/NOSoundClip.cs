using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOSoundClip : NODataState<NOSoundClipData>
	{
		private readonly List<NOSoundInstanceHandle> ActiveInstances = new();

		public NOSoundInstanceHandle Play(INOSoundPlaybackService playbackService, NOSoundPlaybackOptions options = null)
		{
			if (playbackService == null)
				return NOSoundInstanceHandle.Invalid;

			PruneInactiveInstances(playbackService);
			if (ActiveInstances.Count >= Asset.MaxInstances)
			{
				var oldest = ActiveInstances[0];
				playbackService.Stop(oldest, 0f, graceful: false);
				ActiveInstances.RemoveAt(0);
			}

			var instance = playbackService.Play(Asset, options);
			if (instance.IsValid)
				ActiveInstances.Add(instance);
			return instance;
		}

		public void StopAll(INOSoundPlaybackService playbackService, float fadeOutDuration = 0f, bool graceful = true)
		{
			if (playbackService == null)
				return;
			playbackService.StopAll(Asset, fadeOutDuration, graceful);
			ActiveInstances.Clear();
		}

		public bool IsOwned(NOSoundInstanceHandle instanceHandle)
		{
			return ActiveInstances.Contains(instanceHandle);
		}

		public void Unregister(NOSoundInstanceHandle instanceHandle)
		{
			ActiveInstances.Remove(instanceHandle);
		}

		public void Dispose()
		{
			ActiveInstances.Clear();
		}

		private void PruneInactiveInstances(INOSoundPlaybackService playbackService)
		{
			for (var i = ActiveInstances.Count - 1; i >= 0; i--)
			{
				if (!playbackService.IsPlaying(ActiveInstances[i]))
					ActiveInstances.RemoveAt(i);
			}
		}
	}
}
using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOSoundClip : NODataState<NOSoundClipData>
	{
		private readonly LinkedList<INOSoundInstance> ActiveInstances = new();

		private int ActiveCount => ActiveInstances.Count;
		private bool IsPlaying => ActiveCount > 0;
		public bool MaxPlaying => ActiveCount >= Asset.MaxInstances;

		public INOSoundInstance StealOldestInstance()
		{
			var instance = ActiveInstances.First.Value;
			instance.ForceStop();
			return instance;
		}

		public void Register(INOSoundInstance instance)
		{
			if (instance == null)
				return;

			instance.Setup(this, ActiveInstances.AddLast(instance));
		}

		public void Unregister(INOSoundInstance instance)
		{
			if (instance?.Node == null)
				return;

			ActiveInstances.Remove(instance.Node);
			instance.ClearRegistration();
		}

		public void Dispose()
		{
			if (!IsPlaying)
				return;

			var toDispose = new List<INOSoundInstance>(ActiveInstances);
			foreach (var instance in toDispose)
				instance.ForceStop();
			ActiveInstances.Clear();
		}

	}
}
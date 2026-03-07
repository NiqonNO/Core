using System.Collections.Generic;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOSoundClip : NODataState<NOSoundClipData>
	{
		private readonly LinkedList<INOSoundInstance> ActiveInstances = new();

		private int ActiveCount => ActiveInstances.Count;
		private bool IsPlaying => ActiveCount > 0;
		public bool MaxPlaying => ActiveCount >= Asset.MaxInstances;

		public INOSoundInstance GetOldestInstance()
		{
			var instance = ActiveInstances.First;
			ActiveInstances.RemoveFirst();
			ActiveInstances.AddLast(instance);
			return instance.Value;
		}

		public LinkedListNode<INOSoundInstance> Register(INOSoundInstance instance)
		{
			return ActiveInstances.AddLast(instance);
		}

		public void Unregister(LinkedListNode<INOSoundInstance> node)
		{
			if (node == null)
				return;

			ActiveInstances.Remove(node);
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
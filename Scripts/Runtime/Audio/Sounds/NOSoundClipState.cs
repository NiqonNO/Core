using System;
using System.Collections.Generic;

namespace NiqonNO.Core.Audio
{
	public class NOSoundClipState
	{
		public readonly NOSoundClip Data;
		private readonly HashSet<NOSoundEmitter> ActiveEmitters = new();
		
		private int ActiveCount => ActiveEmitters.Count;
		private bool IsPlaying => ActiveCount > 0;
		
		public NOSoundClipState(NOSoundClip data)
		{
			Data = data;
		}
		
		public bool CanPlay()
		{
			if (Data.Loop && IsPlaying) return false;
			if (Data.MaxInstances <= ActiveCount) return false;
			return true;
		}
		
		public void RegisterEmitter(NOSoundEmitter emitter)
		{
			ActiveEmitters.Add(emitter);
		}

		public void UnregisterEmitter(NOSoundEmitter emitter)
		{
			ActiveEmitters.Remove(emitter);
		}

		public void StopAll()
		{
			foreach (var emitter in ActiveEmitters)
			{
				emitter.StopImmediate();
			}

			ActiveEmitters.Clear();
		}
	}
}
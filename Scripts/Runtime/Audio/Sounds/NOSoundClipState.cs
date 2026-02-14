using System;
using System.Collections.Generic;

namespace NiqonNO.Core.Audio
{
	public class NOSoundClipState
	{
		private readonly NOSoundClip Data;
		private readonly Func<NOSoundEmitter> Acquire;
		private readonly Action<NOSoundEmitter> Release;
		private readonly Queue<NOSoundEmitter> ActiveEmitters = new();
		
		private int ActiveCount => ActiveEmitters.Count;
		private bool IsPlaying => ActiveCount > 0;
		
		private bool CanPlay()
		{
			if (Data.Loop && IsPlaying) return false;
			return true;
		}
		
		public NOSoundClipState(NOSoundClip data, 
			Func<NOSoundEmitter> acquire,
			Action<NOSoundEmitter> release)
		{
			Data = data;
			Acquire = acquire;
			Release = release;
		}
		
		public void Play()
		{
			if (!CanPlay()) return;

			var emitter = Acquire();
			if (emitter == null)
				return;
			
			ActiveEmitters.Enqueue(emitter);
			emitter.ConfigureEmitter(Data);
			emitter.Play(ForceStop);
		}

		public void ForceStop()
		{
			if (!IsPlaying) return;
			
			var emitter = ActiveEmitters.Dequeue();
			emitter.Stop();
			Release(emitter);
		}


		public void ForceStopAll()
		{
			while (IsPlaying)
			{
				ForceStop();
			}
		}
	}
}
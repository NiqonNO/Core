using System;
using System.Collections.Generic;
using System.Linq;

namespace NiqonNO.Core.Audio
{
	public class NOSoundClip
	{
		private readonly NOSoundClipData Data;
		private readonly INOSoundEmitterPool EmitterPool;
		private readonly List<NOSoundEmitter> ActiveEmitters = new();
		
		private int ActiveCount => ActiveEmitters.Count;
		private bool IsPlaying => ActiveCount > 0;

		private bool CanPlay => !Data.Loop || !IsPlaying;
		private bool MaxPlaying => ActiveCount >= Data.MaxInstances;
		
		public NOSoundClip(NOSoundClipData data, INOSoundEmitterPool emitterPool)
		{
			Data = data;
			EmitterPool = emitterPool;
		}

		public void Play()
		{
			if (!CanPlay) return;
			var emitter = AllocateEmitter();
			
			emitter.Play();
		}

		NOSoundEmitter AllocateEmitter()
		{
			var emitter = MaxPlaying ? StealEmitter() : EmitterPool.Acquire();
			ActiveEmitters.Add(emitter);
			emitter.ConfigureEmitter(Data, ReturnEmitter);
			return emitter;
			
			NOSoundEmitter StealEmitter()
			{
				var reusedEmitter = ActiveEmitters[0];
				ActiveEmitters.RemoveAt(0);
				return reusedEmitter;
			}
		}

		void ReturnEmitter(NOSoundEmitter emitter)
		{
			ActiveEmitters.Remove(emitter);
			EmitterPool.Return(emitter);
		}

		public void StopAll()
		{
			while (ActiveEmitters.Count > 0)
			{
				ActiveEmitters.First().ForceStop();
			}
		}
	}
}
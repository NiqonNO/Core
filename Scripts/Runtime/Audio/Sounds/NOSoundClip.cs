using System;
using System.Collections.Generic;
using System.Linq;

namespace NiqonNO.Core.Audio
{
	public class NOSoundClip : NODataState<NOSoundClipData>
	{
		[NOInject]
		private INOSoundEmitterPool EmitterPool;
		
		private readonly List<NOSoundEmitter> ActiveEmitters = new();
		
		private int ActiveCount => ActiveEmitters.Count;
		private bool IsPlaying => ActiveCount > 0;

		private bool CanPlay => !Asset.Loop || !IsPlaying;
		private bool MaxPlaying => ActiveCount >= Asset.MaxInstances;

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
			emitter.ConfigureEmitter(Asset, ReturnEmitter);
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
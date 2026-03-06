using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.Core.Audio.Logic;

namespace NiqonNO.Core.Audio.World
{
	public class NOSoundPlaybackManager : NOManagerMonoBehaviour, INOSoundPlaybackService
	{
		[NOInject] private INOSoundEmitterPoolService EmitterPool;
        
        private int NextInstanceId;
        
		private readonly Dictionary<NOSoundInstanceHandle, NOSoundEmitter> EmitterByInstance = new();
		private readonly Dictionary<NOSoundEmitter, NOSoundInstanceHandle> InstanceByEmitter = new();
		private readonly Dictionary<NOSoundInstanceHandle, NOSoundClipData> ClipByInstance = new();
        
        public event Action<NOSoundInstanceHandle, NOSoundClipData> OnInstanceFinished;

        public override void Initialize()
        {
        }

        public override void Dispose()
		{
			EmitterByInstance.Clear();
			InstanceByEmitter.Clear();
			ClipByInstance.Clear();
		}
		
		public NOSoundInstanceHandle Play(NOSoundClipData data, NOSoundPlaybackOptions options = null)
        {
            if (data == null)
                return NOSoundInstanceHandle.Invalid;

            var emitter = EmitterPool.Acquire();
            var instance = new NOSoundInstanceHandle(NextInstanceId++);
            EmitterByInstance[instance] = emitter;
            InstanceByEmitter[emitter] = instance;
            ClipByInstance[instance] = data;

            emitter.Play(data, options, () => ReturnAndNotify(emitter));
            return instance;
        }

        public void StopAll(NOSoundClipData data, float fadeOutDuration = 0f, bool graceful = true)
        {
            if (data == null)
                return;

            var instances = ClipByInstance
                .Where(pair => pair.Value == data)
                .Select(pair => pair.Key)
                .ToArray();

            foreach (var instance in instances)
                Stop(instance, fadeOutDuration, graceful);
        }

        public void Stop(NOSoundInstanceHandle instanceHandle, float fadeOutDuration = 0f, bool graceful = true)
        {
            if (!EmitterByInstance.TryGetValue(instanceHandle, out var emitter))
                return;

            emitter.Stop(fadeOutDuration, graceful);
        }

        public void SetPitch(NOSoundInstanceHandle instanceHandle, float pitch)
        {
            if (!EmitterByInstance.TryGetValue(instanceHandle, out var emitter))
                return;
            emitter.SetPitch(pitch);
        }

        public void SetVolume(NOSoundInstanceHandle instanceHandle, float volume)
        {
            if (!EmitterByInstance.TryGetValue(instanceHandle, out var emitter))
                return;
            emitter.SetVolume(volume);
        }

        public bool IsPlaying(NOSoundInstanceHandle instanceHandle)
        {
            return EmitterByInstance.ContainsKey(instanceHandle);
        }

        private void ReturnAndNotify(NOSoundEmitter emitter)
        {
            if (!InstanceByEmitter.TryGetValue(emitter, out var instanceHandle))
            {
                EmitterPool.Return(emitter);
                return;
            }

            InstanceByEmitter.Remove(emitter);
            EmitterByInstance.Remove(instanceHandle);

            if (ClipByInstance.TryGetValue(instanceHandle, out var clipData))
            {
                ClipByInstance.Remove(instanceHandle);
                OnInstanceFinished?.Invoke(instanceHandle, clipData);
            }

            EmitterPool.Return(emitter);
        }
	}
}
using System;
using System.Collections;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class NOSoundEmitter : NOMonoBehaviour
	{
		[SerializeField] 
		private AudioSource AudioSource;

		private Action<NOSoundEmitter> OnClipFinished;
		private Coroutine FinishRoutine;

		public void Initialize(Action<NOSoundEmitter> onClipFinished)
		{
			OnClipFinished = onClipFinished;
		}
		
		public void Play(NOSoundClipState clipState)
		{
			clipState.RegisterEmitter(this);
			ConfigureEmitter(clipState.Data);
			AudioSource.Play();
			
			if(!AudioSource.loop)
				FinishRoutine = StartCoroutine(WaitForFinish(clipState.UnregisterEmitter));
		}
		
		private void ConfigureEmitter(NOSoundClip data)
		{
			var clip = data.GetClip();
			if (clip == null)
				return;

			AudioSource.clip = clip;
			AudioSource.loop = data.Loop;
			AudioSource.outputAudioMixerGroup = data.MixerGroup;

			AudioSource.volume = data.Volume;
			AudioSource.pitch = data.Pitch;
			AudioSource.panStereo = data.StereoPan;
			AudioSource.spatialBlend = data.SpatialBlend;
			AudioSource.reverbZoneMix = data.ReverbZoneMix;

			AudioSource.dopplerLevel = data.DopplerLevel;
			AudioSource.spread = data.Spread;
			AudioSource.rolloffMode = data.VolumeRolloff;
			AudioSource.minDistance = data.MinDistance;
			AudioSource.maxDistance = data.MaxDistance;
		}
		
		public void StopImmediate()
		{
			if (FinishRoutine != null)
			{
				StopCoroutine(FinishRoutine);
				FinishRoutine = null;
			}

			AudioSource.Stop();
			OnClipFinished?.Invoke(this);
		}
		
		private IEnumerator WaitForFinish(Action<NOSoundEmitter> onFinished)
		{
			yield return new WaitWhile(() => AudioSource.isPlaying);
			onFinished?.Invoke(this);
			OnClipFinished?.Invoke(this);
		}
	}
}
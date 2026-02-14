using System;
using System.Collections;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
	public class NOSoundEmitter : NOMonoBehaviour
	{
		[SerializeField] 
		private AudioSource AudioSource;

		private Coroutine WaitingCoroutine;
		
		public void Initialize()
		{
		}
		
		public void ConfigureEmitter(NOSoundClip data)
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

		public void Play(Action onFinish)
		{
			AudioSource.Play();
			WaitingCoroutine = StartCoroutine(WaitForEnd(onFinish));
		}

		public void Stop()
		{
			if (WaitingCoroutine != null) 
			{
				StopCoroutine(WaitingCoroutine);
				WaitingCoroutine = null;
			}
			
			AudioSource.Stop();
			
		}
		
		private IEnumerator WaitForEnd(Action onFinish)
		{
			yield return new WaitWhile(() => AudioSource.isPlaying);
			onFinish?.Invoke();
			
			
		}
	}
}
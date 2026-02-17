using System;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class NOSoundEmitter : NOMonoBehaviour
	{
		private Action<NOSoundEmitter> OnFinished;
		private bool Configured;
		
		[SerializeField] 
		private AudioSource AudioSource;

		public void Initialize()
		{
			AudioSource??=GetComponent<AudioSource>();
			AudioSource.playOnAwake = false;
			gameObject.SetActive(false);
		}
		
		public void Play()
		{
			if (Configured == false) return;
			gameObject.SetActive(true);
			AudioSource.Play();
		}
		
		public void ConfigureEmitter(NOSoundClipData data, Action<NOSoundEmitter> onClipPlayed)
		{
			var clip = data.GetClip();
			if (clip == null)
				return;

			AudioSource.clip = clip;
			AudioSource.loop = data.Loop;
			AudioSource.priority = data.Priority;
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

			OnFinished = onClipPlayed;

			Configured = true;
		}

		private void Update()
		{
			if (AudioSource.loop) return;
			if (AudioSource.isPlaying) return;

			Stop();
		}

		private void Stop()
		{
			OnFinished?.Invoke(this);
			OnFinished = null;
			Configured = false;
			gameObject.SetActive(false);
		}

		public void ForceStop()
		{
			AudioSource.Stop();
			Stop();
		}
	}
}
using System;
using System.Collections.Generic;
using NiqonNO.Core.Audio.Logic;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
	[RequireComponent(typeof(AudioSource))]
	public class NOSoundEmitter : NOMonoBehaviour, INOSoundInstance
	{
		[SerializeField]
		private NOSoundPoolManager SoundPoolManager;
		[SerializeField]
		private AudioSource AudioSource;

		private Action<INOSoundInstance> OnFinished;

		private NOSoundClip Clip;
		private LinkedListNode<INOSoundInstance> Node;

		private NOSoundFadeHandler FadeHandler;
		
		private bool Plying;
		private bool StopRequested;

		private float FadeInDuration => Clip.Asset.DefaultFadeIn;
		private float FadeOutDuration => Clip.Asset.DefaultFadeOut;

		private float TimeToEnd => AudioSource.clip.length - AudioSource.time;

		public void Initialize()
		{
			AudioSource ??= GetComponent<AudioSource>();
			AudioSource.playOnAwake = false;

			FadeHandler = new NOSoundFadeHandler(SetAudioSourceVolume);
		}

		public INOSoundInstance AssignClip(NOSoundClip clip)
		{
			Clip = clip;
			Node = Clip.Register(this);
			Configure(clip.Asset);
			return this as INOSoundInstance;
		}
		
		public void Reset()
		{
			FinishPlay();
			Configure(Clip.Asset);
		}

		private void Configure(NOSoundClipData data)
		{
			AudioSource.priority = data.Priority;
			AudioSource.outputAudioMixerGroup = data.MixerGroup;

			AudioSource.clip = data.GetClip();
			AudioSource.volume = data.ResolveVolume();
			AudioSource.pitch = data.ResolvePitch();
			
			AudioSource.loop = data.Loop;
			AudioSource.panStereo = data.StereoPan;
			AudioSource.spatialBlend = data.SpatialBlend;
			AudioSource.reverbZoneMix = data.ReverbZoneMix;
			AudioSource.dopplerLevel = data.DopplerLevel;
			AudioSource.spread = data.Spread;
			AudioSource.rolloffMode = data.VolumeRolloff;
			AudioSource.minDistance = data.MinDistance;
			AudioSource.maxDistance = data.MaxDistance;

			FadeHandler.Finish();
			StopRequested = false;
		}

		public void Play(Action<INOSoundInstance> onFinished = null)
		{
			if (Clip == null) return;
			
			OnFinished = onFinished;
			FadeHandler.BeginFadeIn(AudioSource.volume, FadeInDuration);
			AudioSource.Play();
			Plying = true;
		}

		private void Update()
		{
			if (!Plying)
				return;

			FadeHandler.TickFade(Time.deltaTime);

			if (StopRequested)
			{
				if (FadeHandler.IsDone)
				{
					FinishPlay();
					Unregister();
				}
				return;
			}

			if (AudioSource.loop) return;
			if (TimeToEnd <= FadeOutDuration || 
			    !AudioSource.isPlaying)
				Stop();
		}

		public void Stop()
		{
			if (!Plying)
				return;

			StopRequested = true;
			FadeHandler.BeginFadeOut(AudioSource.volume, FadeOutDuration);
		}

		public void ForceStop()
		{
			if (!Plying)
				return;
			
			FinishPlay();
			Unregister();
		}
		
		private void FinishPlay()
		{
			FadeHandler.Finish();
			AudioSource.Stop();
			OnFinished?.Invoke(this);
			OnFinished = null;
			Plying = false;
		}
		private void Unregister()
		{
			Clip.Unregister(Node);
			Node = null;
			Clip = null;
			SoundPoolManager.Return(this as INOSoundInstance);
		}

		public void SetPitch(float pitch)
		{
			AudioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
		}

		public void SetVolume(float volume)
		{
			if(FadeHandler.IsDone)
			{
				SetAudioSourceVolume(volume);
				return;
			}

			if (StopRequested) return;
			FadeHandler.UpdateTargetVolume(volume);
		}

		private void SetAudioSourceVolume(float volume)
		{
			AudioSource.volume = Mathf.Clamp01(volume);
		}
	}
}
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
		private AudioSource AudioSource;
		
		private enum AdvancedLoopState
		{
			None,
			Start,
			Loop,
			End
		}

		private Action OnFinished;
		private bool Configured;
		private float BaseVolume = 1f;
		private float FadeStartVolume;
		private float FadeTarget;
		private float FadeDuration;
		private float FadeElapsed;
		
		private AudioClip LoopClip;
		private AudioClip EndClip;
		private bool StopRequested;
		private AdvancedLoopState LoopState;

		public LinkedListNode<INOSoundInstance> Node { get; private set; }
		public NOSoundClip Owner { get; private set; }
		public bool IsActive => Configured;

		public void Initialize()
		{
			AudioSource ??= GetComponent<AudioSource>();
			AudioSource.playOnAwake = false;
		}

		public void Setup(NOSoundClip owner, LinkedListNode<INOSoundInstance> node)
		{
			Owner = owner;
			Node = node;
		}

		public void ClearRegistration()
		{
			Owner = null;
			Node = null;
		}

		public void Play(NOSoundClipData data, NOSoundPlaybackOptions options, Action onFinished)
		{
			if (data == null)
				return;

			ConfigureCommon(data, options, onFinished);
			if (data.HasAdvancedLoop)
				ConfigureAdvancedLoop(data);
			else
				ConfigureSimple(data);

			Configured = true;
			AudioSource.Play();
			BeginFadeTo(BaseVolume, options?.FadeInDuration ?? data.DefaultFadeIn, setVolumeToZero: true);
		}

		private void ConfigureCommon(NOSoundClipData data, NOSoundPlaybackOptions options, Action onFinished)
		{
			AudioSource.priority = data.Priority;
			AudioSource.outputAudioMixerGroup = data.MixerGroup;

			var resolvedVolume = options?.Volume ?? data.ResolveVolume();
			var resolvedPitch = options?.Pitch ?? data.ResolvePitch();
			BaseVolume = Mathf.Clamp01(resolvedVolume);
			AudioSource.volume = BaseVolume;
			AudioSource.pitch = Mathf.Clamp(resolvedPitch, -3f, 3f);
			AudioSource.panStereo = data.StereoPan;
			AudioSource.spatialBlend = data.SpatialBlend;
			AudioSource.reverbZoneMix = data.ReverbZoneMix;
			AudioSource.dopplerLevel = data.DopplerLevel;
			AudioSource.spread = data.Spread;
			AudioSource.rolloffMode = data.VolumeRolloff;
			AudioSource.minDistance = data.MinDistance;
			AudioSource.maxDistance = data.MaxDistance;

			if (options?.WorldPosition is { } worldPosition)
				transform.position = worldPosition;

			OnFinished = onFinished;
			StopRequested = false;
			LoopState = AdvancedLoopState.None;
			FadeDuration = 0f;
			FadeElapsed = 0f;
		}

		private void ConfigureSimple(NOSoundClipData data)
		{
			AudioSource.clip = data.GetClip();
			AudioSource.loop = data.Loop;
			LoopClip = null;
			EndClip = null;
		}
		
		private void ConfigureAdvancedLoop(NOSoundClipData data)
		{			
			LoopClip = data.GetClip();
			EndClip = data.EndClip;

			if (data.StartClip != null)
			{
				AudioSource.clip = data.StartClip;
				AudioSource.loop = false;
				LoopState = AdvancedLoopState.Start;
				return;
			}

			if (LoopClip != null)
			{
				AudioSource.clip = LoopClip;
				AudioSource.loop = true;
				LoopState = AdvancedLoopState.Loop;
				return;
			}

			AudioSource.clip = EndClip;
			AudioSource.loop = false;
			LoopState = AdvancedLoopState.End;
		}

		public void Stop(float fadeOutDuration = 0f, bool graceful = true)
		{
			if (!Configured)
				return;

			if (graceful && EndClip != null)
			{
				StopRequested = true;
				if (LoopState == AdvancedLoopState.Loop)
					return;
			}

			if (fadeOutDuration > 0f)
			{
				BeginFadeTo(0f, fadeOutDuration);
				return;
			}

			AudioSource.Stop();
			FinalizeStop();
		}

		public void ForceStop()
		{
			if (!Configured)
				return;

			AudioSource.Stop();
			FinalizeStop();
		}

		public void SetPitch(float pitch)
		{
			if (!Configured)
				return;
			AudioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
		}

		public void SetVolume(float volume)
		{
			if (!Configured)
				return;
			BaseVolume = Mathf.Clamp01(volume);
			if (FadeDuration <= 0f)
				AudioSource.volume = BaseVolume;
		}

		private void BeginFadeTo(float targetVolume, float duration, bool setVolumeToZero = false)
		{
			if (duration <= 0f)
			{
				AudioSource.volume = targetVolume;
				FadeDuration = 0f;
				FadeElapsed = 0f;
				return;
			}

			if (setVolumeToZero)
				AudioSource.volume = 0f;
			FadeStartVolume = AudioSource.volume;
			FadeTarget = targetVolume;
			FadeDuration = duration;
			FadeElapsed = 0f;
		}

		private void Update()
		{
			if (!Configured)
				return;

			TickFade();

			if (AudioSource.isPlaying)
				return;

			if (TryAdvanceAdvancedLoop())
				return;

			FinalizeStop();
		}

		private void TickFade()
		{
			if (FadeDuration <= 0f)
				return;

			FadeElapsed += Time.deltaTime;
			var normalizedTime = Mathf.Clamp01(FadeElapsed / FadeDuration);
			AudioSource.volume = Mathf.Lerp(FadeStartVolume, FadeTarget, normalizedTime);

			if (normalizedTime < 1f)
				return;

			FadeDuration = 0f;
			FadeElapsed = 0f;
			if (!Mathf.Approximately(FadeTarget, 0f))
				return;

			AudioSource.Stop();
			FinalizeStop();
		}

		private bool TryAdvanceAdvancedLoop()
		{
			if (LoopState == AdvancedLoopState.None)
				return false;

			switch (LoopState)
			{
				case AdvancedLoopState.Start when StopRequested && EndClip != null:
					AudioSource.clip = EndClip;
					AudioSource.loop = false;
					LoopState = AdvancedLoopState.End;
					AudioSource.Play();
					return true;
				case AdvancedLoopState.Start when LoopClip != null:
					AudioSource.clip = LoopClip;
					AudioSource.loop = true;
					LoopState = AdvancedLoopState.Loop;
					AudioSource.Play();
					return true;
				case AdvancedLoopState.Start when EndClip != null:
					AudioSource.clip = EndClip;
					AudioSource.loop = false;
					LoopState = AdvancedLoopState.End;
					AudioSource.Play();
					return true;
				case AdvancedLoopState.Loop when StopRequested && EndClip != null:
					AudioSource.clip = EndClip;
					AudioSource.loop = false;
					LoopState = AdvancedLoopState.End;
					AudioSource.Play();
					return true;
			}

			return false;
		}

		private void FinalizeStop()
		{
			OnFinished?.Invoke();
			OnFinished = null;
			Configured = false;
			LoopClip = null;
			EndClip = null;
			LoopState = AdvancedLoopState.None;
			FadeDuration = 0f;
			FadeElapsed = 0f;
			
			Owner.Unregister(this);
		}

	}
}
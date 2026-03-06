using System;
using NiqonNO.Core.Audio.Logic;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
	[RequireComponent(typeof(AudioSource))]
	public class NOSoundEmitter : NOMonoBehaviour
	{
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
		private float FadeVelocity;
		private float FadeTarget;
		private float FadeDuration;
		private float FadeElapsed;
		
		private AudioClip LoopClip;
		private AudioClip EndClip;
		private bool StopRequested;
		private AdvancedLoopState LoopState;

		[SerializeField]
		private AudioSource AudioSource;

		public void Initialize()
		{
			AudioSource ??= GetComponent<AudioSource>();
			AudioSource.playOnAwake = false;
			gameObject.SetActive(false);
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
			gameObject.SetActive(true);
			AudioSource.Play();
			BeginFadeIn(options?.FadeInDuration ?? data.DefaultFadeIn);
		}

		private void ConfigureCommon(NOSoundClipData data, NOSoundPlaybackOptions options, Action onFinished)
		{
			AudioSource.priority = data.Priority;
			AudioSource.outputAudioMixerGroup = data.MixerGroup;

			BaseVolume = Mathf.Clamp01(options?.Volume ?? data.Volume);
			AudioSource.volume = BaseVolume;
			AudioSource.pitch = Mathf.Clamp(options?.Pitch ?? data.Pitch, -3f, 3f);
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

			if (graceful && LoopState == AdvancedLoopState.Loop && EndClip != null)
			{
				StopRequested = true;
				return;
			}

			if (fadeOutDuration > 0f)
			{
				FadeTarget = 0f;
				FadeDuration = fadeOutDuration;
				FadeElapsed = 0f;
				FadeVelocity = (AudioSource.volume - FadeTarget) / fadeOutDuration;
				return;
			}

			AudioSource.Stop();
			FinalizeStop();
		}

		public void ForceStop()
		{
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
			AudioSource.volume = BaseVolume;
		}

		private void BeginFadeIn(float duration)
		{
			if (duration <= 0f)
				return;

			AudioSource.volume = 0f;
			FadeTarget = BaseVolume;
			FadeDuration = duration;
			FadeElapsed = 0f;
			FadeVelocity = (FadeTarget - AudioSource.volume) / duration;
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
			AudioSource.volume = Mathf.MoveTowards(AudioSource.volume, FadeTarget, FadeVelocity * Time.deltaTime);
			if (FadeElapsed < FadeDuration)
				return;

			AudioSource.volume = FadeTarget;
			FadeDuration = 0f;
			FadeElapsed = 0f;
			if (Mathf.Approximately(FadeTarget, 0f) && !AudioSource.isPlaying)
				FinalizeStop();
			else if (Mathf.Approximately(FadeTarget, 0f))
				AudioSource.Stop();
		}

		private bool TryAdvanceAdvancedLoop()
		{
			if (LoopState == AdvancedLoopState.None)
				return false;

			switch (LoopState)
			{
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
			gameObject.SetActive(false);
		}

	}
}
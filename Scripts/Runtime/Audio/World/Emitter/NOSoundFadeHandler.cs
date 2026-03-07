using System;
using UnityEngine;

namespace NiqonNO.Core.Audio.World
{
	public class NOSoundFadeHandler
	{
		private Action<float> OnVolumeChanged;
		
		private NOEase Ease;
		private float FadeInitial;
		private float FadeTarget;
		private float FadeDuration;

		private float FadeElapsed = 0;
		public bool IsDone { get; private set; } = true;

		public NOSoundFadeHandler(Action<float> onVolumeChanged)
		{
			OnVolumeChanged = onVolumeChanged;
		}
		
		public void BeginFadeIn(float targetVolume, float duration, NOEase ease = NOEase.Linear) => DoFade(0, targetVolume, duration, ease);
		public void BeginFadeOut(float initialVolume, float duration, NOEase ease = NOEase.Linear) => DoFade(initialVolume,0, duration, ease);

		private void DoFade(float initialVolume, float endVolume, float duration, NOEase ease)
		{
			if (!IsDone) Finish();
			
			if (duration <= 0f)
			{
				OnVolumeChanged.Invoke(endVolume);
				return;
			}
			
			Ease = ease;
			OnVolumeChanged.Invoke(initialVolume);
			FadeInitial = initialVolume;
			FadeTarget = endVolume;
			FadeDuration = duration;

			IsDone = false;
		}
		public void UpdateTargetVolume(float volume)
		{
			FadeTarget = volume;
			FadeDuration = 1 - FadeElapsed;
			FadeElapsed = 0;
		}
		
		public void TickFade(float deltaTime)
		{
			if (IsDone) return;
			if (FadeDuration <= 0f) return;

			FadeElapsed += deltaTime;
			var normalizedTime = Ease.Ease(Mathf.Clamp01(FadeElapsed / FadeDuration));
			OnVolumeChanged.Invoke(Mathf.Lerp(FadeInitial, FadeTarget, normalizedTime));

			if (normalizedTime < 1f) return;

			Finish();
		}

		public void Finish()
		{
			IsDone = true;
			FadeElapsed = 0f;
		}
	}
}
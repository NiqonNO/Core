using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace NiqonNO.Core.Audio.Logic
{
	public class NOSoundClipData : NOData
	{
		[field: SerializeField] 
		public int MaxInstances { get; private set; } = 8;
		
		[field: SerializeField]
		public AudioMixerGroup MixerGroup { get; private set; }

		[field: SerializeField] 
		public bool Loop { get; private set; }
		
		[field: SerializeField, Range(0,256)]
		public int Priority { get; private set; } = 128;
		
		[SerializeField, MinMaxSlider(0,1, true)]
		private Vector2 VolumeRange = Vector2.one;
		public float ResolveVolume() => Random.Range(VolumeRange.x, VolumeRange.y);
		
		[SerializeField, MinMaxSlider(-3,3, true)]
		private Vector2 PitchRange = Vector2.one;
		public float ResolvePitch() => Random.Range(PitchRange.x, PitchRange.y);
		
		[field: SerializeField, Range(-1,1)]
		public float StereoPan { get; private set; } = 0;
		
		[field: SerializeField, Range(0,1)]
		public float SpatialBlend { get; private set; } = 0;
		
		[field: SerializeField, Range(0,1.1f)]
		public float ReverbZoneMix { get; private set; } = 1;
		
		
		[field: SerializeField, BoxGroup("3D Sound Settings"), Range(0, 5)]
		public float DopplerLevel { get; private set; } = 1;
		
		[field: SerializeField, BoxGroup("3D Sound Settings"), Range(0, 360)]
		public float Spread { get; private set; } = 0;
		
		[field: SerializeField, BoxGroup("3D Sound Settings")] 
		public AudioRolloffMode VolumeRolloff { get; private set; }
		
		[field: SerializeField, BoxGroup("3D Sound Settings"), MaxValue(nameof(MaxDistance))]
		public float MinDistance { get; private set; } = 1;
		
		[field: SerializeField, BoxGroup("3D Sound Settings"), MinValue(nameof(MinDistance))]
		public float MaxDistance { get; private set; } = 500;
		
		[field: SerializeField, Min(0f)]
		public float DefaultFadeIn { get; private set; } = 0f;

		[field: SerializeField, Min(0f)]
		public float DefaultFadeOut { get; private set; } = 0f;

		[field: SerializeField, ShowIf(nameof(Loop)), BoxGroup("Advanced Loop")]
		public AudioClip StartClip { get; private set; }

		[field: SerializeField, ShowIf(nameof(Loop)), BoxGroup("Advanced Loop")]
		public AudioClip EndClip { get; private set; }
		public bool HasAdvancedLoop => Loop && (StartClip != null || EndClip != null);

		[SerializeField]
		private AudioClip[] Clips;
		public AudioClip GetClip() => Clips.Length == 0 ? null : Clips[Random.Range(0, Clips.Length)];
	}
}
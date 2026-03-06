using System;
using UnityEngine;

namespace NiqonNO.Core.Audio
{
	[Serializable]
	public class NOSoundPlaybackOptions
	{
		public float? Pitch;
		public float? Volume;
		public float FadeInDuration;
		public Vector3? WorldPosition;
	}
}
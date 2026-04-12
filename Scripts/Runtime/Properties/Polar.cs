using System;
using UnityEngine;

namespace NiqonNO.Core
{
	[Serializable]
	public struct Polar : IEquatable<Polar>
	{
		public static readonly Polar Zero = new(0f, 0f);

		public readonly float R;
		public readonly float T;

		public float Radians => T;
		public float Degrees => T * Mathf.Rad2Deg;
		
		public Polar(float r, float t)
		{
			R = Mathf.Clamp01(r);
			T = NormalizeAngle(t);
		}
		
		public static Polar FromRadians(float radius, float radians)
		{
			return new Polar(radius, radians);
		}

		public static Polar FromDegrees(float radius, float degrees)
		{
			return new Polar(radius, degrees * Mathf.Deg2Rad);
		}
		
		public Vector2 ToCartesian()
		{
			return new Vector2(
				R * MathF.Cos(T),
				R * MathF.Sin(T)
			);
		}

		public static Polar FromCartesian(Vector2 coord) => FromCartesian(coord.x, coord.y);
		public static Polar FromCartesian(float x, float y)
		{
			float r = MathF.Sqrt(x * x + y * y);
			float t = MathF.Atan2(y, x);
			return new Polar(r, t);
		}

		static float NormalizeAngle(float a)
		{
			float twoPi = MathF.PI * 2f;
			a %= twoPi;
			if (a < 0f) a += twoPi;
			return a;
		}
		
		public Vector2 ToVector2()
		{
			return new Vector3(R, T);
		}

		public static Polar FromVector2(Vector2 v)
		{
			return new Polar(v.x, v.y);
		}
		
		public override string ToString()
		{
			return $"({R}, {T})";
		}
		
		public static implicit operator Vector2(Polar b)
		{
			return new Vector2(b.R, b.T);
		}
		
		public bool Equals(Polar other)
		{
			return R.Equals(other.R) && T.Equals(other.T);
		}

		public override bool Equals(object obj)
		{
			return obj is Polar other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(R, T);
		}
	}
}
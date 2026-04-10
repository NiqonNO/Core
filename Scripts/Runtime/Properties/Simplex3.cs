using System;
using NiqonNO.Core.Utility;
using UnityEngine;

namespace NiqonNO.Core
{
	[Serializable]
	public struct Simplex3 : IEquatable<Simplex3>
	{
		private static readonly Vector2 LeftCorner = new(0.0f, 1.0f);
		private static readonly Vector2 TopCorner = new(0.5f, 0.0f);
		private static readonly Vector2 RightCorner = new(1.0f, 1.0f);
		
		public const float Sum = 1f;
		
		public static readonly Simplex3 Identity = new(Sum / 3f, Sum / 3f, Sum / 3f);
		public static readonly Simplex3 Left = new(Sum, 0f, 0f);
		public static readonly Simplex3 Top = new(0f, Sum, 0f);
		public static readonly Simplex3 Right = new(0f, 0f, Sum);

		public readonly float X;
		public readonly float Y;
		public readonly float Z;
		
		public Simplex3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public static Simplex3 FromX(float x)
		{
			var remaining = (Sum - x) * 0.5f;
			return new Simplex3(x, remaining, remaining);
		}
		public static Simplex3 FromY(float y)
		{
			var remaining = (Sum - y) * 0.5f;
			return new Simplex3(remaining, y, remaining);
		}
		public static Simplex3 FromZ(float z)
		{
			var remaining = (Sum - z) * 0.5f;
			return new Simplex3(remaining, remaining, z);
		}
		public static Simplex3 FromXY(float x, float y)
		{
			return new Simplex3(x, y, Sum - x - y);
		}
		public static Simplex3 FromXZ(float x, float z)
		{
			return new Simplex3(x, Sum - x - z, z);
		}
		public static Simplex3 FromYZ(float y, float z)
		{
			return new Simplex3(Sum - y - z, y, z);
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X, Y, Z);
		}

		public static Simplex3 FromVector3(Vector3 v)
		{
			return new Simplex3(v.x, v.y, v.z);
		}

		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}

		public static implicit operator Vector3(Simplex3 b)
		{
			return new Vector3(b.X, b.Y, b.Z);
		}
		
		public static Simplex3 FromCoordinates(Vector2 coordinates) =>
			FromCoordinates(coordinates, LeftCorner, TopCorner, RightCorner);
		public static Simplex3 FromCoordinates(Vector2 coordinates, Vector2 leftCorner, Vector2 topCorner, Vector2 rightCorner)
		{
			var v0 = leftCorner - topCorner;
			var v1 = rightCorner - topCorner;
			var v2 = coordinates - topCorner;

			var d00 = Vector2.Dot(v0, v0);
			var d01 = Vector2.Dot(v0, v1);
			var d11 = Vector2.Dot(v1, v1);
			var d20 = Vector2.Dot(v2, v0);
			var d21 = Vector2.Dot(v2, v1);

			var denom = d00 * d11 - d01 * d01;
			var x = (d11 * d20 - d01 * d21) / denom;
			var z = (d00 * d21 - d01 * d20) / denom;
			var y = 1.0f - x - z;

			if (x < 0)
			{
				var t = Vector2.Dot(coordinates - topCorner, rightCorner - topCorner) /
				        Vector2.Dot(rightCorner - topCorner, rightCorner - topCorner);
				t = Mathf.Clamp01(t);
				return FromYZ(1.0f - t, t);
			}

			if (y < 0)
			{
				var t = Vector2.Dot(coordinates - rightCorner, leftCorner - rightCorner) /
				        Vector2.Dot(leftCorner - rightCorner, leftCorner - rightCorner);
				t = Mathf.Clamp01(t);
				return FromXZ(t, 1.0f - t);
			}

			if (z < 0)
			{
				var t = Vector2.Dot(coordinates - leftCorner, topCorner - leftCorner) /
				        Vector2.Dot(topCorner - leftCorner, topCorner - leftCorner);
				t = Mathf.Clamp01(t);
				return FromXY(1.0f - t, t);
			}

			return new Simplex3(x, y, z);
		}
		public static Vector2 ToPosition(Simplex3 value) =>
			ToPosition(value, LeftCorner, TopCorner, RightCorner);
		public static Vector2 ToPosition(Simplex3 value, Vector2 leftCorner, Vector2 topCorner, Vector2 rightCorner)
		{
			return value.X * leftCorner +
			       value.Y * topCorner +
			       value.Z * rightCorner;
		}

		public static Vector3 Clamp(Vector3 barycentric, float minValue, float maxValue, BarycentricConstraint constraint)
		{
			return DenormalizeValue(
				Clamp01(
					NormalizeValue(
						barycentric, minValue, maxValue), 
					constraint), 
				minValue, maxValue);
		}
		
		public static Simplex3 Clamp01(Vector3 barycentric, BarycentricConstraint constraint)
		{
			barycentric.x = Mathf.Clamp01(barycentric.x);
			barycentric.y = Mathf.Clamp01(barycentric.y);
			barycentric.z = Mathf.Clamp01(barycentric.z);

			var currentSum = barycentric.x + barycentric.y + barycentric.z;
			return constraint switch
			{
				BarycentricConstraint.X => ClampConstrained(0),
				BarycentricConstraint.Y => ClampConstrained(1),
				BarycentricConstraint.Z => ClampConstrained(2),
				_ => currentSum == 0 ? Identity : FromVector3(barycentric / currentSum)
			};

			Simplex3 ClampConstrained(int axis)
			{
				var axisNext = (int)Mathf.Repeat(axis + 1, 3);
				var axisPrev = (int)Mathf.Repeat(axis + 2, 3);

				if (currentSum == 0)
				{
					var halfDelta = (1 - barycentric[axis]) / 2.0f;
					barycentric[axisNext] = halfDelta;
					barycentric[axisPrev] = halfDelta;
					return FromVector3(barycentric);
				}

				var sumDelta = 1 - currentSum;
				var sum = barycentric[axisNext] + barycentric[axisPrev];
				var delta = sumDelta * (sum > 0 ? barycentric[axisNext] / sum : 0.5f);

				barycentric[axisNext] = Mathf.Clamp(barycentric[axisNext] + delta, 0, 1 - barycentric[axis]);
				barycentric[axisPrev] = 1 - barycentric[axis] - barycentric[axisNext];
				return FromVector3(barycentric);
			}
		}
		
		public static Simplex3 NormalizeValue(Vector3 barycentric, float minValue, float maxValue) => FromVector3((barycentric - Vector3.one * minValue) / (maxValue - minValue));

		public static Vector3 DenormalizeValue(Simplex3 barycentric, float minValue, float maxValue) => Vector3.one * minValue + (Vector3)barycentric * (maxValue - minValue);

		public static Vector3 Round(Vector3 barycentric)
		{
			var floored = new Vector3Int(
				Mathf.FloorToInt(barycentric.x),
				Mathf.FloorToInt(barycentric.y),
				Mathf.FloorToInt(barycentric.z)
			);

			var targetSum = Mathf.RoundToInt(barycentric.x + barycentric.y + barycentric.z);
			var currentSum = floored.x + floored.y + floored.z;
			var delta = targetSum - currentSum;

			if (delta == 0)
				return floored;

			float fx = barycentric.x - floored.x;
			float fy = barycentric.y - floored.y;
			float fz = barycentric.z - floored.z;
			
			var frac = new (float f, int i)[] { (fx,0), (fy,1), (fz,2) };
			Array.Sort(frac, (a,b) => b.f.CompareTo(a.f));

			for (int i = 0; i < delta && i < 4; i++)
			{
				switch (frac[i].i)
				{
					case 0: floored.x++; break;
					case 1: floored.y++; break;
					case 2: floored.z++; break;
				}
			}

			return floored;
		}

		public bool Equals(Simplex3 other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

		public override bool Equals(object obj)
		{
			return obj is Simplex3 other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z);
		}
	}
}
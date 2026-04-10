using NiqonNO.Core.Utility;
using UnityEngine;

namespace NiqonNO.Core
{
	public struct Barycentric4
	{
		private static readonly Vector2 SquareBottomLeftCorner = new(0.0f, 1.0f);
		private static readonly Vector2 SquareTopLeftCorner = new(0.0f, 0.0f);
		private static readonly Vector2 SquareBottomRightCorner = new(1.0f, 1.0f);
		private static readonly Vector2 SquareTopRightCorner = new(1.0f, 0.0f);
		
		public const float Sum = 1f;
		
		public static readonly Barycentric4 Identity = new(Sum / 4f, Sum / 4f, Sum / 4f, Sum / 4f);
		public static readonly Barycentric4 LeftBot = new(Sum, 0f, 0f, 0f);
		public static readonly Barycentric4 LeftTop = new(0f, Sum, 0f, 0f);
		public static readonly Barycentric4 RightBot = new(0f, 0f, Sum, 0f);
		public static readonly Barycentric4 RightTop = new(0f, 0f, 0f, Sum);

		public readonly float X;
		public readonly float Y;
		public readonly float Z;
		public readonly float W;
		
		public Barycentric4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		public static Barycentric4 FromX(float x)
		{
			var remaining = (Sum - x) / 0.3f;
			return new Barycentric4(x, remaining, remaining, remaining);
		}
		public static Barycentric4 FromY(float y)
		{
			var remaining = (Sum - y) / 0.3f;
			return new Barycentric4(remaining, y, remaining, remaining);
		}
		public static Barycentric4 FromZ(float z)
		{
			var remaining = (Sum - z) / 0.3f;
			return new Barycentric4(remaining, remaining, z, remaining);
		}
		public static Barycentric4 FromW(float w)
		{
			var remaining = (Sum - w) / 0.3f;
			return new Barycentric4(remaining, remaining, remaining, w);
		}
		public static Barycentric4 FromXY(float x, float y)
		{
			var remaining = (Sum - x - y) * 0.5f;
			return new Barycentric4(x, y, remaining, remaining);
		}
		public static Barycentric4 FromXZ(float x, float z)
		{
			var remaining = (Sum - x - z) * 0.5f;
			return new Barycentric4(x, remaining, z, remaining);
		}
		public static Barycentric4 FromYZ(float y, float z)
		{
			var remaining = (Sum - y - z) * 0.5f;
			return new Barycentric4(remaining, remaining, y, z);
		}
		public static Barycentric4 FromXW(float x, float w)
		{
			var remaining = (Sum - x - w) * 0.5f;
			return new Barycentric4(x, remaining, remaining, w);
		}
		public static Barycentric4 FromYW(float y, float w)
		{
			var remaining = (Sum - y - w) * 0.5f;
			return new Barycentric4(remaining, y, remaining, w);
		}
		public static Barycentric4 FromZW(float z, float w)
		{
			var remaining = (Sum - z - w) * 0.5f;
			return new Barycentric4(remaining, remaining, z, w);
		}
		public static Barycentric4 FromXYZ(float x, float y, float z)
		{
			var remaining = (Sum - x - y - z);
			return new Barycentric4(x, y, z, remaining);
		}
		public static Barycentric4 FromYZW(float y, float z, float w)
		{
			var remaining = (Sum - y - z - w);
			return new Barycentric4(remaining, y, z, w);
		}
		public static Barycentric4 FromXZW(float x, float z, float w)
		{
			var remaining = (Sum - x - z - w);
			return new Barycentric4(x, remaining, z, w);
		}
		public static Barycentric4 FromXYW(float x, float y, float w)
		{
			var remaining = (Sum - x - y - w);
			return new Barycentric4(x, y, remaining, w);
		}

		public Vector4 ToVector4()
		{
			return new Vector4(X, Y, Z);
		}

		public static Barycentric4 FromVector4(Vector4 v)
		{
			return new Barycentric4(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}

		public static implicit operator Vector4(Barycentric4 b)
		{
			return new Vector4(b.X, b.Y, b.Z, b.W);
		}
		
		public static Barycentric4 FromCoordinates(Vector2 coordinates) =>
			FromCoordinates(coordinates, SquareBottomRightCorner, SquareTopLeftCorner, SquareBottomLeftCorner);
		public static Barycentric4 FromCoordinates(Vector2 coordinates, Vector2 br, Vector2 tl, Vector2 bl)
		{
			var right = br - bl;
			var up = tl - bl;
			var local = coordinates - bl;

			var denom = right.x * up.y - right.y * up.x;

			if (Mathf.Approximately(denom, 0))
				return Barycentric4.Identity;

			var u = (local.x * up.y - local.y * up.x) / denom;
			var v = (right.x * local.y - right.y * local.x) / denom;

			u = Mathf.Clamp01(u);
			v = Mathf.Clamp01(v);

			var x = (1 - u) * (1 - v);
			var y = (1 - u) * v;
			var z = u * (1 - v);
			var w = u * v;

			return new Barycentric4(x, y, z, w);
		}
		
		public static Vector2 ToPosition(Barycentric4 value) =>
			ToPosition(value, SquareBottomLeftCorner, SquareTopLeftCorner, SquareBottomRightCorner, SquareTopRightCorner);
		public static Vector2 ToPosition(Barycentric4 value, Vector2 bl, Vector2 tl, Vector2 br, Vector2 tr)
		{
			return value.X * bl +
			       value.Y * tl +
			       value.Z * br +
			       value.W * tr;
		}
		
		public static Vector4 Clamp(Vector4 barycentric, float minValue, float maxValue, BarycentricConstraint constraint)
		{
			return DenormalizeValue(
				Clamp01(
					NormalizeValue(
						barycentric, minValue, maxValue), 
					constraint), 
				minValue, maxValue);
		}
		
		public static Barycentric4 Clamp01(Vector4 barycentric, BarycentricConstraint constraint)
		{
			barycentric.x = Mathf.Clamp01(barycentric.x);
			barycentric.y = Mathf.Clamp01(barycentric.y);
			barycentric.z = Mathf.Clamp01(barycentric.z);
			barycentric.w = Mathf.Clamp01(barycentric.w);

			var currentSum = barycentric.x + barycentric.y + barycentric.z + barycentric.w;
			return constraint switch
			{
				BarycentricConstraint.X => ClampConstrained(0),
				BarycentricConstraint.Y => ClampConstrained(1),
				BarycentricConstraint.Z => ClampConstrained(2),
				BarycentricConstraint.W => ClampConstrained(3),
				_ =>  currentSum == 0 ? Barycentric4.Identity : Barycentric4.FromVector4(barycentric / currentSum),
			};

			Barycentric4 ClampConstrained(int axis)
			{
				int a = axis;
				int b = (axis + 1) % 4;
				int c = (axis + 2) % 4;
				int d = (axis + 3) % 4;
				
				if (currentSum == 0)
				{
					float share = (1 - barycentric[a]) / 3f;
					barycentric[b] = share;
					barycentric[c] = share;
					barycentric[d] = share;
					return Barycentric4.FromVector4(barycentric);
				}
				
				float sumOther = barycentric[b] + barycentric[c] + barycentric[d];
				float sumDelta = 1 - currentSum;

				float wb = sumOther > 0 ? barycentric[b] / sumOther : 1f / 3f;
				float wc = sumOther > 0 ? barycentric[c] / sumOther : 1f / 3f;
				float wd = sumOther > 0 ? barycentric[d] / sumOther : 1f / 3f;

				barycentric[b] += Mathf.Clamp01(sumDelta * wb);
				barycentric[c] += Mathf.Clamp01(sumDelta * wc);
				barycentric[d] += Mathf.Clamp01(sumDelta * wd);

				float remain = 1 - barycentric[a];
				float sumBC = barycentric[b] + barycentric[c] + barycentric[d];

				if (sumBC > 0)
				{
					float scale = remain / sumBC;
					barycentric[b] *= scale;
					barycentric[c] *= scale;
					barycentric[d] *= scale;
				}
				else
				{
					float share = remain / 3f;
					barycentric[b] = barycentric[c] = barycentric[d] = share;
				}

				return Barycentric4.FromVector4(barycentric);
			}
		}
		
		public static Barycentric4 NormalizeValue(Vector4 barycentric, float minValue, float maxValue) => Barycentric4.FromVector4((barycentric - Vector4.one * minValue) / (maxValue - minValue));

		public static Vector4 DenormalizeValue(Barycentric4 barycentric, float minValue, float maxValue) => Vector4.one * minValue + (Vector4)barycentric * (maxValue - minValue);

		public static Vector4 Round(Vector4 barycentric)
		{
			var floored = new Vector4Int(
				Mathf.FloorToInt(barycentric.x),
				Mathf.FloorToInt(barycentric.y),
				Mathf.FloorToInt(barycentric.z),
				Mathf.FloorToInt(barycentric.w)
			);

			var targetSum = Mathf.RoundToInt(barycentric.x + barycentric.y + barycentric.z + barycentric.w);
			var currentSum = floored.x + floored.y + floored.z + floored.w;
			var delta = targetSum - currentSum;

			if (delta == 0)
				return floored;

			float fx = barycentric.x - floored.x;
			float fy = barycentric.y - floored.y;
			float fz = barycentric.z - floored.z;
			float fw = barycentric.w - floored.w;
			
			var frac = new (float f, int i)[]
			{
				(fx,0),(fy,1),(fz,2),(fw,3)
			};

			System.Array.Sort(frac, (a,b) => b.f.CompareTo(a.f));

			for (int i = 0; i < delta && i < 4; i++)
			{
				switch (frac[i].i)
				{
					case 0: floored.x++; break;
					case 1: floored.y++; break;
					case 2: floored.z++; break;
					case 3: floored.w++; break;
				}
			}

			return floored;
		}
	}
}
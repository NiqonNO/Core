using UnityEngine;

namespace NiqonNO.Core.Utility
{
	public class NOBarycentricMath
	{
		public static Vector3 GetBarycentricFromPosition(Vector2 position, Vector2 leftCorner, Vector2 topCorner, Vector2 rightCorner)
		{
			var v0 = leftCorner - topCorner;
			var v1 = rightCorner - topCorner;
			var v2 = position - topCorner;

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
				var t = Vector2.Dot(position - topCorner, rightCorner - topCorner) /
				        Vector2.Dot(rightCorner - topCorner, rightCorner - topCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(0.0f, 1.0f - t, t);
			}

			if (y < 0)
			{
				var t = Vector2.Dot(position - rightCorner, leftCorner - rightCorner) /
				        Vector2.Dot(leftCorner - rightCorner, leftCorner - rightCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(t, 0.0f, 1.0f - t);
			}

			if (z < 0)
			{
				var t = Vector2.Dot(position - leftCorner, topCorner - leftCorner) /
				        Vector2.Dot(topCorner - leftCorner, topCorner - leftCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(1.0f - t, t, 0.0f);
			}

			return new Vector3(x, y, z);
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
		
		public static Vector3 Clamp01(Vector3 barycentric, BarycentricConstraint constraint)
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
				_ => currentSum == 0 ? Vector3.one / 3.0f : barycentric / currentSum
			};

			Vector3 ClampConstrained(int axis)
			{
				var axisNext = (int)Mathf.Repeat(axis + 1, 3);
				var axisPrev = (int)Mathf.Repeat(axis + 2, 3);

				if (currentSum == 0)
				{
					var halfDelta = (1 - barycentric[axis]) / 2.0f;
					barycentric[axisNext] = halfDelta;
					barycentric[axisPrev] = halfDelta;
					return barycentric;
				}

				var sumDelta = 1 - currentSum;
				var sum = barycentric[axisNext] + barycentric[axisPrev];
				var delta = sumDelta * (sum > 0 ? barycentric[axisNext] / sum : 0.5f);

				barycentric[axisNext] = Mathf.Clamp(barycentric[axisNext] + delta, 0, 1 - barycentric[axis]);
				barycentric[axisPrev] = 1 - barycentric[axis] - barycentric[axisNext];
				return barycentric;
			}
		}
		
		public static Vector3 NormalizeValue(Vector3 barycentric, float minValue, float maxValue) => (barycentric - Vector3.one * minValue) / (maxValue - minValue);

		public static Vector3 DenormalizeValue(Vector3 barycentric, float minValue, float maxValue) => Vector3.one * minValue + barycentric * (maxValue - minValue);

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

			var fracX = barycentric.x - floored.x;
			var fracY = barycentric.y - floored.y;
			var fracZ = barycentric.z - floored.z;

			var minF = Mathf.Min(fracX, Mathf.Min(fracY, fracZ));
			var maxF = Mathf.Max(fracX, Mathf.Max(fracY, fracZ));
			var midF = fracX + fracY + fracZ - minF - maxF;

			var threshold = delta == 1 ? maxF : midF;
			if (fracX >= threshold)
			{
				floored.x += 1;
				delta--;
			}

			if (fracY >= threshold && delta > 0)
			{
				floored.y += 1;
				delta--;
			}

			if (fracZ >= threshold && delta > 0) floored.z += 1;
			return floored;
		}
	}
}
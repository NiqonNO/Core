using UnityEngine;

namespace NiqonNO.Core.Utility
{
	public static class NOVector2Extensions
	{
		public static Vector2 Abs(this Vector2 vector)
		{
			return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		}

		public static Vector2 Clamp(this Vector2 vector, float minValue, float maxValue)
		{
			return new Vector2(
				Mathf.Clamp(vector.x, minValue, maxValue),
				Mathf.Clamp(vector.y, minValue, maxValue));
		}
	}
}
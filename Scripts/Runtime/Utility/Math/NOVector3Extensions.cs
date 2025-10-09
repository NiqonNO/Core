using UnityEngine;

namespace NiqonNO.Core.Utility
{
	public static class NOVector3Extensions
	{
		public static Vector3 Abs(this Vector3 vector)
		{
			return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
		}

		public static Vector3 Clamp(this Vector3 vector, float minValue, float maxValue)
		{
			return new Vector3(
				Mathf.Clamp(vector.x, minValue, maxValue),
				Mathf.Clamp(vector.y, minValue, maxValue),
				Mathf.Clamp(vector.z, minValue, maxValue));
		}
	}
}
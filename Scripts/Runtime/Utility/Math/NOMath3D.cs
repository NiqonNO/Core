using UnityEngine;

namespace NiqonNO.Core.Utility
{
	public static class NOMath3D
	{
		public static Vector3 Remap(Vector3 value, float inMin, float inMax, float outMin, float outMax)
		{
			return new Vector3(
				NOMath.Remap(value.x, inMin, inMax, outMin, outMax),
				NOMath.Remap(value.y, inMin, inMax, outMin, outMax),
				NOMath.Remap(value.z, inMin, inMax, outMin, outMax)
			);
		}
		
		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
		}
		
		public static float InverseLerpUnclamped(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
		}
	}
}
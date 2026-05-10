namespace NiqonNO.Core.Utility
{
	public static class NOMath
	{
		public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
		}
	}
}
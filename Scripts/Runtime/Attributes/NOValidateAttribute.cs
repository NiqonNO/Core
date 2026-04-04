using System;

namespace NiqonNO.Core
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class NOValidateAttribute : Attribute
	{
		public string MethodName { get; }

		public NOValidateAttribute(string methodName)
		{
			MethodName = methodName;
		}
	}
}
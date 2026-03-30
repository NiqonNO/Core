using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NiqonNO.Core
{
	public partial class NOContainer
	{
		private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();
		private static readonly NOContainerRegistry Registry = new();

		public static void ResetStaticState()
		{
			Registry.ResetState();
			FieldCache.Clear();
		}
		
		public static void EnqueueForInitialization(string scope, INOInitializable initializable) => Registry.EnqueueForInitialization(scope, initializable);
		
		private static FieldInfo[] GetInjectableFields(Type type)
		{
			if(FieldCache.TryGetValue(type, out var info)) return info;
			info = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(f => f.GetCustomAttribute<NOInjectAttribute>() != null)
				.ToArray();
			FieldCache.Add(type, info);
			return info;
		}
	}
}
using UnityEngine;

namespace NiqonNO.Core
{
	public class NOData : NOScriptableObject
	{
		[field: SerializeField]
		public string ID { get; private set; }
	}
}
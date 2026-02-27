using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
	public class NODataSet<TData> : NOScriptableObject 
		where TData : NOData
	{
		[field: SerializeField] 
		public List<TData> Assets { get; private set; }
	}
}
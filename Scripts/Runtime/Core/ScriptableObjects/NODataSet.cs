using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core
{
	public class NODataSet<TData> : NOScriptableObject 
		where TData : NOData
	{
		[field: SerializeField] 
		public List<TData> Assets { get; private set; }
		
#if UNITY_EDITOR
		[Sirenix.OdinInspector.Button]
		void GatherAll()
		{
			string typeName = typeof(TData).Name;
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");
			
			Assets.Clear();

			foreach (string guid in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				TData data = UnityEditor.AssetDatabase.LoadAssetAtPath<TData>(path);
				if (data != null)
					Assets.Add(data);
			}

			Debug.Log($"Found {guids.Length} assets of type {typeName}, assigned {Assets.Count} of them.");
		}
#endif
	}
}
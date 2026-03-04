using NiqonNO.Core;
using UnityEngine;

public class NOFactory
{
	private NOContainer Container;
	public NOFactory(NOContainer container)
	{
		Container = container;
	}

	public T CreateAsset<T>() where T : NOScriptableObject
	{
		T item = ScriptableObject.CreateInstance<T>();
		Container.Inject(item);
		return item;
	}
}
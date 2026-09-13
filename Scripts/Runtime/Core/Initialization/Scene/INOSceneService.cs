using UnityEngine.Events;

namespace NiqonNO.Core.Scene
{
	public interface INOSceneService : INOService
	{
		public void SwitchScene(string sceneToLoad, string sceneToUnload);
		public void LoadScene(string scene);
		public void UnloadScene(string scene);

		public void AddOnLoadingStartedEvent(UnityAction action);
		public void RemoveOnLoadingStartedEvent(UnityAction action);

		public void AddOnLoadingEndedEvent(UnityAction action);
		public void RemoveOnLoadingEndedEvent(UnityAction action);
	}
}
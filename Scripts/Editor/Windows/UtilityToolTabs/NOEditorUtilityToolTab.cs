using Sirenix.OdinInspector.Editor;

namespace NiqonNO.Core.Editor
{
	public abstract class NOEditorUtilityToolTab
	{
		public virtual string TabName => GetType().Name;
		public virtual int Order => 0;
		
		private PropertyTree Tree;

		public void DrawGUI()
		{
			Tree ??= PropertyTree.Create(this);
			Tree.UpdateTree();
			Tree.Draw(false);
		}
	}
}
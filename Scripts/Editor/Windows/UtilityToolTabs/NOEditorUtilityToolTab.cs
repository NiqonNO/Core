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
			
			Tree.UpdateTree();
			Tree.Draw(false);
		}

		public void Enable()
		{
			Tree = PropertyTree.Create(this);
		}
		public void Dispose()
		{
			Tree.Dispose();
			Tree = null;
		}
	}
}
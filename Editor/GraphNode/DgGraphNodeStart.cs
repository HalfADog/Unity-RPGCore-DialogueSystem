using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Start)]
	public class DgGraphNodeStart : DialogueGraphNode
	{
		public DgGraphNodeStart(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Output, false);
			capabilities -= Capabilities.Deletable;
			AddToClassList("start-node");
		}
	}
}

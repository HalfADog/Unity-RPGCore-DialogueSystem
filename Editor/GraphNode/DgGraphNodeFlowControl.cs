using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Flow)]
	public class DgGraphNodeFlowControl : DialogueGraphNode
	{
		public DgGraphNodeFlowControl(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Input, true);
			GeneratePort("True", Direction.Output, false);
			GeneratePort("False", Direction.Output, false);
			AddToClassList("flow-node");
		}
		public override void UpdataNode()
		{
			title = nodeData.Name;
		}
	}
}

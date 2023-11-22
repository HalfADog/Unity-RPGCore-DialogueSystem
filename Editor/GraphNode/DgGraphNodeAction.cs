using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Action)]
	public class DgGraphNodeAction : DialogueGraphNode
	{
		public DgGraphNodeAction(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Input, true);
			GeneratePort("", Direction.Output, false);
			AddToClassList("action-node");
		}

		public override void UpdataNode()
		{
			title = nodeData.Name;
		}
	}
}

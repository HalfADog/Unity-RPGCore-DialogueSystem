using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Random)]
	public class DgGraphNodeRandom : DialogueGraphNode
	{
		public DgGraphNodeRandom(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Input, true);
			GeneratePort("", Direction.Output, true);
			AddToClassList("random-node");
		}
		public override void UpdataNode()
		{
			title = nodeData.Name;
		}
	}
}

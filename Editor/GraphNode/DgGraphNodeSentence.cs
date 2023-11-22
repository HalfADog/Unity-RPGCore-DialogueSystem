using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Sentence)]
	public class DgGraphNodeSentence : DialogueGraphNode
	{
		private Label contentLabel;
		public DgGraphNodeSentence(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Input, true);
			GeneratePort("", Direction.Output, false);
			contentLabel = new Label();
			contentLabel.name = "sentence-label";
			outputContainer.Add(contentLabel);
			outputContainer.style.flexDirection = FlexDirection.RowReverse;
			contentLabel.AddToClassList("output-port-label");
			UpdataNode();
			AddToClassList("sentence-node");
		}

		public override void UpdataNode()
		{
			title = $"[{nodeData.Get<DgNodeSentence>().speaker.ToString()}] {nodeData.Name}";
			contentLabel.text = nodeData.Get<DgNodeSentence>().Content;
		}
	}
}

using RPGCore.Dialogue.Runtime;
using UnityEditor.Experimental.GraphView;

namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Choice)]
	public class DgGraphNodeChoice : DialogueGraphNode
	{
		public DgGraphNodeChoice(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}

		protected override void InitNode()
		{
			base.InitNode();
			GeneratePort("", Direction.Input, true);
			foreach (var choiceContent in (nodeData as DgNodeChoice).Choices)
			{
				GeneratePort(choiceContent, Direction.Output, false);
			}
			AddToClassList("choice-node");
		}

		public override void UpdataNode()
		{
			title = nodeData.Name;
			int i = 0;
			foreach (var choiceContent in (nodeData as DgNodeChoice).Choices)
			{
				if (i < outputPorts.Count)
				{
					outputPorts[i].portName = choiceContent;
				}
				else
				{
					GeneratePort(choiceContent, Direction.Output, false);
				}
				i++;
			}
			if (outputPorts.Count > i)
			{
				for (int j = i; j < outputPorts.Count; j++)
				{
					RemovePort(outputPorts[j]);
				}
				outputPorts.RemoveRange(i, outputPorts.Count - i);
			}
		}
	}
}
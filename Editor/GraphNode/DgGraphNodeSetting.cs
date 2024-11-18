using RPGCore.Dialogue.Editor;
using RPGCore.Dialogue.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPGCore.Dialogue.Editor
{
	[DialogueGraphNode(Type = DgNodeType.Setting)]
	public class DgGraphNodeSetting : DialogueGraphNode
	{
		public DgGraphNodeSetting(DgNodeBase nodeData, DialogueEditorWindow window) : base(nodeData, window)
		{
		}
		protected override void InitNode()
		{
			base.InitNode();
			AddToClassList("setting-node");
			RefreshPorts();
			RefreshExpandedState();
		}
	}
}

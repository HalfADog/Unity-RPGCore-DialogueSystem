using RPGCore.Dialogue.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	public class DialogueGraphNode : Node
	{
		public DgNodeBase nodeData { get; private set; }
		public Vector2 position { get; private set; }
		public DialogueEditorWindow editorWindow { get; private set; }
		public Port inputPort;
		public List<Port> outputPorts;

		public DialogueGraphNode(DgNodeBase nodeData, DialogueEditorWindow window)
		//:base("Assets/Scripts/DialogueSystem/Editor/UI/DialogueGraphNode.uxml")
		{
			//styleSheets.Clear();
			//AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/DialogueSystem/Editor/UI/DialogueGraphNode.uss")
			styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphNode"));
			this.nodeData = nodeData;
			this.editorWindow = window;
			outputPorts = new List<Port>();
			InitNode();
		}

		public Port GeneratePort(string portName, Direction direction, bool multiConnection = true)
		{
			Port port = null;
			if (direction == Direction.Input)
			{
				inputPort = InstantiatePort(
					Orientation.Horizontal,
					Direction.Input,
					multiConnection ? Port.Capacity.Multi : Port.Capacity.Single,
					typeof(bool)
				);
				port = inputPort;
				inputPort.name = "input-port";
				inputContainer.Add(inputPort);
			}
			if (direction == Direction.Output)
			{
				var oPort = InstantiatePort(
					Orientation.Horizontal,
					Direction.Output,
					multiConnection ? Port.Capacity.Multi : Port.Capacity.Single,
					typeof(bool)
				);
				port = oPort;
				oPort.name = "output-port";
				oPort.style.unityFontStyleAndWeight = FontStyle.Bold;
				outputPorts.Add(oPort);
				outputContainer.Add(oPort);
			}
			port.portName = portName;
			RefreshExpandedState();
			RefreshPorts();
			return port;
		}

		protected void RemovePort(Port generatePort)
		{
			var targetEdge = editorWindow.graphView.edges
				.ToList()
				.Where(x => x.output.node == generatePort.node
						&& outputContainer
							.Query<Port>()
							.ToList()
							.FindIndex(match =>
							{
								return x.output == generatePort;
							}) != -1
				);
			if (!targetEdge.Any())
			{
				outputContainer.Remove(generatePort);
				RefreshPorts();
				RefreshExpandedState();
				return;
			}
			var edge = targetEdge.First();
			edge.input.Disconnect(edge);
			editorWindow.graphView.RemoveElement(targetEdge.First());

			outputContainer.Remove(generatePort);
			RefreshPorts();
			RefreshExpandedState();
		}

		public override void OnSelected()
		{
			base.OnSelected();
			editorWindow.CurrentSelectedNode = null;
			editorWindow.CurrentSelectedNode = this;
			//LogElementName(this, 1);
			//Debug.Log(titleContainer.name);
		}
		protected virtual void InitNode()
		{
			title = nodeData.Name;
		}
		public override void OnUnselected()
		{
			base.OnUnselected();
			UpdataNode();
			editorWindow.CurrentSelectedNode = null;
		}
		public virtual void UpdataNode()
		{
		}

		private void LogElementName(VisualElement element, int layer)
		{
			var elements = element.Children();
			foreach (var item in elements)
			{
				string l = "";
				for (int i = 0; i < layer; i++)
				{
					l += "----";
				}
				l += "|";
				Debug.Log($"{l} [{item.name}]");
				LogElementName(item, layer + 1);
			}
		}
	}
}

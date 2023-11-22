using RPGCore.Dialogue.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	public class DialogueEditorGraphView : GraphView
	{
		private DialogueEditorWindow editorWindow;
		private DialogueEditorSearchWindow searchWindow;

		public DialogueEditorGraphView(DialogueEditorWindow window)
		{
			this.editorWindow = window;
			//AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/DialogueSystem/Editor/UI/DialogueGraphView.uss")
			styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphView"));
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			Insert(0, new GridBackground());
			MakeSearchTree();
		}

		private void MakeSearchTree()
		{
			searchWindow = ScriptableObject.CreateInstance<DialogueEditorSearchWindow>();
			searchWindow.Init(editorWindow, this);
			nodeCreationRequest = context =>
			{
				if (editorWindow.CanEditor)
					SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
			};
		}

		//连接两个节点时调用 获取到当前节点端口能够连接到的其余节点端口
		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> compatiblePorts = new List<Port>();
			ports.ForEach(
				(port) =>
				{
					if (startPort.node != port.node && //不能自己连接自己
						startPort.direction != port.direction)//不能input连input output连output
					{
						compatiblePorts.Add(port);
					}
				}
			);
			return compatiblePorts;
		}

		public DialogueGraphNode MakeNode(DgNodeBase dgNode, Vector2 position)
		{
			DialogueGraphNode graphNode = GenerateGraphNode(dgNode, editorWindow);
			graphNode.SetPosition(new Rect(position, graphNode.GetPosition().size));
			AddElement(graphNode);
			return graphNode;
		}

		public Edge MakeEdge(Port oput, Port iput)
		{
			var edge = new Edge { output = oput, input = iput };
			edge?.input.Connect(edge);
			edge?.output.Connect(edge);
			AddElement(edge);
			return edge;
		}

		public Edge MakeEdge(DialogueGraphNode outputNode, DialogueGraphNode inputNode, int outputPortIndex = 0)
		{
			return MakeEdge(outputNode?.outputPorts[outputPortIndex], inputNode?.inputPort);
		}

		private DialogueGraphNode GenerateGraphNode(DgNodeBase nodeData, DialogueEditorWindow editorWindow)
		{
			List<Type> graphNodeTypes = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.GetName().Name.Contains("Assembly")))
			{
				List<Type> types = assembly.GetTypes().Where(type =>
				{
					return type.IsClass && !type.IsAbstract && type.GetCustomAttribute<DialogueGraphNodeAttribute>() != null;
				}).ToList();
				graphNodeTypes.AddRange(types);
			}
			foreach (var graphNodeType in graphNodeTypes)
			{
				if (graphNodeType.GetCustomAttribute<DialogueGraphNodeAttribute>().Type == nodeData.Type)
				{
					return Activator.CreateInstance(graphNodeType, args: new object[] { nodeData, editorWindow }) as DialogueGraphNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 生成节点视图
		/// </summary>
		/// <param name="itemData"></param>
		public void GenerateNodeGraphView(DialogueItemDataSO itemData)
		{
			if (itemData == null)
			{
				return;
			}
			//生成默认的节点
			if (itemData.dgNodes.Count == 0)
			{
				var start = MakeNode(DialogueEditorUtility.CreateDialogueNodeData<DgNodeStart>(editorWindow.CurrentOpenedGroupData), new Vector2(350, 300));
				var end = MakeNode(DialogueEditorUtility.CreateDialogueNodeData<DgNodeEnd>(editorWindow.CurrentOpenedGroupData), new Vector2(500, 300));
				MakeEdge(start, end);
				return;
			}
			//生成节点
			List<DgNodeBase> nullNodes = new List<DgNodeBase>();
			foreach (var node in itemData.dgNodes)
			{
				if(node==null)nullNodes.Add(node);
				else MakeNode(node, node.graphViewPosition);
			}
			//删除莫名其妙出现的空节点 问题可能出现在SaveNodeGraphView中
			foreach (var node in nullNodes) 
			{
				itemData.dgNodes.Remove(node);
			}
			//连接节点
			var graphNodeList = nodes.Select(node => node as DialogueGraphNode).ToList();
			//var dataNodeList = graphNodeList.Select(node=>node.nodeData).ToList();
			foreach (var node in graphNodeList)
			{
				Port outputPort = null;
				Port inputPort = null;
				int outputPortIndex = 0;
				foreach (var guid in node.nodeData.nextNodesGuid)
				{
					if (!string.IsNullOrWhiteSpace(guid))
					{
						var nextgNode = graphNodeList.Find(node => node.nodeData.Guid == guid);
						outputPort = node.outputPorts[outputPortIndex];
						inputPort = nextgNode.inputPort;
						MakeEdge(outputPort, inputPort);
					}
					if (node.outputPorts.Count - 1 > outputPortIndex)
					{
						outputPortIndex++;
					}
				}
			}
			UpdateViewTransform(itemData.GraphViewPortPosition, itemData.GraphViewPortScale);
		}

		public void SaveNodeGraphView()
		{
			if (editorWindow.CurrentOpenedGroupData == null) return;
			//将移除的节点从资源中删除
			List<DgNodeBase> nodesToRemove = new List<DgNodeBase>();
			foreach (var node in editorWindow.CurrentOpenedGroupData.GetOpenedEditorItem().dgNodes)
			{
				if (!nodes.Select(node => (node as DialogueGraphNode).nodeData).Contains(node))
				{
					nodesToRemove.Add(node);
				}
			}
			foreach (var rnode in nodesToRemove)
			{
				DialogueEditorUtility.DeleteDialogueNodeData(rnode);
				editorWindow.CurrentOpenedGroupData.GetOpenedEditorItem().dgNodes.Remove(rnode);
			}
			//保存位置并清空链接关系
			foreach (var node in nodes.Select(node => node as DialogueGraphNode))
			{
				node.nodeData.graphViewPosition = node.GetPosition().position;
				node.nodeData.nextNodesGuid.Clear();
			}
			//保存链接信息
			foreach (var edge in edges.ToList())
			{
				if (edge.output == null || edge.input == null) break;
				var outputNode = edge.output.node as DialogueGraphNode;
				var inputNode = edge.input.node as DialogueGraphNode;
				int outputPortIndex = outputNode.outputPorts.FindIndex(port => port.portName == edge.output.portName);
				if (edge.output.capacity == Port.Capacity.Multi)
				{
					outputNode.nodeData.nextNodesGuid.Add(inputNode.nodeData.Guid);
				}
				else
				{
					if (outputPortIndex > outputNode.nodeData.nextNodesGuid.Count - 1)
					{
						int count = outputPortIndex - outputNode.nodeData.nextNodesGuid.Count + 1;
						outputNode.nodeData.nextNodesGuid.AddRange(new string[count]);
					}
					outputNode.nodeData.nextNodesGuid[outputPortIndex] = inputNode.nodeData.Guid;
				}
			}
			//保存当前graphview的位置缩放信息
			editorWindow.CurrentOpenedGroupData.GetOpenedEditorItem().SaveGraphViewPortInfomation(viewTransform.position, viewTransform.scale);
		}

		public void ClearGraphView()
		{
			foreach (var node in nodes)
			{
				RemoveElement(node);
			}
			foreach (var edge in edges)
			{
				RemoveElement(edge);
			}
		}
	}
}

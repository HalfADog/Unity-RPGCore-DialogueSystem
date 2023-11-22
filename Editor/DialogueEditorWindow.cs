using RPGCore.Dialogue.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	public class DialogueEditorWindow : EditorWindow
	{
		private static DialogueEditorWindow windowInstance;
		//界面上的各个控件
		private VisualElement graphViewPanel;
		private VisualElement inspectorPanel;
		private VisualElement remindPanel;
		private VisualElement dialogueWindowPanel;
		private ToolbarButton saveBtn;
		private ToolbarButton openBtn;
		private ObjectField openedGroupField;
		private ToolbarButton createGroupBtn;
		private ToolbarButton createItemBtn;
		private Label editInfomationLabel;
		private ListView groupItemHierarchyListView;

		//当前打开的对话group
		private static DialogueGroupDataSO currentOpenedGroupData = null;
		public DialogueGroupDataSO CurrentOpenedGroupData
		{
			get { return currentOpenedGroupData; }
			set { currentOpenedGroupData = value; }
		}
		//当前界面上的graphview
		public DialogueEditorGraphView graphView { get; private set; }
		//是否可以编辑 如果当前没有资源被打开则不能编辑
		public bool CanEditor => currentOpenedGroupData != null;
		//当前选中的对话节点
		private DialogueGraphNode currentSelectedNode;
		public DialogueGraphNode CurrentSelectedNode
		{
			get => currentSelectedNode;
			set
			{
				currentSelectedNode = value;
				UpdateNodeInspector(currentSelectedNode?.nodeData);
			}
		}
		//对话窗口
		private EditorDialogueWindow dialogueWindow;

		//保存
		private bool isSaved = false;
		//private
		[OnOpenAsset(1)]
		public static bool OpenWindowWithAsset(int instanceId, int line)
		{
			UnityEngine.Object openAsset = EditorUtility.InstanceIDToObject(instanceId);
			if (openAsset is DialogueGroupDataSO)
			{
				if (windowInstance != null)
				{
					if (currentOpenedGroupData.name != (openAsset as DialogueGroupDataSO).name)
					{
						windowInstance.graphView.SaveNodeGraphView();
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						windowInstance.CurrentOpenedGroupData = openAsset as DialogueGroupDataSO;
						windowInstance.UpdateEditorWindow(false);
					}
				}
				else
				{
					currentOpenedGroupData = openAsset as DialogueGroupDataSO;
					windowInstance = GetWindow<DialogueEditorWindow>();
					windowInstance.titleContent = new GUIContent("Dialogue Editor Window", Resources.Load<Texture>("DialogueGroupIcon"));
					windowInstance.minSize = new Vector2(800, 600);
				}
			}
			return false;
		}

		[MenuItem("Tools/Dialogue Editor Window")]
		public static void OpenWindow()
		{
			windowInstance = GetWindow<DialogueEditorWindow>();
			windowInstance.titleContent = new GUIContent("Dialogue Editor Window", Resources.Load<Texture>("DialogueGroupIcon"));
			windowInstance.minSize = new Vector2(800, 600);
		}

		private void OnEnable()
		{
			Debug.Log("Dialogue Editor Window Open");
			//加载主界面
			//VisualTreeAsset editorVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
			//		"Assets/Scripts/DialogueSystem/Editor/Resources/DialogueEditorWindow.uxml"
			//);
			VisualTreeAsset editorVisualTree = Resources.Load<VisualTreeAsset>("DialogueEditorWindow");
			TemplateContainer editorVisualTreeInstance = editorVisualTree.CloneTree();
			editorVisualTreeInstance.StretchToParentSize();
			rootVisualElement.Add(editorVisualTreeInstance);
			//获取界面主要元素
			inspectorPanel = rootVisualElement.Q<VisualElement>("InspectorPanel");
			graphViewPanel = rootVisualElement.Q<VisualElement>("GraphViewPanel");
			remindPanel = rootVisualElement.Q<VisualElement>("RemindPanel");
			dialogueWindowPanel = rootVisualElement.Q<VisualElement>("DialogueWindowPanel");
			saveBtn = rootVisualElement.Q<ToolbarButton>("Save");
			openBtn = rootVisualElement.Q<ToolbarButton>("Open");
			openedGroupField = rootVisualElement.Q<ObjectField>("OpenedGroup");
			createGroupBtn = rootVisualElement.Q<ToolbarButton>("CreateGroup");
			createItemBtn = rootVisualElement.Q<ToolbarButton>("CreateItem");
			editInfomationLabel = rootVisualElement.Q<Label>("EditInfomation");
			groupItemHierarchyListView = rootVisualElement.Q<ListView>("HierarchyPanel");
			groupItemHierarchyListView.makeItem = () =>
			{
				var item = new EditorHierarchyItem();
				return item;
			};
			groupItemHierarchyListView.bindItem = (item, index) =>
			{
				if (index < currentOpenedGroupData.dialogueItems.Count)
				{
					(item as EditorHierarchyItem).ItemNameLabel.text = currentOpenedGroupData.dialogueItems[index].name.Split("-").Last();
					(item as EditorHierarchyItem).name = (item as EditorHierarchyItem).ItemNameLabel.text;
					(item as EditorHierarchyItem).DeleteItemBtn.RegisterCallback<ClickEvent>(ce => { DeleteItem(currentOpenedGroupData.dialogueItems[index]); });
				}
			};
			groupItemHierarchyListView.fixedItemHeight = 24;
			groupItemHierarchyListView.onSelectionChange += OnDialogueItemSelectionChange;
			//对话框
			dialogueWindow = new EditorDialogueWindow(dialogueWindowPanel, "DialogueWindow");
			//设置控件回调函数
			saveBtn.RegisterCallback<ClickEvent>(ce => { graphView.SaveNodeGraphView(); AssetDatabase.SaveAssets(); });
			openBtn.RegisterCallback<ClickEvent>(ce => { OpenGroup(); });
			createGroupBtn.RegisterCallback<ClickEvent>(ce => { CreateNewGroup(); });
			createItemBtn.RegisterCallback<ClickEvent>(ce => { CreateNewItem(); });
			//加载GraphView
			graphView = new DialogueEditorGraphView(this);
			graphView.StretchToParentSize();
			graphViewPanel.Add(graphView);

			//初始化editor
			InitEditorWindow();
		}

		private void OnInspectorUpdate()
		{
			if (currentOpenedGroupData != null)
			{
				string itemDataName = currentOpenedGroupData.GetOpenedEditorItem().name.Split("-").Last();
				//更新高亮显示当前打开的item 这句话放在OnEnable里会出问题 可能是因为当时Hierarchy中还不存在任何VisualElement
				groupItemHierarchyListView.Q<EditorHierarchyItem>(itemDataName)?.OnSelected();
				editInfomationLabel.text = $"[{currentOpenedGroupData.name}] -> [{itemDataName.Split("-").Last()}]";
				//自动保存
				AutoSave();
			}
		}

		private void OnDisable()
		{
			graphView.SaveNodeGraphView();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			windowInstance = null;
		}

		/// <summary>
		/// 初始化编辑器
		/// </summary>
		private void InitEditorWindow()
		{
			if (currentOpenedGroupData == null)
			{
				remindPanel.style.visibility = Visibility.Visible;
				return;
			}
			remindPanel.style.visibility = Visibility.Hidden;
			//生成对话节点视图
			DialogueItemDataSO itemData = currentOpenedGroupData.GetOpenedEditorItem();
			graphView.GenerateNodeGraphView(itemData);
			graphView.UpdateViewTransform(itemData.GraphViewPortPosition, itemData.GraphViewPortScale);
			//更新hierarchy视图
			UpdateHierarchyView();
		}

		/// <summary>
		/// 更新Inspector视图
		/// </summary>
		/// <param name="nodeData"></param>
		public void UpdateNodeInspector(DgNodeBase nodeData)
		{
			if (nodeData == null)
			{
				inspectorPanel.Clear();
				return;
			}
			//获取到当前节点中所有序列化数据
			SerializedObject serializedNode = new SerializedObject(nodeData);
			SerializedProperty nodeProperty = serializedNode.GetIterator();
			nodeProperty.Next(true);
			string[] ignorePropertiesName = { "m_Script", "guid", "nextNodesGuid", "graphViewPosition" };
			//遍历所有序列化数据
			while (nodeProperty.NextVisible(false))
			{
				if (ignorePropertiesName.Contains(nodeProperty.name)) continue;

				EditorInspectorItem inspectorItem = new EditorInspectorItem();
				inspectorItem.FieldNameLabel.text = nodeProperty.name;
				inspectorItem.Field.bindingPath = nodeProperty.propertyPath;
				inspectorItem.Field.SetEnabled(nodeProperty.name != "type");
				inspectorItem.Bind(serializedNode);
				inspectorPanel.Add(inspectorItem);
			}
		}


		/// <summary>
		/// 更新整个窗口的显示
		/// </summary>
		public void UpdateEditorWindow(bool SaveNodeBeforUpdate = true)
		{
			if (currentOpenedGroupData == null)
			{
				remindPanel.style.visibility = Visibility.Visible;
				return;
			}
			remindPanel.style.visibility = Visibility.Hidden;
			if (SaveNodeBeforUpdate) graphView.SaveNodeGraphView();
			graphView.ClearGraphView();
			//生成对话节点视图
			graphView.GenerateNodeGraphView(currentOpenedGroupData.GetOpenedEditorItem());
			//更新hierarchy视图
			UpdateHierarchyView();
			//更新inspector视图
			UpdateNodeInspector(null);
		}

		/// <summary>
		/// 更新hierarchy界面
		/// </summary>
		private void UpdateHierarchyView()
		{
			//更新时要重新绑定itemsource 这很重要
			groupItemHierarchyListView.itemsSource = currentOpenedGroupData.dialogueItems;
			groupItemHierarchyListView.Rebuild();
			int selectedIndex = currentOpenedGroupData.dialogueItems.IndexOf(currentOpenedGroupData.GetOpenedEditorItem());
			groupItemHierarchyListView.SetSelectionWithoutNotify(new int[] { selectedIndex });
		}

		/// <summary>
		/// 创建一个新的Group并打开
		/// </summary>
		/// <param name="newGroupName"></param>
		/// <param name="description"></param>
		public void CreateNewGroup()
		{
			//新建group
			dialogueWindow.OpenDialogue(
				"Create New Group",
				EditorDialogueType.CreateAsset,
				ok =>
				{
					string name = dialogueWindow.GetCreateAssetName();
					string description = dialogueWindow.GetCreateAssetDescription();
					if (string.IsNullOrWhiteSpace(name) || DialogueEditorUtility.DialogueGroupExist(name))
					{
						dialogueWindow.CloseDialogue();
						dialogueWindow.OpenDialogue(
							"Create Group Failed",
							EditorDialogueType.Message,
							ok => { dialogueWindow.CloseDialogue(); },
							cancel => { dialogueWindow.CloseDialogue(); },
							$"Asset {name} has already exist."
						);
					}
					else
					{
						currentOpenedGroupData = DialogueEditorUtility.CreateDialogueGroupData(name, description);
						UpdateEditorWindow();
						dialogueWindow.CloseDialogue();
					}
				},
				cancel =>
				{
					dialogueWindow.CloseDialogue();
				}
			);
		}

		/// <summary>
		/// 在当前Group中创建一个新的Item并打开
		/// </summary>
		public void CreateNewItem()
		{
			dialogueWindow.OpenDialogue(
				"Create New Item",
				EditorDialogueType.CreateAsset,
				ok =>
				{
					string name = dialogueWindow.GetCreateAssetName();
					string description = dialogueWindow.GetCreateAssetDescription();
					if (string.IsNullOrWhiteSpace(name) || DialogueEditorUtility.DialogueItemExists(currentOpenedGroupData, name))
					{
						dialogueWindow.CloseDialogue();
						dialogueWindow.OpenDialogue(
							"Create Item Failed",
							EditorDialogueType.Message,
							ok => { dialogueWindow.CloseDialogue(); },
							cancel => { dialogueWindow.CloseDialogue(); },
							$"Asset {name} has already exist."
						);
					}
					else
					{
						DialogueItemDataSO itemData = DialogueEditorUtility.CreateDialogueItemData(currentOpenedGroupData, name, description); ;
						currentOpenedGroupData.SetOpenedEditorItem(itemData.name);
						UpdateEditorWindow();
						dialogueWindow.CloseDialogue();
					}
				},
				cancel =>
				{
					dialogueWindow.CloseDialogue();
				}
			);
		}

		public void DeleteItem(DialogueItemDataSO itemToDelete)
		{
			if (currentOpenedGroupData.dialogueItems.Count == 1)
			{
				dialogueWindow.OpenDialogue(
					"Attention",
					EditorDialogueType.Message,
					ok => { dialogueWindow.CloseDialogue(); },
					cancel => { dialogueWindow.CloseDialogue(); },
					$"Each group must have at least one Dialogue Item."
				);
				return;
			}
			dialogueWindow.OpenDialogue(
				"Confirm",
				EditorDialogueType.Message,
				ok =>
				{
					bool deletedIsCurrent = currentOpenedGroupData.GetOpenedEditorItem().name == itemToDelete.name;
					DialogueEditorUtility.DeleteDialogueItemData(currentOpenedGroupData, itemToDelete);
					//如果当前删除的item是当前打开的
					if (deletedIsCurrent)
					{
						currentOpenedGroupData.SetOpenedEditorItem(currentOpenedGroupData.dialogueItems.FirstOrDefault().name);
						//这里不执行保存操作
						UpdateEditorWindow(false);
					}
					else
					{
						UpdateEditorWindow();
					}
					dialogueWindow.CloseDialogue();
				},
				cancel => { dialogueWindow.CloseDialogue(); },
				$"Do you want to delete this Dialogue Item?"
			);
		}

		public void OpenGroup()
		{
			string openedPath = EditorUtility.OpenFilePanelWithFilters("Select Group", "", new string[] { "DialogueGroupDataSO", "asset" });
			Debug.Log(openedPath);
		}

		/// <summary>
		/// 当当前选中的DialogueItem改变时调用
		/// </summary>
		/// <param name="selectedItems"></param>
		public void OnDialogueItemSelectionChange(IEnumerable<object> selectedItems)
		{
			DialogueItemDataSO itemData = selectedItems.FirstOrDefault() as DialogueItemDataSO;
			DialogueItemDataSO previousItem = currentOpenedGroupData.GetOpenedEditorItem();
			if (itemData != null && itemData.name != previousItem.name)
			{
				//Debug.Log($"Change opened item to {itemData.name}");
				graphView.SaveNodeGraphView();
				graphView.ClearGraphView();
				//生成对话节点视图
				groupItemHierarchyListView.Q<EditorHierarchyItem>(previousItem.name.Split("-").Last())?.DeSelected();
				currentOpenedGroupData.SetOpenedEditorItem(itemData.name);
				graphView.GenerateNodeGraphView(currentOpenedGroupData.GetOpenedEditorItem());
				groupItemHierarchyListView.Q<EditorHierarchyItem>(itemData.name.Split("-").Last()).OnSelected();
				//更新label名称显示
				editInfomationLabel.text = $"[{currentOpenedGroupData.name}] -> [{currentOpenedGroupData.GetOpenedEditorItem().name.Split("-").Last()}]";
			}
		}

		/// <summary>
		/// 自动保存 每10s保存一次
		/// </summary>
		public void AutoSave()
		{
			if (System.DateTime.Now.Second % 10 == 0)
			{
				if (!isSaved)
				{
					graphView.SaveNodeGraphView();
					isSaved = true;
				}
			}
			else
			{
				isSaved = false;
			}
		}
	}
}

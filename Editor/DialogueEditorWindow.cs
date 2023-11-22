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
		//�����ϵĸ����ؼ�
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

		//��ǰ�򿪵ĶԻ�group
		private static DialogueGroupDataSO currentOpenedGroupData = null;
		public DialogueGroupDataSO CurrentOpenedGroupData
		{
			get { return currentOpenedGroupData; }
			set { currentOpenedGroupData = value; }
		}
		//��ǰ�����ϵ�graphview
		public DialogueEditorGraphView graphView { get; private set; }
		//�Ƿ���Ա༭ �����ǰû����Դ�������ܱ༭
		public bool CanEditor => currentOpenedGroupData != null;
		//��ǰѡ�еĶԻ��ڵ�
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
		//�Ի�����
		private EditorDialogueWindow dialogueWindow;

		//����
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
			//����������
			//VisualTreeAsset editorVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
			//		"Assets/Scripts/DialogueSystem/Editor/Resources/DialogueEditorWindow.uxml"
			//);
			VisualTreeAsset editorVisualTree = Resources.Load<VisualTreeAsset>("DialogueEditorWindow");
			TemplateContainer editorVisualTreeInstance = editorVisualTree.CloneTree();
			editorVisualTreeInstance.StretchToParentSize();
			rootVisualElement.Add(editorVisualTreeInstance);
			//��ȡ������ҪԪ��
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
			//�Ի���
			dialogueWindow = new EditorDialogueWindow(dialogueWindowPanel, "DialogueWindow");
			//���ÿؼ��ص�����
			saveBtn.RegisterCallback<ClickEvent>(ce => { graphView.SaveNodeGraphView(); AssetDatabase.SaveAssets(); });
			openBtn.RegisterCallback<ClickEvent>(ce => { OpenGroup(); });
			createGroupBtn.RegisterCallback<ClickEvent>(ce => { CreateNewGroup(); });
			createItemBtn.RegisterCallback<ClickEvent>(ce => { CreateNewItem(); });
			//����GraphView
			graphView = new DialogueEditorGraphView(this);
			graphView.StretchToParentSize();
			graphViewPanel.Add(graphView);

			//��ʼ��editor
			InitEditorWindow();
		}

		private void OnInspectorUpdate()
		{
			if (currentOpenedGroupData != null)
			{
				string itemDataName = currentOpenedGroupData.GetOpenedEditorItem().name.Split("-").Last();
				//���¸�����ʾ��ǰ�򿪵�item ��仰����OnEnable�������� ��������Ϊ��ʱHierarchy�л��������κ�VisualElement
				groupItemHierarchyListView.Q<EditorHierarchyItem>(itemDataName)?.OnSelected();
				editInfomationLabel.text = $"[{currentOpenedGroupData.name}] -> [{itemDataName.Split("-").Last()}]";
				//�Զ�����
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
		/// ��ʼ���༭��
		/// </summary>
		private void InitEditorWindow()
		{
			if (currentOpenedGroupData == null)
			{
				remindPanel.style.visibility = Visibility.Visible;
				return;
			}
			remindPanel.style.visibility = Visibility.Hidden;
			//���ɶԻ��ڵ���ͼ
			DialogueItemDataSO itemData = currentOpenedGroupData.GetOpenedEditorItem();
			graphView.GenerateNodeGraphView(itemData);
			graphView.UpdateViewTransform(itemData.GraphViewPortPosition, itemData.GraphViewPortScale);
			//����hierarchy��ͼ
			UpdateHierarchyView();
		}

		/// <summary>
		/// ����Inspector��ͼ
		/// </summary>
		/// <param name="nodeData"></param>
		public void UpdateNodeInspector(DgNodeBase nodeData)
		{
			if (nodeData == null)
			{
				inspectorPanel.Clear();
				return;
			}
			//��ȡ����ǰ�ڵ����������л�����
			SerializedObject serializedNode = new SerializedObject(nodeData);
			SerializedProperty nodeProperty = serializedNode.GetIterator();
			nodeProperty.Next(true);
			string[] ignorePropertiesName = { "m_Script", "guid", "nextNodesGuid", "graphViewPosition" };
			//�����������л�����
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
		/// �����������ڵ���ʾ
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
			//���ɶԻ��ڵ���ͼ
			graphView.GenerateNodeGraphView(currentOpenedGroupData.GetOpenedEditorItem());
			//����hierarchy��ͼ
			UpdateHierarchyView();
			//����inspector��ͼ
			UpdateNodeInspector(null);
		}

		/// <summary>
		/// ����hierarchy����
		/// </summary>
		private void UpdateHierarchyView()
		{
			//����ʱҪ���°�itemsource �����Ҫ
			groupItemHierarchyListView.itemsSource = currentOpenedGroupData.dialogueItems;
			groupItemHierarchyListView.Rebuild();
			int selectedIndex = currentOpenedGroupData.dialogueItems.IndexOf(currentOpenedGroupData.GetOpenedEditorItem());
			groupItemHierarchyListView.SetSelectionWithoutNotify(new int[] { selectedIndex });
		}

		/// <summary>
		/// ����һ���µ�Group����
		/// </summary>
		/// <param name="newGroupName"></param>
		/// <param name="description"></param>
		public void CreateNewGroup()
		{
			//�½�group
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
		/// �ڵ�ǰGroup�д���һ���µ�Item����
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
					//�����ǰɾ����item�ǵ�ǰ�򿪵�
					if (deletedIsCurrent)
					{
						currentOpenedGroupData.SetOpenedEditorItem(currentOpenedGroupData.dialogueItems.FirstOrDefault().name);
						//���ﲻִ�б������
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
		/// ����ǰѡ�е�DialogueItem�ı�ʱ����
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
				//���ɶԻ��ڵ���ͼ
				groupItemHierarchyListView.Q<EditorHierarchyItem>(previousItem.name.Split("-").Last())?.DeSelected();
				currentOpenedGroupData.SetOpenedEditorItem(itemData.name);
				graphView.GenerateNodeGraphView(currentOpenedGroupData.GetOpenedEditorItem());
				groupItemHierarchyListView.Q<EditorHierarchyItem>(itemData.name.Split("-").Last()).OnSelected();
				//����label������ʾ
				editInfomationLabel.text = $"[{currentOpenedGroupData.name}] -> [{currentOpenedGroupData.GetOpenedEditorItem().name.Split("-").Last()}]";
			}
		}

		/// <summary>
		/// �Զ����� ÿ10s����һ��
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

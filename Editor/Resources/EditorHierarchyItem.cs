using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	public class EditorHierarchyItem : VisualElement
	{
		private static VisualTreeAsset itemVisualTreeAsset;
		public static VisualTreeAsset ItemVisualTreeAsset
		{
			get
			{
				if (itemVisualTreeAsset == null)
				{
					itemVisualTreeAsset = Resources.Load<VisualTreeAsset>("HierarchyItem");
					//AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/DialogueSystem/Editor/UI/HierarchyItem.uxml");
				}
				return itemVisualTreeAsset;
			}
		}

		public Label ItemNameLabel { get; private set; }
		public Button DeleteItemBtn { get; private set; }
		private TemplateContainer container;

		public EditorHierarchyItem()
		{
			container = ItemVisualTreeAsset.CloneTree();
			Add(container);
			ItemNameLabel = container.Q<Label>("ItemName");
			DeleteItemBtn = container.Q<Button>("DeleteItem");
		}

		public void OnSelected()
		{
			container.Q("Root").RemoveFromClassList("normal");
			container.Q("Root").AddToClassList("selected");
		}
		public void DeSelected()
		{
			container.Q("Root").RemoveFromClassList("selected");
			container.Q("Root").AddToClassList("normal");
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
	public class EditorInspectorItem : VisualElement
	{
		private static VisualTreeAsset itemVisualTreeAsset;
		public static VisualTreeAsset ItemVisualTreeAsset
		{
			get
			{
				if (itemVisualTreeAsset == null)
				{
					itemVisualTreeAsset = Resources.Load<VisualTreeAsset>("InspectorItem");
					//AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/DialogueSystem/Editor/UI/InspectorItem.uxml");
				}
				return itemVisualTreeAsset;
			}
		}

		public Label FieldNameLabel { get; private set; }
		public PropertyField Field { get; private set; }
		private TemplateContainer container;

		public EditorInspectorItem()
		{
			container = ItemVisualTreeAsset.CloneTree();
			Add(container);
			FieldNameLabel = container.Q<Label>("FieldName");
			Field = container.Q<PropertyField>("Field");
			Field.label = "";
		}
	}
}

using RPGCore.Dialogue.Runtime;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPGCore.Dialogue.Editor
{
	public static class DialogueEditorUtility
	{
		//资源保存的路径
		private static readonly string dialogueItemSavePath = "Assets/Datas/Dialogue/Item/";
		private static readonly string dialogueGroupSavePath = "Assets/Datas/Dialogue/Group/";
		public static bool DialogueGroupExist(string groupName)
		{
			string[] result = AssetDatabase.FindAssets("t:DialogueGroupDataSO", new string[] { dialogueGroupSavePath });
			result = result.Select(s => AssetDatabase.GUIDToAssetPath(s).Split("/").Last()).ToArray();
			for (int i = 0; i < result.Length; i++)
			{
				if (result[i].Split(".")[0] == groupName)
				{
					return true;
				}
			}
			return false;
		}

		public static bool DialogueItemExists(DialogueGroupDataSO currentGroup, string itemName)
		{
			foreach (var item in currentGroup.dialogueItems)
			{
				if (item.name.Split("-").Last() == itemName)
				{
					return true;
				}
			}
			return false;
		}

		public static DialogueGroupDataSO CreateDialogueGroupData(string name, string description)
		{
			DialogueGroupDataSO groupData = ScriptableObject.CreateInstance<DialogueGroupDataSO>();
			groupData.name = name;
			groupData.Description = description;
			DialogueItemDataSO itemData = ScriptableObject.CreateInstance<DialogueItemDataSO>();
			itemData.name = name + "-default";
			groupData.dialogueItems.Add(itemData);
			AssetDatabase.CreateAsset(groupData, dialogueGroupSavePath + groupData.name + ".asset");
			AssetDatabase.CreateAsset(itemData, dialogueItemSavePath + itemData.name + ".asset");
			EditorUtility.SetDirty(groupData);
			EditorUtility.SetDirty(itemData);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return groupData;
		}

		public static DialogueItemDataSO CreateDialogueItemData(DialogueGroupDataSO groupData, string name, string description)
		{
			DialogueItemDataSO itemData = ScriptableObject.CreateInstance<DialogueItemDataSO>();
			itemData.name = groupData.name + "-" + name;
			itemData.Description = description;
			groupData.dialogueItems.Add(itemData);
			AssetDatabase.CreateAsset(itemData, dialogueItemSavePath + itemData.name + ".asset");
			EditorUtility.SetDirty(itemData);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return itemData;
		}

		public static T CreateDialogueNodeData<T>(DialogueGroupDataSO groupData) where T : DgNodeBase
		{
			return CreateDialogueNodeData(typeof(T), groupData) as T;
		}

		public static DgNodeBase CreateDialogueNodeData(Type type, DialogueGroupDataSO groupData)
		{
			DgNodeBase node = ScriptableObject.CreateInstance(type) as DgNodeBase;
			node.name = node.Guid;
			//保存功能
			DialogueItemDataSO itemDataSO = groupData.GetOpenedEditorItem();
			itemDataSO.dgNodes.Add(node);
			AssetDatabase.AddObjectToAsset(node, itemDataSO);
			EditorUtility.SetDirty(node);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log($"Create a {type.ToString()} node.");
			return node;
		}

		public static void DeleteDialogueItemData(DialogueGroupDataSO groupData, DialogueItemDataSO itemData)
		{
			if (groupData.dialogueItems.Contains(itemData))
			{
				groupData.dialogueItems.Remove(itemData);
				Debug.Log($"Delete item data : {itemData.name}");
				AssetDatabase.DeleteAsset(dialogueItemSavePath + itemData.name + ".asset");
				EditorUtility.SetDirty(groupData);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		public static void DeleteDialogueNodeData(DgNodeBase nodeToDelete)
		{
			AssetDatabase.RemoveObjectFromAsset(nodeToDelete);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void MakeSureDataLegalization(DialogueGroupDataSO groupDataSO) 
		{
			for (int i = 0; i < groupDataSO.dialogueItems.Count; i++) 
			{
				var item = groupDataSO.dialogueItems[i];
				//Debug.Log(AssetDatabase.GetAssetPath(item));
				UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(item));
				for (int j = 0; j < objects.Length; j++) 
				{
					if (objects[j] == null) 
					{
						throw new Exception("出现了空的资源 请手动删除！");
					}
				}
			}
		}
	}
}

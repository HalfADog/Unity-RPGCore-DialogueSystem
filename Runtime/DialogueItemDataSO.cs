using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPGCore.Dialogue;

namespace RPGCore.Dialogue.Runtime
{
	public class DialogueItemDataSO : ScriptableObject
	{
		public string Description;
		public List<DgNodeBase> dgNodes = new List<DgNodeBase>();
#if UNITY_EDITOR
		public Vector3 GraphViewPortPosition = new Vector3();

		public Vector3 GraphViewPortScale = new Vector3(1, 1, 1);
#endif

#if UNITY_EDITOR

		public void SaveGraphViewPortInfomation(Vector3 position, Vector3 scale)
		{
			GraphViewPortPosition = position;
			GraphViewPortScale = scale;
		}
#endif
		/// <summary>
		/// ���ص�ǰItem��ʼ�ڵ�
		/// </summary>
		/// <returns></returns>
		public IDgNode StartNode()
		{
			return dgNodes.Find(node => node.Type == DgNodeType.Start);
		}
	}
}

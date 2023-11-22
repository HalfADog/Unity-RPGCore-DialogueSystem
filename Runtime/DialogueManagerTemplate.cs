using RPGCore.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
    /// <summary>
    /// �Ի�������ģ�壬�ṩ�˻����ķ�����ʵ��ʹ����Ҫ�̳д���ʵ�ָ���ϸ��
    /// </summary>
    public abstract class DialogueManagerTemplate<T> : Singleton<T> where T : new()
    {
        private DialogueGroupDataSO currentActiveDialogueGroup;
        /// <summary>
        /// ��ǰ����ʹ�õ�DialogueGroup����
        /// </summary>
        public DialogueGroupDataSO CurrentActiveDialogueGroup
		{
            get { return currentActiveDialogueGroup; }
            set { currentActiveDialogueGroup = value; }
        }

        private bool isAnyDialogueBeExecuting;
        /// <summary>
        /// ��ǰ�Ƿ�����ִ��һ���Ի�
        /// </summary>
        public bool IsAnyDialogueBeExecuting
		{
            get { return isAnyDialogueBeExecuting; }
        }

        //��ǰ�Ի��ڵ�
        protected IDgNode currentDialogueNode;
		//��һ���Ի��ڵ�
		protected IDgNode previousDialogueNode;
		protected string itemNameToChange;

        /// <summary>
        /// ��ʼ�Ի�
        /// </summary>
        public virtual void StartDialogue(DialogueGroupDataSO groupData) 
        {
            isAnyDialogueBeExecuting = true;
            currentActiveDialogueGroup = groupData;
            DialogueItemDataSO item = currentActiveDialogueGroup.GetActiveItem();
            foreach (var node in item.dgNodes)
            {
                node.NextNodes.AddRange(item.dgNodes.Where(n=>node.nextNodesGuid.Contains(n.Guid)));
            }
            currentDialogueNode = currentActiveDialogueGroup.GetActiveItem().StartNode();
        }

        /// <summary>
        /// �����Ի�
        /// </summary>
        public virtual void StopDialogue() 
        {
            isAnyDialogueBeExecuting = false;
            //ִ��item�л�
            if (!string.IsNullOrWhiteSpace(itemNameToChange)) 
            {
                if (!CurrentActiveDialogueGroup.SetActiveItem(itemNameToChange)) 
                {
                    Debug.LogError($"{currentActiveDialogueGroup.name} Change Item to {itemNameToChange} failed.");
                }
            }
            currentActiveDialogueGroup = null;
            currentDialogueNode = null;
            previousDialogueNode = null;
        }

		/// <summary>
		/// ���ô˷���������ת����ǰ�Ի�����һ���ڵ�
		/// </summary>
		/// <param name="param"></param>
		public void MoveNext(object param)
        {
            //��¼ǰһ��ִ�еĽڵ�
            previousDialogueNode = currentDialogueNode;
            //�ƶ�����һ���ڵ�
            currentDialogueNode = currentDialogueNode.GetNext(param);
            //����ǰ�ڵ�
            ProcessDialogueNode(currentDialogueNode);
        }

        public abstract void ProcessDialogueNode(IDgNode currentNode);

        /// <summary>
        /// �л���ǰִ�е�Group�ļ����Item
        /// ������ǰ�Ի�����ʱ�Ż�ִ���л�
        /// </summary>
        public void ChangeExecutingGroupActiveItem(string itemName)
        {
			itemNameToChange = itemName;
		}
	}
}

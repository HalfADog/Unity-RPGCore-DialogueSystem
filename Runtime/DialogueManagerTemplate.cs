using RPGCore.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
    /// <summary>
    /// 对话管理器模板，提供了基本的方法，实际使用需要继承此类实现更多细节
    /// </summary>
    public abstract class DialogueManagerTemplate<T> : Singleton<T> where T : new()
    {
        private DialogueGroupDataSO currentActiveDialogueGroup;
        /// <summary>
        /// 当前正在使用的DialogueGroup数据
        /// </summary>
        public DialogueGroupDataSO CurrentActiveDialogueGroup
		{
            get { return currentActiveDialogueGroup; }
            set { currentActiveDialogueGroup = value; }
        }

        private bool isAnyDialogueBeExecuting;
        /// <summary>
        /// 当前是否正在执行一个对话
        /// </summary>
        public bool IsAnyDialogueBeExecuting
		{
            get { return isAnyDialogueBeExecuting; }
        }

        //当前对话节点
        protected IDgNode currentDialogueNode;
		//上一个对话节点
		protected IDgNode previousDialogueNode;
		protected string itemNameToChange;

        /// <summary>
        /// 开始对话
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
        /// 结束对话
        /// </summary>
        public virtual void StopDialogue() 
        {
            isAnyDialogueBeExecuting = false;
            //执行item切换
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
		/// 调用此方法处理跳转到当前对话的下一个节点
		/// </summary>
		/// <param name="param"></param>
		public void MoveNext(object param)
        {
            //记录前一次执行的节点
            previousDialogueNode = currentDialogueNode;
            //移动到下一个节点
            currentDialogueNode = currentDialogueNode.GetNext(param);
            //处理当前节点
            ProcessDialogueNode(currentDialogueNode);
        }

        public abstract void ProcessDialogueNode(IDgNode currentNode);

        /// <summary>
        /// 切换当前执行的Group的激活的Item
        /// 仅当当前对话结束时才会执行切换
        /// </summary>
        public void ChangeExecutingGroupActiveItem(string itemName)
        {
			itemNameToChange = itemName;
		}
	}
}

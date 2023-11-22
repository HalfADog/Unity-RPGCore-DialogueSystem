using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	/// <summary>
	/// 对话节点类型枚举
	/// </summary>
	public enum DgNodeType 
    {
        Start,//开始对话节点
        End,//结束对话节点
        Sentence,//对话内容节点
        Choice,//对话选项节点
        Random,//随机选择节点
        Action,//动作节点
        Flow,//流程控制
    }
    [Serializable]
    public class DgNodeBase : ScriptableObject,IDgNode
    {
        //节点标识符
        [SerializeField]
        private string guid;
        public string Guid { get { return guid; }  }
		//节点类型
		[SerializeField]
		private DgNodeType type;
        public DgNodeType Type { get { return type; }  }
        //节点名称
        [SerializeField]
        private new string name;
        public string Name
        {
            get => name;
            protected set => name = value;
        }
        //节点的下一个节点 必须在使用时初始化
        public List<IDgNode> NextNodes { get; protected set; } = new List<IDgNode>();
        //节点的下一个节点的GUID 用来记录节点间关系
        public List<string> nextNodesGuid = new List<string>();
#if UNITY_EDITOR
        public Vector2 graphViewPosition;
#endif
        public DgNodeBase(DgNodeType type)
        {
            this.guid = System.Guid.NewGuid().ToString();
            this.type = type;
        }

        /// <summary>
        /// 获取当前节点的下一个节点
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual IDgNode GetNext(object param)
		{
            return null;
		}
		public T GetNext<T>(object param) where T : IDgNode
		{
            IDgNode node = GetNext(param);
            if (node != null) 
            {
                return (T)node;
            }
            return default(T);
		}

        /// <summary>
        /// 为当前节点添加下一个节点
        /// </summary>
        /// <param name="dgNode"></param>
		public virtual void AddNext(IDgNode dgNode)
		{
            if (dgNode != null)
            {
				NextNodes.Add(dgNode);
                nextNodesGuid.Add(dgNode.Guid);
            }
		}

        /// <summary>
        /// 将当前节点转化为指定类型的节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T Get<T>() where T : IDgNode
		{
			return this.ConvertTo<T>();
		}
	}
}

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	/// <summary>
	/// �Ի��ڵ�����ö��
	/// </summary>
	public enum DgNodeType 
    {
        Start,//��ʼ�Ի��ڵ�
        End,//�����Ի��ڵ�
        Sentence,//�Ի����ݽڵ�
        Choice,//�Ի�ѡ��ڵ�
        Random,//���ѡ��ڵ�
        Action,//�����ڵ�
        Flow,//���̿���
    }
    [Serializable]
    public class DgNodeBase : ScriptableObject,IDgNode
    {
        //�ڵ��ʶ��
        [SerializeField]
        private string guid;
        public string Guid { get { return guid; }  }
		//�ڵ�����
		[SerializeField]
		private DgNodeType type;
        public DgNodeType Type { get { return type; }  }
        //�ڵ�����
        [SerializeField]
        private new string name;
        public string Name
        {
            get => name;
            protected set => name = value;
        }
        //�ڵ����һ���ڵ� ������ʹ��ʱ��ʼ��
        public List<IDgNode> NextNodes { get; protected set; } = new List<IDgNode>();
        //�ڵ����һ���ڵ��GUID ������¼�ڵ���ϵ
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
        /// ��ȡ��ǰ�ڵ����һ���ڵ�
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
        /// Ϊ��ǰ�ڵ������һ���ڵ�
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
        /// ����ǰ�ڵ�ת��Ϊָ�����͵Ľڵ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T Get<T>() where T : IDgNode
		{
			return this.ConvertTo<T>();
		}
	}
}

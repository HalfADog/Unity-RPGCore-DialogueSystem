using RPGCore.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	[Serializable]
	public class DgNodeActionBase : DgNodeBase, IDgNode
	{
        public Action OnAction { get; protected set; }

        public DgNodeActionBase() : base(DgNodeType.Action)
        {

        }
		public override IDgNode GetNext(object param)
		{
			if (NextNodes.Count() >= 1)
			{
				return NextNodes[0];
			}
			return null;
		}

		public void SetAction(Action action) 
		{
			OnAction = null;
			OnAction = action;
		}
	}
}
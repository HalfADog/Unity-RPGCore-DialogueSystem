using RPGCore.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	public class DgNodeFlowControlBase : DgNodeBase, IDgNode
	{
		public Func<bool> Condition { get; protected set; }
		public DgNodeFlowControlBase() : base(DgNodeType.Flow)
		{
		}

		public override IDgNode GetNext(object param)
		{
			if (Condition != null && NextNodes.Count>=2) 
			{
				bool result = Condition.Invoke();
				return result ? NextNodes[0] : NextNodes[1];
			}
			return null;
		}

		public void SetCondition(Func<bool> condition) 
		{
			Condition = null;
			this.Condition = condition;
		}
	}
}

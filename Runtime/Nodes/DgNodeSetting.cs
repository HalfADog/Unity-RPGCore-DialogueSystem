using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	[Serializable]
	public class DgNodeSetting : DgNodeBase, IDgNode
	{
		public DgNodeSetting() : base(DgNodeType.Setting)
		{
			Name = "Setting";
		}
	}
}

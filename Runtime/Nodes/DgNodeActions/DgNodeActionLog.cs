using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	[DialogueNode(Path = "Action/Log")]
	public class DgNodeActionLog : DgNodeActionBase
    {
        public string message;
        public DgNodeActionLog()
        {
            Name = "Log";
            SetAction(() =>
            {
                Debug.Log(message);
            });
        }
    }
}

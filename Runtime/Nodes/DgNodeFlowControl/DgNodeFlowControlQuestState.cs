using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
    [DialogueNode(Path = "Flow Control/Quest State")]
    public class DgNodeFlowControlQuestState : DgNodeFlowControlBase
    {
        //Test
        public bool IsQuestFinished;
        public DgNodeFlowControlQuestState()
        {
            Name = "Quest State";
            SetCondition(() => 
            {
                return IsQuestFinished;
			});
        }
    }
}
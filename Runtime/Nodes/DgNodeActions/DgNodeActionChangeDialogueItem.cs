using RPGCore.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
    [DialogueNode(Path = "Action/Change Dialogue Item")]
    public class DgNodeActionChangeDialogueItem : DgNodeActionBase
    {
        public string ItemName;

        public DgNodeActionChangeDialogueItem()
        {
            Name = "Change Dialogue Item";
            SetAction(() =>
            {
                DialogueManager.Instance.ChangeExecutingGroupActiveItem(ItemName);
            });
        }
    }
}

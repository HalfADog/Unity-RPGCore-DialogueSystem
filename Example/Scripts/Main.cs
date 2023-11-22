using RPGCore.Dialogue.Runtime;
using UnityEngine;

public class Main : MonoBehaviour
{
    public DialogueGroupDataSO currentExecuteDialogueGroup;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            DialogueManager.Instance.StartDialogue(currentExecuteDialogueGroup);
        }
    }
}

using RPGCore.Dialogue.Runtime;
using RPGCore.UI;
public class DialogueManager : DialogueManagerTemplate<DialogueManager>
{
	public DialoguePanel dialoguePanel;
	public override void StartDialogue(DialogueGroupDataSO groupData)
	{
		base.StartDialogue(groupData);
		dialoguePanel = UIManager.Instance.ShowPanel<DialoguePanel>();
		dialoguePanel.OnMoveNext = () => MoveNext(null);
		dialoguePanel.OnChoiceSelected = param => { MoveNext(param); };
		MoveNext(null);
	}

	public override void StopDialogue()
	{
		base.StopDialogue();
		UIManager.Instance.HidePanel<DialoguePanel>();
		dialoguePanel = null;
	}

	public override void ProcessDialogueNode(IDgNode currentNode)
	{
		DgNodeType nodeType = currentNode.Type;
		//如果上一个处理的节点是选择节点则将选择面板隐藏
		if (previousDialogueNode.Type == DgNodeType.Choice) 
		{
			dialoguePanel.HideChoices();
		}
		switch (nodeType)
		{
			case DgNodeType.Start:
				break;
			case DgNodeType.End:
				StopDialogue();
				break;
			case DgNodeType.Sentence:
				DgNodeSentence sentence = currentNode.Get<DgNodeSentence>();
				dialoguePanel.sentenceContent.text = sentence.Content;
				///ATTENTION:仅测试用
				dialoguePanel.speakerContent.text = sentence.speaker.ToString();
				break;
			case DgNodeType.Choice:
				DgNodeChoice choices = currentNode.Get<DgNodeChoice>();
				foreach (var choice in choices.Choices)
				{
					dialoguePanel.AddChoiceItem(choice);
				}
				dialoguePanel.ShowChoices();
				break;
			case DgNodeType.Random://执行到Random节点后立即再次执行
				MoveNext(null);
				break;
			case DgNodeType.Action://执行到Action节点后立即再次执行
				currentNode.Get<DgNodeActionBase>().OnAction();
				MoveNext(null);
				break;
			case DgNodeType.Flow://执行到Flow节点后立即再次执行
				MoveNext(null);
				break;
		}
	}
}

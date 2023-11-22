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
		//�����һ������Ľڵ���ѡ��ڵ���ѡ���������
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
				///ATTENTION:��������
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
			case DgNodeType.Random://ִ�е�Random�ڵ�������ٴ�ִ��
				MoveNext(null);
				break;
			case DgNodeType.Action://ִ�е�Action�ڵ�������ٴ�ִ��
				currentNode.Get<DgNodeActionBase>().OnAction();
				MoveNext(null);
				break;
			case DgNodeType.Flow://ִ�е�Flow�ڵ�������ٴ�ִ��
				MoveNext(null);
				break;
		}
	}
}

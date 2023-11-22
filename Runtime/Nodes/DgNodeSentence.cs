using System.Linq;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	/// <summary>
	/// 说话的角色类型
	/// </summary>
	public enum SentenceTalker 
	{
		Player,
		NPC,
		Aside
	}
	[DialogueNode(Path = "Sentence")]
	public class DgNodeSentence : DgNodeBase, IDgNode
	{
		[SerializeField]
		public SentenceTalker speaker;
		[SerializeField]
		[Multiline(6)]
		private string content;
		public string Content => content;
        public DgNodeSentence():base(DgNodeType.Sentence)
        {
			Name = "Sentence";
			this.content = "";
		}
		public DgNodeSentence(string content) : base(DgNodeType.Sentence)
		{
			Name = "Sentence";
			this.content = content;
		}
		public override IDgNode GetNext(object param)
		{
			if (NextNodes.Count() >= 1)
			{
				return NextNodes[0];
			}
			return null;
		}

		public void SetContent(string content)
		{
			if (!string.IsNullOrWhiteSpace(content))
			{
				this.content = content;
			}
		}
	}
}

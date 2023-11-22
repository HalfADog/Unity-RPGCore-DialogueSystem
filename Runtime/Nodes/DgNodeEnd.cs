namespace RPGCore.Dialogue.Runtime
{
	[DialogueNode(Path = "End")]
	public class DgNodeEnd : DgNodeBase, IDgNode
	{
		public DgNodeEnd() : base(DgNodeType.End)
		{
			Name = "End";
		}
	}
}

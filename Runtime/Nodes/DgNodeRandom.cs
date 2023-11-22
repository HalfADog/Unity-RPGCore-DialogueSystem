using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	[DialogueNode(Path = "Random")]
	public class DgNodeRandom : DgNodeBase, IDgNode
	{
		public DgNodeRandom() : base(DgNodeType.Random)
		{
			Name = "Random";
		}

		public override IDgNode GetNext(object param)
		{
			if (NextNodes.Count == 0)
				return null;
			int randomIndex = Random.Range(0,NextNodes.Count);
			return NextNodes[randomIndex];
		}
	}
}

using System;
using System.Linq;

namespace RPGCore.Dialogue.Runtime
{
	[Serializable]
	public class DgNodeStart : DgNodeBase, IDgNode
    {
        public DgNodeStart() : base(DgNodeType.Start)
        {
			Name = "Start";
        }

		public override IDgNode GetNext(object param)
		{
			if (NextNodes.Count() >= 1)
			{
				return NextNodes[0];
			}
			return null;
		}
		public override void AddNext(IDgNode dgNode)
		{
			if (NextNodes.Count() == 0)
			{
				NextNodes.Add(dgNode);
				nextNodesGuid.Add(dgNode.Guid);
			}
		}
	}
}

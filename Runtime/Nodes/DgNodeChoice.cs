using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGCore.Dialogue.Runtime
{
	[DialogueNode(Path = "Choice")]
	public class DgNodeChoice : DgNodeBase, IDgNode
	{
        [SerializeField]
        private List<string> choices;
		public List<string> Choices { get { return choices; } }
        public DgNodeChoice() : base(DgNodeType.Choice)
        {
            Name = "Choice";
            choices = new List<string>(5);
        }

		public override IDgNode GetNext(object param)
		{
            if (param != null) {
                int choice = (int)param;
                if (choice < 0||choice>=Choices.Count()) 
                {
                    return null;
                }
                return NextNodes[choice];
            }
            return null;
		}

		public void AddChoice(string choice) 
        {
            if (!string.IsNullOrWhiteSpace(choice))
            {
                Choices.Add(choice);
            }
        }
    }
}

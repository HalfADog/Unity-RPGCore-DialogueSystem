using RPGCore.Dialogue.Runtime;
using System;

namespace RPGCore.Dialogue.Editor
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DialogueGraphNodeAttribute : Attribute
	{
		public DgNodeType Type;
	}
}

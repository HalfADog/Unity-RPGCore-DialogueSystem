using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TwoPaneSplitViewHorizontalExposed : TwoPaneSplitView
{
	public new class UxmlFactory : UxmlFactory<TwoPaneSplitViewHorizontalExposed, UxmlTraits>
	{ }
	public TwoPaneSplitViewHorizontalExposed()
	{
		this.orientation = TwoPaneSplitViewOrientation.Horizontal;
	}
}

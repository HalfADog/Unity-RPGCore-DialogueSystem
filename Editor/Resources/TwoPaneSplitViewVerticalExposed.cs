using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TwoPaneSplitViewVerticalExposed : TwoPaneSplitView
{
	public new class UxmlFactory : UxmlFactory<TwoPaneSplitViewVerticalExposed, UxmlTraits>
	{ }
	public TwoPaneSplitViewVerticalExposed()
	{
		this.orientation = TwoPaneSplitViewOrientation.Vertical;
	}
}

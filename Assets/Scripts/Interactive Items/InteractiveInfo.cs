using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveInfo : InteractiveItem 
{
	// Inspector Assigned
	[SerializeField] private string _infoText;

	// ---------------------------------------------------------------------------
	// Name	:	GetText (Override)
	// Desc	:	Returns the text to display in the HUD
	// ---------------------------------------------------------------------------
	public override string	GetText()
	{
		return _infoText;
	}
}

using System;
using UnityEngine;
using Model;

public class CombatAnimation : IAnimation 
{
	public Unit _otherUnit;
	public UnitAvatar _unit;
	public TileVector _attackPosition;
	private bool seen = false;

	public CombatAnimation (UnitAvatar unit, Unit otherUnit, TileVector destination)
	{
		_otherUnit = otherUnit;
		_unit = unit;
		_attackPosition = destination;
	}

	public bool ApplyAnimation(float time)
	{
		var otherQueue = _otherUnit._avatar._moveQueue;
		var thisQueue = _unit._moveQueue;
		var ownUnit = thisQueue.Peek() as CombatAnimation;
		var otherUnit = thisQueue.Peek() as CombatAnimation;
		//Utils.Print (thisQueue.Peek());
		if (ownUnit != null && otherUnit != null) {
			seen = true;
			Utils.Print ("Units Same"+Time.fixedTime);
//			return true;
		} 
		if( otherUnit.seen == true) 
		{
			Utils.Print ("SAME"+Time.fixedTime);
			return true;
		}
		return false;
	}
}



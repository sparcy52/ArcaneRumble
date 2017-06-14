using System;
using UnityEngine;
using Model;

public class CombatAnimation : SyncedAnimation<CombatAnimation, CombatAnimation> 
{	
	public CombatAnimation (Unit attacker, Unit defender) : base (attacker.Avatar, defender.Avatar)
	{
		
	}

	public override bool ApplyAnimation(float time)
	{
		return Sync();
	}
}



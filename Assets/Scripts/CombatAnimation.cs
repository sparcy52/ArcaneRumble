﻿using System;
using UnityEngine;
using Model;

public class CombatAnimation : SyncedAnimation<CombatAnimation, CombatAnimation>
{
	private float _elapsed = 0;
	private int _stage = 0;
	
	public CombatAnimation(Unit attacker, Unit defender) : base(attacker.Avatar, defender.Avatar)
	{
		
	}

	public override bool ApplyAnimation(float time)
	{
		if (Sync()) 
		{
			Color color;
			float flashTime;
			switch (_stage)
			{
				case 0:
					color = GuiComponents.GetHitPrimaryColor();
					flashTime = GuiComponents.GetHitPrimaryTime();
					break;
				case 1:
					color = GuiComponents.GetHitSecondaryColor();
					flashTime = GuiComponents.GetHitSecondaryTime();
					break;
				default:
					Target.ResetPaint();
					Target.Scale = 1;
					return true;
			}
			if (Target == null) return true;
			Target.Paint(color);
			Target.Scale = 1.1f;

			_elapsed += time;
			if (_elapsed > flashTime)
			{
				_stage++;
				_elapsed = 0;
			}
		}
		return false;
	}
}



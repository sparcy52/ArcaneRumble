﻿using System;
using UnityEngine;
using Model;

public class CombatAnimation : SyncedAnimation<CombatAnimation, CombatAnimation>
{
	private float _elapsed = 0;
	private int _stage = 0;
	private int _attackerEnergy;
	private int _defenderHealth;
	
	public CombatAnimation(Unit attacker, Unit defender, int damage) : base(attacker.Avatar, defender.Avatar)
	{
		// TODO: use damage numbers with advanced bar system
		_attackerEnergy = attacker.Energy;
		_defenderHealth = defender.Health;
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
					Owner.UpdateEnergy(_attackerEnergy);
					Target.UpdateHealth(_defenderHealth);
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



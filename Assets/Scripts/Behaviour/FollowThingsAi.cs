using System;
using System.CodeDom;
using System.Collections.Generic;
using Model;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/FollowThingsAi")]
public class FollowThingsAi : UnitAi
{
	public bool FollowEnemies;
	public bool FollowObjectives;
	public bool DefendObjectives;
	public bool AttackObjectives;

	public override TurnPlan GetMovementPlan(Unit unit, World world)
	{
		if (world == null) throw new Exception("Missing World!");
		return new FollowThingsPlan(unit, world, this);
	}
}

public class FollowThingsPlan : TurnPlan
{
	private Unit _unitTarget;
	private Objective _objectiveTarget;
	private TileVector _objectivePos;	// assumes that Objectives can't move!

	private FollowThingsAi _parent;

	public FollowThingsPlan(Unit unit, World world, FollowThingsAi parent) : base(unit, world)
	{
		var last = GetLastMove<FollowThingsPlan>();
		if (last != null)
		{
			_unitTarget = last._unitTarget;
			_objectiveTarget = last._objectiveTarget;
			_objectivePos = last._objectivePos;
		}
		_parent = parent;
	}

	public override Move GetNextMove()
	{
		FindTarget();
		return Pursue();
	}

	public void FindTarget()
	{
		// check if we already have targets, and that they are still valid
		if (_unitTarget != null &&_unitTarget.IsDead()) _unitTarget = null;
		if (_objectiveTarget != null && !ShouldFollowObjective(_objectiveTarget)) _objectiveTarget = null;
		
		if (_unitTarget != null || _objectiveTarget != null) return;	// don't find new target if we already have one
		
		TileVector? closestTargetPos = null;

		for (int w = 0; w < World.W; w++) 
		{
			for (int e = 0; e < World.E; e++) 
			{
				var pos = new TileVector (w, e);
				if (World[pos] == null) continue;
				
				bool hasTarget = false;
				Hex h = World [w, e];

				if(h.Occupant != null && h.Occupant.Owner != Unit.Owner && _parent.FollowEnemies)
				{
					hasTarget = true;
				}
				
				if(h.Objective != null)
				{
					hasTarget = ShouldFollowObjective(h.Objective);
				}

				if (hasTarget)
				{
					if (!closestTargetPos.HasValue)
					{
						closestTargetPos = pos;
					}
					else
					{
						var distance = TileVector.Distance (pos, Unit.Position);
						if (distance < TileVector.Distance(closestTargetPos.Value, Unit.Position))
						{
							closestTargetPos = pos;
						}
					}
				}
				
			}
		}

		if (!closestTargetPos.HasValue) return;	// didn't find anything
		
		if (_parent.FollowEnemies) {
			_unitTarget = World [closestTargetPos.Value.W, closestTargetPos.Value.E].Occupant;
		} else {
			_objectiveTarget = World [closestTargetPos.Value.W, closestTargetPos.Value.E].Objective;
			_objectivePos = closestTargetPos.Value;
		}
	}

	public Move Pursue()
	{
		if (_unitTarget != null) // persue units in precedence to objectives
		{
			var dir = Unit.Position.GetApproximateDirectionTo(_unitTarget.Position);
			if (dir.HasValue) return Step(dir.Value);
			else return Halt();
		}
		if (_objectiveTarget != null)
		{
			if (_objectivePos == Unit.Position)	// if we don't need to move
			{
				if (_unitTarget != null)
				{
					var dir = Unit.Position.GetApproximateDirectionTo(_unitTarget.Position);
					if (dir.HasValue) return Turn(dir.Value);
					return Halt();
				}
				else return Halt();
			}
			return Step(Unit.Position.GetApproximateDirectionTo(_objectivePos).Value);
		}
		return Step(RelativeDirection.Forward);
	}

	private bool ShouldFollowObjective(Objective o)
	{
		if (_parent.AttackObjectives && o.controllingPlayer != Unit.Owner)
		{
			return true;
		}
		if (_parent.DefendObjectives && o.controllingPlayer == Unit.Owner) 
		{
			return true;
		}
		if (_parent.FollowObjectives && o.controllingPlayer == null) 
		{
			return true;
		}
		return false;
	}
}

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
		return new FollowThingsPlan(unit, world, this);
	}
}

public class FollowThingsPlan : TurnPlan
{
	private Unit _unitTarget;
	private Objective _objectiveTarget;
	private readonly Unit _unit;
	private readonly World _world;

	private FollowThingsAi _parent;

	public FollowThingsPlan(Unit unit, World world, FollowThingsAi parent) : base(unit, world)
	{
		_unit = unit;
		_parent = parent;
		if (GetLastMove<FollowThingsPlan> () == null) 
		{
			FindTarget ();
		}
		else
		{
			//_target = GetLastMove<FollowThingsPlan>()._target;	
		}
		_world = world;
	}

	public override Move GetNextMove()
	{
//		if (_target != null) return Step(Pursue());
//		else 
		return Step(RelativeDirection.Forward);
	}

	public void FindTarget()
	{
		TileVector? closestTargetPos = null;

		for (int w = 0; w < _world.W; w++) 
		{
			for (int e = 0; e < _world.E; e++) 
			{
				var pos = new TileVector (w, e);
				bool hasTarget = false;
				Hex h = _world [w, e];

				if(h.Occupant != null && h.Occupant.Owner.num != _unit.Owner.num && _parent.FollowEnemies) // add a check for invalid spaces?
				{
					hasTarget = true;
				}

				else if(h.objective != null)
					
				{
					if (_parent.AttackObjectives && !h.objective.controllingPlayer.Equals (_unit.Owner.num)) 
					{
						hasTarget = true;
					}
					if (_parent.DefendObjectives && h.objective.controllingPlayer.Equals (_unit.Owner.num)) 
					{
						hasTarget = true;
					}
					if (_parent.FollowObjectives && h.objective.controllingPlayer == null) 
					{
						hasTarget = true;
					}

				}
				var distance = TileVector.Distance (pos, _unit.Position);
				if (!closestTargetPos.HasValue) 
				{
					closestTargetPos = pos;
				} 
				else if (hasTarget && distance < TileVector.Distance (closestTargetPos.Value, _unit.Position)) 
				{
					closestTargetPos = pos;
				}
			}

		}

		if (_parent.FollowEnemies) {
			_unitTarget = _world [closestTargetPos.Value.W, closestTargetPos.Value.E].Occupant;
		} else {
			_objectiveTarget = _world [closestTargetPos.Value.W, closestTargetPos.Value.E].objective;
		}
	}

	public CardinalDirection Pursue()
	{
		return CardinalDirection.North;
		//get direction to move in for current target
	}
}

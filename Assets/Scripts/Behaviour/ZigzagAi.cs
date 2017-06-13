using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/ZigzagAi")]
	public class ZigzagAi : UnitAi
	{
		public RelativeDirection TurnDirection = RelativeDirection.ForwardLeft;

		public override TurnPlan GetMovementPlan(Unit unit, World world)
		{
			return new ZigzagPlan(unit, world, TurnDirection);
		}
	}

	public class ZigzagPlan : TurnPlan
	{
		private readonly RelativeDirection _turnDirection;
		private readonly CardinalDirection _targetDirection;

		public ZigzagPlan(Unit unit, World world, RelativeDirection initialTurn) : base(unit, world)
		{
			var last = GetLastMove<ZigzagPlan>();
			if (last != null)	// subsequent move: take turn opposite to last one
			{
				this._turnDirection = last._turnDirection.Mirror();
				this._targetDirection = this.Unit.Facing.Turn(this._turnDirection);
			}
			else 				// first move: use unit's facing direction
			{
				this._turnDirection = initialTurn.Mirror();
				this._targetDirection = Unit.Facing;
			}
		}

		public override Move GetNextMove()
		{
			return Step(_targetDirection);
		}
	}
}
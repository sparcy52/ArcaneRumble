
using System.CodeDom;
using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/CircleWalkAi")]
	public class CircleWalkAi : UnitAi
	{
		public bool TurnRight = false;

		public override TurnPlan GetMovementPlan(Unit unit, World world)
		{
			return new CircleWalkPlan(unit, world, TurnRight);
		}
	}

	public class CircleWalkPlan : TurnPlan
	{
		private readonly bool _turnRight;

		public CircleWalkPlan(Unit unit, World world, bool turnRight) : base(unit, world)
		{
			_turnRight = turnRight;
		}

		public override Move GetNextMove()
		{
			if (_turnRight) return Step(RelativeDirection.ForwardRight);
			else return Step(RelativeDirection.ForwardLeft);
		}
	}
}
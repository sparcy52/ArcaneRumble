using System;
using Model;
using UnityEngine;

namespace Behaviour
{
	namespace LinearWalk
	{
		[Serializable]
		public enum LinearWalkWallBehaviour
		{
			Stop,		// Stop moving
			Reverse,	// Turn 180 degrees
			Deflect,	// Change to move in the direction of the wall
			Follow,		// Hug the wall until the original direction is available again
		}
		
		[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/LinearWalkAi")]
		public class LinearWalkAi : UnitAi
		{
			// TODO: implement
			/// <summary>
			/// Defines the behaviour for when the Unit can no longer walk in its desired direction
			/// Stop:		Stop moving
			///	Reverse:	Turn 180 degrees
			///	Deflect:	Change to move in the direction of the wall
			///	Follow:		Hug the wall until the original direction is available again
			/// </summary>
			public LinearWalkWallBehaviour WallBehaviour = LinearWalkWallBehaviour.Stop;

			/// <summary>
			/// If true, stops the Unit's desired direction from being changed by external factors
			/// </summary>
			public bool ForceTargetDirection = false;
		
			public override TurnPlan GetMovementPlan(Unit unit, World world)
			{
				return new LinearWalkPlan(unit, world, ForceTargetDirection);
			}
		}
	
		public class LinearWalkPlan : TurnPlan
		{
			private CardinalDirection _target;
			private bool _forceTargetDirection;
			
			public LinearWalkPlan(Unit unit, World world, bool forceTargetDirection) : base(unit, world)
			{
				var prev = GetLastMove<LinearWalkPlan>();
				_target = prev != null ? prev._target : unit.Facing;
				
				_forceTargetDirection = forceTargetDirection;
				
			}

			public override Move GetNextMove()
			{
				if (_forceTargetDirection) return Step(_target);
				else
				{
					return Step(RelativeDirection.Forward);
				}
			}
		}
	}
}
using Model;
using UnityEngine;

public class MoveAnimation : IAnimation
{
	// bleed values: as a factor of the unit's speed values
	private const float RotationMovementBleed = 0.075f;	// when to start moving after rotating
	private const float IdleAnimationBleed = 0.05f;		// when to stop the walk animation before reaching destination

	// unity components
	private UnitAvatar _unit;

	// transition tracking fields
	//private Vector3 _originPos;
	private Vector3 _targetPos;

	private Quaternion _targetRot;

	[System.Obsolete("Use the destination and direction constructor instead")]
	public MoveAnimation(UnitAvatar unit, TileVector from, TileVector to)
	{
		_targetRot = (to - from).GetBearingRotation();
		_targetPos = to.ToVector3();
		_unit = unit;
	}

	public MoveAnimation(UnitAvatar unit, TileVector destination, CardinalDirection direction)
	{
		_unit = unit;
		_targetPos = destination.ToVector3();
		_targetRot = direction.GetBearingRotation();
	}

	public bool ApplyAnimation(float time)
	{
		// check if we should keep the unit in the walking animation
		if (Vector3.Distance(_unit.Position, _targetPos) >= _unit.MoveSpeed * IdleAnimationBleed)
		{
			_unit.Animator.SetBool("Walking", true);
		}
		else
		{
			_unit.Animator.SetBool("Walking", false);
		}

		// do rotation
		_unit.Rotation = Quaternion.RotateTowards(_unit.Rotation,
			_targetRot, time * _unit.TurnSpeed);

		// do position if we have rotate far enough
		if (Quaternion.Angle(_unit.Rotation, _targetRot) <= _unit.TurnSpeed * RotationMovementBleed)
		{
			_unit.Position = Vector3.MoveTowards(_unit.Position,
				_targetPos, time * _unit.MoveSpeed);

			if (_unit.Position == _targetPos) return true; // animation complete
		}

		return false;
	}
}
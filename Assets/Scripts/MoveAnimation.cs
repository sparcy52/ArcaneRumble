using Model;
using UnityEngine;

public class MoveAnimation : IAnimation
{
	// bleed values: as a factor of the unit's speed values
	private const float RotationMovementBleed = 0.075f;	// when to start moving after rotating
	private const float IdleAnimationBleed = 0.05f;		// when to stop the walk animation before reaching destination

	// unity components
	private UnitAvatar _avatar;

	private Vector3 _targetPos;

	private Quaternion _targetRot;

	private float _targetEnergy;

	public MoveAnimation(Unit unit, TileVector destination, CardinalDirection direction, int energyCost)
	{
		_avatar = unit.Avatar;
		_targetPos = destination.ToVector3();
		_targetRot = direction.GetBearingRotation();
		// TODO: implement energycost, advanced bar animation
		_targetEnergy = unit.Energy;
	}

	public bool ApplyAnimation(float time)
	{
		// check if we should keep the unit in the walking animation
		if (Vector3.Distance(_avatar.Position, _targetPos) >= _avatar.MoveSpeed * IdleAnimationBleed)
		{
			_avatar.Animator.SetBool("Walking", true);
		}
		else
		{
			_avatar.Animator.SetBool("Walking", false);
		}

		// do rotation
		_avatar.Rotation = Quaternion.RotateTowards(_avatar.Rotation,
			_targetRot, time * _avatar.TurnSpeed);

		// do position if we have rotate far enough
		if (Quaternion.Angle(_avatar.Rotation, _targetRot) <= _avatar.TurnSpeed * RotationMovementBleed)
		{
			_avatar.Position = Vector3.MoveTowards(_avatar.Position,
				_targetPos, time * _avatar.MoveSpeed);

			if (_avatar.Position == _targetPos && _avatar.Rotation == _targetRot)  // animation complete
			{
				_avatar.UpdateEnergy(_targetEnergy);
				return true;
			}
		}

		return false;
	}
}
using System;
using System.Collections.Generic;
using Model;

/// <summary>
/// Abstract class which provides functionality for ensuring that two animations take place at the same time.
/// </summary>
/// <typeparam name="T">The type of SyncedAnimation that O will wait for. Usually the implementing class</typeparam>
/// <typeparam name="O">The type of the other SyncedAnimation to wait for.</typeparam>
public abstract class SyncedAnimation<T, O> : IAnimation 
	where T : SyncedAnimation<T, O>
	where O : SyncedAnimation<O, T>
{
	public readonly UnitAvatar Owner;
	public readonly UnitAvatar Target;

	private O _targetAnim;
	private ICollection<object> _synchAnchors = new HashSet<object>();
	
	protected SyncedAnimation(UnitAvatar owner, UnitAvatar target)
	{
		Owner = owner;
		Target = target;
	}

	/// <summary>
	/// Method for a single synchronisation between SyncedAnimations.
	/// Returns true once the target animation has also called Sync.
	/// Use Sync(object) to synchronise multiple points within an animation.
	/// Using both in conjunction is undefined.
	/// </summary>
	/// <returns>true when Sync has been called on both SyncedAnimations</returns>
	protected bool Sync()
	{
		if (_targetAnim == null)
		{
			var other = Target.CurrentAnimation as O;
			if (other != null && other.Target == this.Owner)
			{
				_targetAnim = other;
			}
			else return false;
		}
		return _targetAnim._targetAnim == this;
	}
	
	/// <summary>
	/// Method for arbitrary synchronisation between SyncedAnimations.
	/// Returns true once the target animation calls Sync with an equivalent anchor.
	/// </summary>
	/// <param name="anchor"></param>
	/// <returns>true when the Animations are synchronised to the given anchor</returns>
	protected bool Sync(object anchor)
	{
		_synchAnchors.Add(anchor);
		return Sync() && _targetAnim._synchAnchors.Contains(anchor);
	}

	public abstract bool ApplyAnimation(float time);
}

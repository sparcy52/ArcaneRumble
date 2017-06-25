using Model;
using UnityEngine;

/// <summary>
/// Class from which all AI implementations should derive.
///
/// Each UnitAi class should represent a kind of Unit behaviour. Any public fields in this
/// class should modify the behaviour in a generalised way, so that multiple instances of it
/// can exhibit similar, but slightly different behaviour. These will be accessable in the Unity editor.
/// Each Unit type will then reference an instance of this class which it will use to generate its movement plans.
///
/// Assume that multiple Units may reference the same instance of a UnitAi, so do not store
/// state relating to specific Units here.
/// </summary>
public abstract class UnitAi : ScriptableObject
{
	/// <summary>
	/// The main method of this interface. Produce a MovementPlan for the given Unit that corresponds to
	/// the parameters (if any) of this UnitAi instance.
	/// </summary>
	/// <param name="unit">the Unit to construct the move for</param>
	/// <param name="world">the game World which the Unit is in</param>
	/// <returns>a MovementPlan for this Unit, for the current turn</returns>
	public abstract TurnPlan GetMovementPlan(Unit unit, World world);
	
	/// <summary>
	/// Estimate what cells the Unit will traverse given its next movement plan.
	/// Optional method. Base implementation returns an array of zero.
	/// </summary>
	/// <param name="unit">the Unit to construct the move for</param>
	/// <param name="world">the game World which the Unit is in</param>
	/// <returns>Estimated results of calls to GetMovementPlan</returns>
	public virtual StepPreview[] GetPreview(Unit unit, World world)
	{
		return new StepPreview[0];
	}

	/// <summary>
	/// Used to improve Unit placement ergonomics. Used to offset Unit rotation when it is
	/// selected, such that it tends towards the general direction that the player has selected.
	/// Optional method. Base implementation returns RelativeDirection.Foward.
	/// </summary>
	/// <returns>a hint to help align unit selection</returns>
	public virtual RelativeDirection PreviewDirectionHint()
	{
		return RelativeDirection.Forward;
	}

	/// <summary>
	/// Used to improve Unit placement ergonomics. When the Unit is mirrored prior to placement,
	/// apply this rotation to it (or its mirror, if un-mirroing) to help align the paths.
	/// Optional method. Base implementation returns RelativeDirection.Foward.
	/// </summary>
	/// <returns>a hint to simplify unit mirroring during placement</returns>
	public virtual RelativeDirection PreviewMirrorHint()
	{
		return RelativeDirection.Forward;
	}
}

public struct StepPreview
{
	public readonly TileVector Pos;
	public readonly int Index;
	
	public StepPreview(TileVector pos, int index)
	{
		Pos = pos;
		Index = index;
	}
}
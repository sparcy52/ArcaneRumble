using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public string Name;
	public Color Color;
	public Loadout Loadout;
	public bool MirrorDefault;	// determines unit turn direction in TurnPlan.AdjustForWalls

	public List<TileVector> PlayerPlacables { get; set;}
	public int CapturedObjectives { get { return _capturedObjectives.Count; } }
	private readonly HashSet<Objective> _capturedObjectives = new HashSet<Objective>();

	public int TotalDeployedUnits { get; private set; }
	public int ActiveUnits { get { return _activeUnits.Count; } }
	private readonly HashSet<Unit> _activeUnits = new HashSet<Unit>();
	
	public int DestroyedUnits { get; private set; }
	public int SelfDestroyedUnits { get; private set; }	// just in case we need this...

	public GameObject[] Units	// simple alias for the loadout
	{
		get { return Loadout.Units; }
	}

	public bool TakeObjective(Objective obj)
	{
		if (obj.controllingPlayer != null)
		{
			if (obj.controllingPlayer == this) return false;
			else
			{
				obj.controllingPlayer._capturedObjectives.Remove(obj);
			}
		}
		obj.controllingPlayer = this;
		_capturedObjectives.Add(obj);
		return true;
	}

	public void DeployUnit(Unit unit)
	{
		if (unit.Owner != this || _activeUnits.Contains(unit)) throw new ArgumentException();
		TotalDeployedUnits++;
		_activeUnits.Add(unit);
	}
	
	public void DestroyUnit(Unit unit)
	{
		// TODO: some pretty nasty code fragmentation here - probably move this with the rest of combat code.
		if (!unit.IsDead()) throw new ArgumentException("Attempted to destroy non-dead unit");
		
		if (unit.Owner != this) DestroyedUnits++;
		else 					SelfDestroyedUnits++;

		if (!unit.Owner._activeUnits.Remove(unit))
		{
			throw new ArgumentException("Attempted to destory already destroyed unit");
		}
		
		unit.Kill();
	}
}
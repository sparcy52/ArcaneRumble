using System.Collections.Generic;
using Model;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public string Name;
	public Color Color;
	public Loadout Loadout;
	
	public int CapturedObjectives { get { return _capturedObjectives.Count; } }
	private readonly List<Objective> _capturedObjectives = new List<Objective>();
	
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
}
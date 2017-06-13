using System;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Represents a set of abilities and unit types which may differ from player to player.
/// By extension, this should be used to specify classes/factions.
/// </summary>
[CreateAssetMenu(fileName = "Assets/NewLoadout")]	// TODO: choose a better folder
public class Loadout : ScriptableObject
{
	// TODO: this class seems redundant right now... but i make individual loadouts so goddamn often that it's worth it
	public GameObject[] Units;
	// TODO: any class related stuff can go right here!

	private void OnValidate()
	{
		// this is a static check to ensure that bad prefabs are not added
		foreach (var unit in Units)
		{
			if (unit == null)
			{
				Debug.LogError(
					string.Format("Null prefab in Loadout {0}", unit)
				);				
			}
			else if (unit.GetComponent<UnitAvatar>() == null)
			{
				Debug.LogError(
					string.Format("Prefab {0} in Loadout {1} does not contain a UnitAvatar script.", unit, this)
				);
			}
		}
	}
}
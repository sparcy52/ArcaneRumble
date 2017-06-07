using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public string Name;
	public Color Color;
	public Loadout Loadout;

	public GameObject[] Units	// simple alias for the loadout
	{
		get { return Loadout.Units; }
	}
}
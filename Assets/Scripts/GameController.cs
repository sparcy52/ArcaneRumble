using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{	
	public Player[] Players = new Player[2];

    public int MapSize = 20;
    public int NumberOfMaps = 1;

	private WorldController _worldController;

	public void DoTurn()
	{
		_worldController.DoTurn();
	}
	
	private void Start()
	{
        int map = Random.Range(0, NumberOfMaps);
		var world = new World(map, Players);
		_worldController = new WorldController(world);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote)) DoTurn();
	}

	/// <summary>
	/// Creates a new Unit from a prefab. This includes adding it to the game logic, and creating a visual
	/// representation. The prefab provided should have a UnitAvatar script attatched, as well as any
	/// requirements of that script. This operation will fail if the given position is already occupied.
	/// </summary>
	/// <param name="unitPrefab">the prefab of the unit to create</param>
	/// <param name="pos">the TileVector position to spawn it</param>
	/// <param name="dir">the Direction for it to be facing</param>
	/// <param name="owner">the Player owner of the Unit</param>
	/// <returns>true if the operation was successful; false if not</returns>
	public bool MakeUnit(GameObject unitPrefab, TileVector pos, CardinalDirection dir, Player owner)
	{
		UnitAvatar avatar = Instantiate(unitPrefab).GetComponent<UnitAvatar>();

		var rend = avatar.gameObject.GetComponentsInChildren<MeshRenderer>();

		foreach (var r in rend) {
			foreach (var m in r.materials) {
				if (m.HasProperty ("_Color")) {
					m.color = owner.Color;
				}
			}
		}
			
		Unit unit = new Unit(avatar, pos, dir, owner);
		avatar.SetUnit(unit);

		if (!_worldController.AddUnit(unit))	// oops! bad unit placement, so delete the unit as if nothing happened
		{
			Destroy(avatar.gameObject);
			return false;
		}
		else return true;
	}
	
	/*
	private void RenderWorld(World world)
	{
		for (var w = 0; w < world.W; w++)
		{
			for (var e = 0; e < world.E; e++)
			{
				var hex = world[w, e];
				if (hex != null)
				{
					var tile = Instantiate(HexModels[(int) hex.Type]);
					tile.transform.position = new TileVector(w, e).ToVector3();
					_hexInstances.Add(tile);
				}
			}
		}
	}
	private void CleanWorld()
	{
		if (instancedTiles == null) {
			instancedTiles = GameObject.FindGameObjectsWithTag ("EditorTile");
		}
		foreach (GameObject tile in instancedTiles) {
			DestroyImmediate (tile);
		}
	}
	*/
}

﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{	
	public WorldGenerator WorldGenerator;
	public World World { get { return WorldGenerator.World; }}
	
	public Player[] Players = new Player[2];
	private Dictionary<Player, bool> _playerUnitPlaced; // TODO: refactor resource management into Player class

    public int MapSize = 20;
    public int NumberOfMaps = 1;

	public int RoundNumber { get { return _worldController != null ? _worldController.RoundNumber : 0; } }
	private WorldController _worldController;

	public void DoTurn()
	{
		_worldController.DoTurn();
	}
	
	private void Start()
	{
		_worldController = new WorldController(WorldGenerator.World);
		_playerUnitPlaced = new Dictionary<Player, bool>();
		foreach (var player in Players)
		{
			_playerUnitPlaced[player] = false;
		}
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
	/// <param name="mirrored">whether or not the Unit is mirrored</param>
	/// <returns>true if the operation was successful; false if not</returns>
	public bool MakeUnit(GameObject unitPrefab, Player owner, TileVector pos, CardinalDirection dir, bool mirrored)
	{
		if (_playerUnitPlaced[owner]) return false;	// stop players placing more than one unit
		
		var avatar = Instantiate(unitPrefab).GetComponent<UnitAvatar>();			
		var unit = avatar.CreateUnit(owner, pos, dir, mirrored);
		
		if (!_worldController.AddUnit(unit)) // oops! bad unit placement, so delete the unit as if nothing happened
		{
			Destroy(avatar.gameObject);
			return false;
		}
		else 	// successful placement
		{
			avatar.SetUnit(unit);
			_playerUnitPlaced[owner] = true;	// set player as placed a unit
			
			var allPlaced = true;				// check if all players have placed a unit
			foreach (var placed in _playerUnitPlaced.Values)
			{
				if (!placed)
				{
					allPlaced = false;
					break;
				}
			}
			if (allPlaced)						// reset players' placement status and run game
			{
				_worldController.DoTurn();
				foreach (var player in Players)
				{
					_playerUnitPlaced[player] = false;
				}
			}
			return true;
		}
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

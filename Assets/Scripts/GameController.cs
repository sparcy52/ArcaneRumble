using System.Collections.Generic;
using UnityEngine;
using Model;

public class GameController : MonoBehaviour
{
	public GameObject[] TestUnits;

	public GameObject[] HexModels;
	private List<GameObject> _hexInstances = new List<GameObject>();

	public int UnitCount = 3;

	public Player[] players = new Player[2];

    public int mapsize = 20;
    public int numberOfMaps = 1;

	private WorldController _worldController;

	public Color player_1_Unit_Color = Color.red;		//player 1's unit color
	public Color player_2_Unit_Color = Color.blue;		//player 2's unit color

	private GameObject go;

	private void Start()
	{
        int map = Random.Range(0, numberOfMaps);
		var world = new World(map, mapsize);
		_worldController = new WorldController(world);
		RenderWorld(world);
		players[0] = new Player(1);
		players[1] = new Player(2);

		for (var i = 0; i < UnitCount; i++)
		{
			var pos = i % 2 == 0 ? new TileVector(i, 0) : new TileVector(0, i);
			MakeUnit(TestUnits[i%TestUnits.Length], pos, CardinalDirection.North, players[i%2]);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z)) _worldController.DoTurn();
	}

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

		Renderer[] rend = avatar.gameObject.GetComponentsInChildren<MeshRenderer>();

		foreach (Renderer r in rend) {
			foreach (Material m in r.materials) {
				if (m.HasProperty ("_Color")) {
					if (owner.num == 1) {
						m.color = player_1_Unit_Color;
					} else {
						m.color = player_2_Unit_Color;
					}
				}
			}
		}
			
		Unit unit = new Unit(avatar, pos, dir, owner);
		owner.AddUnit (unit);
		avatar.SetPositionAndOrientation(pos, dir);

		if (!_worldController.AddUnit(unit))	// oops! bad unit placement, so delete the unit as if nothing happened
		{
			Destroy(avatar.gameObject);
			return true;
		}
		else return true;
	}
}

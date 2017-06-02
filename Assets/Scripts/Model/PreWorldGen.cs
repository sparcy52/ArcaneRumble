using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PreWorldGen : MonoBehaviour {

    
    public int NumberOfMaps = 1;
    public int MapSize = 20;
    public GameObject[] HexModels;
    private List<GameObject> _hexInstances = new List<GameObject>();
    private GameObject[] instancedTiles;
    public Player[] TempPlayers = new Player[2];

    void Start () {
        TempPlayers[0] = new Player(1);
        TempPlayers[1] = new Player(2);
        int map = Random.Range(0, NumberOfMaps);
        var world = new World(map, TempPlayers);
        CleanWorld();
        RenderWorld(world);
    }
	
	// Update is called once per frame
	void Update () {
		
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
                    var tile = Instantiate(HexModels[(int)hex.Type], this.transform, true);
                    tile.tag = "Tile";
                    tile.transform.position = new TileVector(w, e).ToVector3();
                    _hexInstances.Add(tile);
                    // Debug.Log(tile.ToString() + w + e);
                }
            }
        }
    }
    private void CleanWorld()
    {
		if (instancedTiles == null) {
			instancedTiles = GameObject.FindGameObjectsWithTag ("Tile");
		}
		foreach (GameObject tile in instancedTiles) {
			DestroyImmediate (tile);
		}
    }
}

using System;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PreWorldGen : MonoBehaviour {

    public GameController GameController;
    public int NumberOfMaps = 1;
    public int MapSize = 20;
    public GameObject[] HexModels;
    private List<GameObject> _hexInstances = new List<GameObject>();
    private GameObject[] instancedTiles;

    void Start () {
        if (GameController == null) throw new Exception("Please link a GameController to the PreWorldGen script");
        int map = Random.Range(0, NumberOfMaps);
        var world = new World(map, GameController.Players);
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

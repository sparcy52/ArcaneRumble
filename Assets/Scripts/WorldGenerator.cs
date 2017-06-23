using System;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour {

    public GameController GameController;
    public int NumberOfMaps = 1;
    public int MapSize = 20;
    public GameObject[] HexModels;
    private List<GameObject> _hexInstances = new List<GameObject>();
    private GameObject[] instancedTiles;
    public TextAsset[] maplist;

    public World World
    {
        get
        {
            if (_world == null)
            {
                _world = new World(maplist, GameController.Players);
            }
            return _world;
        }
    }
    private World _world;
        
    void Start () {
        if (GameController == null) throw new Exception("Please link a GameController to the WorldGenerator script");
        // int map = Random.Range(0, NumberOfMaps);
        CleanWorld();
        RenderWorld(World);
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
                    if(hex.Type == HexType.Objective)
                    {
                        tile.GetComponent<DObjective>().setHex(hex);
                    }
                   /* if(hex.Type == HexType.Deploy)
                    {
                        //tile.GetComponent<Renderer>().material.color = Color.red;
                        //tile.GetComponent<Renderer>().material.color = hex.Owner.Colour
                    }
                    // Debug.Log(tile.ToString() + w + e);*/
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

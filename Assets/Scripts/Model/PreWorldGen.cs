using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PreWorldGen : MonoBehaviour {

    
    public int NumberOfMaps = 1;
    public int MapSize = 20;
    private WorldController _worldController;
    public GameObject[] HexModels;
    private List<GameObject> _hexInstances = new List<GameObject>();
    private GameObject[] instancedTiles;

    void Awake () {
        int map = Random.Range(0, NumberOfMaps);
        var world = new World(map);
        _worldController = new WorldController(world);
        int numberOfObjects = world.E * world.W;
        instancedTiles = new GameObject[numberOfObjects];
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
                    var tile = Instantiate(HexModels[(int)hex.Type]);
                    tile.transform.position = new TileVector(w, e).ToVector3();
                    _hexInstances.Add(tile);
                    Debug.Log(tile.ToString() + w + e);
                }
            }
        }
    }
    private void CleanWorld()
    {
        foreach(GameObject obj in _hexInstances)
        {
            Debug.Log(obj.ToString());
            Destroy(obj);
            
        }
    }
}

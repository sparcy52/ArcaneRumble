using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DObjective : MonoBehaviour {
    public GameObject CurrentObjective;
    private Hex objectiveHex;
    private Renderer[] orenderer;

    public void setHex (Hex hex)
    {
        objectiveHex = hex;
    }
	void Start () {
        CurrentObjective = this.gameObject;
        orenderer = CurrentObjective.GetComponentsInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(objectiveHex.Owner != null)
        {
            orenderer[9].material.color = objectiveHex.Objective.controllingPlayer.Color;
        }
        //orenderer.material.color = objectiveHex.Owner.Color;
        

    }
}

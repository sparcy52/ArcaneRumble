using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DObjective : MonoBehaviour {
    public GameObject CurrentObjective;
    private Hex objectiveHex;
    private Renderer orenderer;

    public void setHex (Hex hex)
    {
        objectiveHex = hex;
    }
	void Start () {
        CurrentObjective = this.gameObject;
        orenderer = CurrentObjective.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        //orenderer.material.color = objectiveHex.Owner.Color;
        orenderer.material.color = Color.red;
    }
}

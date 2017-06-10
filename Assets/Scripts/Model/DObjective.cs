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
        if (objectiveHex.Owner != null && objectiveHex.Occupant.Owner != null)
        {
            if (objectiveHex.Occupant.Owner != objectiveHex.Owner)
            {
                orenderer.material.color = Color.red;
                //orenderer.material.color = ojbectiveHex.Occupant.Owner.Colour;
                objectiveHex.Owner = objectiveHex.Occupant.Owner;
            }
        }
        //else do nothing
	}
}

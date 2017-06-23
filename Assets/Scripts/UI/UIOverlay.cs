using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

    public GameController gameController;
    public Player p1;
    public Player p2;

    public Text roundNumber;
    public Text p1objectives;
    public Text p2objectives;
    public Text p1killed;
    public Text p2killed;

    private string roundNum;
    private string p1objs;
    private string p2objs;
    private string p1kill;
    private string p2kill;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
        roundNum = "" + gameController.RoundNumber.ToString();
        roundNumber.text = roundNum;

        p1objs = "objectives " + p1.CapturedObjectives.ToString();
        p1objectives.text = p1objs;

        p2objs = "objectives " + p2.CapturedObjectives.ToString();
        p2objectives.text = p2objs;

        p1kill = "mechs destroyed " + p1.DestroyedUnits.ToString();
        p1killed.text = p1kill;

        p2kill = "mechs destroyed " + p2.DestroyedUnits.ToString();
        p2killed.text = p2kill;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;

public class UnitPlacer2 : MonoBehaviour {

	public GameObject[] Units;
	public GameController gameController;

	private bool lockInHex = false;		
	private bool lockUp = false;		//1
	private bool lockDown = false;		//1

	private bool light_l = false;		//2
	private bool mid_l = false;			//2
	private bool heavy_l = false;		//2

	public GUISkin customSkin1;

	private int currentPlayer = 0;

	Transform _t;
	TileVector _pos = new TileVector(0,5);
	CardinalDirection _facingUp = CardinalDirection.Northeast;
	CardinalDirection _facingDown = CardinalDirection.Southeast;


	// Use this for initialization
	void Start () {
		_t = GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {

		if (lockInHex == false) {
			if (Input.GetKeyDown (KeyCode.O))
				MovePos (CardinalDirection.North);
			if (Input.GetKeyDown (KeyCode.L))
				MovePos (CardinalDirection.South);
			if (Input.GetKeyDown (KeyCode.K))
				MovePos (CardinalDirection.Southwest);
			if (Input.GetKeyDown (KeyCode.Semicolon))
				MovePos (CardinalDirection.Southeast);
			if (Input.GetKeyDown (KeyCode.I))
				MovePos (CardinalDirection.Northwest);
			if (Input.GetKeyDown (KeyCode.P))
				MovePos (CardinalDirection.Northeast);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
			lockInHex = !lockInHex;
		}

		for (int i = 0; i < Units.Length; i++) {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				mid_l = true;
			} 
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				light_l = true;
			} 
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				heavy_l = true;
			} else if (Input.GetKeyDown (KeyCode.M)) {
				lockUp = true;
			} else if (Input.GetKeyDown (KeyCode.Comma)) {
				lockDown = true;
			}
			else if (lockInHex == true && mid_l == true && lockUp == true) {
				PlaceMech (Units [1], _facingUp);
			}
			else if (lockInHex == true && mid_l == true && lockDown == true) {
				PlaceMech (Units [1], _facingDown);
			}
			else if (lockInHex == true && light_l == true && lockUp == true) {
				PlaceMech (Units [0], _facingUp);
			}
			else if (lockInHex == true && light_l == true && lockDown == true) {
				PlaceMech (Units [0], _facingDown);
			}
			else if (lockInHex == true && heavy_l == true && lockUp == true) {
				PlaceMech (Units [2], _facingUp);
			}
			else if (lockInHex == true && heavy_l == true && lockDown == true) {
				PlaceMech (Units [2], _facingDown);
			}

		}

	}

	//	void OnGUI(){
	//		GUI.skin = customSkin1;
	////		if (GUI.Button (new Rect (Screen.width / 2.5f, Screen.height / 20, 120, 30), "Start")) {
	////			Debug.Log ("Start button is pressed");
	////		}
	//		for (int i = 0; i < Units.Length; i++) {
	//			if (lockInHex == true && GUI.Button (new Rect (Screen.width / 20, Screen.height / 20 + Screen.height / 8.5f * i, 100, 25), Units [i].name)) {
	//					PlaceMech (Units[i]);
	//			}
	//
	//		}
	//	}

	void MovePos(CardinalDirection direction) {
		_pos = _pos + direction;
		_t.position = _pos.ToVector3 ();

	}

	void Reset(){
		lockInHex = false;		
		lockUp = false;			//1
		lockDown = false;		//1
		light_l = false;		//2
		mid_l = false;			//2
		heavy_l = false;		//2
	}


	public void PlaceMech(GameObject GameUnit, CardinalDirection _facing)
	{
		Reset ();
		if (gameController.MakeUnit(GameUnit, _pos, _facing, gameController.Players[1]))
		{
			//			currentPlayer = (currentPlayer + 1) % gameController.Players.Length;
			if (currentPlayer == 0) gameController.DoTurn();
		}
	}



}

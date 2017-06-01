using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;

public class UnitPlacer1 : MonoBehaviour {

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
	TileVector _pos = new TileVector(0,0);
	CardinalDirection _facingUp = CardinalDirection.Northeast;
	CardinalDirection _facingDown = CardinalDirection.Southeast;


	// Use this for initialization
	void Start () {
		_t = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (lockInHex == false) {
			if (Input.GetKeyDown (KeyCode.W))
				MovePos (CardinalDirection.North);
			if (Input.GetKeyDown (KeyCode.S))
				MovePos (CardinalDirection.South);
			if (Input.GetKeyDown (KeyCode.A))
				MovePos (CardinalDirection.Southwest);
			if (Input.GetKeyDown (KeyCode.D))
				MovePos (CardinalDirection.Southeast);
			if (Input.GetKeyDown (KeyCode.Q))
				MovePos (CardinalDirection.Northwest);
			if (Input.GetKeyDown (KeyCode.E))
				MovePos (CardinalDirection.Northeast);
		}

		if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.B)) {
			lockInHex = true;
		}

		//first way
//		for (int i = 0; i < Units.Length; i++) {
//			if (Input.GetKeyDown (KeyCode.F) && Input.GetKey (KeyCode.V)) {
//				PlaceMech (Units [1], _facingUp);
//			}
//			if (Input.GetKeyDown (KeyCode.G) && Input.GetKey (KeyCode.V)) {
//				PlaceMech (Units [1], _facingDown);
//			}
//			if (Input.GetKeyDown (KeyCode.F) && Input.GetKey (KeyCode.C)) {
//				PlaceMech (Units [0], _facingUp);
//			}
//			if (Input.GetKeyDown (KeyCode.G) && Input.GetKey (KeyCode.C)) {
//				PlaceMech (Units [0], _facingDown);
//			}
//			if (Input.GetKeyDown (KeyCode.F) && Input.GetKey (KeyCode.B)) {
//				PlaceMech (Units [2], _facingUp);
//			}
//			if (Input.GetKeyDown (KeyCode.G) && Input.GetKey (KeyCode.B)) {
//				PlaceMech (Units [2], _facingDown);
//			}
//		}

		//second way
		for (int i = 0; i < Units.Length; i++) {
			if (Input.GetKeyDown (KeyCode.V)) {
				mid_l = true;
			} 
			if (Input.GetKeyDown (KeyCode.C)) {
				light_l = true;
			} 
			if (Input.GetKeyDown (KeyCode.B)) {
				heavy_l = true;
			} else if (Input.GetKeyDown (KeyCode.F)) {
				lockUp = true;
			} else if (Input.GetKeyDown (KeyCode.G)) {
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
			

//		if (Input.GetKeyDown (KeyCode.F)) {
//			if (lockInHex == true && lockUp == true) {
//				PlaceMech (Units [1], _facingUp);
//			}
//		}
//		if (Input.GetKeyDown (KeyCode.G)) {
//			if (lockInHex == true && lockDown == true) {
//				PlaceMech (Units [1], _facingDown);
//			}
//		}
//
//		if (Input.GetKeyDown (KeyCode.B)) {
//			if (lockInHex == true && Input.GetKeyDown (KeyCode.W)) {
//				PlaceMech (Units [2],_facingUp);
//			}
//			if (lockInHex == true && Input.GetKeyDown (KeyCode.S)) {
//				PlaceMech (Units [2],_facingDown);
//			}
//		}

//		if (lockInHex == true && SelectedUnit != null) {
//			if (Input.GetKeyDown(KeyCode.S)) {
//				
//			} else if (Input.GetKeyUp(KeyCode.W)) {
//				
//			}
//		}

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
		if (gameController.MakeUnit(GameUnit, _pos, _facing, gameController.Players[0]))
		{
//			currentPlayer = (currentPlayer + 1) % gameController.Players.Length;
			if (currentPlayer == 0) gameController.DoTurn();
		}
	}



}

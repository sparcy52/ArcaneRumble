using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;

public class UnitPlacer : MonoBehaviour {

	public GameObject[] Units;
	public GameController gameController;
	private GameObject SelectedUnit;
	private bool lockInHex = false;


	Transform _t;
	TileVector _pos = new TileVector(0,0);
	CardinalDirection _facing = CardinalDirection.North;

	// Use this for initialization
	void Start () {
		_t = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
//
//		Buttons = Buttons.GetComponent<Button>();
//		Buttons.onClick.AddListener(SelectUnit);

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

		if (Input.GetKeyDown(KeyCode.Space)) {
			lockInHex = true;
		}

		if (Input.GetKeyDown(KeyCode.B)) {
			lockInHex = false;
		}

//		if (lockInHex == true && SelectedUnit != null) {
//			if (Input.GetKeyDown(KeyCode.S)) {
//				
//			} else if (Input.GetKeyUp(KeyCode.W)) {
//				
//			}
//		}

	}

	void OnGUI(){
		if (GUI.Button (new Rect (Screen.width / 2.5f, Screen.height / 20, 120, 30), "Start")) {
			Debug.Log ("Start button is pressed");
		}
		for (int i = 0; i < Units.Length; i++) {
			if (lockInHex == true && GUI.Button (new Rect (Screen.width / 20, Screen.height / 20 + Screen.height / 8.5f * i, 100, 25), Units [i].name)) {
					PlaceMech (Units[i]);
					Debug.Log ("123");
			}

		}
	}

	void MovePos(CardinalDirection direction) {
		_pos = _pos + direction;
		_t.position = _pos.ToVector3 ();

	}


	public void PlaceMech(GameObject GameUnit){
		gameController.MakeMech (GameUnit,_pos,_facing,gameController.players[0]);
	}



}

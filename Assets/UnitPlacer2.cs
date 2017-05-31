using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;

public class UnitPlacer2 : MonoBehaviour {

	public GameObject[] Units;
	public GameController gameController;
	private GameObject go;
	private bool lockInHex = false;

	public GUISkin customSkin1;

	private int currentPlayer = 0;

	Transform _t;
	TileVector _pos = new TileVector(0,0);
	CardinalDirection _facing = CardinalDirection.North;

	// Use this for initialization
	void Start () {
		_t = GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {

		if (lockInHex == false) {
			if (Input.GetKeyDown (KeyCode.P))
				MovePos (CardinalDirection.North);
			if (Input.GetKeyDown (KeyCode.Semicolon))
				MovePos (CardinalDirection.South);
			if (Input.GetKeyDown (KeyCode.L))
				MovePos (CardinalDirection.Southwest);
			if (Input.GetKeyDown (KeyCode.Quote))
				MovePos (CardinalDirection.Southeast);
			if (Input.GetKeyDown (KeyCode.O))
				MovePos (CardinalDirection.Northwest);
			if (Input.GetKeyDown (KeyCode.LeftBracket))
				MovePos (CardinalDirection.Northeast);
		}

		if (Input.GetKeyDown(KeyCode.Return)) {
			lockInHex = !lockInHex;
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

	void OnGui(){
		GUI.skin = customSkin1;

		for (int i = 0; i < Units.Length; i++) {
			if (lockInHex == true && GUI.Button (new Rect (Screen.width / 1.25f, Screen.height / 20 + Screen.height / 8.5f * i, 100, 25), Units [i].name)) {
				PlaceMech (Units[i]);
			}

		}
	}

	void MovePos(CardinalDirection direction) {
		_pos = _pos + direction;
		_t.position = _pos.ToVector3 ();

	}


	public void PlaceMech(GameObject GameUnit)
	{
		lockInHex = false;
		if (gameController.MakeUnit(GameUnit, _pos, _facing, gameController.Players[currentPlayer]))
		{
			currentPlayer = (currentPlayer + 1) % gameController.Players.Length;
			if (currentPlayer == 0) gameController.DoTurn();
		}
	}

}

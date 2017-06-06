using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;

public class UnitPlacer : MonoBehaviour {

	public GUISkin CustomSkin1;
	
	public GameController GameController;
	public int PlayerNum = 0;	// TODO: refine semantics
	public Player Player;
	
	/// <summary>
	/// List of units availible to this Placer
	/// </summary>
	public GameObject[] Units;

	/// <summary>
	/// Key bindings to select the units in the array.
	/// </summary>
	public KeyCode[] UnitSelectionKeys = {
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
	};

	public KeyCode MoveNorthKey 				= KeyCode.W; 
	public KeyCode MoveNortheastKey				= KeyCode.E; 
	public KeyCode MoveSoutheastKey				= KeyCode.D; 	
	public KeyCode MoveSouthKey					= KeyCode.S; 
	public KeyCode MoveSouthwestKey				= KeyCode.A; 
	public KeyCode MoveNorthwestKey				= KeyCode.Q; 
	
	public KeyCode RotateAnticlockwiseKey		= KeyCode.Z;
	public KeyCode RotateClockwiseKey			= KeyCode.X;

	private Transform _t;
	private GameObject _preview;
	
	private int _selectedUnit 					= -1;
	private TileVector _selectedPos 			= new TileVector(0, 0);
	private CardinalDirection _selectedDir 		= CardinalDirection.North;

	// Use this for initialization
	void Start () {
		_t = GetComponent<Transform> ();
		Player = GameController.Players[PlayerNum];
	}
	
	// Update is called once per frame
	void Update () {
		Player = GameController.Players[PlayerNum];	// TODO: remove this hack
		
		// MOVEMENT
		if (Input.GetKeyDown(MoveNorthKey)) 	MovePos(CardinalDirection.North);
		if (Input.GetKeyDown(MoveNortheastKey)) MovePos(CardinalDirection.Northeast);
		if (Input.GetKeyDown(MoveSoutheastKey)) MovePos(CardinalDirection.Southeast);
		if (Input.GetKeyDown(MoveSouthKey)) 	MovePos(CardinalDirection.South);
		if (Input.GetKeyDown(MoveSouthwestKey)) MovePos(CardinalDirection.Southwest);
		if (Input.GetKeyDown(MoveNorthwestKey)) MovePos(CardinalDirection.Northwest);
		
		if (Input.GetKeyDown(RotateAnticlockwiseKey)) RotateDir(RelativeDirection.ForwardLeft);
		if (Input.GetKeyDown(RotateClockwiseKey)) 	  RotateDir(RelativeDirection.ForwardRight);

		// UNIT SELECTION / PLACEMENT
		for (var i = 0; i < UnitSelectionKeys.Length; i++)
		{
			if (Input.GetKeyDown(UnitSelectionKeys[i]))
			{
				if (_selectedUnit == i)	// place unit on double tap of selection key
				{
					if (GameController.MakeUnit(Units[i], _selectedPos, _selectedDir, Player))
					{
						_selectedUnit = -1;		// unit creation sucessful - deallocate preview
						Destroy(_preview);
						_preview = null;
					}
				}
				else 					// select another unit
				{
					_selectedUnit = i;
					if (_preview != null) Destroy(_preview);
					_preview = Instantiate(Units[i], _t, false);
					_preview.transform.localPosition = Vector3.zero;
					_preview.transform.localRotation = Quaternion.identity;
					break;
				}
			}
		}
	}

	private void MovePos(CardinalDirection direction) {
		_selectedPos = _selectedPos + direction;
		_t.position = _selectedPos.ToVector3();
	}

	private void RotateDir(RelativeDirection direction)
	{
		_selectedDir = _selectedDir.Turn(direction);
		_t.rotation = _selectedDir.GetBearingRotation();
	}
}

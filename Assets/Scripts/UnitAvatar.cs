using System;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class UnitAvatar : MonoBehaviour
{
	// Behavioural and stat variables
	public int MaxHealth;
	public int MaxEnergy;

	private GameObject _hpBar;
	private GameObject _eBar;
	private Unit _unit;
	private BarScript _hpScript;
	private BarScript _eScript;

	public UnitAi Ai;

	// Comsmetic variables
	public float MoveSpeed = 5f;
	public float TurnSpeed = 180f;

	// Unity components
	public Animator Animator { get; protected set; }
	//public Transform Transform { get; protected set; }

	// Movement state trackers
	private Queue<IAnimation> _moveQueue = new Queue<IAnimation>();
	public IAnimation CurrentAnimation { get { return _moveQueue.Count > 0 ? _moveQueue.Peek() : null; } }

	private bool _kill = false;

	public Quaternion Rotation
	{
		set { transform.rotation = value; }
		get { return transform.rotation; }
	}

	public Vector3 Position
	{
		set { transform.position = value; }
		get { return transform.position; }
	}

	public float HpPercent {
		get { return (float) _unit.Health / _unit.MaxHealth; }
	}

	public float EpPercent {
		get { return (float) _unit.Energy / _unit.MaxEnergy; }
	}

	protected void Start ()
	{
		// IMPLEMENTATION NOTE: defer functionality from Start to SetUnit
		Animator = GetComponent<Animator>();
	}

	/// <summary>
	/// Effectively a constructor to be called after GameObjects with this Script are instantiated.
	/// </summary>
	/// <param name="unit">The Unit to base this off</param>
	public void SetUnit(Unit unit)
	{
		if (unit.Avatar != this) throw new ArgumentException("Assigned Unit already has an Avatar");
		_unit = unit;
		Position = unit.Position.ToVector3();
		Rotation = unit.Facing.GetBearingRotation();
		
		// paint unit
		var rend = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (var r in rend) {
			foreach (var m in r.materials) {
				if (m.HasProperty ("_Color")) {
					m.color = _unit.Owner.Color;
				}
			}
		}

		// make UI elements
		_hpBar = Instantiate(GuiComponents.GetHpBar ());
		_eBar = Instantiate(GuiComponents.GetEpBar ());
		_hpScript =	_hpBar.GetComponent<BarScript> ();
		_eScript =	_eBar.GetComponent<BarScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_unit == null) return;

//		HpPercent = Mathf.Clamp((float) _unit.Health / _unit.MaxHealth,0f ,1f);
//		Debug.Log ("current health: "+_unit.Health);
//		Debug.Log ("Max health: "+_unit.MaxHealth);
//		Debug.Log ("Percent: "+HpPercent);
		_hpBar.transform.position = new Vector3 (this.Position.x, this.Position.y+0.6f, this.Position.z-1f);
		_hpScript.SetPercent (HpPercent);
//		Debug.Log (HpPercent);
//		EpPercent = (float) _unit.Energy / _unit.MaxEnergy;
//		Debug.Log (_unit.Energy);
		_eBar.transform.position = new Vector3 (this.Position.x, this.Position.y+0.4f, this.Position.z-1f);
		_eScript.SetPercent (EpPercent);
	}



	public void Kill()
	{
		_kill = true;
	}

	void CleanUp(){
		Destroy(gameObject);
		Destroy (_hpBar);
		Destroy (_eBar);
	}

	void FixedUpdate()
	{
		if (_moveQueue.Count > 0)
		{
			if (_moveQueue.Peek().ApplyAnimation(Time.deltaTime))
			{
				_moveQueue.Dequeue();
			}
		} 
		else if (_kill)
		{
			CleanUp ();
		}
	}

	public void ApplyMove(Move move)
	{
		if (move.IsHalt()) return;
		_moveQueue.Enqueue(new MoveAnimation(this, move.Destination, move.Unit.Facing.Turn(move.Direction)));
	}

	public void ApplyCombat(Unit otherUnit, TileVector attackPosition)
	{
		//if (move.IsHalt()) return;
		_moveQueue.Enqueue(new CombatAnimation(_unit, otherUnit));
	}
}
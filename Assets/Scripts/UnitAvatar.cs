﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class UnitAvatar : MonoBehaviour
{
	// Behavioural and stat variables
	public int MaxHealth;
	public int MaxEnergy;

	private GameObject _hpBar;
	private GameObject _epBar;
	private Unit _unit;
	private BarScript _hpScript;
	private BarScript _eScript;

	public UnitAi Ai;

	// Comsmetic variables
	public float MoveSpeed = 5f;
	public float TurnSpeed = 180f;

	// Unity components
	public Animator Animator { get; protected set; }
	private MeshRenderer[] _renderers;

	// Movement state trackers
	private Queue<IAnimation> _animQueue = new Queue<IAnimation>();
	public IAnimation CurrentAnimation { get { return _animQueue.Count > 0 ? _animQueue.Peek() : null; } }

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

	private Vector3 _initScale;
	private float _scaleMod = 1;
	
	public float Scale
	{
		set
		{
			_scaleMod = value;
			transform.localScale = _initScale * value;
		}
		get { return _scaleMod; }
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
		_initScale = transform.localScale;
	}

	/// <summary>
	/// Create a Model Unit which is specified by the parameters of this Avatar.
	/// </summary>
	public Unit CreateUnit(Player owner, TileVector position, CardinalDirection facing, bool mirrored)
	{
		return new Unit(this, owner, position, facing, mirrored);
	}
	
	/// <summary>
	/// Links this Avatar to a Unit which it represents. This serves as a constructor in practice, 
	/// moving this class from a template which defines a Unit, to a visual representation of it.
	/// </summary>
	/// <param name="unit">The Unit to link this too</param>
	public void SetUnit(Unit unit)
	{
		if (unit.Avatar != this) throw new ArgumentException("Assigned Unit has a different Avatar");
		if (_unit != null) throw new ArgumentException("This Avatar has already been assigned a Unit");
		
		_unit = unit;
		Position = unit.Position.ToVector3();
		Rotation = unit.Facing.GetBearingRotation();		

		// make UI elements
		_hpBar = Instantiate(GuiComponents.GetHpBar ());
		_epBar = Instantiate(GuiComponents.GetEpBar ());
		_hpScript =	_hpBar.GetComponent<BarScript> ();
		_eScript =	_epBar.GetComponent<BarScript> ();
		
		_renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		ResetPaint();
	}
	
	// Update is called once per frame
	void Update () {
		if (_unit == null) return;

		_hpBar.transform.position = new Vector3 (this.Position.x, this.Position.y+0.6f, this.Position.z-1f);
		_hpScript.SetPercent (HpPercent);
		_epBar.transform.position = new Vector3 (this.Position.x, this.Position.y+0.4f, this.Position.z-1f);
		_eScript.SetPercent (EpPercent);
	}

	public void Paint(Color color)
	{
		foreach (var r in _renderers) {
			foreach (var m in r.materials) {
				if (m.HasProperty ("_Color"))
				{
					m.color = color;
				}
			}
		}
	}

	public void ResetPaint()
	{
		Paint(_unit.Owner.Color);
	}

	public void Kill()
	{
		_animQueue.Enqueue(new DestroyAnimation(this));
	}

	public void Destroy(){
		Destroy(gameObject);
		Destroy (_hpBar);
		Destroy (_epBar);
	}

	void FixedUpdate()
	{
		if (_animQueue.Count > 0)
		{
			if (_animQueue.Peek().ApplyAnimation(Time.deltaTime))
			{
				_animQueue.Dequeue();
			}
		}
	}

	public void ApplyMove(Move move)
	{
		if (move.IsHalt()) return;
		_animQueue.Enqueue(new MoveAnimation(this, move.Destination, move.Unit.Turn(move.Direction)));
	}

	public void ApplyCombat(Unit otherUnit, TileVector attackPosition)
	{
		//if (move.IsHalt()) return;
		_animQueue.Enqueue(new CombatAnimation(_unit, otherUnit));
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class UnitAvatar : MonoBehaviour
{
	// Behavioural and stat variables
	public int MaxHealth;
	public int MaxEnergy;

	private float HpPercent;
	private GameObject _hpBar;
	private Unit _unit;

	public UnitAi Ai;

	// Comsmetic variables
	public float MoveSpeed = 5f;
	public float TurnSpeed = 180f;

	// Unity components
	public Animator Animator { get; protected set; }
	//public Transform Transform { get; protected set; }

	// Movement state trackers
	private Queue<MoveAnimation> _moveQueue = new Queue<MoveAnimation>();

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

	protected void Start ()
	{
		// IMPLEMENTATION NOTE: defer functionality from Start to SetUnit
		Animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if (_unit == null) return;
		HpPercent = _unit.Health / _unit.MaxHealth;
		_hpBar.transform.position = new Vector3 (this.Position.x, this.Position.y+3f, this.Position.z);
		SetHpBar (HpPercent);	
	}

	public void SetHpBar(float health){
		_hpBar.transform.localScale = new Vector3 (health,_hpBar.transform.localScale.y,_hpBar.transform.localScale.z);
	}

	public void Kill()
	{
		_kill = true;
	}

	private void OnDestroy()
	{
		Destroy (_hpBar);
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
			Destroy(this.gameObject);
		}
	}

	public void SetUnit(Unit unit)
	{
		_unit = unit;
		Position = unit.Position.ToVector3();
		Rotation = unit.Facing.GetBearingRotation();
		
		_hpBar = Instantiate(GuiComponents.GetHpBar ());
	}

	public void ApplyMove(Move move)
	{
		if (move.IsHalt()) return;
		_moveQueue.Enqueue(new MoveAnimation(this, move.Destination, move.Unit.Facing.Turn(move.Direction)));
	}
}
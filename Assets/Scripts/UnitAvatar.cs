using System;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class UnitAvatar : MonoBehaviour
{
	// Behavioural and stat variables
	public int MaxHealth;
	public int MaxEnergy;

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
		//Transform = GetComponent<Transform>();
		Animator = GetComponent<Animator>();
	}

	public void Kill()
	{
		_kill = true;
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
			Destroy(gameObject);
		}
	}

	public void SetPositionAndOrientation(TileVector tv, CardinalDirection dir)
	{
		Position = tv.ToVector3();
		Rotation = dir.GetBearingRotation();
	}

	public void ApplyMove(Move move)
	{
		if (move.IsHalt()) return;
		_moveQueue.Enqueue(new MoveAnimation(this, move.Destination, move.Unit.Facing.Turn(move.Direction)));
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using FMODUnity;

public class UnitAvatar : MonoBehaviour
{
	// Behavioural and stat variables
	public int MaxHealth;
	public int MaxEnergy;
	[FMODUnity.EventRef]
	public string heavySound = "event:/HeavyDeploy";
	public string medSound = "event:/MediumDeploy";
	public string lightSound = "event:/ScoutDeploy";

	public UnitAi Ai;
	
	// Comsmetic variables
	public float MoveSpeed = 5f;
	public float TurnSpeed = 180f;

	// Unity components
	public Animator Animator { get; protected set; }
	public MeshRenderer[] PaintComponents;
	
	private MeshRenderer[] _allRenderers;
	private MeshRenderer[] AllRenderers
	{
		get
		{
			if (_allRenderers == null)
			{
				_allRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
				foreach (var r in _allRenderers) {
					foreach (var m in r.materials) {
						if (m.HasProperty ("_Color"))
						{
							_initialColors.Add(m, m.color);
						}
					}
				}
			}
			return _allRenderers;
		}
	}
	
	private Dictionary<Material, Color> _initialColors = new Dictionary<Material, Color>();

	// Movement state trackers
	private Queue<IAnimation> _animQueue = new Queue<IAnimation>();
	public IAnimation CurrentAnimation { get { return _animQueue.Count > 0 ? _animQueue.Peek() : null; } }

	private Unit _unit;
	
	private BarScript _hpBar;
	private BarScript _epBar;
	
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

	protected void Start ()
	{
		// IMPLEMENTATION NOTE: defer functionality from Start to SetUnit
		Animator = GetComponent<Animator>();
		_initScale = transform.localScale;
	}

	private void Update()
	{
		if (_unit == null) return;
		
		_hpBar.gameObject.transform.position = new 
			Vector3 (this.Position.x, this.Position.y+0.6f, this.Position.z-1f);
		_epBar.gameObject.transform.position = new 
			Vector3 (this.Position.x, this.Position.y+0.4f, this.Position.z-1f);
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
		_hpBar = Instantiate(GuiComponents.GetHpBar ()).GetComponent<BarScript>();
		_epBar = Instantiate(GuiComponents.GetEpBar ()).GetComponent<BarScript>();
		
		TeamColorPaint();
		Utils.Print (_unit.Avatar.Ai.name);
		//SendMessage("Play");
		var avatarName = _unit.Avatar.Ai.name;

		if (avatarName.Equals ("ArcZigZagAi")) 
		{
			FMODUnity.RuntimeManager.PlayOneShot (heavySound, Position);
		}
		else if (avatarName.Equals ("Linear")) 
		{
			FMODUnity.RuntimeManager.PlayOneShot (lightSound, Position);
		}
		else if (avatarName.Equals ("BasicZigZag")) 
		{
			FMODUnity.RuntimeManager.PlayOneShot (medSound, Position);
		}
		//FMODUnity.RuntimeManager.PlayOneShot (sound, Position);
	}

	public void EnqueueAnimation(IAnimation anim)
	{
		_animQueue.Enqueue(anim);
	}
	
	public void UpdateHealth(float health)
	{
		var hpPercent = health / _unit.MaxHealth;
		_hpBar.SetPercent (hpPercent);
	}

	public void UpdateEnergy(float energy)
	{
		var epPercent = energy / _unit.MaxHealth;
		_epBar.SetPercent (epPercent);
	}

	public void Paint(Color color)
	{
		foreach (var r in AllRenderers) {
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
		foreach (var r in AllRenderers) {
			foreach (var m in r.materials) {
				if (m.HasProperty ("_Color"))
				{
					m.color = _initialColors[m];
				}
			}
		}
		TeamColorPaint();
	}

	private void TeamColorPaint()
	{
		foreach (var r in PaintComponents) {
			foreach (var m in r.materials) {
				if (m.HasProperty ("_Color"))
				{
					m.color = _unit.Owner.Color;
				}
			}
		}
	}

	public void Kill()
	{
		_animQueue.Enqueue(new DestroyAnimation(this));
	}

	public void Destroy(){
		Destroy(gameObject);
		Destroy (_hpBar.gameObject);
		Destroy (_epBar.gameObject);
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
}
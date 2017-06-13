using System;

namespace Model
{
	public sealed class Unit
	{
		public delegate TurnPlan AiMoveMethod(Unit unit, World world);

		public const int UnitBadMovesTimeout = 5;

		// Accessors
		public TileVector Position { get; private set; }
		public CardinalDirection Facing { get; private set; }

		public TurnPlan LastMove { get; private set; }
		private TurnPlan _currentMove;

		// Unit stats and behaviour
		public readonly Player Owner; // TODO: update with better semantics

		public readonly int MaxHealth;
		public int Health { get; private set; }

		public readonly int MaxEnergy;
		public int Energy { get; private set; }

		// AI Hooks
		private readonly AiMoveMethod _moveMethod;
		// we might need more, for example, combat modifiers

		// Private fields
		public readonly UnitAvatar _avatar;	// Unity representation

		public Unit (UnitAvatar avatar, TileVector position, CardinalDirection facing, Player owner)
		{
			_avatar = avatar;

			Position = position;
			Facing = facing;

			MaxHealth = _avatar.MaxHealth;
			Health = MaxHealth;

			MaxEnergy = _avatar.MaxEnergy;
			Energy = MaxEnergy;

			_moveMethod = _avatar.Ai.GetMovementPlan;
			Owner = owner;

		}

		public TurnPlan GetMovementPlan(World world)
		{
			LastMove = _currentMove;
			_currentMove = _moveMethod(this, world);
			return _currentMove;
		}

		public bool IsDead()
		{
			return Health <= 0;
		}

		public void Kill()
		{
			_avatar.Kill();
		}
		
		public bool CanMove()
		{
			// TODO: this will probably need refinement!
			return Energy > 0;
		}

		public void Reset()
		{
			// TODO: refine
			Energy = MaxEnergy;
		}

		public void ApplyMove(Move move)
		{
			// TODO: ensure units don't go into negative energy
			if (move.Unit != this) throw new ArgumentException("Applied move does not reference this unit");

			_avatar.ApplyMove(move); // animate avatar (do this first, as it needs to read fields here)

			Facing = Facing.Turn(move.Direction);
			Position = move.Destination;
			Energy -= move.EnergyCost;
		}

		public void ApplyCombat(Combat combat)
		{
			Unit other;		// determine which unit is not this one
			if (combat.Unit1 == this) 		other = combat.Unit2;
			else if (combat.Unit2 == this) 	other = combat.Unit1;
			else throw new ArgumentException("Applied combat does not reference this unit");

			// turn to face other unit
			var combatDirection = Position.GetApproximateDirectionTo(other.Position);
			if (!combatDirection.HasValue) throw new Exception();	// could not face other unit - probably same pos

			Move.Turn(_currentMove, combatDirection.Value.Cross(Facing)).Accept();	// bit gross, but should be fine
			
			if (this.Energy <= 0) return;	// skip combat
			
			_avatar.ApplyCombat(other,other.Position);
			
			// do combat - deal damage to other unit, lose energy
			var damage = Math.Min(this.Energy, other.Health); 	// TODO: implement buff mechanics here. 
			other.Health -= damage;			// NB: care should be taken when using this.Health or other.Energy in
			this.Energy -= damage;			// combat logic, as one side of combat will be resolved before the other.
		}
	}
}
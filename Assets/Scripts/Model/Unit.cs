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
		public bool Mirrored { get; private set; }				// experimental

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
		public readonly UnitAvatar Avatar;	// Unity representation

		public Unit (UnitAvatar avatar, Player owner, TileVector position, CardinalDirection facing, bool mirrored)
		{
			// IMPLEMENTATION NOTE: Unit construction should not have any side effects. if you need to keep track
			//						of new units being created, you should call that from WorldController.AddUnit. 
			Avatar = avatar;

			_moveMethod = Avatar.Ai.GetMovementPlan;
			Owner = owner;
			
			Position = position;
			Facing = facing;

			MaxHealth = Avatar.MaxHealth;
			Health = MaxHealth;

			MaxEnergy = Avatar.MaxEnergy;
			Energy = MaxEnergy;

			Mirrored = mirrored;
		}

		public TurnPlan GetMovementPlan(World world)
		{
			LastMove = _currentMove;
			_currentMove = _moveMethod(this, world);
			return _currentMove;
		}

		/// <summary>
		/// Gets the Direction that this Unit would face after receiving a a turn in the given direction
		/// Note that this does not modify the Unit in any way, and should be used for analytic purposes only.
		/// This method takes mirroring into account - use with caution
		/// </summary>
		/// <param name="relative">the RelativeDirection to turn</param>
		/// <returns>the CardinalDirection that this Unit would face after turning</returns>
		public CardinalDirection Turn(RelativeDirection relative)
		{
			var trueDirection = Mirrored ? relative.Mirror() : relative;
			return Facing.Turn(trueDirection);
		}

		/// <summary>
		/// Gets the Direction that this Unit would need to turn to face the given direction.
		/// Note that this does not modify the Unit in any way, and should be used for analytic purposes only.
		/// This method takes mirroring into account - use with caution
		/// </summary>
		/// <param name="to">the CardinalDirection that this Unit would face after turning</param>
		/// <returns>the RelativeDirection to turn to face the Direction</returns>
		public RelativeDirection Cross(CardinalDirection to)
		{
			var trueDirection = Mirrored ? to.Turn(RelativeDirection.Back) : to;
			return Facing.Cross(trueDirection);
		}
		
		public bool IsDead()
		{
			return Health <= 0;
		}

		public void Kill()
		{
			Avatar.Kill();
		}
		
		public bool CanMove()
		{
			return Energy > 0;
		}

		public void Reset()
		{
			Energy = MaxEnergy;
		}

		public void ApplyMove(Move move)
		{
			// TODO: ensure units don't go into negative energy
			if (move.Unit != this) throw new ArgumentException("Applied move does not reference this unit");

			Avatar.ApplyMove(move); // animate avatar (do this first, as it needs to read fields here)

			Facing = Turn(move.Direction);
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
			
			if (this.Energy <= 0) return;	// skip combat
			
			Move.Turn(_currentMove, Cross(combatDirection.Value)).Accept();	// bit gross, but should be fine
			Avatar.ApplyCombat(other,other.Position);
			
			// do combat - deal damage to other unit, lose energy
			var damage = Math.Min(this.Energy, other.Health); 	// TODO: implement buff mechanics here. 
			other.Health -= damage;			// NB: care should be taken when using this.Health or other.Energy in
			this.Energy -= damage;			// combat logic, as one side of combat will be resolved before the other.
		}
	}
}
using System;
using System.Collections.Generic;

namespace Model
{
	/// <summary>
	/// Class which represents a TurnPlan's desired step for a given frame of simulation
	/// To construct a Move, use one of the static factory methods provided.
	/// </summary>
	public sealed class Move
	{
		/// <summary>
		/// The TurnPlan that this move is a part of.
		/// </summary>
		public readonly TurnPlan Owner;

		/// <summary>
		/// The Unit which this Move represents an action on.
		/// </summary>
		public Unit Unit { get { return Owner.Unit; }}

		/// <summary>
		/// The position that the Unit will move to.
		/// </summary>
		public readonly TileVector Destination;

		/// <summary>
		/// The direction the Unit will turn to make the move.
		/// </summary>
		public readonly RelativeDirection Direction;

		public readonly int EnergyCost;

		private Move(TurnPlan owner, RelativeDirection direction, TileVector destination, int energyCost)
		{
			Owner = owner;
			Direction = direction;
			Destination = destination;
			EnergyCost = energyCost;
		}

		public void Accept()
		{
			Owner.AcceptMove(this);
		}

		public void Reject()
		{
			var newMove = Turn(Owner, Direction);
			Owner.RejectMove(this, newMove);
		}

		/// <summary>
		/// Constructe a Move which represents a turn followed by a single tile step.
		/// </summary>
		/// <param name="owner">the TurnPlan which created this move</param>
		/// <param name="direction">the RelativeDirection for it to turn and move in</param>
		/// <returns></returns>
		public static Move Step(TurnPlan owner, RelativeDirection direction)
		{
			var destination = owner.Unit.Position + owner.Unit.Turn(direction);
			return new Move(owner, direction, destination, 2);
		}

		/// <summary>
		/// Construct a Move which represents a unit turning in place.
		/// </summary>
		/// <param name="owner">the TurnPlan which created this move</param>
		/// <param name="direction">the RelativeDirection for it to turn to</param>
		/// <returns></returns>
		public static Move Turn(TurnPlan owner, RelativeDirection direction)
		{
			return new Move(owner, direction, owner.Unit.Position, 1);
		}

		/// <summary>
		/// Construct a Move which represents a unit staying completely stationary.
		/// </summary>
		/// <param name="owner">the TurnPlan which created this move</param>
		/// <returns></returns>
		public static Move Halt(TurnPlan owner)
		{
			return new Move(owner, RelativeDirection.Forward, owner.Unit.Position, 0);
		}

		/// <summary>
		/// Determine if this Move is a Unit taking a step.
		/// </summary>
		/// <returns>true if it is</returns>
		public bool IsStep()
		{
			return Destination != Unit.Position;
		}

		/// <summary>
		/// Determine if this Move is a Unit turning in place.
		/// </summary>
		/// <returns>true if it is</returns>
		public bool IsTurn()
		{
			return Destination == Unit.Position;
		}

		/// <summary>
		/// Determine if this Move is a Unit remaining in place, and not turning.
		/// </summary>
		/// <returns>true or false</returns>
		public bool IsHalt()
		{
			return Destination == Unit.Position && Direction == RelativeDirection.Forward;
		}

		// MOVE PRIORITY FUNCTIONS

		private static readonly Comparer<Move> PriorityComparerInstance = new MovePriorityComparer();

		private sealed class MovePriorityComparer : Comparer<Move>
		{
			public override int Compare(Move x, Move y)
			{
				// IMPLEMENT MOVE PRIORITY HERE
				if (x == null || y == null) throw new ArgumentNullException();
				return x.Unit.Energy - y.Unit.Energy;
			}
		}

		public static Comparer<Move> PriorityComparer
		{
			get { return PriorityComparerInstance; }
		}
	}
}
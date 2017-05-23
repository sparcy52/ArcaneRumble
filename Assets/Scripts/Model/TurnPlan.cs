﻿using System;
using System.Collections.Generic;

namespace Model
{
	/// <summary>
	/// Abstract class to represent a Unit's desired set of actions for a single turn of the game.
	/// Implementors only need to override the class's main method, GetNextMove().
	/// </summary>
	public abstract class TurnPlan
	{
		public const int TimeoutAttempts = 5;

		public readonly Unit Unit;
		public readonly World World;

		private int _timeout = TimeoutAttempts;

		public abstract Move GetNextMove();

		protected TurnPlan(Unit unit, World world)
		{
			Unit = unit;
			World = world;
		}

		// COMMUNICATION METHODS

		/// <summary>
		/// Indicates that the given move has been accepted by the WorldController.
		/// Calling this function will also update the game state by executing the Move.
		/// </summary>
		/// <param name="accepted">the Move that was accepted</param>
		public void AcceptMove(Move accepted)
		{
			ApplyMove(accepted);
			OnAccept(accepted);
		}

		/// <summary>
		/// Indicates that the given move has been rejected by the WorldController.
		/// Calling this function will also update the game state by executing the <c>replacement</c> Move.
		/// </summary>
		/// <param name="rejected">the Move that was proposed, but rejected</param>
		/// <param name="replacement">the Move that should be executed in <c>rejected's</c> place</param>
		public void RejectMove(Move rejected, Move replacement)
		{
			ApplyMove(replacement);
			OnReject(rejected, replacement);
			if (--_timeout == 0)
			{
				Utils.Printf("Timed out TurnPlan {0} on move {1}", this, rejected);
				OnTimeout();
			}
		}

		public bool IsActive()
		{
			return _timeout > 0 && Unit.CanMove();
		}

		// SUBCLASS HOOKS

		protected virtual void OnAccept(Move acceted)
		{

		}

		protected virtual void OnReject(Move rejected, Move replacement)
		{

		}

		protected virtual void OnTimeout()
		{

		}

		// HELPER METHODS

		protected T GetLastMove<T>() where T : TurnPlan
		{
			return Unit.LastMove as T;
		}
		
		private void ApplyMove(Move move)
		{
			if (move.IsStep())	// update the board if this unit is moving position
			{
				var origin = World[Unit.Position];
				if (origin.Occupant == Unit) // NullReferenceException here indicates a move from a bad position
				{
					origin.Occupant = null;
				}
				World[move.Destination].Occupant = Unit;
			}
			Unit.ApplyMove(move);
		}

		// MOVE CREATION

		protected Move Halt()
		{
			return Move.Halt(this);
		}

		protected Move Turn(RelativeDirection relative)
		{
			return Move.Turn(this, relative);
		}

		protected Move Turn(CardinalDirection cardinal)
		{
			return this.Turn(Unit.Facing.Cross(cardinal));
		}

		protected Move Step(RelativeDirection relative)
		{
			return Move.Step(this, relative);
		}

		protected Move Step(CardinalDirection cardinal)
		{
			return this.Step(Unit.Facing.Cross(cardinal));
		}
	}
}

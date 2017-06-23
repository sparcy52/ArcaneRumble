﻿using System;
using System.Collections.Generic;
 using UnityEngine;

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

		protected TurnPlan(Unit unit, World world)
		{
			Unit = unit;
			World = world;
		}
		
		public abstract Move GetNextMove();

		// SUBCLASS HOOKS

		protected virtual void OnAccept(Move accepted)
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
		
		protected bool BlockedByWall(RelativeDirection dir)
		{
			return BlockedByWall(Unit.Turn(dir));	// convert to cardinal direction
		}
		
		protected bool BlockedByWall(CardinalDirection dir)
		{
			var hex = World[Unit.Position + dir];
			return hex == null || hex.Impassable;
		}

		protected RelativeDirection AdjustForWalls(RelativeDirection dir)
		{
			var cardinal = AdjustForWalls(Unit.Turn(dir));	// adjusting via cardinal directions makes adjustment
			return Unit.Cross(cardinal);					// result independent of unit chirality (mirrored-ness)
		}

		protected CardinalDirection AdjustForWalls(CardinalDirection dir)
		{
			if (!BlockedByWall(dir)) return dir;
			var scanDirection = Unit.Owner.MirrorDefault ? -1 : 1;
			for (var i = 1; i <= 5; i++)	// sweep side to side until first exit found
			{
				dir = dir.ArcClockwise(i * scanDirection);
				if (!BlockedByWall(dir)) return dir;
				scanDirection *= -1;		// swap scan direction
			}
			#if DEBUG
				Debug.LogWarningFormat("Unit at {0} has no available exits!", Unit.Position);
			#endif
			return dir;	// should only happen if terrain changes behind unit
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
			return this.Turn(Unit.Cross(cardinal));
		}

		protected Move Step(RelativeDirection relative)
		{
			return Move.Step(this, relative);
		}

		protected Move Step(CardinalDirection cardinal)
		{
			return this.Step(Unit.Cross(cardinal));
		}
		
		// COMMUNICATION METHODS
		
		public bool IsActive()
		{
			return _timeout > 0 && Unit.CanMove();
		}

		/// <summary>
		/// Indicates that the given move has been accepted by the WorldController.
		/// Calling this function will also update the game state by executing the Move.
		/// </summary>
		/// <param name="accepted">the Move that was accepted</param>
		public void AcceptMove(Move accepted)
		{
			OnAccept(accepted);
			ApplyMove(accepted);
		}

		/// <summary>
		/// Indicates that the given move has been rejected by the WorldController.
		/// Calling this function will also update the game state by executing the <c>replacement</c> Move.
		/// Note that the replacement move will also trigger OnAccept
		/// </summary>
		/// <param name="rejected">the Move that was proposed, but rejected</param>
		/// <param name="replacement">the Move that should be executed in <c>rejected's</c> place</param>
		public void RejectMove(Move rejected, Move replacement)
		{
			OnReject(rejected, replacement);
			AcceptMove(replacement);
			if (--_timeout == 0)
			{
				Utils.Printf("Timed out TurnPlan {0} on move {1}", this, rejected);
				OnTimeout();
			}
		}
		
		// PRIVATE UTILITIES
		
		private void ApplyMove(Move move)
		{
			if (move.IsStep())	// update the board if this unit is moving position
			{
				var origin = World[Unit.Position];
				if (origin.Occupant == Unit) // NullReferenceException here indicates a move from a bad position
				{
					origin.Occupant = null;
				}

				var destination = World[move.Destination];
				destination.Occupant = Unit;
				if (destination.HasObjective())
				{
					// TODO: refactor into WorldController for mechanical refinement
					Unit.Owner.TakeObjective(destination.Objective);
				}
			}
			Unit.ApplyMove(move);
		}
	}
}

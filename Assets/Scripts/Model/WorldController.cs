using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	public class WorldController
	{
		public int RoundNumber { get; private set; }
		
		private World _world;
		private ICollection<Unit> _units;

		public WorldController(World world)
		{
			_world = world;
			_units = new HashSet<Unit>();
		}
		
		/// <summary>
		/// Add a Unit to the game. Note that Unit objects are instantiated alongside UnitAvatars,
		/// so creation should be done elsewhere, and plug back into here.
		/// Units passed to this method should not be aliased by the caller.
		/// </summary>
		/// <param name="newUnit"></param>
		/// <returns>true if the unit was successfully inserted</returns>
		public bool AddUnit(Unit newUnit)
		{
			var hex = _world[newUnit.Position];
			if (IsHexPlaceable(newUnit.Owner, hex))
			{
				hex.Occupant = newUnit;
				_units.Add(newUnit);
				newUnit.Owner.DeployUnit(newUnit);
//				Utils.Print ("Placed");
				//newUnit. ("Play");
				return true;
			}
			else return false;
		}
		
        public bool IsHexPlaceable(Player player, Hex hex)
        {
            if (hex != null && hex.Placeable && hex.Owner == player && hex.Occupant == null)
            //if (hex != null && hex.Occupant == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Simulate the game for a single turn.
        /// </summary>
        public void DoTurn()
		{	
			var activeUnits = new Dictionary<Unit, TurnPlan>();

			foreach (var unit in _units)
			{
				unit.Reset();
				activeUnits.Add(unit, unit.GetMovementPlan(_world));
			}
			
			#if DEBUG 
			DebugLogFrame();
			#endif
			
			while (activeUnits.Count > 0) // main loop: iterate through each simulation frame
			{	
				// STEP 1: COMBAT DETERMINATION
				var pendingCombat = new HashSet<Combat>(); // stops duplication of the same combat
				var unitsInCombat = new HashSet<Unit>();

				foreach (var unit in activeUnits.Keys)
				{
					foreach (var adj in unit.Position.Adjacent().Select(pos => _world[pos])) // for each adjacent Hex
					{
						if (adj == null) continue;
						
						var neighbour = adj.Occupant;
						if (neighbour != null && neighbour.Owner!= unit.Owner)
						{
							pendingCombat.Add(new Combat(unit, neighbour));
							unitsInCombat.Add(unit);
							unitsInCombat.Add(neighbour);
						}
					}
				}

				// STEP 2: COMBAT RESOLUTION
				var pendingCombatSorted = new List<Combat>(pendingCombat);
				pendingCombatSorted.Sort(Combat.PriorityComparer);	// sort by priortiy
				
				foreach (var combat in pendingCombatSorted)
				{
					var unit1 = combat.Unit1;
					var unit2 = combat.Unit2;

					if (!unit1.IsDead() && !unit2.IsDead())
					{
						combat.Apply();
						
						FinaliseCombat(unit1, unit2, activeUnits);	// lambda/local-function would be
						FinaliseCombat(unit2, unit1, activeUnits);	// ideal here! outdated version of C#
					}
				}
				
				// STEP 3: MOVE PRE-PROCESSING				
				var moveDestinations = new Dictionary<TileVector, List<MoveResolver>>(); // where units want to move to
				var moveOrigins		 = new Dictionary<TileVector, MoveResolver>(); 		 // where units are moving from
				
				foreach (var unit in _units) 			// collect all units' moves into the Dictionaries, using plans
				{
					TurnPlan plan = null;
					if (activeUnits.ContainsKey(unit)) 	// only active units can make moves - bind a plan
					{
						plan = activeUnits[unit];
						if (!plan.IsActive())			// unit can no longer make moves, so remove from active units
						{
							activeUnits.Remove(unit);
							plan = null;				// unbind plan for this frame
						}
					}
					
					var canMove   = plan != null && !unitsInCombat.Contains(plan.Unit);
					var move 	  = canMove? plan.GetNextMove() : null;
					var mayVacate = canMove && move.IsStep();

					// if move is a step, moves here are conditional upon it. otherwise, deny outright
					var resolver = mayVacate ? MoveResolver.Of(move) : MoveResolver.Deny();
					moveOrigins.Add(unit.Position, resolver);
					
					if (mayVacate) // add resolver to the move's intended destination for processing later
					{
						var hex = _world[move.Destination];
						if (hex != null && !hex.Impassable) // if target is valid
						{
							List<MoveResolver> movesToDestination;
							if (moveDestinations.ContainsKey(move.Destination))
							{
								movesToDestination = moveDestinations[move.Destination];
							}
							else // initialise the list if it doesn't exist yet
							{
								movesToDestination = new List<MoveResolver>(6);
								moveDestinations.Add(move.Destination, movesToDestination);
							}
							movesToDestination.Add(resolver);
						}
						else resolver.Resolve(false);	// reject if move is illegal
					}
				}

				// STEP 4: MOVE CONFLICT RESOLUTION
				foreach (var entry in moveDestinations) // check all future mech positions, finalise moves
				{
					var destination = entry.Key;
					var moves 		= entry.Value;

					var best = 0;
					for (var i = 1; i < moves.Count; i++)	// reject all moves here except the best one
					{
						if (Move.PriorityComparer.Compare(moves[i].Move, moves[best].Move) > 0)
						{
							moves[best].Resolve(false);
							best = i;
						}
						else 	// not the best move: reject
						{
							moves[i].Resolve(false);
						}
					}
					var dependency = moveOrigins.ContainsKey(destination) ? moveOrigins[destination] : null;
					
					if (dependency != null) moves[best].SetParent(dependency);
					else 					moves[best].Resolve(true);
				}
			
				#if DEBUG 
				DebugLogFrame();
				#endif
			}
			
			RoundNumber++;
		}

		private void FinaliseCombat(Unit attacker, Unit defender, IDictionary<Unit, TurnPlan> activeUnits)
		{
			if (defender.IsDead())
			{
				activeUnits.Remove(defender);
				_world[defender.Position].Occupant = null; // NullRef here indicates bad unit position
				_units.Remove(defender);
				attacker.Owner.DestroyUnit(defender);
			}
		}

		/// <summary>
		/// Class which is used to solve complex dependencies between moves.
		/// Moves in the graph are automatically accepted and applied once it becomes solvable.
		/// </summary>
		private class MoveResolver
		{
			public readonly Move Move;
			public bool? Resolved { get; private set; }

			private MoveResolver _root;	// so we can detect cycles
			private List<MoveResolver> _dependents = new List<MoveResolver>();

			private MoveResolver(Move move)
			{
				_root = this;
				Move = move;
			}

			public static MoveResolver Of(Move move)
			{
				return new MoveResolver(move);
			}

			public static MoveResolver Deny()
			{
				var block = new MoveResolver(null);
				block.Resolved = false;
				return block;
			}

			public bool Resolve(bool outcome)
			{
				if (Resolved.HasValue) return false;	// already been resolved

				Resolved = outcome;
				if (outcome)
				{
					Move.Accept();
				}
				else
				{
					Move.Reject();
				}

				foreach (var dep in _dependents)
				{
					dep.Resolve(outcome);
				}
				return true;
			}

			public void SetParent(MoveResolver parent)
			{
				if (this._root != this) throw new Exception("Assigned multiple dependencies");

				if (parent.Resolved.HasValue) // already been decided: propogate result
				{
					Resolve(parent.Resolved.Value);
				}
				else if (parent._root == this)			// cyclic, unresolved dependencies: resolve true
				{
					Utils.Printf("Resolved cycle starting at move: {0}", Move);
					Resolve(true);
				}
				else 							// wait until a decision is made later
				{
					SetRoot(parent._root);
					parent._root._dependents.Add(this);	// add to root to shorten call stack
				}
			}

			private void SetRoot(MoveResolver root)
			{
				if (root == _root) throw new Exception("I think this implies an unresolved cycle");
				_root = root;
				foreach (var dependent in _dependents)
				{
					dependent.SetRoot(root);
				}
			}
		}
		
#if DEBUG
		private int _debugFrameCount = 0; 
	
		private void DebugLogFrame()
		{
			var fname = string.Format("Debug/frame{0:00000}.txt", _debugFrameCount++);
			System.IO.Directory.CreateDirectory("Debug");
			var file = new System.IO.StreamWriter(fname);

			{
				var line = new StringBuilder();
				line.Append("W\\E ");
				for (var i = 0; i < _world.E; i += 2)
				{
					line.AppendFormat("{0:0}  ", i);
				}
				file.WriteLine(line);
			}
			
			var orphanedUnits = new HashSet<Unit>();
			var mapUnits = new HashSet<Unit>();
			
			for (var w = 0; w < _world.W; w++)
			{
				var line = new StringBuilder();
				line.AppendFormat("{0:0}   ", w);
				for (var e = 0; e < _world.E; e++)
				{
					var hex = _world[w, e];
					if (hex != null)
					{
						if (hex.Occupant != null)
						{
							line.Append(UnicodeChar(hex.Occupant.Facing));
							mapUnits.Add(hex.Occupant);
							if (!_units.Contains(hex.Occupant)) orphanedUnits.Add(hex.Occupant);
						}
						else line.Append('_');
					}
					else line.Append(" ");
				}
				file.WriteLine(line);
			}
			file.WriteLine();

			var missingUnits = new HashSet<Unit>();
			
			foreach (var unit in _units)
			{
				if (orphanedUnits.Contains(unit)) continue;
				if (!mapUnits.Contains(unit))
				{
					missingUnits.Add(unit);
					continue;
				}
				file.WriteLine(
					"{0,-8}{1,-9} HP:{2,-3}EP:{3,-3}",unit.Position, unit.Facing, unit.Health, unit.Energy
				);	
			}
			
			if (missingUnits.Count > 0)
			{
				file.WriteLine("UNMAPPED UNITS:");
				foreach (var unit in missingUnits)
				{
					UnityEngine.Debug.LogAssertionFormat(
						"Unmapped Units detected: see file {0}. Please save your debug directory and report this.", 
						fname);
					file.WriteLine(
						"{0,-8}{1,-9} HP:{2,-3}EP:{3,-3}",unit.Position, unit.Facing, unit.Health, unit.Energy
					);	
				}
			}
			
			if (orphanedUnits.Count > 0)
			{
				file.WriteLine("ORPHANED UNITS:");
				foreach (var unit in orphanedUnits)
				{
					UnityEngine.Debug.LogAssertionFormat(
						"Orphaned Units detected: see file {0}. Please save your debug directory and report this.", 
						fname);
					file.WriteLine(
						"{0,-8}{1,-9} HP:{2,-3}EP:{3,-3}",unit.Position, unit.Facing, unit.Health, unit.Energy
					);	
				}
			}
				
			file.WriteLine("---------------");
			file.WriteLine();
			file.Close();
		}
		
		private static char UnicodeChar(CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.North:
					return '↖';
				case CardinalDirection.Northeast:
					return '↑';
				case CardinalDirection.Southeast:
					return '→';
				case CardinalDirection.South:
					return '↘';
				case CardinalDirection.Southwest:
					return '↓';
				case CardinalDirection.Northwest:
					return '←';
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
#endif
}
using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/PredefinedPathAi")]
	public class PredefinedPathAi : UnitAi
	{
		public RelativeDirection[] Path;
		
		public override TurnPlan GetMovementPlan(Unit unit, World world)
		{
			return new PredefinedPathPlan(Path, unit, world);
		}

		public override StepPreview[] GetPreview(Unit unit, World world)
		{
			var pos = unit.Position;
			var facing = unit.Facing;
			var energy = unit.MaxEnergy;

			var index = 0;
			var preview = new List<StepPreview>();

			for (var round = 0; round < 3; round++)		// some code duplication in here. ideally TurnPlans would 
			{											// have a more functional design, but that is too much work now
				while (energy > 0) 						
				{	 									
					facing = facing.Turn(Path[index]);
					pos += facing;
					preview.Add(new StepPreview(pos, round));

					index = (index + 1) % Path.Length;
					energy -= 2; // assumes that moves cost 2 energy
				}
				energy = unit.MaxEnergy;
			}
			
			return preview.ToArray();
		}

		/// <summary>
		/// Allows subclasses to override the path in a procedural manner.
		/// </summary>
		/// <returns>the new path to use</returns>
		protected virtual RelativeDirection[] SetPath()
		{
			return Path;
		}

		private void OnValidate()
		{
			Path = SetPath();
		}
	}

	public class PredefinedPathPlan : TurnPlan
	{
		private readonly RelativeDirection[] _path;
		private int _index = 0;
		private RelativeDirection? _nextOverride = null;
		
		public PredefinedPathPlan(RelativeDirection[] path, Unit unit, World world) : base(unit, world)
		{
			var last = GetLastMove<PredefinedPathPlan>();
			if (last != null)
			{
				_path = last._path;
				_index = last._index;
				_nextOverride = last._nextOverride;
			}
			else
			{
				_path = (RelativeDirection[]) path.Clone();
			}
		}

		public override Move GetNextMove()
		{
			return Step(NextDirection());
		}

		private RelativeDirection NextDirection()
		{
			return _nextOverride.HasValue ? _nextOverride.Value : _path[_index];			
		}
		
		protected override void OnAccept(Move accepted)	// handle incrementing the index counter here
		{
			base.OnAccept(accepted);
			
			var expectedDirection = NextDirection();
			if (accepted.Direction == expectedDirection)// ensure that applied moves match what is expected
			{
				if (!accepted.IsStep())					// facing the correct direction, but did not step
				{
					_nextOverride = RelativeDirection.Forward;
				}
				else 									// move was correct: increment index
				{
					_index = (_index + 1) % _path.Length;
					_nextOverride = null;
				}
			}
			else 										// wrong direction: override next move to compensate
			{
				_nextOverride = expectedDirection.Cross(accepted.Direction);
			}
		}
	}
}
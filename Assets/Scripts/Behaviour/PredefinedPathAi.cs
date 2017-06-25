using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/PredefinedPathAi")]
	public class PredefinedPathAi : UnitAi
	{
		public bool FollowPathStrict = true;
		public RelativeDirection DirectionHint = RelativeDirection.Forward;
		public RelativeDirection MirrorHint = RelativeDirection.Forward;
		public RelativeDirection[] Path;
		
		public override TurnPlan GetMovementPlan(Unit unit, World world)
		{
			return new PredefinedPathPlan(Path, FollowPathStrict, unit, world);
		}
		
		/// <summary>
		/// Allows subclasses to override the path in a procedural manner.
		/// </summary>
		/// <returns>the new path to use</returns>
		protected virtual RelativeDirection[] SetPath()
		{
			return Path;
		}
		
		/// <summary>
		/// Allows subclasses to override the direction hint in a procedural manner.
		/// </summary>
		/// <returns>the new direction hint to use</returns>
		protected virtual RelativeDirection SetDirectionHint()
		{
			return DirectionHint;
		}
		
		/// <summary>
		/// Allows subclasses to override the mirror hint in a procedural manner.
		/// </summary>
		/// <returns>the new mirror hint to use</returns>
		protected virtual RelativeDirection SetMirrorHint()
		{
			return MirrorHint;
		}

		private void OnValidate()
		{
			Path = SetPath();
			MirrorHint = SetMirrorHint();
			DirectionHint = SetDirectionHint();
		}
		
		public override RelativeDirection PreviewMirrorHint()
		{
			return MirrorHint;
		}

		public override RelativeDirection PreviewDirectionHint()
		{
			return DirectionHint;
		}
		
		public override StepPreview[] GetPreview(Unit unit, World world)
		{
			var pos = unit.Position;
			var facing = unit.Facing;
			var energy = unit.MaxEnergy;

			var preview = new StepPreview[Math.Max(4, Path.Length)];

			var index = 0;
			for (var round = 0; index < preview.Length; round++)
			{
				while (energy > 0 && index < preview.Length)
				{
					var pathIndex = index % Path.Length;
					facing = facing.Turn(unit.Mirrored ? Path[pathIndex].Mirror() : Path[pathIndex]);
					pos += facing;
					preview[index++] = new StepPreview(pos, round);
					
					energy -= 2; // assumes that moves cost 2 energy
				}
				energy = unit.MaxEnergy;
			}
			
			return preview;
		}
	}

	public class PredefinedPathPlan : TurnPlan
	{
		private readonly RelativeDirection[] _path;
		private readonly bool _strict;
		private int _index = 0;
		private RelativeDirection? _nextOverride = null;
		
		public PredefinedPathPlan(RelativeDirection[] path, bool strict, Unit unit, World world) : base(unit, world)
		{
			var last = GetLastMove<PredefinedPathPlan>();
			if (last != null)
			{
				_path = last._path;
				_strict = last._strict;
				_index = last._index;
				_nextOverride = last._nextOverride;
			}
			else
			{
				_strict = strict;
				_path = (RelativeDirection[]) path.Clone();
			}
		}

		public override Move GetNextMove()
		{
			var intention = NextDirection();
			var adjusted = AdjustForWalls(intention);
			if (intention != adjusted)		
			{									
				_nextOverride = adjusted;	// set an override direction. this would mean we skip a step in the pattern
				_index--;					// so we have to deincrement the index. it is not queried until override is
			}								// cleared, so this 'naive' approach is fine (it could go out of range).
			return Step(adjusted);
			//return Step(NextDirection());
		}

		private RelativeDirection NextDirection()
		{
			return _nextOverride.HasValue ? _nextOverride.Value : _path[_index];			
		}
		
		protected override void OnAccept(Move accepted)	// handle incrementing the index counter here
		{
			base.OnAccept(accepted);
			
			var expectedDirection = NextDirection();	// ensure that applied moves match what is expected
			if (accepted.Direction == expectedDirection || !_strict)
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
			{											// non-strict doesn't care if it turns the wrong way
				_nextOverride = expectedDirection.Cross(accepted.Direction);
			}
		}
	}
}
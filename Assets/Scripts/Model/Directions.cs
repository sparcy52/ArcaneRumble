using System;

namespace Model
{
	[Serializable]
	public enum CardinalDirection
	{
		North,
		Northeast,
		Southeast,
		South,
		Southwest,
		Northwest
	}

	[Serializable]
	public enum RelativeDirection
	{
		Forward,
		ForwardRight,
		BackRight,
		Back,
		BackLeft,
		ForwardLeft
	}

	public static class Extensions
	{
		/// <summary>
		/// Gets the unit vector represented by a specified direction
		/// </summary>
		/// <param name="d">a cardinal direction</param>
		/// <returns>A position with components either -1, 0, or 1</returns>
		/// <exception cref="ArgumentOutOfRangeException">if d out of range</exception>
		public static TileVector GetTileVector(this CardinalDirection d)
		{
			switch (d)
			{
				case CardinalDirection.North:
					return new TileVector(-1, -1);
				case CardinalDirection.Northeast:
					return new TileVector(-1, 0);
				case CardinalDirection.Southeast:
					return new TileVector(0, 1);
				case CardinalDirection.South:
					return new TileVector(1, 1);
				case CardinalDirection.Southwest:
					return new TileVector(1, 0);
				case CardinalDirection.Northwest:
					return new TileVector(0, -1);
				default:
					throw new ArgumentOutOfRangeException("d", d, null);
			}
		}

		public static int GetBearing(this CardinalDirection d)
		{
			return (int) d * 60;
		}

		// RELATIVE DIRECTION UTLITIES - use these where possible

		/// <summary>
		/// Find a direction relative to this one, using a RelativeDirection
		/// eg: North.Turn(ForwardRight) = Northeast
		/// 	Southwest.Turn(BackLeft) = Southeast
		/// </summary>
		/// <param name="facing">the initial, forward-pointing direction</param>
		/// <param name="relative">a relative direction to be applied to it</param>
		/// <returns>the CardinalDirection that is 'relative' to 'facing'</returns>
		public static CardinalDirection Turn(this CardinalDirection facing, RelativeDirection relative)
		{
			return facing.ArcClockwise((int) relative);
		}

		/// <summary>
		/// Find the relative direction of this direction to another. Specifically, the other
		/// direction relative to this one.
		///	note: for two directions A and B:
		/// 	A.Turn(A.Cross(B)) = B
		/// </summary>
		/// <param name="facing">the direction for other to be found relative </param>
		/// <param name="other">an other direction</param>
		/// <returns>'other' relative to 'facing'</returns>
		public static RelativeDirection Cross(this CardinalDirection facing, CardinalDirection other)
		{
			return (RelativeDirection) facing.GetArcLinear(other);
		}
		
		public static RelativeDirection Cross(this RelativeDirection facing, RelativeDirection other)
		{
			return Cross((CardinalDirection) facing, (CardinalDirection) other);
		}

		/// <summary>
		/// Mirrors a RelativeDirection such that Left switches to Right and vise-versa
		/// </summary>
		/// <param name="relative">the RelativeDirection to mirror</param>
		/// <returns>its mirror</returns>
		public static RelativeDirection Mirror(this RelativeDirection relative)
		{
			return (RelativeDirection) Wrap(6 -(int) relative);
		}


		// ARC UTILITIES - use these when you need to numerically quantify rotation

		/// <summary>
		/// Find the direction specified by rotating this direction a number of 30 degree steps, clockwise.
		/// </summary>
		/// <param name="d">direction to start from</param>
		/// <param name="steps">number of 30 degree steps to rotate, clockwise</param>
		/// <returns>a new direction</returns>
		public static CardinalDirection ArcClockwise(this CardinalDirection d, int steps)
		{
			return (CardinalDirection) Wrap((int) d + steps);
		}

		/// <summary>
		/// Find the RelativeDirection equivalent to a number of 30 degree steps in a clockwise direction.
		/// This can then be applied to a CardinalDirection with the Turn method
		/// </summary>
		/// <param name="steps">number of 30 degree steps to rotate, to convert to an enum</param>
		/// <returns>the resulting RelativeDirection</returns>
		public static RelativeDirection DirectionFromArc(this int steps)
		{
			return (RelativeDirection) Wrap(steps);
		}

		/// <summary>
		/// Get the number of arc steps, in clockwise direction, from this CardinalDirection to another.
		/// </summary>
		/// <param name="facing">original direction</param>
		/// <param name="to">number of 30 degree steps to rotate it by</param>
		/// <returns>an int between 0 and 5 (inclusive)</returns>
		public static int GetArcLinear(this CardinalDirection facing, CardinalDirection to)
		{
			return Wrap((int) to - (int) facing);
		}

		/// <summary>
		/// Get the minimum number of arc steps, in either direction, from this CardinalDirection to another.
		/// The steps are encoded in clockwise direction, so a negative result indicates stepping anticlockwise.
		/// </summary>
		/// <param name="facing">original direction</param>
		/// <param name="to">direction to rotate to</param>
		/// <returns>an int between -2 and 3 (inclusive)</returns>
		public static int GetArcMinimum(this CardinalDirection facing, CardinalDirection to)
		{
			int steps = facing.GetArcLinear(to);
			if (steps > 3) return 3 - steps;
			else return steps;
		}
		
		// INTERNAL UTILITIES

		private static int Wrap(int d)
		{
			return (d % 6 + 6) % 6;		// add 6 then mod again to correctly map negative input
		}
	}
}
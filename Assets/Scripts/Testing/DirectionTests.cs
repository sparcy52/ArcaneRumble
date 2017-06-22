using NUnit.Framework;
using Model;
using Cd = Model.CardinalDirection;
using Rd = Model.RelativeDirection;

namespace Testing
{	
	[TestFixture]
	public static class DirectionTests
	{
		[TestCase(Cd.Southeast, 0, Cd.Southeast)]
		[TestCase(Cd.Southwest, 6, Cd.Southwest)]
		[TestCase(Cd.Northeast, -36, Cd.Northeast)]
		[TestCase(Cd.North, 3, Cd.South)]
		[TestCase(Cd.North, 7, Cd.Northeast)]
		[TestCase(Cd.Southwest, 4, Cd.Southeast)]
		[TestCase(Cd.Northeast, -5, Cd.Southeast)]
		[TestCase(Cd.Southwest, -2, Cd.Southeast)]
		[TestCase(Cd.South, -8, Cd.Northeast)]
		public static void ArcClockwise(CardinalDirection init, int arc, CardinalDirection expected)
		{
			Assert.AreEqual(expected, init.ArcClockwise(arc));
		}
	}
}
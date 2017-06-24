using System.Text;
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

		[Test]
		public static void Mirror_Exhaustive()
		{ 
			var output = new StringBuilder();
			for (var i = 0; i < 6; i++)
			{
				var expected = (RelativeDirection) Cd.North.ArcClockwise(-i);
				var mirror = (RelativeDirection) Cd.North.ArcClockwise(i);
				Assert.AreEqual(expected, mirror.Mirror());
				output.AppendFormat("{0} | {1}\n", expected, mirror);
			}
			Assert.Pass(output.ToString());
		}

		[Test]
		public static void CrossTurnConsistency_Exhaustive()
		{
			var output = new StringBuilder();
			for (var i = 0; i < 6; i++)
			{
				var fromDirection = Cd.North.ArcClockwise(i);
				for (var j = 0; j < 6; j++)
				{
					var toDirection = Cd.North.ArcClockwise(j);
					var cross = fromDirection.Cross(toDirection);
					var turn = fromDirection.Turn(cross);
					Assert.AreEqual(toDirection, turn, "from: {0} cross: {1}", fromDirection, cross);
					output.AppendFormat("{0} * {2} -> {1}\n", fromDirection, toDirection, cross);
				}
			}
			Assert.Pass(output.ToString());
		}
	}
}
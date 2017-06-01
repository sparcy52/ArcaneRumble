using System;
using Model;
using NUnit.Framework;

namespace Testing
{
	[TestFixture]
	public class TileVectorTests
	{
		private TileVector Tv(int w, int e)
		{
			return new TileVector(w, e);
		}

		/// <summary>
		/// Tests GetApproximateDirection for a given TileVector.
		/// 
		/// This test is specifically used for the six cardinal directions.
		/// 
		/// This test automatically scales up the given vectors, so use small arguments for best coverage.
		/// </summary>
		/// <param name="w">W component of the TileVector</param>
		/// <param name="e">E component of the TileVector</param>
		/// <param name="expected">the expected result of GetApproximateDirection</param>
		[TestCase(1, 0, CardinalDirection.Southwest)]
		[TestCase(0, 1, CardinalDirection.Southeast)]
		[TestCase(1, 1, CardinalDirection.South)]
		[TestCase(-1, -1, CardinalDirection.North)]
		[TestCase(-1, 0, CardinalDirection.Northeast)]
		[TestCase(0, -1, CardinalDirection.Northwest)]
		public void ApproximateDirection_Simple(int w, int e, CardinalDirection expected)
		{
			var v = Tv(w, e);
			
			for (var i = 0; i < 3; i++)
			{
				Assert.AreEqual(expected, v.GetApproximateDirection().Value, "v=" + v.ToString());
				v *= 2;
			}
		}

		
		/// <summary>
		/// Tests GetApproximateDirection for a given TileVector.
		/// 
		/// This test is designed for vectors which sit evenly between two of the cardinal directions.
		/// As such, this takes two expected arguments, and will pass if EITHER of them is returned.
		/// 
		/// This test automatically scales up the given vectors, so use small arguments for best coverage.
		/// </summary>
		/// <param name="w">W component of the TileVector</param>
		/// <param name="e">E component of the TileVector</param>
		/// <param name="exp1">the expected result of GetApproximateDirection</param>
		/// <param name="exp2">the expected result of GetApproximateDirection</param>
		[TestCase(2, 1, CardinalDirection.Southwest, CardinalDirection.South)]
		[TestCase(1, 2, CardinalDirection.Southeast, CardinalDirection.South)]
		// TODO: more test cases!
		public void ApproximateDirection_Complex_Ambivalent(int w, int e, 
			CardinalDirection exp1, CardinalDirection exp2)
		{
			var v = Tv(w, e);
			for (var i = 0; i < 3; i++)
			{
				var dir = v.GetApproximateDirection().Value;
				if (dir != exp1 && dir != exp2)
				{
					// bit of a hack to show the assertion in a reasonable manner
					Assert.AreEqual(string.Format("{0} or {1}", exp1, exp2), dir, "v=" + v.ToString());
				}
				else
				{
					Assert.Pass("v={0}=>{1}", v, dir);
				}
				v *= 2;
			}
		}
		
		/// <summary>
		/// Tests GetApproximateDirection for a given TileVector.
		/// 
		/// This test is specifically targeted at non-cardinal vectors that are more closely aligned to
		/// one direction than the other.
		/// 
		/// This test automatically scales up the given vectors, so use small arguments for best coverage.
		/// </summary>
		/// <param name="w">W component of the TileVector</param>
		/// <param name="e">E component of the TileVector</param>
		/// <param name="expected">the expected result of GetApproximateDirection</param>
		[TestCase(-1, 2, CardinalDirection.Southeast)]
		[TestCase(1, 3, CardinalDirection.Southeast)]
		[TestCase(2, 3, CardinalDirection.South)]
		[TestCase(-3, -1, CardinalDirection.Northeast)]
		[TestCase(-2, 1, CardinalDirection.Northeast)]
		[TestCase(-3, -2, CardinalDirection.North)]
		[TestCase(-2, -3, CardinalDirection.North)]
		[TestCase(-2, -5, CardinalDirection.Northwest)]
		[TestCase(-1, -4, CardinalDirection.Northwest)]
		[TestCase(4, -1, CardinalDirection.Southwest)]
		[TestCase(5, 1, CardinalDirection.Southwest)]
		public void ApproximateDirection_Complex_Strict(int w, int e, CardinalDirection expected)
		{
			var v = Tv(w, e);			
			for (var i = 0; i < 3; i++)
			{
				Assert.AreEqual(expected, v.GetApproximateDirection().Value, 
					//"v={0} @{1}°", v.ToString());
					"v={0}", v.ToString());
				v *= 2;
			}
			//Assert.Pass(v.GetBearing() + "°");
		}
	}
}
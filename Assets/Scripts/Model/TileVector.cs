using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Model
{
	/// <summary>
	/// Representation of (discrete) hexagonal space
	/// </summary>
    public struct TileVector
    {
	    public readonly int W;
	    public readonly int E;

	    /// <summary>
	    /// Construct a new TileVector
	    /// </summary>
	    /// <param name="w">the W or SOUTHWEST component</param>
	    /// <param name="e">the E or SOUTHEAST component</param>
	    public TileVector(int w, int e)
	    {
		    W = w;
		    E = e;
	    }

	    public ICollection<TileVector> Adjacent(bool includeSelf = false)
	    {
		    var adjacent = new List<TileVector>();
		    if (includeSelf) adjacent.Add(this);

		    for (int i = 0; i < 6; i++)
		    {
			    var dir = CardinalDirection.North.ArcClockwise(i);
			    adjacent.Add(this + dir);
		    }
		    return adjacent;
	    }

	    public CardinalDirection? GetApproximateDirection()
	    {
		    var bearing = GetBearing();
		    if (float.IsNaN(bearing)) return null;
		    var dir = (int) (bearing + 30) % 360 / 60;	// round to nearest int from 0-5	    
		    return (CardinalDirection) dir;
	    }

	    public CardinalDirection? GetApproximateDirectionTo(TileVector other)
	    {
		    Utils.Printf("FROM: {0} TO: {1} DIR: {2}", this, other, (other - this).GetApproximateDirection());
		    return (other - this).GetApproximateDirection();
	    }

	    public float GetBearing()
	    {
		    if (W == 0 && E == 0) return float.NaN;
		    var angleSum = 0;
		    angleSum += CardinalDirection.Southwest.GetBearing() * W;
		    angleSum += CardinalDirection.Southeast.GetBearing() * E;
		    if (angleSum < 0) angleSum = 360 - angleSum;
		    return (float) angleSum / (W + E) % 360;
	    }

	    /// <summary>
	    /// Get distance
	    /// </summary>
	    /// <param name="a"></param>
	    /// <param name="b"></param>
	    /// <returns></returns>
	    public static int Distance(TileVector a, TileVector b)
	    {
		    TileVector diff = b - a;
		    return Math.Max(diff.W, diff.E);
	    }

	    // OPERATORS ON TILEVECTORS

	    public static TileVector operator +(TileVector a, TileVector b)
	    {
		    return new TileVector(a.W + b.W, a.E + b.E);
	    }

	    public static TileVector operator -(TileVector a, TileVector b)
	    {
		    return new TileVector(a.W - b.W, a.E - b.E);
	    }

	    public static TileVector operator -(TileVector a)
	    {
		    return new TileVector(-a.W, -a.W);
	    }

	    public static bool operator ==(TileVector a, TileVector b)
	    {
		    return a.Equals(b);
	    }

	    public static bool operator !=(TileVector a, TileVector b)
	    {
		    return !a.Equals(b);
	    }

	    // OPERATORS ON DIRECTION ENUMS

	    public static TileVector operator +(TileVector a, CardinalDirection d)
	    {
		    return a + d.GetTileVector();
	    }

	    public static TileVector operator -(TileVector a, CardinalDirection d)
	    {
		    return a - d.GetTileVector();
	    }

	    // OVERRRIDE METHODS

	    public override string ToString()
	    {
		    return String.Format("({0}, {1})", W, E);
	    }

	    public override bool Equals(object obj)
	    {
		    // Check for null values and compare run-time types.
		    if (obj == null || GetType() != obj.GetType())
			    return false;

		    TileVector t = (TileVector) obj;
		    return W == t.W && E == t.E;
	    }

	    public override int GetHashCode()
	    {
		    return W ^ E;
	    }
    }
}
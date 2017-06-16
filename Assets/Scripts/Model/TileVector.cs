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
		    if (W == 0 && E == 0) return null;	// zero vector has no direction
	
		    // use anon structs to tidy up logic significantly
		    var w = new {size = Math.Abs(W), dir = W > 0 ? CardinalDirection.Southwest : CardinalDirection.Northeast};
		    var e = new {size = Math.Abs(E), dir = E > 0 ? CardinalDirection.Southeast : CardinalDirection.Northwest};
		    var s = new {size = 0, dir = CardinalDirection.South};	// south component, default to zero
		    
		    if (Math.Sign(W) == Math.Sign(E))	// south = (1, 1), north = (-1, -1) - extract factor from W and E
		    {
			    s = new	// assign south
			    {
				    size = Math.Min(w.size, e.size), 
				    dir = W > 0 ? CardinalDirection.South : CardinalDirection.North
			    };
	
			    w = new {size = w.size - s.size, dir = w.dir};		// update w and e by subtracting the s
			    e = new {size = e.size - s.size, dir = e.dir};		// vector from them
		    }
		    
		    // find and return vector with greatest magnitude
		    if (w.size >= e.size)
		    {
			    return w.size >= s.size ? w.dir : s.dir;
		    }
		    else
		    {
			    return e.size >= s.size ? e.dir : s.dir;		    
		    }
	    }

	    public CardinalDirection? GetApproximateDirectionTo(TileVector other)
	    {
		    return (other - this).GetApproximateDirection();
	    }

//	    public float GetBearing()
//	    {
//		    if (W == 0 && E == 0) return float.NaN;
//		    var mean = 0.0f;
//
//		    // negate negative components to simplify possibility space
//		    if (W >= 0) mean += CardinalDirection.Southwest.GetBearing() * W;
//		    else 		mean -= CardinalDirection.Northeast.GetBearing() * W;
//		    if (E >= 0) mean += CardinalDirection.Southeast.GetBearing() * E;
//		    else 		mean -= CardinalDirection.Northwest.GetBearing() * E;
//
//		    mean /= Math.Abs(W) + Math.Abs(E); 				// take the mean angle
//		    
//		    if (W < 0 && E < 0) 		// edge case: move interval midpoint from 180 to 0 degrees
//		    {		
//			    mean = -180 - mean;
//			    while (mean < 0)		// correct negative values
//			    {
//				    mean += 360;
//			    }
//			}
//		    
//		    //Utils.Printf("{0}=>{1}", this, mean);
//		    return mean;
//	    }

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

	    public static TileVector operator *(TileVector a, int b)
	    {
		    return new TileVector(a.W*b, a.E*b);
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

		    var t = (TileVector) obj;
		    return W == t.W && E == t.E;
	    }

	    public override int GetHashCode()
	    {
		    return W ^ E;
	    }
    }
}
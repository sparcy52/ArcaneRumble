using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/ZigzagAi")]
	public class ZigzagAi : PredefinedPathAi
	{
		public int QuaterLength = 1;
		public RelativeDirection TurnDirection = RelativeDirection.ForwardLeft;
		
		protected override RelativeDirection[] SetPath()
		{
			var path = new RelativeDirection[QuaterLength * 4];	// initial value is Forward
			path[QuaterLength]     = TurnDirection;
			path[QuaterLength * 3] = TurnDirection.Mirror();
			return path;
		}

		protected override RelativeDirection SetDirectionHint()
		{
			return RelativeDirection.Forward;
		}

		protected override RelativeDirection SetMirrorHint()
		{
			return TurnDirection;
		}
	}
}

using System.CodeDom;
using Model;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(fileName = "Assets/Scripts/Behaviour/CircleWalkAi")]
	public class CircleWalkAi : PredefinedPathAi
	{
		public int SideLength = 1;
		public RelativeDirection TurnDirection = RelativeDirection.ForwardLeft;
		
		protected override RelativeDirection[] SetPath()
		{
			var path = new RelativeDirection[SideLength * 6];
			for (var i = 0; i < 6; i++)
			{
				path[i * SideLength] = TurnDirection;
			}
			return path;
		}
	}
}
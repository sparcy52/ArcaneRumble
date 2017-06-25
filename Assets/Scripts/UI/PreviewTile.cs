using UnityEngine;

public class PreviewTile : MonoBehaviour
{
	private MeshRenderer[] _renderers;
	private MeshRenderer[] Renderers
	{
		get
		{
			if (_renderers == null)
			{
				_renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
			}
			return _renderers;
		}
	}

	public void Paint(Color color)
	{
		foreach (var r in Renderers)
		{
			foreach (var m in r.materials)
			{
				if (m.HasProperty("_Color"))
				{
					m.color = color;
				}
			}
		}
	}

//	public void ResetPaint()
//	{
//		Color color = Player.Color;
//		color.a = 0.4f;
//		Paint(color);
//	}
}

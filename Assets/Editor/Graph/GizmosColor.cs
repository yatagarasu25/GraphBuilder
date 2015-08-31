using System;
using UnityEngine;

internal class GizmosColor : IDisposable
{
	private Color oldGizmosColor;

	public GizmosColor(Color color)
	{
		oldGizmosColor = Gizmos.color;
		Gizmos.color = color;
	}

	public void Dispose()
	{
		Gizmos.color = oldGizmosColor;
	}
}
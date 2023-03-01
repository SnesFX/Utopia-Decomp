using System;
using UnityEngine;

[Serializable]
public class ItemQuality : ScriptableObject
{
	[SerializeField]
	protected string _displayName = "Common";

	[SerializeField]
	protected Color _displayColor = Color.white;

	[SerializeField]
	protected float _multiplier = 1f;

	public string displayName
	{
		get
		{
			return _displayName;
		}
	}

	public Color displayColor
	{
		get
		{
			return _displayColor;
		}
	}

	public float multiplier
	{
		get
		{
			return Mathf.Clamp(_multiplier, 0f, float.MaxValue);
		}
	}
}

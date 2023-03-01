using System;
using UnityEngine;

[Serializable]
public class ObjectPlacerEntry
{
	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private GameObject _gameObject;

	[SerializeField]
	private float _scale = 1f;

	public string name
	{
		get
		{
			return _name;
		}
	}

	public GameObject gameObject
	{
		get
		{
			return _gameObject;
		}
	}

	public float scale
	{
		get
		{
			return _scale;
		}
	}
}

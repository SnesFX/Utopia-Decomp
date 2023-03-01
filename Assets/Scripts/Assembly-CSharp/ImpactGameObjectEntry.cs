using System;
using UnityEngine;

[Serializable]
public class ImpactGameObjectEntry
{
	[SerializeField]
	private string _key = string.Empty;

	[SerializeField]
	private GameObject _gameObject;

	public string key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	public GameObject gameObject
	{
		get
		{
			return _gameObject;
		}
		set
		{
			_gameObject = value;
		}
	}
}

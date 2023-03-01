using System;
using UnityEngine;

[Serializable]
public class ItemProperty : ScriptableObject
{
	[SerializeField]
	protected string _displayName = "Property";

	public string displayName
	{
		get
		{
			return _displayName;
		}
	}
}

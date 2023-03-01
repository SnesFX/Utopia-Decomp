using System;
using UnityEngine;

[Serializable]
public class AvatarStateIndex
{
	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private int _index;

	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}
}

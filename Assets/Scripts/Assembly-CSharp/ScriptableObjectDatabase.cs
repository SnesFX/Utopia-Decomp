using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableObjectDatabase<T> : ScriptableObject where T : ScriptableObject
{
	[SerializeField]
	protected internal List<T> _contents = new List<T>();

	public List<T> contents
	{
		get
		{
			return contents;
		}
	}
}

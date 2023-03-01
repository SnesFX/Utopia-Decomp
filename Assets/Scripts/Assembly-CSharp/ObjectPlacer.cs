using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPlacer : MonoBehaviour
{
	[SerializeField]
	private List<ObjectPlacerEntry> _objects = new List<ObjectPlacerEntry>();

	public List<ObjectPlacerEntry> objects
	{
		get
		{
			return _objects;
		}
	}
}

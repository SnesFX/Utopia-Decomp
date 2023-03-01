using System.Collections.Generic;
using UnityEngine;

public sealed class GravityData
{
	private List<GravityEntry> _fields = new List<GravityEntry>();

	private Vector3 _force = Vector3.zero;

	public List<GravityEntry> fields
	{
		get
		{
			return _fields;
		}
		set
		{
			_fields = value;
		}
	}

	public Vector3 force
	{
		get
		{
			return _force;
		}
		set
		{
			_force = value;
		}
	}
}

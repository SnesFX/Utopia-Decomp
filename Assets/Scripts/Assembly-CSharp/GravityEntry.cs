using UnityEngine;

public sealed class GravityEntry
{
	private GravityField _field;

	private Collider _collider;

	private Vector3 _force = Vector3.zero;

	public GravityField field
	{
		get
		{
			return _field;
		}
		set
		{
			_field = value;
		}
	}

	public Collider collider
	{
		get
		{
			return _collider;
		}
		set
		{
			_collider = value;
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

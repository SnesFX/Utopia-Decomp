using UnityEngine;

public struct GravityInfo
{
	private Collider _collider;

	private Vector3 _force;

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

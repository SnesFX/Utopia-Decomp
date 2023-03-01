using UnityEngine;

public sealed class PhysicsVolumeEntry
{
	private PhysicsVolume _volume;

	private Vector3 _centerOfBuoyancy = Vector3.zero;

	private Vector3 _surfacePoint = Vector3.zero;

	private Vector3 _surfaceNormal = Vector3.zero;

	public PhysicsVolume volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
		}
	}

	public Vector3 centerOfBuoyancy
	{
		get
		{
			return _centerOfBuoyancy;
		}
		set
		{
			_centerOfBuoyancy = value;
		}
	}

	public Vector3 surfacePoint
	{
		get
		{
			return _surfacePoint;
		}
		set
		{
			_surfacePoint = value;
		}
	}

	public Vector3 surfaceNormal
	{
		get
		{
			return _surfaceNormal.normalized;
		}
		set
		{
			_surfaceNormal = value.normalized;
		}
	}
}

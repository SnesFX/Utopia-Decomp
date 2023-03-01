using UnityEngine;

public struct PhysicsVolumeInfo
{
	private Vector3 _centerOfBuoyancy;

	private Vector3 _surfacePoint;

	private Vector3 _surfaceNormal;

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

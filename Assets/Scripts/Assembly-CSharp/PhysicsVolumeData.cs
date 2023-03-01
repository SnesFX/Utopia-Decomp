using System.Collections.Generic;
using UnityEngine;

public sealed class PhysicsVolumeData
{
	private List<PhysicsVolumeEntry> _volumes = new List<PhysicsVolumeEntry>();

	private PhysicsVolume _mainVolume;

	private Collider _mainCollider;

	private float _density;

	private float _submersion;

	private Vector3 _centerOfSubmersion = Vector3.zero;

	private Vector3 _centerOfBuoyancy = Vector3.zero;

	private Vector3 _surfacePoint = Vector3.zero;

	private Vector3 _surfaceNormal = Vector3.zero;

	public List<PhysicsVolumeEntry> volumes
	{
		get
		{
			return _volumes;
		}
		set
		{
			_volumes = value;
		}
	}

	public PhysicsVolume mainVolume
	{
		get
		{
			return _mainVolume;
		}
		set
		{
			_mainVolume = value;
		}
	}

	public Collider mainCollider
	{
		get
		{
			return _mainCollider;
		}
		set
		{
			_mainCollider = value;
		}
	}

	public PhysicsSubstance mainPhysicsSubstance
	{
		get
		{
			return (!_mainVolume) ? null : _mainVolume.substance;
		}
	}

	public float density
	{
		get
		{
			return _density;
		}
		set
		{
			_density = value;
		}
	}

	public float submersion
	{
		get
		{
			return Mathf.Clamp01(_submersion);
		}
		set
		{
			_submersion = Mathf.Clamp01(value);
		}
	}

	public Vector3 centerOfSubmersion
	{
		get
		{
			return _centerOfSubmersion;
		}
		set
		{
			_centerOfSubmersion = value;
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

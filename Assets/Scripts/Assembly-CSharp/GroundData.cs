using UnityEngine;

public sealed class GroundData
{
	private GameObject _gameObject;

	private Rigidbody _rigidbody;

	private Collider _collider;

	private PhysicMaterial _material;

	private Vector3 _normal = Vector3.zero;

	private Vector3 _point = Vector3.zero;

	private Vector3 _surfaceVelocity = Vector3.zero;

	private bool _isFluid;

	public GameObject gameObject
	{
		get
		{
			return _gameObject;
		}
		set
		{
			_gameObject = value;
		}
	}

	public Rigidbody rigidbody
	{
		get
		{
			return _rigidbody;
		}
		set
		{
			_rigidbody = value;
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

	public PhysicMaterial material
	{
		get
		{
			return _material;
		}
		set
		{
			_material = value;
		}
	}

	public float friction
	{
		get
		{
			return (!(material != null)) ? 0.6f : material.staticFriction;
		}
	}

	public string materialName
	{
		get
		{
			return (!(material != null)) ? string.Empty : material.name.Replace(" (Instance)", string.Empty);
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			return (!(rigidbody != null)) ? Vector3.zero : rigidbody.angularVelocity;
		}
	}

	public Vector3 normal
	{
		get
		{
			return _normal.normalized;
		}
		set
		{
			_normal = value.normalized;
		}
	}

	public Vector3 point
	{
		get
		{
			return _point;
		}
		set
		{
			_point = value;
		}
	}

	public Vector3 surfaceVelocity
	{
		get
		{
			return _surfaceVelocity;
		}
		set
		{
			_surfaceVelocity = value;
		}
	}

	public Vector3 velocity
	{
		get
		{
			return (!(rigidbody != null)) ? Vector3.zero : rigidbody.velocity;
		}
	}

	public bool isFluid
	{
		get
		{
			return _isFluid;
		}
		set
		{
			_isFluid = value;
		}
	}

	public Vector3 GetVelocityAtPoint()
	{
		return (!rigidbody) ? Vector3.zero : (velocity + Vector3.Cross(rigidbody.transform.position - point, angularVelocity));
	}

	public Vector3 GetVelocityAtPoint(Vector3 vPoint)
	{
		return (!rigidbody) ? Vector3.zero : (velocity + Vector3.Cross(rigidbody.transform.position - vPoint, angularVelocity));
	}

	public Vector3 GetVelocityOnPoint()
	{
		return GetVelocityAtPoint() + Utility.RelativeVector(surfaceVelocity, normal);
	}

	public Vector3 GetVelocityOnPoint(Vector3 vPoint)
	{
		return GetVelocityAtPoint(vPoint) + Utility.RelativeVector(surfaceVelocity, normal);
	}

	public void SetData(Collider oCollider, Vector3 vPoint, Vector3 vNormal)
	{
		collider = oCollider;
		gameObject = oCollider.gameObject;
		rigidbody = oCollider.GetComponent<Rigidbody>();
		material = ((!(oCollider is TerrainCollider)) ? oCollider.material : (oCollider as TerrainCollider).material);
		point = vPoint;
		normal = vNormal;
		surfaceVelocity = Vector3.zero;
	}

	public void Clear()
	{
		gameObject = null;
		collider = null;
		point = Vector3.zero;
		normal = Vector3.zero;
		surfaceVelocity = Vector3.zero;
	}
}

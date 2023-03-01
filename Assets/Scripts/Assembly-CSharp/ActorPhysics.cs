using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Actor))]
[DisallowMultipleComponent]
public class ActorPhysics : MonoBehaviour
{
	public enum PhysicsQuality
	{
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3
	}

	private Actor _actor;

	[SerializeField]
	private Collider _boundingCollider;

	[SerializeField]
	private LayerMask _gravityLayers = default(LayerMask);

	[SerializeField]
	private LayerMask _fluidLayers = default(LayerMask);

	[SerializeField]
	private LayerMask _groundLayers = default(LayerMask);

	private PortalData _portalData = new PortalData();

	private GravityData _gravityData = new GravityData();

	private PhysicsVolumeData _physicsVolumeData = new PhysicsVolumeData();

	private GroundData _ground = new GroundData();

	private Vector3 _gravity = Physics.gravity;

	private Rigidbody _rigidbody;

	private Rigidbody2D _rigidbody2D;

	[SerializeField]
	private Vector3 _center = Vector3.zero;

	[SerializeField]
	private Vector3 _centerOfMass = Vector3.zero;

	[SerializeField]
	private Vector3 _size = Vector3.one;

	[SerializeField]
	private bool _useGravity = true;

	[SerializeField]
	private float _mass = 1f;

	[SerializeField]
	private float _density = 1000f;

	[SerializeField]
	private float _drag;

	[SerializeField]
	private float _angularDrag = 0.05f;

	[SerializeField]
	private PhysicsQuality _physicsQuality = PhysicsQuality.Medium;

	[SerializeField]
	private int _physicsVoxels = 3;

	public Actor actor
	{
		get
		{
			return (!_actor) ? (_actor = GetComponent<Actor>()) : _actor;
		}
	}

	public Collider boundingCollider
	{
		get
		{
			return (!_boundingCollider) ? (_boundingCollider = GetComponent<Collider>()) : _boundingCollider;
		}
	}

	public PortalData portalData
	{
		get
		{
			return _portalData;
		}
	}

	public GravityData gravityData
	{
		get
		{
			return _gravityData;
		}
	}

	public PhysicsVolumeData physicsVolumeData
	{
		get
		{
			return _physicsVolumeData;
		}
	}

	public GroundData ground
	{
		get
		{
			return _ground;
		}
	}

	public bool grounded
	{
		get
		{
			return (ground.collider != null) & (ground.normal != Vector3.zero);
		}
		set
		{
			if (!value)
			{
				_ground.Clear();
			}
		}
	}

	public LayerMask gravityLayers
	{
		get
		{
			return _gravityLayers;
		}
	}

	public LayerMask fluidLayers
	{
		get
		{
			return _fluidLayers;
		}
	}

	public LayerMask groundLayers
	{
		get
		{
			return _groundLayers;
		}
	}

	public float angularDrag
	{
		get
		{
			return Mathf.Clamp(_angularDrag, 0f, float.MaxValue);
		}
		set
		{
			_angularDrag = Mathf.Clamp(value, 0f, float.MaxValue);
			if ((bool)rigidbody)
			{
				rigidbody.angularDrag = 0f;
			}
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			return (!rigidbody) ? Vector3.zero : rigidbody.angularVelocity;
		}
		set
		{
			if ((bool)rigidbody)
			{
				rigidbody.angularVelocity = value;
			}
		}
	}

	public Vector3 center
	{
		get
		{
			return _center;
		}
		set
		{
			_center = value;
		}
	}

	public Vector3 centerOfMass
	{
		get
		{
			return _centerOfMass;
		}
		set
		{
			_centerOfMass = value;
			if ((bool)rigidbody)
			{
				rigidbody.centerOfMass = value;
			}
		}
	}

	public float drag
	{
		get
		{
			return Mathf.Clamp(_drag, 0f, float.MaxValue);
		}
		set
		{
			_drag = Mathf.Clamp(value, 0f, float.MaxValue);
			if ((bool)rigidbody)
			{
				rigidbody.drag = 0f;
			}
		}
	}

	public Vector3 gravity
	{
		get
		{
			return gravityData.force;
		}
		protected set
		{
			gravityData.force = value;
		}
	}

	public float mass
	{
		get
		{
			return Mathf.Clamp(_mass, 0f, float.MaxValue);
		}
		set
		{
			_mass = Mathf.Clamp(value, 0f, float.MaxValue);
			if ((bool)rigidbody)
			{
				rigidbody.mass = value;
			}
		}
	}

	public float density
	{
		get
		{
			return Mathf.Clamp(_density, 0f, float.MaxValue);
		}
		set
		{
			_density = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float radius
	{
		get
		{
			return (!(size.x > size.z)) ? (size.z * 0.5f) : (size.x * 0.5f);
		}
	}

	public Rigidbody rigidbody
	{
		get
		{
			return (!_rigidbody) ? (_rigidbody = GetComponent<Rigidbody>()) : _rigidbody;
		}
	}

	public Rigidbody2D rigidbody2D
	{
		get
		{
			return (!_rigidbody2D) ? (_rigidbody2D = GetComponent<Rigidbody2D>()) : _rigidbody2D;
		}
	}

	public Vector3 size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
		}
	}

	public Vector3 up
	{
		get
		{
			return -gravity.normalized;
		}
	}

	public bool useGravity
	{
		get
		{
			return _useGravity;
		}
		set
		{
			_useGravity = value;
			if ((bool)rigidbody)
			{
				rigidbody.useGravity = value;
			}
		}
	}

	public Vector3 velocity
	{
		get
		{
			if ((bool)rigidbody)
			{
				return rigidbody.velocity;
			}
			return Vector3.zero;
		}
		set
		{
			if ((bool)rigidbody)
			{
				rigidbody.velocity = value;
			}
		}
	}

	public PhysicsQuality physicsQuality
	{
		get
		{
			return _physicsQuality;
		}
		set
		{
			_physicsQuality = value;
		}
	}

	public int physicsVoxels
	{
		get
		{
			return Mathf.Clamp(_physicsVoxels, 0, 5);
		}
		set
		{
			_physicsVoxels = Mathf.Clamp(value, 0, 5);
		}
	}

	public Vector3 worldCenter
	{
		get
		{
			return base.transform.position + base.transform.rotation * center;
		}
	}

	public Vector3 worldCenterOfMass
	{
		get
		{
			return base.transform.position + base.transform.rotation * centerOfMass;
		}
	}

	public Vector3 worldTopPoint
	{
		get
		{
			return worldCenter + base.transform.up * size.y * 0.5f;
		}
	}

	public Vector3 worldBottomPoint
	{
		get
		{
			return worldCenter - base.transform.up * size.y * 0.5f;
		}
	}

	public event ActorPortalEventHandler PortalShifted;

	public void AddForce(Vector3 vForce)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddForce(vForce);
		}
	}

	public void AddForce(Vector3 vForce, ForceMode oForceMode)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddForce(vForce, oForceMode);
		}
	}

	public void AddForceAtPosition(Vector3 vForce, Vector3 vPosition)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddForceAtPosition(vForce, vPosition);
		}
	}

	public void AddForceAtPosition(Vector3 vForce, Vector3 vPosition, ForceMode oForceMode)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddForceAtPosition(vForce, vPosition, oForceMode);
		}
	}

	public void AddRelativeForce(Vector3 vForce)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddRelativeForce(vForce);
		}
	}

	public void AddRelativeForce(Vector3 vForce, ForceMode oForceMode)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddRelativeForce(vForce, oForceMode);
		}
	}

	public void AddRelativeTorque(Vector3 vTorque)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddRelativeTorque(vTorque);
		}
	}

	public void AddRelativeTorque(Vector3 vTorque, ForceMode oForceMode)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddRelativeTorque(vTorque, oForceMode);
		}
	}

	public void AddTorque(Vector3 vTorque)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddTorque(vTorque);
		}
	}

	public void AddTorque(Vector3 vTorque, ForceMode oForceMode)
	{
		if ((bool)rigidbody)
		{
			rigidbody.AddTorque(vTorque, oForceMode);
		}
	}

	public void MovePosition(Vector3 vPosition)
	{
		if ((bool)rigidbody)
		{
			rigidbody.MovePosition(vPosition);
		}
	}

	public void MoveRotation(Quaternion qRotation)
	{
		if ((bool)rigidbody)
		{
			rigidbody.MoveRotation(qRotation);
		}
	}

	public void EnteredPortal(Portal oPortal)
	{
	}

	public void ShiftedPortal(Vector3 vPosition, Quaternion qRotation)
	{
	}

	public void ExitedPortal(Portal oPortal)
	{
	}

	public void EnteredGravityField(GravityField oField)
	{
		if (!(oField == null) && gravityData.fields.FindIndex((GravityEntry o) => o.field == oField) == -1)
		{
			gravityData.fields.Add(new GravityEntry
			{
				field = oField
			});
			OnGravityEnter(oField);
		}
	}

	public void ExitedGravityField(GravityField oField)
	{
		if ((bool)oField)
		{
			OnGravityExit(oField);
		}
		gravityData.fields.RemoveAll((GravityEntry o) => o.field == oField);
	}

	public void EnteredPhysicsVolume(PhysicsVolume oVolume)
	{
		if (!(oVolume == null) && physicsVolumeData.volumes.FindIndex((PhysicsVolumeEntry o) => o.volume == oVolume) == -1)
		{
			physicsVolumeData.volumes.Add(new PhysicsVolumeEntry
			{
				volume = oVolume
			});
			OnPhysicsVolmeEnter(oVolume);
		}
	}

	public void ExitedPhysicsVolume(PhysicsVolume oVolume)
	{
		if ((bool)oVolume)
		{
			OnPhysicsVolumeExit(oVolume);
		}
		physicsVolumeData.volumes.RemoveAll((PhysicsVolumeEntry o) => o.volume == oVolume);
	}

	protected void UpdatePortals()
	{
	}

	protected void UpdateGravity()
	{
		Vector3 zero = Vector3.zero;
		bool flag = false;
		for (int i = 0; i < gravityData.fields.Count; i++)
		{
			if ((bool)gravityData.fields[i].field)
			{
				GravityInfo oInfo = default(GravityInfo);
				gravityData.fields[i].field.GetGravityInfo(worldCenterOfMass, gravityData.fields[i].force, out oInfo);
				gravityData.fields[i].collider = oInfo.collider;
				gravityData.fields[i].force = oInfo.force;
				if (gravityData.fields[i].field.overrideGlobalGravity)
				{
					flag = true;
				}
				zero += oInfo.force;
			}
		}
		if (!flag)
		{
			zero += Physics.gravity;
		}
		gravityData.force = zero;
	}

	protected void UpdateVolumes()
	{
		PhysicsVolume mainVolume = null;
		Collider mainCollider = null;
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = 0f;
		float num2 = 0f;
		float num3 = float.MinValue;
		int num4 = physicsVoxels;
		if (physicsVolumeData.volumes.Count > 0)
		{
			mainVolume = physicsVolumeData.volumes[0].volume;
			Bounds bounds = ((!boundingCollider) ? new Bounds(worldCenter, size) : boundingCollider.bounds);
			Quaternion quaternion = Quaternion.FromToRotation(base.transform.up, up);
			for (int i = 0; i < num4; i++)
			{
				for (int j = 0; j < num4; j++)
				{
					float x = size.x;
					float num5 = Vector3.Dot(up, bounds.size);
					float z = size.z;
					float num6 = Mathf.Clamp(x * 0.5f, 0f, num5);
					Vector3 vector2 = base.transform.right * ((num4 <= 1) ? 0f : (x * -0.5f + (float)i * (x / (float)(num4 - 1))));
					vector2 = base.transform.forward * ((num4 <= 1) ? 0f : (z * -0.5f + (float)j * (z / (float)(num4 - 1)))) + vector2;
					vector2 = quaternion * vector2;
					Vector3 vector3 = bounds.center + vector2 + up * (num5 * 0.5f);
					Vector3 vector4 = bounds.center + vector2 - up * (num5 * 0.5f);
					RaycastHit hitInfo;
					if (Vector3.Distance(vector3, vector4) > 0f && Physics.Linecast(vector3, vector4, out hitInfo, fluidLayers, QueryTriggerInteraction.Collide))
					{
						num += Mathf.Clamp01(1f - hitInfo.distance / Vector3.Distance(vector3, vector4));
						zero += hitInfo.normal;
						Vector3 vector5 = Utility.VectorInDirection(hitInfo.point, up);
						vector = ((!(Vector3.Dot(vector5 - vector, up) > 0f)) ? vector : vector5);
						mainCollider = hitInfo.collider;
					}
					else
					{
						num += 1f;
					}
				}
			}
			for (int k = 0; k < physicsVolumeData.volumes.Count; k++)
			{
				num2 += physicsVolumeData.volumes[k].volume.density;
			}
			num2 = ((physicsVolumeData.volumes.Count <= 0) ? 1.225f : (num2 / (float)physicsVolumeData.volumes.Count));
			num = ((num4 <= 0) ? 1f : (num / (float)(num4 * num4)));
			zero2 = worldCenter + up * size.y * (-0.5f + num * 0.5f);
		}
		else
		{
			num2 = 0f;
			num = 0f;
			zero2 = worldCenter + up * size.y * (-0.5f + num * 0.5f);
		}
		physicsVolumeData.surfacePoint = Utility.RelativeVector(worldCenter, up) + vector;
		physicsVolumeData.surfaceNormal = zero;
		physicsVolumeData.mainVolume = mainVolume;
		physicsVolumeData.mainCollider = mainCollider;
		physicsVolumeData.density = num2;
		physicsVolumeData.submersion = num;
		physicsVolumeData.centerOfBuoyancy = zero2;
		physicsVolumeData.centerOfSubmersion = zero2;
	}

	protected void UpdatePhysics()
	{
	}

	protected void ApplyPhysics()
	{
		if ((bool)rigidbody && !rigidbody.IsSleeping())
		{
			CancelDefaultGravity();
			ApplyGravity();
			ApplyVolumeBuoyancy();
			ApplyVolumeDrag();
		}
	}

	protected void CancelDefaultGravity()
	{
		AddForce(-Physics.gravity * mass);
	}

	protected void ApplyGravity()
	{
		AddForce(gravity * mass);
	}

	protected void ApplyVolumeBuoyancy()
	{
		if (physicsVolumeData.density > 0f && density > 0f)
		{
			AddForceAtPosition(-gravity * mass * (physicsVolumeData.density / density) * physicsVolumeData.submersion, physicsVolumeData.centerOfBuoyancy);
		}
	}

	public void ApplyVolumeDrag()
	{
		if (mass > 0f)
		{
			AddForceAtPosition(Vector3.ClampMagnitude(-velocity, physicsVolumeData.density * velocity.magnitude * 0.5f * drag * Time.deltaTime / mass) * physicsVolumeData.submersion, physicsVolumeData.centerOfSubmersion);
			AddTorque(Vector3.ClampMagnitude(-angularVelocity, physicsVolumeData.density * angularVelocity.magnitude * 0.5f * angularDrag * Time.deltaTime / mass) * physicsVolumeData.submersion);
		}
	}

	private void Awake()
	{
		if ((bool)rigidbody)
		{
			rigidbody.angularDrag = 0f;
			rigidbody.drag = 0f;
			rigidbody.mass = mass;
			rigidbody.centerOfMass = centerOfMass;
		}
	}

	private void FixedUpdate()
	{
		if (physicsQuality != 0)
		{
			UpdatePortals();
			UpdateGravity();
			UpdateVolumes();
			UpdatePhysics();
			ApplyPhysics();
		}
	}

	private void OnPortalEnter(Portal portal)
	{
	}

	private void OnPortalShift(Vector3 vDeltaPosition, Quaternion qDeltaRotation)
	{
	}

	private void OnPortalExit(Portal portal)
	{
	}

	private void OnGravityEnter(GravityField field)
	{
	}

	private void OnGravityExit(GravityField field)
	{
	}

	private void OnPhysicsVolmeEnter(PhysicsVolume volume)
	{
	}

	private void OnPhysicsVolumeExit(PhysicsVolume volume)
	{
	}
}

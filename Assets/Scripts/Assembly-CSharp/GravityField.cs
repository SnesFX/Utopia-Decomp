using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class GravityField : Actor
{
	[SerializeField]
	private GravityFieldType _fieldType;

	[SerializeField]
	private LayerMask _gravityLayers = default(LayerMask);

	[SerializeField]
	private bool _overrideGlobalGravity = true;

	[SerializeField]
	private bool _overrideGravityFields = true;

	[SerializeField]
	private float _strength = 9.81f;

	public GravityFieldType fieldType
	{
		get
		{
			return _fieldType;
		}
		set
		{
			_fieldType = value;
		}
	}

	public LayerMask gravityLayers
	{
		get
		{
			return _gravityLayers;
		}
		set
		{
			_gravityLayers = value;
		}
	}

	public bool overrideGlobalGravity
	{
		get
		{
			return _overrideGlobalGravity;
		}
		set
		{
			_overrideGlobalGravity = value;
		}
	}

	public bool overrideGravityFields
	{
		get
		{
			return _overrideGravityFields;
		}
		set
		{
			_overrideGravityFields = value;
		}
	}

	public float strength
	{
		get
		{
			return _strength;
		}
		set
		{
			_strength = value;
		}
	}

	public virtual void GetGravityInfo(Vector3 vPosition, Vector3 vCurrentForce, out GravityInfo oInfo)
	{
		GravityInfo gravityInfo = default(GravityInfo);
		gravityInfo.collider = GetComponent<Collider>();
		GravityInfo gravityInfo2 = gravityInfo;
		RaycastHit hitInfo3;
		if (fieldType == GravityFieldType.Directional)
		{
			gravityInfo2.force = -base.transform.up * strength;
		}
		else if (fieldType == GravityFieldType.Spherical)
		{
			gravityInfo2.force = (base.transform.position - vPosition).normalized * strength;
		}
		else if (fieldType == GravityFieldType.Centered)
		{
			Vector3 position = base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Linecast(vPosition, position, out hitInfo, gravityLayers))
			{
				gravityInfo2.collider = hitInfo.collider;
				gravityInfo2.force = -hitInfo.normal * strength;
			}
		}
		else if (fieldType == GravityFieldType.Freeform)
		{
			RaycastHit hitInfo2;
			if (Physics.Raycast(vPosition, vCurrentForce, out hitInfo2, float.PositiveInfinity, gravityLayers))
			{
				gravityInfo2.collider = hitInfo2.collider;
				gravityInfo2.force = -hitInfo2.normal * strength;
			}
			else if (Physics.Linecast(vPosition, base.transform.position, out hitInfo2, gravityLayers))
			{
				gravityInfo2.collider = hitInfo2.collider;
				gravityInfo2.force = -hitInfo2.normal * strength;
			}
			else
			{
				gravityInfo2.force = (base.transform.position - vPosition).normalized * strength;
			}
		}
		else if (fieldType == GravityFieldType.FreeformNoDefaults && Physics.Raycast(vPosition, vCurrentForce, out hitInfo3, float.PositiveInfinity, gravityLayers))
		{
			gravityInfo2.collider = hitInfo3.collider;
			gravityInfo2.force = -hitInfo3.normal * strength;
		}
		oInfo = gravityInfo2;
	}

	private void Awake()
	{
	}

	private void OnTriggerEnter(Collider oCollider)
	{
		ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			component.EnteredGravityField(this);
		}
	}

	private void OnTriggerExit(Collider oCollider)
	{
		ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			component.ExitedGravityField(this);
		}
	}
}

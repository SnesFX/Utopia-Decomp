using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class Portal : Actor
{
	[SerializeField]
	protected internal Portal _link;

	[SerializeField]
	protected internal Camera _renderCamera;

	[SerializeField]
	protected internal Collider _mount;

	[SerializeField]
	protected internal Vector3 _exitDirection = Vector3.forward;

	public Vector3 exitDirection
	{
		get
		{
			if (_exitDirection == Vector3.zero)
			{
				_exitDirection = Vector3.forward;
			}
			return _exitDirection.normalized;
		}
		set
		{
			if (value == Vector3.zero)
			{
				value = Vector3.forward;
			}
			_exitDirection = value.normalized;
		}
	}

	public Portal link
	{
		get
		{
			return _link;
		}
		set
		{
			_link = value;
		}
	}

	public Collider mount
	{
		get
		{
			return _mount;
		}
		set
		{
			_mount = value;
		}
	}

	public Camera renderCamera
	{
		get
		{
			return _renderCamera;
		}
		set
		{
			_renderCamera = value;
		}
	}

	public Matrix4x4 GetShiftMatrix()
	{
		if ((bool)link && link != this)
		{
			return Matrix4x4.TRS(link.transform.position - base.transform.position, Quaternion.Inverse(base.transform.rotation) * link.transform.rotation, Vector3.zero);
		}
		return Matrix4x4.zero;
	}

	public Quaternion GetDeltaRotation()
	{
		if ((bool)link)
		{
			return Quaternion.Inverse(base.transform.rotation) * link.transform.rotation;
		}
		return Quaternion.identity;
	}

	public Vector3 GetDeltaPosition(Vector3 vPosition)
	{
		return GetShiftedPosition(vPosition) - vPosition;
	}

	public Vector3 GetShiftedPosition(Vector3 vPosition)
	{
		if ((bool)link)
		{
			vPosition = GetDeltaRotation() * (vPosition - base.transform.position) + link.transform.position;
		}
		return vPosition;
	}

	public Vector3 GetShiftedVector(Vector3 vVector)
	{
		return GetDeltaRotation() * vVector;
	}

	private void OnWillRenderObject()
	{
		if ((bool)renderCamera && link.renderCamera != Camera.current && renderCamera != Camera.current)
		{
			if ((bool)link)
			{
				Vector3 position = base.transform.InverseTransformPoint(Camera.current.transform.position);
				Quaternion quaternion = Quaternion.Inverse(base.transform.rotation) * Camera.current.transform.rotation;
				position = link.transform.TransformPoint(position);
				quaternion = link.transform.rotation * quaternion;
				renderCamera.transform.position = position;
				renderCamera.transform.rotation = quaternion;
				renderCamera.fieldOfView = Camera.current.fieldOfView;
				renderCamera.Render();
			}
			else
			{
				renderCamera.transform.position = base.transform.position;
				renderCamera.transform.rotation = base.transform.rotation;
			}
		}
	}

	private void OnTriggerEnter(Collider oCollider)
	{
		if ((bool)_link)
		{
			if ((bool)_mount)
			{
				Physics.IgnoreCollision(oCollider, _mount, true);
			}
			ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
			if ((bool)component)
			{
				component.EnteredPortal(this);
			}
		}
		else if ((bool)_mount)
		{
			Physics.IgnoreCollision(oCollider, _mount, false);
		}
	}

	private void OnTriggerStay(Collider oCollider)
	{
		if ((bool)_link)
		{
			if ((bool)oCollider.attachedRigidbody && Vector3.Dot(oCollider.bounds.center - base.transform.position, base.transform.rotation * exitDirection) < 0f)
			{
				Quaternion deltaRotation = GetDeltaRotation();
				Quaternion rotation = deltaRotation * oCollider.transform.rotation;
				Vector3 shiftedPosition = GetShiftedPosition(oCollider.transform.position);
				Vector3 shiftedVector = GetShiftedVector(oCollider.attachedRigidbody.velocity);
				Vector3 angularVelocity = deltaRotation * oCollider.attachedRigidbody.angularVelocity;
				Vector3 deltaPosition = GetDeltaPosition(oCollider.transform.position);
				oCollider.transform.position = shiftedPosition;
				oCollider.attachedRigidbody.velocity = shiftedVector;
				oCollider.attachedRigidbody.angularVelocity = angularVelocity;
				oCollider.transform.rotation = rotation;
				ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
				if ((bool)component)
				{
					component.ShiftedPortal(deltaPosition, deltaRotation);
				}
			}
		}
		else if ((bool)_mount)
		{
			Physics.IgnoreCollision(oCollider, _mount, false);
		}
	}

	private void OnTriggerExit(Collider oCollider)
	{
		if ((bool)_mount)
		{
			Physics.IgnoreCollision(oCollider, _mount, false);
		}
		ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			component.ExitedPortal(this);
		}
	}
}

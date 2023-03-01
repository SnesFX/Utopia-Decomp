using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Motobug : Badnik
	{
		public enum State
		{
			Default = 0,
			Alarm = 1,
			Attack = 2
		}

		[SerializeField]
		private State _state;

		[SerializeField]
		private LayerMask _groundLayers = default(LayerMask);

		[SerializeField]
		private float _groundRaycastRange = 0.25f;

		public State state
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		public LayerMask groundLayers
		{
			get
			{
				return _groundLayers;
			}
		}

		public float groundRaycastRange
		{
			get
			{
				return Mathf.Clamp(_groundRaycastRange, 0f, float.MaxValue);
			}
		}

		public void SetState(State oState)
		{
			if (state == oState)
			{
				return;
			}
			switch (oState)
			{
			case State.Default:
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Default");
				}
				break;
			case State.Alarm:
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Alarm");
				}
				break;
			case State.Attack:
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Attack");
				}
				break;
			}
			state = oState;
		}

		private void FixedUpdate()
		{
			if ((bool)base.rigidbody)
			{
				bool flag = false;
				Vector3 normal = Vector3.up;
				RaycastHit hitInfo;
				if (Physics.Raycast(base.transform.position + base.transform.up * groundRaycastRange * 0.5f, -base.transform.up, out hitInfo, groundRaycastRange, groundLayers, QueryTriggerInteraction.Ignore))
				{
					flag = true;
					normal = hitInfo.normal;
				}
				Vector3 vector = Utility.RelativeVector(Physics.gravity, normal);
				Vector3 vector2 = Vector3.ClampMagnitude(Utility.RelativeVector(base.transform.rotation * base.desiredDrive, normal), base.speed);
				vector2 -= vector * Time.deltaTime;
				Vector3 forward = Utility.RelativeVector(base.transform.rotation * base.desiredAngle, normal);
				if (forward.magnitude == 0f)
				{
					forward = base.transform.forward;
				}
				if ((bool)base.avatar)
				{
					base.avatar.drive = Quaternion.Inverse(base.transform.rotation) * vector2;
				}
				base.rigidbody.velocity = vector2 + Utility.VectorInDirection(base.rigidbody.velocity, normal);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(forward, normal), base.rotateSpeed * Time.deltaTime);
			}
		}
	}
}

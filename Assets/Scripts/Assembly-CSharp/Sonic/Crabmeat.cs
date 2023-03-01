using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Crabmeat : Badnik
	{
		public enum State
		{
			Scuttle = 0,
			Fire = 1
		}

		[SerializeField]
		private State _state;

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

		private void FixedUpdate()
		{
			if ((bool)base.rigidbody)
			{
				Vector3 vector = Vector3.ClampMagnitude(Utility.RelativeVector(base.transform.rotation * base.desiredDrive, -Physics.gravity.normalized), base.speed * Time.deltaTime);
				Vector3 forward = Utility.RelativeVector(base.transform.rotation * base.desiredAngle, -Physics.gravity.normalized);
				if (forward.magnitude == 0f)
				{
					forward = base.transform.forward;
				}
				base.rigidbody.MovePosition(base.transform.position + vector);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(forward, -Physics.gravity.normalized), base.rotateSpeed * Time.deltaTime);
			}
			if ((bool)base.avatar)
			{
				if (state == State.Fire)
				{
					base.avatar.SetState("Fire");
				}
				else
				{
					base.avatar.SetState("Scuttle");
				}
			}
		}
	}
}

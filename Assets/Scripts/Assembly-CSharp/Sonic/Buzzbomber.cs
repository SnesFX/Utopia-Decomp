using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Buzzbomber : Badnik
	{
		public enum State
		{
			Fly = 0,
			Attack = 1
		}

		[SerializeField]
		private State _state;

		public State state
		{
			get
			{
				return _state;
			}
			protected set
			{
				_state = value;
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
			case State.Fly:
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Fly");
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
				Vector3 vector = base.transform.rotation * base.desiredDrive - base.rigidbody.velocity;
				Vector3 forward = Utility.RelativeVector(base.transform.rotation * base.desiredAngle, -Physics.gravity.normalized);
				if (forward.magnitude == 0f)
				{
					forward = base.transform.forward;
				}
				if ((bool)base.avatar)
				{
					base.avatar.drive = Quaternion.Inverse(base.transform.rotation) * vector;
				}
				base.rigidbody.velocity = base.rigidbody.velocity + vector - Physics.gravity * Time.deltaTime;
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(forward, -Physics.gravity.normalized), base.rotateSpeed * Time.deltaTime);
			}
		}
	}
}

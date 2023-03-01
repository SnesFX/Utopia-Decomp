using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	[RequireComponent(typeof(CharacterMotor))]
	public class CharacterStateDamage : CharacterState
	{
		[SerializeField]
		private bool _endWhenGrounded = true;

		[SerializeField]
		private bool _jumpToEnd = true;

		[SerializeField]
		private float _friction = 1f;

		public bool endWhenGrounded
		{
			get
			{
				return _endWhenGrounded;
			}
			set
			{
				_endWhenGrounded = value;
			}
		}

		public bool jumpToEnd
		{
			get
			{
				return _jumpToEnd;
			}
			set
			{
				_jumpToEnd = value;
			}
		}

		public float friction
		{
			get
			{
				return Mathf.Clamp(_friction, 0f, float.MaxValue);
			}
			set
			{
				_friction = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public override string GetStateName()
		{
			return "Damage";
		}

		public override Quaternion GetControlRotation(Quaternion qView)
		{
			Vector3 toDirection = ((!base.motor) ? Vector3.up : ((!base.motor.grounded) ? base.motor.up : base.motor.ground.normal));
			return Quaternion.FromToRotation(qView * Vector3.up, toDirection) * qView;
		}

		public override float GetTopSpeed()
		{
			return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue((!base.motor.grounded) ? "Free Top Speed" : "Ground Top Speed");
		}

		public override float GetSpeed()
		{
			return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue((!base.motor.grounded) ? "Free Speed" : "Ground Speed");
		}

		public override void EngageJump()
		{
			if ((bool)base.motor && jumpToEnd)
			{
				base.motor.SetStateSelection(CharacterStateSelection.Default);
			}
		}

		public override void DisengageJump()
		{
		}

		public override void OnStateUpdate()
		{
			if ((bool)base.motor)
			{
				DefaultMotionData defaultMotionData = new DefaultMotionData();
				defaultMotionData.desiredDrive = base.motor.desiredDrive;
				defaultMotionData.desiredAngle = ((!(base.motor.velocity.magnitude > 0.1f)) ? base.motor.transform.forward : (-base.motor.velocity));
				defaultMotionData.freeTopSpeed = 0f;
				defaultMotionData.freeSpeed = 0f;
				defaultMotionData.freeAcceleration = 0f;
				defaultMotionData.freeDeceleration = 0f;
				defaultMotionData.freeBrakePower = 0f;
				defaultMotionData.freeHandling = 0f;
				defaultMotionData.freeYawSpeed = 0f;
				defaultMotionData.freeYawAcceleration = 0f;
				defaultMotionData.freePitchRollSpeed = base.character.GetAttributeValue("Free Pitch Roll Speed");
				defaultMotionData.groundTopSpeed = 0f;
				defaultMotionData.groundSpeed = 0f;
				defaultMotionData.groundAcceleration = 0f;
				defaultMotionData.groundDeceleration = 0f;
				defaultMotionData.groundBrakePower = 0f;
				defaultMotionData.groundHandling = 0f;
				defaultMotionData.groundYawSpeed = 0f;
				defaultMotionData.groundYawAcceleration = 0f;
				defaultMotionData.groundPitchRollSpeed = base.character.GetAttributeValue("Ground Pitch Roll Speed");
				defaultMotionData.friction = friction;
				defaultMotionData.strength = 0f;
				defaultMotionData.downforceIntrinsic = 0f;
				defaultMotionData.downforcePower = 0f;
				defaultMotionData.downforceMax = 0f;
				defaultMotionData.tractionIntrinsic = 0f;
				defaultMotionData.tractionPower = 0f;
				defaultMotionData.tractionMax = 0f;
				defaultMotionData.gravityPowerBoost = 1f;
				defaultMotionData.freeBrakeThreshold = 1f;
				defaultMotionData.groundBrakeThreshold = 1f;
				defaultMotionData.groundNormalAdherence = 1f;
				defaultMotionData.adhereToGroundHeight = true;
				defaultMotionData.adhereToGroundMotion = false;
				DefaultMotionData o = defaultMotionData;
				ApplyDefaultMotion(o);
				if ((bool)base.character.avatar)
				{
					base.character.avatar.SetState(GetStateName());
					base.character.avatar.drive = base.motor.drive;
					base.character.avatar.angularDrive = base.motor.angularDrive;
					base.character.avatar.grounded = base.motor.grounded;
					base.character.avatar.brake = base.motor.brake;
				}
				if (base.motor.grounded & endWhenGrounded)
				{
					base.motor.SetStateSelection(CharacterStateSelection.Default);
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CharacterStatePeelout : CharacterState
	{
		public enum Substate
		{
			Charge = 0,
			Run = 1
		}

		public enum SustainOption
		{
			None = 0,
			Sustain = 1,
			Release = 2
		}

		[SerializeField]
		private Substate _substate;

		[Range(-1f, 0f)]
		[SerializeField]
		private float _freeBrakeThreshold = -0.707f;

		[Range(-1f, 0f)]
		[SerializeField]
		private float _groundBrakeThreshold = -0.707f;

		private bool _sustained;

		[SerializeField]
		private float _gravityPowerBoost = 1f;

		private float _charge;

		public Substate substate
		{
			get
			{
				return _substate;
			}
			set
			{
				_substate = value;
			}
		}

		public float freeBrakeThreshold
		{
			get
			{
				return _freeBrakeThreshold;
			}
			set
			{
				_freeBrakeThreshold = value;
			}
		}

		public float groundBrakeThreshold
		{
			get
			{
				return _groundBrakeThreshold;
			}
			set
			{
				_groundBrakeThreshold = value;
			}
		}

		public bool sustained
		{
			get
			{
				return _sustained;
			}
			set
			{
				_sustained = value;
			}
		}

		public float gravityPowerBoost
		{
			get
			{
				return Mathf.Clamp(_gravityPowerBoost, 0f, float.MaxValue);
			}
			set
			{
				_gravityPowerBoost = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float charge
		{
			get
			{
				return Mathf.Clamp(_charge, 0f, float.MaxValue);
			}
			set
			{
				_charge = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public override string GetStateName()
		{
			switch (substate)
			{
			case Substate.Charge:
				return "Peelout";
			case Substate.Run:
				return "Default";
			default:
				return "Default";
			}
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
			if ((bool)base.motor)
			{
				base.motor.ApplyJumpStopForce(base.character.GetAttributeValue("Jump Stop Power"));
				base.motor.SetStateSelection(CharacterStateSelection.Default);
			}
		}

		public override void DisengageJump()
		{
			if ((bool)base.motor)
			{
				base.motor.ApplyJumpStopForce(base.character.GetAttributeValue("Jump Stop Power"));
			}
		}

		public override void OnStateEnter()
		{
			if ((bool)base.motor)
			{
				substate = Substate.Charge;
			}
		}

		public override void OnStateUpdate()
		{
			if (!base.motor)
			{
				return;
			}
			DefaultMotionData defaultMotionData = new DefaultMotionData();
			defaultMotionData.desiredDrive = base.motor.desiredDrive;
			defaultMotionData.desiredAngle = base.motor.desiredAngle;
			defaultMotionData.freeTopSpeed = base.character.GetAttributeValue("Free Top Speed");
			defaultMotionData.freeSpeed = base.character.GetAttributeValue("Free Speed");
			defaultMotionData.freeAcceleration = ((substate != 0) ? base.character.GetAttributeValue("Free Acceleration") : 0f);
			defaultMotionData.freeDeceleration = base.character.GetAttributeValue("Free Deceleration");
			defaultMotionData.freeBrakePower = base.character.GetAttributeValue("Free Brake");
			defaultMotionData.freeHandling = base.character.GetAttributeValue("Free Handling");
			defaultMotionData.freeYawSpeed = base.character.GetAttributeValue("Free Yaw Speed");
			defaultMotionData.freeYawAcceleration = base.character.GetAttributeValue("Free Yaw Acceleration");
			defaultMotionData.freePitchRollSpeed = base.character.GetAttributeValue("Free Pitch Roll Speed");
			defaultMotionData.groundTopSpeed = base.character.GetAttributeValue("Ground Top Speed");
			defaultMotionData.groundSpeed = base.character.GetAttributeValue("Ground Speed");
			defaultMotionData.groundAcceleration = base.character.GetAttributeValue("Ground Acceleration");
			defaultMotionData.groundDeceleration = base.character.GetAttributeValue((substate != 0) ? "Ground Deceleration" : "Ground Brake");
			defaultMotionData.groundBrakePower = base.character.GetAttributeValue("Ground Brake");
			defaultMotionData.groundHandling = base.character.GetAttributeValue("Ground Handling");
			defaultMotionData.groundYawSpeed = base.character.GetAttributeValue("Ground Yaw Speed");
			defaultMotionData.groundYawAcceleration = base.character.GetAttributeValue("Ground Yaw Acceleration");
			defaultMotionData.groundPitchRollSpeed = base.character.GetAttributeValue("Ground Pitch Roll Speed");
			defaultMotionData.friction = base.character.GetAttributeValue("Ground Friction");
			defaultMotionData.strength = base.character.GetAttributeValue("Ground Strength");
			defaultMotionData.downforceIntrinsic = base.character.GetAttributeValue("Downforce Intrinsic");
			defaultMotionData.downforcePower = base.character.GetAttributeValue("Downforce Power");
			defaultMotionData.downforceMax = base.character.GetAttributeValue("Downforce Maximum");
			defaultMotionData.tractionIntrinsic = base.character.GetAttributeValue("Traction Intrinsic");
			defaultMotionData.tractionPower = base.character.GetAttributeValue("Traction Power");
			defaultMotionData.tractionMax = base.character.GetAttributeValue("Traction Maximum");
			defaultMotionData.buoyancyIntrinsic = base.character.GetAttributeValue("Buoyancy Intrinsic");
			defaultMotionData.buoyancyPower = base.character.GetAttributeValue("Buoyancy Power");
			defaultMotionData.buoyancyMax = base.character.GetAttributeValue("Buoyancy Maximum");
			defaultMotionData.gravityPowerBoost = 1f;
			defaultMotionData.freeBrakeThreshold = freeBrakeThreshold;
			defaultMotionData.groundBrakeThreshold = groundBrakeThreshold;
			defaultMotionData.groundNormalAdherence = 0f;
			defaultMotionData.adhereToGroundHeight = true;
			defaultMotionData.adhereToGroundMotion = true;
			defaultMotionData.steeringBasedFacing = true;
			defaultMotionData.faceBrakeDirection = false;
			DefaultMotionData defaultMotionData2 = defaultMotionData;
			if (substate == Substate.Charge)
			{
				defaultMotionData2.freeTopSpeed = base.motor.velocity.magnitude - defaultMotionData2.freeBrakePower * Time.deltaTime;
				defaultMotionData2.groundTopSpeed = base.motor.velocity.magnitude - defaultMotionData2.groundBrakePower * Time.deltaTime;
				defaultMotionData2.groundSpeed = defaultMotionData2.groundTopSpeed;
			}
			ApplyDefaultMotion(defaultMotionData2);
			base.motor.brake = false;
			if ((bool)base.character.avatar)
			{
				base.character.avatar.SetState(GetStateName());
				if (substate == Substate.Charge)
				{
					base.character.avatar.drive = Vector3.forward * charge;
				}
				else
				{
					base.character.avatar.drive = base.motor.drive;
				}
				base.character.avatar.angularDrive = base.motor.angularDrive;
				base.character.avatar.grounded = base.motor.grounded;
				base.character.avatar.brake = false;
			}
		}
	}
}

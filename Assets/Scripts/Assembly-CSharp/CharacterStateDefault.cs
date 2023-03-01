using System;
using UnityEngine;

[Serializable]
public class CharacterStateDefault : CharacterState
{
	public enum Substate
	{
		Free = 0,
		Ground = 1,
		Crouch = 2,
		Prone = 3,
		Jump = 4
	}

	[SerializeField]
	private Substate _substate;

	[SerializeField]
	[Range(-1f, 0f)]
	private float _freeBrakeThreshold = -0.707f;

	[SerializeField]
	[Range(-1f, 0f)]
	private float _groundBrakeThreshold = -0.707f;

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

	public override string GetStateName()
	{
		switch (substate)
		{
		case Substate.Free:
			return "Default";
		case Substate.Ground:
			return "Default";
		case Substate.Crouch:
			return "Crouch";
		case Substate.Prone:
			return "Prone";
		case Substate.Jump:
			return "Jump";
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
		if ((bool)base.motor && base.motor.grounded)
		{
			substate = Substate.Jump;
			base.motor.ApplyJumpForce(base.motor.ground.normal, base.character.GetAttributeValue("Jump Power"), base.character.GetAttributeValue("Jump Height"));
			base.motor.TriggerJumped();
		}
	}

	public override void DisengageJump()
	{
		if (!(!base.character | !base.motor) && substate == Substate.Jump && !base.motor.grounded)
		{
			base.motor.ApplyJumpStopForce(base.character.GetAttributeValue("Jump Stop Power"));
		}
	}

	public override void OnStateUpdate()
	{
		if (!base.character | !base.motor)
		{
			return;
		}
		switch (substate)
		{
		case Substate.Free:
			if (base.motor.grounded)
			{
				substate = Substate.Ground;
			}
			break;
		case Substate.Jump:
			if (base.motor.grounded)
			{
				substate = Substate.Ground;
			}
			break;
		case Substate.Ground:
			if (!base.motor.grounded)
			{
				substate = Substate.Free;
			}
			break;
		case Substate.Crouch:
			if (!base.motor.grounded)
			{
				substate = Substate.Free;
			}
			else
			{
				substate = Substate.Ground;
			}
			break;
		case Substate.Prone:
			if (!base.motor.grounded)
			{
				substate = Substate.Free;
			}
			else
			{
				substate = Substate.Ground;
			}
			break;
		}
		DefaultMotionData defaultMotionData = new DefaultMotionData();
		defaultMotionData.desiredDrive = base.motor.desiredDrive;
		defaultMotionData.desiredAngle = base.motor.desiredAngle;
		defaultMotionData.freeTopSpeed = base.character.GetAttributeValue("Free Top Speed");
		defaultMotionData.freeSpeed = base.character.GetAttributeValue("Free Speed");
		defaultMotionData.freeAcceleration = base.character.GetAttributeValue("Free Acceleration");
		defaultMotionData.freeDeceleration = base.character.GetAttributeValue("Free Deceleration");
		defaultMotionData.freeBrakePower = base.character.GetAttributeValue("Free Brake");
		defaultMotionData.freeHandling = base.character.GetAttributeValue("Free Handling");
		defaultMotionData.freeYawSpeed = base.character.GetAttributeValue("Free Yaw Speed");
		defaultMotionData.freeYawAcceleration = base.character.GetAttributeValue("Free Yaw Acceleration");
		defaultMotionData.freePitchRollSpeed = base.character.GetAttributeValue("Free Pitch Roll Speed");
		defaultMotionData.groundTopSpeed = base.character.GetAttributeValue("Ground Top Speed");
		defaultMotionData.groundSpeed = base.character.GetAttributeValue("Ground Speed");
		defaultMotionData.groundAcceleration = base.character.GetAttributeValue("Ground Acceleration");
		defaultMotionData.groundDeceleration = base.character.GetAttributeValue("Ground Deceleration");
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
	}
}

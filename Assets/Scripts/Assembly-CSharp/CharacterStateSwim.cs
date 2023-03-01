using System;
using UnityEngine;

[Serializable]
public class CharacterStateSwim : CharacterState
{
	public override Quaternion GetControlRotation(Quaternion qView)
	{
		Vector3 toDirection = ((!base.motor) ? Vector3.up : base.motor.up);
		return Quaternion.FromToRotation(qView * Vector3.up, toDirection) * qView;
	}

	public override float GetTopSpeed()
	{
		return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue("Swim Top Speed");
	}

	public override float GetSpeed()
	{
		return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue("Swim Seed");
	}

	public override void OnStateUpdate()
	{
		if (!(!base.character | !base.motor))
		{
			SwimMotionData swimMotionData = new SwimMotionData();
		}
	}

	public void ApplySwimMotion(SwimMotionData o)
	{
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

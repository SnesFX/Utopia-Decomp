using System;
using UnityEngine;

[Serializable]
public class CharacterStateFly : CharacterState
{
	public enum Substate
	{
		Direct = 0,
		Hover = 1,
		Soar = 2
	}

	[SerializeField]
	private Substate _substate;

	[SerializeField]
	private float _gravityPowerBoost = 1f;

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

	public override Quaternion GetControlRotation(Quaternion qView)
	{
		Vector3 toDirection = ((!base.motor) ? Vector3.up : base.motor.up);
		return Quaternion.FromToRotation(qView * Vector3.up, toDirection) * qView;
	}

	public override float GetTopSpeed()
	{
		return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue("Fly Top Speed");
	}

	public override float GetSpeed()
	{
		return (!base.character || !base.motor) ? 0f : base.character.GetAttributeValue("Fly Speed");
	}

	public override void OnStateUpdate()
	{
		if (!(!base.character | !base.motor))
		{
			FlyMotionData flyMotionData = new FlyMotionData();
			flyMotionData.desiredDrive = base.motor.desiredDrive;
			flyMotionData.desiredAngle = base.motor.desiredAngle;
			flyMotionData.topSpeed = base.character.GetAttributeValue("Fly Top Speed");
			flyMotionData.speed = base.character.GetAttributeValue("Fly Speed");
			flyMotionData.acceleration = base.character.GetAttributeValue("Fly Acceleration");
			flyMotionData.deceleration = base.character.GetAttributeValue("Fly Deceleration");
			flyMotionData.handling = base.character.GetAttributeValue("Fly Handling");
			flyMotionData.pitchSpeed = base.character.GetAttributeValue("Fly Pitch Speed");
			flyMotionData.yawSpeed = base.character.GetAttributeValue("Fly Yaw Speed");
			flyMotionData.rollSpeed = base.character.GetAttributeValue("Fly Roll Speed");
			flyMotionData.strength = base.character.GetAttributeValue("Fly Strength");
			flyMotionData.gravityPowerBoost = gravityPowerBoost;
			FlyMotionData o = flyMotionData;
			ApplyFlyMotion(o);
		}
	}

	public void ApplyFlyMotion(FlyMotionData o)
	{
		if (!base.motor)
		{
			return;
		}
		Quaternion rotation = base.motor.transform.rotation;
		Vector3 gravity = base.motor.gravity;
		Vector3 vector = ((!(base.motor.density > 0f)) ? Vector3.zero : (-gravity * (base.motor.physicsVolumeData.density / base.motor.density)));
		Vector3 rhs = gravity + vector;
		Vector3 vector2 = -gravity.normalized;
		Vector3 vector3 = base.transform.rotation * o.desiredDrive;
		Vector3 vVector = Utility.RelativeVector(base.transform.rotation * o.desiredAngle, vector2);
		Vector3 vector4 = base.transform.rotation * base.motor.drive;
		Vector3 vector5 = base.transform.rotation * base.motor.angularDrive;
		Vector3 vector6 = base.motor.velocity;
		Vector3 angularVelocity = base.motor.angularVelocity;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		Vector3 zero4 = Vector3.zero;
		Vector3 zero5 = Vector3.zero;
		Vector3 zero6 = Vector3.zero;
		Vector3 axis = Vector3.zero;
		Vector3 axis2 = Vector3.zero;
		Vector3 axis3 = Vector3.zero;
		float angle = 0f;
		float angle2 = 0f;
		float angle3 = 0f;
		float drag = base.motor.drag;
		float angularDrag = base.motor.angularDrag;
		float density = base.motor.physicsVolumeData.density;
		float submersion = base.motor.physicsVolumeData.submersion;
		float num = Utility.ClampRadian(o.yawSpeed * ((float)Math.PI / 180f), vector4.magnitude, o.handling) * 57.29578f;
		float num2 = 0f;
		float num3 = Mathf.Clamp(Vector3.Dot((vector3 - vector4).normalized, rhs), 0f, float.MaxValue);
		float num4 = vector6.magnitude + Mathf.Clamp(Vector3.Dot(vector6.normalized, gravity), 0f, gravity.magnitude) * o.gravityPowerBoost * Time.fixedDeltaTime;
		vector3 = Vector3.ClampMagnitude(vector3, Mathf.Clamp(vector6.magnitude + Mathf.Clamp(Vector3.Dot(vector6.normalized, rhs), 0f, rhs.magnitude) * o.gravityPowerBoost * Time.fixedDeltaTime, o.speed, o.topSpeed));
		bool flag = vector3.magnitude > 0f;
		bool flag2 = Vector3.Dot(vector4.normalized, vector3.normalized) < o.brakeThreshold;
		bool flag3 = !flag2 & (Vector3.Dot(Utility.RelativeVector(vVector, base.transform.forward), vector6) < 0f);
		bool flag4 = Vector3.Dot(vector4, vector3.normalized) > -0.1f && vector3.magnitude >= vector4.magnitude;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = ((!flag4) ? o.deceleration : ((!base.motor.ground.isFluid) ? o.acceleration : 0f));
		vector3 = ((!(vector6.magnitude > o.speed)) ? Vector3.ClampMagnitude(vector3, o.speed) : Vector3.ClampMagnitude(Vector3.ClampMagnitude(vector3, vector6.magnitude), o.topSpeed));
		if (flag2)
		{
			vVector = Utility.RelativeVector(vector6, base.transform.up).normalized;
		}
		num2 = ((!flag3) ? o.yawSpeed : num);
		Quaternion.FromToRotation(base.transform.forward, Utility.RelativeVector(vVector, base.transform.up).normalized).ToAngleAxis(out angle2, out axis2);
		if (Vector3.Dot(axis2, base.transform.up) < 0f)
		{
			angle2 *= -1f;
			axis2 *= -1f;
		}
		float num8 = num2 * Time.fixedDeltaTime;
		num8 = Mathf.Clamp(angle2, 0f - num8, num8);
		base.transform.rotation = Quaternion.AngleAxis(num8, base.transform.up) * base.transform.rotation;
		angle2 = Mathf.Clamp(angle2, 0f - num2, num2);
		if (flag3)
		{
			Quaternion.FromToRotation(Utility.RelativeVector(vector4, base.transform.up), Utility.RelativeVector(vector3, base.transform.up)).ToAngleAxis(out angle3, out axis3);
			if (Vector3.Dot(axis3, base.transform.up) < 0f)
			{
				angle3 *= -1f;
				axis3 *= -1f;
			}
			angle3 = Mathf.Clamp(angle3, 0f - Mathf.Abs(num8), Mathf.Abs(num8));
			rotation = Quaternion.AngleAxis(angle3, base.transform.up);
			float num9 = Vector3.Dot(Utility.RelativeVector(vector4, base.transform.up).normalized, vector6);
			Vector3 vector7 = vector6.normalized * num9;
			vector4 = rotation * vector4;
			vector6 = rotation * vector7 + (vector6 - vector7);
		}
		zero = vector3 - vector4;
		zero = ((!flag4) ? (vector4.normalized * Vector3.Dot(vector4.normalized, zero)) : zero);
		num6 = ((!(rhs.magnitude > 0f)) ? 1f : Mathf.Clamp01((o.strength - rhs.magnitude) / rhs.magnitude));
		float num10 = Vector3.Dot(vector4.normalized, rhs.normalized * (rhs.magnitude - o.strength));
		num3 = Vector3.Dot(zero.normalized, rhs);
		Vector3 vector8 = -vector4.normalized * Mathf.Clamp(Vector3.Dot(-vector4.normalized, rhs.normalized * (rhs.magnitude - o.strength)) * Time.fixedDeltaTime, 0f, vector4.magnitude);
		Vector3 vector9 = vector3.normalized * Mathf.Clamp(Vector3.Dot(vector3.normalized, rhs) * Time.fixedDeltaTime * o.gravityPowerBoost, 0f, float.MaxValue);
		zero = Vector3.ClampMagnitude(zero, Mathf.Clamp(Vector3.Dot(zero.normalized, vector6 - vector4), 0f, zero.magnitude) + num7 * num6 * Time.fixedDeltaTime) + ((!(num3 <= 0f)) ? Vector3.zero : vector8) + vector9;
		zero += Vector3.ClampMagnitude(Utility.RelativeVector(-(vector4 + zero), vector3.normalized), Mathf.Clamp(o.deceleration * Time.fixedDeltaTime - zero.magnitude, 0f, o.deceleration * Time.fixedDeltaTime));
		zero2 += zero;
		zero4 += vector4 + zero4 - vector6 - Physics.gravity * Time.fixedDeltaTime;
		Quaternion.FromToRotation(base.transform.forward, Utility.RelativeVector(vVector, base.transform.up)).ToAngleAxis(out angle, out axis);
		angle = Mathf.Clamp(angle, 0f, Vector3.Angle(vector4.normalized, (vector4 + zero2).normalized));
		angle = Mathf.Clamp(angle, 0f, o.yawSpeed * Time.fixedDeltaTime);
		if (Vector3.Dot(axis, base.transform.up) < 0f)
		{
			angle *= -1f;
			axis *= -1f;
		}
		base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.up) * base.transform.rotation;
		base.motor.drive = Quaternion.Inverse(base.transform.rotation) * (vector4 + zero2);
		base.motor.angularDrive = Quaternion.Inverse(base.transform.rotation) * (base.transform.up * angle2 * ((float)Math.PI / 180f));
		base.motor.velocity = vector6 + zero4;
		base.motor.angularVelocity = Vector3.zero;
		base.motor.brake = flag2;
		Quaternion.FromToRotation(base.transform.up, vector2).ToAngleAxis(out angle, out axis);
		angle = Mathf.Clamp(angle, (0f - o.pitchSpeed) * Time.fixedDeltaTime, o.pitchSpeed * Time.fixedDeltaTime);
		base.transform.RotateAround(base.motor.worldBottomPoint, axis, angle);
		if (base.motor.mass > 0f && base.motor.physicsVolumeData.volumes.Count > 0)
		{
			base.motor.drive += Vector3.ClampMagnitude(-base.motor.drive, density * base.motor.drive.magnitude * 0.5f * drag * Time.fixedDeltaTime / base.motor.mass);
			base.motor.velocity += Vector3.ClampMagnitude(-base.motor.velocity, density * base.motor.velocity.magnitude * 0.5f * drag * Time.fixedDeltaTime / base.motor.mass);
		}
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

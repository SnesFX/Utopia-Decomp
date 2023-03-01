using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(CharacterMotor))]
public abstract class CharacterState : MonoBehaviour
{
	private Character _character;

	private CharacterMotor _motor;

	public Character character
	{
		get
		{
			return (!_character) ? (_character = GetComponent<Character>()) : _character;
		}
	}

	public CharacterMotor motor
	{
		get
		{
			return (!_motor) ? (_motor = GetComponent<CharacterMotor>()) : _motor;
		}
	}

	public virtual string GetStateName()
	{
		return string.Empty;
	}

	public virtual Quaternion GetControlRotation(Quaternion qView)
	{
		Vector3 toDirection = ((!motor) ? Vector3.up : ((!motor.grounded) ? motor.up : motor.ground.normal));
		return Quaternion.FromToRotation(qView * Vector3.up, toDirection) * qView;
	}

	public virtual float GetSpeed()
	{
		return 0f;
	}

	public virtual float GetTopSpeed()
	{
		return 0f;
	}

	public void ApplyDefaultMotion(DefaultMotionData o)
	{
		if (!motor)
		{
			return;
		}
		Quaternion rotation = motor.transform.rotation;
		Vector3 gravity = motor.gravity;
		Vector3 vector = ((!(motor.density > 0f)) ? Vector3.zero : (-gravity * (motor.physicsVolumeData.density / motor.density)));
		Vector3 vector2 = gravity + vector;
		Vector3 vector3 = -gravity.normalized;
		Vector3 vector4 = base.transform.rotation * o.desiredDrive;
		Vector3 vector5 = base.transform.rotation * o.desiredAngle;
		Vector3 point = motor.ground.point;
		Vector3 normal = motor.ground.normal;
		Vector3 surfacePoint = motor.physicsVolumeData.surfacePoint;
		Vector3 surfaceNormal = motor.physicsVolumeData.surfaceNormal;
		Collider collider = null;
		RaycastHit hitInfo;
		if (Physics.Linecast(motor.worldTopPoint, motor.worldBottomPoint - base.transform.up * motor.stepHeight, out hitInfo, motor.fluidLayers, QueryTriggerInteraction.Collide))
		{
			surfacePoint = hitInfo.point;
			surfaceNormal = hitInfo.normal;
			collider = hitInfo.collider;
		}
		float num = Vector3.Dot(Utility.RelativeVector(motor.velocity, normal), rotation * motor.drive.normalized);
		num = Mathf.Clamp(o.downforceIntrinsic + o.downforcePower * num, 0f, o.downforceMax);
		float num2 = Vector3.Dot(Utility.RelativeVector(motor.velocity, motor.physicsVolumeData.surfaceNormal), rotation * motor.drive.normalized);
		num2 = Mathf.Clamp(o.buoyancyIntrinsic + o.buoyancyPower * num2, 0f, o.buoyancyMax) * (motor.physicsVolumeData.density * 0.001f);
		float num3 = Vector3.Dot(vector2, -normal);
		float min = Mathf.Clamp(num3, 0f, vector2.magnitude);
		float num4 = Mathf.Clamp(num + num3, min, num);
		float drag = motor.drag;
		float angularDrag = motor.angularDrag;
		float density = motor.physicsVolumeData.density;
		float submersion = motor.physicsVolumeData.submersion;
		if (motor.grounded && num4 > Vector3.Dot(vector2, normal))
		{
			if (o.adhereToGroundMotion)
			{
				motor.drive = Utility.RelativeVector(motor.drive.normalized, Quaternion.Inverse(base.transform.rotation) * normal) * motor.drive.magnitude;
				motor.velocity = Utility.RelativeVector(motor.velocity.normalized, normal) * motor.velocity.magnitude;
			}
			Vector3 velocityAtPoint = motor.ground.GetVelocityAtPoint(motor.ground.point);
			Vector3 vector6 = Utility.RelativeVector(velocityAtPoint, normal);
			Vector3 vector7 = velocityAtPoint - vector6;
			Vector3 angularVelocity = motor.ground.angularVelocity;
			Vector3 vector8 = Utility.RelativeVector(angularVelocity, normal);
			Vector3 vector9 = motor.ground.angularVelocity - vector8;
			Vector3 vector10 = motor.velocity - velocityAtPoint;
			Vector3 vector11 = motor.angularVelocity - angularVelocity;
			Vector3 vector12 = Utility.RelativeVector(vector2, normal);
			Vector3 vector13 = vector2 - vector12;
			Vector3 vector14 = Utility.RelativeVector(vector10, normal);
			Vector3 vector15 = vector10 - vector14;
			Vector3 vector16 = Utility.VectorInDirection(vector11, base.transform.up);
			Vector3 vector17 = vector11 - vector16;
			Vector3 vector18 = Utility.RelativeVector(base.transform.rotation * motor.drive, normal);
			Vector3 vector19 = base.transform.rotation * motor.drive - vector18;
			Vector3 vector20 = Utility.RelativeVector(vector4, normal);
			Vector3 vector21 = vector4 - vector20;
			Vector3 vector22 = Utility.RelativeVector(vector5, base.transform.up).normalized;
			if (vector22.magnitude == 0f)
			{
				vector22 = base.transform.forward;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			Vector3 zero5 = Vector3.zero;
			Vector3 zero6 = Vector3.zero;
			Vector3 zero7 = Vector3.zero;
			Vector3 zero8 = Vector3.zero;
			Vector3 zero9 = Vector3.zero;
			Vector3 zero10 = Vector3.zero;
			Vector3 axis = Vector3.zero;
			Vector3 axis2 = Vector3.zero;
			Vector3 axis3 = Vector3.zero;
			Vector3 zero11 = Vector3.zero;
			Vector3 zero12 = Vector3.zero;
			float angle = 0f;
			float angle2 = 0f;
			float angle3 = 0f;
			float num5 = ((!(gravity.magnitude > 0f)) ? 1f : (1f - Mathf.Clamp01(Vector3.Dot(vector, -gravity) / gravity.sqrMagnitude)));
			float num6 = Mathf.Clamp(o.tractionIntrinsic + o.tractionPower * num4, min, o.tractionMax);
			float num7 = motor.ground.friction * 1.6666666f * o.friction;
			float num8 = num7 * num6;
			float num9 = ((!(num8 > 0f)) ? 0f : Mathf.Clamp01((num8 - vector12.magnitude) / num8));
			float num10 = Utility.ClampRadian(o.groundYawSpeed * ((float)Math.PI / 180f), vector18.magnitude, o.groundHandling * num5) * 57.29578f;
			float num11 = 0f;
			float num12 = Mathf.Clamp(Vector3.Dot((vector20 - vector18).normalized, vector12), 0f, float.MaxValue);
			float num13 = vector14.magnitude + Mathf.Clamp(Vector3.Dot(vector14.normalized, gravity), 0f, gravity.magnitude) * o.gravityPowerBoost * Time.fixedDeltaTime;
			vector20 = Vector3.ClampMagnitude(vector20, Mathf.Clamp(vector14.magnitude + Mathf.Clamp(Vector3.Dot(vector14.normalized, vector2), 0f, vector2.magnitude) * o.gravityPowerBoost * Time.fixedDeltaTime, o.groundSpeed, o.groundTopSpeed));
			bool flag = vector20.magnitude > 0f;
			bool flag2 = Vector3.Dot(vector18.normalized, vector20.normalized) < o.groundBrakeThreshold;
			bool flag3 = !flag2 & (Vector3.Dot(Utility.RelativeVector(vector22, base.transform.forward), vector14) < 0f);
			bool flag4 = Vector3.Dot(vector18, vector20.normalized) > -0.1f && vector20.magnitude >= vector18.magnitude;
			float num14 = 0f;
			float num15 = 0f;
			float num16 = ((!flag4) ? o.groundDeceleration : ((!motor.ground.isFluid) ? o.groundAcceleration : 0f)) * num5;
			float num17 = Vector3.Dot(point - motor.worldBottomPoint, normal);
			zero12 = num17 * normal / Time.fixedDeltaTime - vector15;
			motor.MovePosition(base.transform.position + base.transform.up * Vector3.Dot(point - motor.worldBottomPoint, normal));
			rotation = Quaternion.Euler(angularVelocity * 57.29578f * Time.fixedDeltaTime);
			if (vector22 == Vector3.zero)
			{
				vector22 = rotation * base.transform.forward;
				vector5 = rotation * vector5;
				base.transform.rotation = rotation * base.transform.rotation;
			}
			if (flag2 && o.faceBrakeDirection)
			{
				vector22 = Utility.RelativeVector(vector14, base.transform.up).normalized;
			}
			num11 = ((!flag3 || !o.steeringBasedFacing) ? o.groundYawSpeed : num10);
			Quaternion.FromToRotation(base.transform.forward, vector22).ToAngleAxis(out angle2, out axis2);
			if (Vector3.Dot(vector22, base.transform.right) < 0f)
			{
				angle2 *= -1f;
				axis2 *= -1f;
			}
			float num18 = num11 * Time.fixedDeltaTime;
			num18 = Mathf.Clamp(angle2, 0f - num18, num18);
			base.transform.rotation = Quaternion.AngleAxis(num18, base.transform.up) * base.transform.rotation;
			angle2 = Mathf.Clamp(angle2, 0f - num11, num11);
			if (flag3)
			{
				Quaternion.FromToRotation(vector18, vector20).ToAngleAxis(out angle3, out axis3);
				if (Vector3.Dot(axis3, base.transform.up) < 0f)
				{
					angle3 *= -1f;
					axis3 *= -1f;
				}
				num18 = Mathf.Clamp(Mathf.Abs(num18), 0f, num10 * Time.deltaTime);
				angle3 = Mathf.Clamp(angle3, 0f - num18, num18);
				rotation = Quaternion.AngleAxis(angle3, base.transform.up);
				float num19 = Vector3.Dot(vector18.normalized, vector14) * Mathf.Clamp01(num7);
				Vector3 vector23 = vector14.normalized * num19;
				vector18 = rotation * vector18;
				vector14 = rotation * vector23 + (vector14 - vector23);
			}
			if (!flag2)
			{
				zero11 = vector20 - vector18;
				zero11 = ((!flag4) ? (vector18.normalized * Vector3.Dot(vector18.normalized, zero11)) : zero11);
				num15 = ((!(vector12.magnitude > 0f)) ? 1f : Mathf.Clamp01((o.strength - vector12.magnitude) / vector12.magnitude));
				float num20 = Vector3.Dot(vector18.normalized, vector12.normalized * (vector12.magnitude - o.strength));
				num12 = Vector3.Dot(zero11.normalized, vector12);
				Vector3 vector24 = -vector18.normalized * Mathf.Clamp(Vector3.Dot(-vector18.normalized, vector12.normalized * (vector12.magnitude - o.strength)) * Time.fixedDeltaTime, 0f, vector18.magnitude);
				Vector3 vector25 = vector20.normalized * Mathf.Clamp(Vector3.Dot(vector20.normalized, vector12) * Time.fixedDeltaTime * o.gravityPowerBoost, 0f, float.MaxValue);
				zero11 = Vector3.ClampMagnitude(zero11, Mathf.Clamp(Vector3.Dot(zero11.normalized, vector14 - vector18), 0f, zero11.magnitude) + num16 * num15 * Time.fixedDeltaTime) + ((!(num12 <= 0f)) ? Vector3.zero : vector24) + vector25;
				zero11 += Vector3.ClampMagnitude(Utility.RelativeVector(-(vector18 + zero11), vector20.normalized), Mathf.Clamp(o.groundDeceleration * Time.fixedDeltaTime - zero11.magnitude, 0f, o.groundDeceleration * Time.fixedDeltaTime));
				zero += zero11;
				zero2 += Vector3.ClampMagnitude(vector18 + zero - vector14 - vector12 * Time.fixedDeltaTime, num8 * Time.fixedDeltaTime);
				zero2 += (vector12 - Physics.gravity) * Time.fixedDeltaTime;
				Quaternion.FromToRotation(base.transform.forward, vector22).ToAngleAxis(out angle, out axis);
				angle = Mathf.Clamp(angle, 0f, Vector3.Angle(vector18.normalized, (vector18 + zero).normalized));
				angle = Mathf.Clamp(angle, 0f, o.groundYawSpeed * Time.fixedDeltaTime);
				if (Vector3.Dot(axis, base.transform.up) < 0f)
				{
					angle *= -1f;
					axis *= -1f;
				}
				base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.up) * base.transform.rotation;
			}
			else
			{
				zero2 += Vector3.ClampMagnitude(-vector14, num8 * Time.fixedDeltaTime);
				zero2 += (vector12 - Physics.gravity) * Time.fixedDeltaTime;
				zero += zero2;
				vector18 = vector14;
				vector19 = vector15;
			}
			vector15 = Vector3.zero;
			vector19 = Vector3.zero;
			vector17 = Vector3.zero;
			motor.drive = Quaternion.Inverse(base.transform.rotation) * (vector18 + vector19 + zero);
			motor.angularDrive = Quaternion.Inverse(base.transform.rotation) * (base.transform.up * angle2 * ((float)Math.PI / 180f));
			motor.velocity = vector14 + vector15 + zero2 + velocityAtPoint;
			motor.angularVelocity = vector17 + vector16 + zero3 + angularVelocity;
			motor.brake = flag2;
			zero6 = ((!(vector2.magnitude > 0f)) ? normal : Vector3.Slerp(vector3, normal, num / vector2.magnitude + o.groundNormalAdherence));
			zero7 = motor.worldBottomPoint;
			Quaternion.FromToRotation(base.transform.up, zero6).ToAngleAxis(out angle, out axis);
			angle = Mathf.Clamp(angle, (0f - o.groundPitchRollSpeed) * Time.fixedDeltaTime, o.groundPitchRollSpeed * Time.fixedDeltaTime);
			base.transform.RotateAround(zero7, axis, angle);
			if (motor.mass > 0f && motor.physicsVolumeData.volumes.Count > 0)
			{
				motor.drive += Vector3.ClampMagnitude(-motor.drive, density * motor.drive.magnitude * 0.5f * drag * Time.fixedDeltaTime / motor.mass);
				motor.velocity += Vector3.ClampMagnitude(-motor.velocity, density * motor.velocity.magnitude * 0.5f * drag * Time.fixedDeltaTime / motor.mass);
			}
			return;
		}
		Vector3 vector26 = Utility.RelativeVector(vector2, vector3);
		Vector3 vector27 = vector2 - vector26;
		Vector3 vector28 = Utility.RelativeVector(motor.velocity, vector3);
		Vector3 vector29 = motor.velocity - vector28;
		Vector3 vector30 = Utility.RelativeVector(vector4, motor.up);
		Vector3 vector31 = vector4 - vector30;
		Vector3 normalized = Utility.RelativeVector(vector30, motor.up).normalized;
		Vector3 vector32 = Vector3.zero;
		Vector3 zero13 = Vector3.zero;
		Vector3 zero14 = Vector3.zero;
		Vector3 axis4 = Vector3.zero;
		Vector3 axis5 = Vector3.zero;
		Vector3 axis6 = Vector3.zero;
		float num21 = ((!(gravity.magnitude > 0f)) ? 1f : (1f - Mathf.Clamp01(Vector3.Dot(vector, -gravity) / gravity.sqrMagnitude)));
		float angle4 = 0f;
		float angle5 = 0f;
		float angle6 = 0f;
		float num22 = Utility.ClampRadian(o.freeYawSpeed * ((float)Math.PI / 180f), vector28.magnitude, o.freeHandling * num21) * 57.29578f;
		float freeYawSpeed = o.freeYawSpeed;
		float num23 = 0f;
		vector30 = ((!(vector28.magnitude > o.freeSpeed)) ? Vector3.ClampMagnitude(vector30, o.freeSpeed) : Vector3.ClampMagnitude(Vector3.ClampMagnitude(vector30, vector28.magnitude), o.freeTopSpeed));
		bool flag5 = vector30.magnitude > 0f;
		bool flag6 = Vector3.Dot(vector28.normalized, vector30.normalized) < o.freeBrakeThreshold;
		bool flag7 = !flag6 & (Vector3.Dot(Utility.RelativeVector(normalized, base.transform.forward), vector28) < 0f);
		if (flag6 && o.faceBrakeDirection)
		{
			normalized = Utility.RelativeVector(vector28, base.transform.up).normalized;
		}
		num23 = ((!flag7 || !o.steeringBasedFacing) ? freeYawSpeed : num22);
		Quaternion.FromToRotation(Utility.RelativeVector(base.transform.forward, vector3).normalized, normalized).ToAngleAxis(out angle5, out axis5);
		if (Vector3.Dot(normalized, base.transform.right) < 0f)
		{
			angle5 *= -1f;
			axis5 *= -1f;
		}
		float num24 = num23 * Time.fixedDeltaTime;
		num24 = Mathf.Clamp(angle5, 0f - num24, num24);
		base.transform.rotation = Quaternion.AngleAxis(num24, base.transform.up) * base.transform.rotation;
		angle5 = Mathf.Clamp(angle5, 0f - num23, num23);
		if (flag7)
		{
			Quaternion.FromToRotation(vector28, vector30).ToAngleAxis(out angle6, out axis6);
			if (Vector3.Dot(axis6, base.transform.up) < 0f)
			{
				angle6 *= -1f;
				axis6 *= -1f;
			}
			num24 = Mathf.Clamp(Mathf.Abs(num24), 0f, num22 * Time.deltaTime);
			num24 = Mathf.Clamp(angle6, 0f - num24, num24);
			rotation = Quaternion.AngleAxis(num24, base.transform.up);
			vector28 = rotation * vector28;
			float num25 = Vector3.Dot(base.transform.rotation * motor.drive.normalized, vector28);
			Vector3 vector33 = vector28.normalized * num25;
			vector28 = vector28 - vector33 + rotation * vector33;
		}
		if (!flag6)
		{
			Vector3 vector34 = vector30 - vector28;
			if (Vector3.Dot(vector28, vector30.normalized) > -0.1f && vector30.magnitude >= vector28.magnitude)
			{
				vector34 = Vector3.ClampMagnitude(vector30 - vector28, o.freeAcceleration * num21 * Time.fixedDeltaTime);
				vector34 += Vector3.ClampMagnitude(Utility.RelativeVector(-(vector28 + vector34), vector30.normalized), Mathf.Clamp(o.freeDeceleration * num21 * Time.fixedDeltaTime - vector34.magnitude, 0f, o.freeDeceleration * num21 * Time.fixedDeltaTime));
				vector32 = vector34;
			}
			else
			{
				vector34 = vector28.normalized * Vector3.Dot(vector28.normalized, vector30 - vector28);
				vector32 = Vector3.ClampMagnitude(vector34, o.freeDeceleration * num21 * Time.fixedDeltaTime);
			}
		}
		else
		{
			vector32 += Vector3.ClampMagnitude(-(vector28 + vector26 * Time.fixedDeltaTime), o.freeBrakePower * Time.fixedDeltaTime);
		}
		vector32 += (vector2 - Physics.gravity) * Time.fixedDeltaTime;
		motor.velocity = vector28 + vector29 + vector32;
		motor.drive = Quaternion.Inverse(motor.transform.rotation) * motor.velocity;
		motor.angularDrive = Quaternion.Inverse(motor.transform.rotation) * (base.transform.up * angle5 * ((float)Math.PI / 180f));
		motor.brake = flag6;
		Quaternion.FromToRotation(motor.transform.up, vector3).ToAngleAxis(out angle4, out axis4);
		angle4 = Mathf.Clamp(angle4, (0f - o.freePitchRollSpeed) * Time.fixedDeltaTime, o.freePitchRollSpeed * Time.fixedDeltaTime);
		motor.transform.RotateAround(motor.worldCenter, axis4, angle4);
		if (motor.mass > 0f && motor.physicsVolumeData.volumes.Count > 0)
		{
			motor.drive += Vector3.ClampMagnitude(-motor.drive, density * motor.drive.magnitude * 0.5f * drag * submersion * Time.fixedDeltaTime / motor.mass);
			motor.velocity += Vector3.ClampMagnitude(-motor.velocity, density * motor.velocity.magnitude * 0.5f * drag * submersion * Time.fixedDeltaTime / motor.mass);
		}
	}

	public virtual void EngageJump()
	{
	}

	public virtual void DisengageJump()
	{
	}

	public virtual void EngageDrop()
	{
	}

	public virtual void DisengageDrop()
	{
	}

	public virtual void OnStateEnter()
	{
	}

	public virtual void OnStateExit()
	{
	}

	public virtual void OnStateUpdate()
	{
	}
}

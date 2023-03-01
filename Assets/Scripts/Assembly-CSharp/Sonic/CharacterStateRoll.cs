using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CharacterStateRoll : CharacterState
	{
		public enum Substate
		{
			Crouch = 0,
			Charge = 1,
			Roll = 2,
			Free = 3,
			Jump = 4,
			HomingAttack = 5
		}

		[SerializeField]
		private Substate _substate = Substate.Roll;

		[SerializeField]
		private TagContainer _blockingTags = new TagContainer();

		[Range(-1f, 0f)]
		[SerializeField]
		private float _freeBrakeThreshold = -0.707f;

		[SerializeField]
		[Range(-1f, 0f)]
		private float _groundBrakeThreshold = -0.707f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _homingDotThreshold = 0.5f;

		[SerializeField]
		private float _rollThreshold = 10f;

		private bool _sustained;

		[SerializeField]
		private float _gravityPowerBoost = 1f;

		[SerializeField]
		private float _impactDetectionBuffer = 1f;

		[SerializeField]
		private float _minimumBounceSpeed = 15f;

		private float _charge;

		[SerializeField]
		private float _homingBoostSpeed = 10f;

		[SerializeField]
		private float _homingAttackDuration = 0.5f;

		private float _timer;

		[SerializeField]
		private ImpactSetupData _impact = new ImpactSetupData();

		[SerializeField]
		private ImpactEffectSetupData _invulnerability = new ImpactEffectSetupData();

		[SerializeField]
		private SpecialEffectHandler _homingEffect = new SpecialEffectHandler();

		[SerializeField]
		private SpecialEffectHandler _jumpDashEffect = new SpecialEffectHandler();

		private HomingAttackTarget _homingTarget;

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

		public TagContainer blockingTags
		{
			get
			{
				return _blockingTags;
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

		public float homingDotThreshold
		{
			get
			{
				return _homingDotThreshold;
			}
			set
			{
				_homingDotThreshold = value;
			}
		}

		public float rollingThreshold
		{
			get
			{
				return Mathf.Clamp(_rollThreshold, 0f, float.MaxValue);
			}
		}

		public HomingAttackTarget homingTarget
		{
			get
			{
				return _homingTarget;
			}
			protected set
			{
				_homingTarget = value;
			}
		}

		public ImpactSetupData impact
		{
			get
			{
				return _impact;
			}
		}

		public ImpactEffectSetupData invulnerability
		{
			get
			{
				return _invulnerability;
			}
		}

		public SpecialEffectHandler homingEffect
		{
			get
			{
				return _homingEffect;
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

		public float impactDetectionBuffer
		{
			get
			{
				return Mathf.Clamp(_impactDetectionBuffer, 0f, float.MaxValue);
			}
			set
			{
				_impactDetectionBuffer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float minimumBounceSpeed
		{
			get
			{
				return Mathf.Clamp(_minimumBounceSpeed, 0f, float.MaxValue);
			}
			set
			{
				_minimumBounceSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float charge
		{
			get
			{
				return (substate != Substate.Charge) ? 0f : Mathf.Clamp(_charge, 0f, float.MaxValue);
			}
			set
			{
				_charge = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float homingBoostSpeed
		{
			get
			{
				return Mathf.Clamp(_homingBoostSpeed, 0f, float.MaxValue);
			}
			set
			{
				_homingBoostSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float homingAttackDuration
		{
			get
			{
				return Mathf.Clamp(_homingAttackDuration, 0f, float.MaxValue);
			}
			set
			{
				_homingAttackDuration = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float timer
		{
			get
			{
				return Mathf.Clamp(_timer, 0f, float.MaxValue);
			}
			set
			{
				_timer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public override string GetStateName()
		{
			return "Roll";
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
			if ((bool)base.motor && base.motor.grounded && ((substate == Substate.Roll) | (substate == Substate.Crouch)))
			{
				base.motor.ApplyJumpStopForce(base.character.GetAttributeValue("Jump Stop Power"));
			}
		}

		public override void DisengageJump()
		{
			if ((bool)base.motor && !base.motor.grounded && ((substate == Substate.Free) | (substate == Substate.Jump)))
			{
				base.motor.ApplyJumpStopForce(base.character.GetAttributeValue("Jump Stop Power"));
			}
		}

		public void HomingAttack(HomingAttackTarget oTarget)
		{
			if ((bool)base.motor && !base.motor.grounded && ((substate == Substate.Jump) | (substate == Substate.Free)))
			{
				substate = Substate.HomingAttack;
				homingTarget = oTarget;
				if ((bool)homingTarget)
				{
					base.motor.velocity = (homingTarget.worldOffset - base.motor.worldCenter).normalized * base.motor.velocity.magnitude;
					homingEffect.Spawn();
					timer = homingAttackDuration;
				}
				else
				{
					Vector3 normalized = Utility.RelativeVector(base.motor.transform.forward, base.motor.up).normalized;
					normalized *= Mathf.Clamp(Vector3.Dot(base.motor.velocity, normalized), homingBoostSpeed, base.motor.velocity.magnitude);
					base.motor.velocity = normalized + base.motor.up * Mathf.Clamp(Vector3.Dot(base.motor.velocity, base.motor.up), 0f, base.motor.velocity.magnitude);
					_jumpDashEffect.Spawn();
				}
			}
		}

		public void ApplyImpact(Actor oActor)
		{
			if ((bool)base.motor && !(base.motor.state != this) && (bool)oActor && oActor.alive && (bool)oActor.impactable)
			{
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = oActor;
				impactApplicationData.instigator = base.character;
				impactApplicationData.source = base.character;
				ImpactApplicationData oData = impactApplicationData;
				oActor.Impact(oData);
				if (!base.motor.grounded)
				{
					float num = Vector3.Dot(base.motor.velocity, base.motor.up);
					Vector3 velocity = base.motor.up * Mathf.Clamp(0f - num, minimumBounceSpeed, float.MaxValue);
					velocity += Utility.RelativeVector(base.motor.velocity, base.motor.up);
					base.motor.velocity = velocity;
					substate = Substate.Jump;
				}
			}
		}

		protected void HandleImpactIncoming(object sender, ActorEventArgs e)
		{
			if ((bool)base.motor && !((base.motor.state != this) | (substate == Substate.Crouch)) && e.context is ContextImpactIncoming)
			{
				ActorImpactEventArgs actorImpactEventArgs = e as ActorImpactEventArgs;
				ImpactApplicationData impactData = actorImpactEventArgs.impactData;
				if (blockingTags.AnyTagsMatch(impactData.blockedByTags))
				{
					impactData.prevented = true;
				}
			}
		}

		public override void OnStateEnter()
		{
			if (!base.motor)
			{
				return;
			}
			if (base.motor.grounded)
			{
				if (base.motor.drive.magnitude == 0f)
				{
					substate = Substate.Crouch;
				}
				else
				{
					substate = Substate.Roll;
				}
			}
			else if ((substate != Substate.Jump) & (substate != Substate.HomingAttack))
			{
				substate = Substate.Free;
			}
			homingTarget = null;
		}

		public override void OnStateExit()
		{
			if ((bool)base.motor)
			{
				sustained = false;
				substate = Substate.Free;
				base.character.statistics.RemoveImpactEffect(invulnerability.behaviour);
				homingTarget = null;
			}
		}

		public override void OnStateUpdate()
		{
			if (!base.motor)
			{
				return;
			}
			if (substate == Substate.HomingAttack && (bool)homingTarget)
			{
				timer -= Time.deltaTime;
				if (timer == 0f)
				{
					homingTarget = null;
				}
			}
			if (substate == Substate.HomingAttack && (bool)homingTarget)
			{
				Vector3 lhs = homingTarget.worldOffset - base.motor.worldCenter;
				if (Vector3.Dot(lhs, base.motor.velocity) > homingDotThreshold && (homingTarget.worldOffset - base.motor.worldCenter).magnitude > homingTarget.radius)
				{
					lhs = lhs.normalized * Mathf.Clamp(base.motor.velocity.magnitude, base.motor.character.GetAttributeValue("Free Speed"), base.motor.velocity.magnitude);
					lhs -= Physics.gravity * Time.deltaTime;
					Vector3 normalized = Utility.RelativeVector(lhs, base.motor.up).normalized;
					Quaternion rotation = Quaternion.LookRotation((!(normalized != Vector3.zero)) ? base.motor.transform.forward : normalized, base.motor.up);
					base.motor.transform.rotation = rotation;
					base.motor.velocity = lhs;
					base.motor.drive = Quaternion.Inverse(base.transform.rotation) * lhs;
					base.motor.angularDrive = Vector3.zero;
					base.motor.angularVelocity = Vector3.zero;
				}
				else
				{
					substate = Substate.Free;
				}
			}
			if ((substate != Substate.HomingAttack) | !homingTarget)
			{
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
				defaultMotionData.groundSpeed = 0f;
				defaultMotionData.groundAcceleration = 0f;
				defaultMotionData.groundDeceleration = base.character.GetAttributeValue("Ground Deceleration");
				defaultMotionData.groundBrakePower = base.character.GetAttributeValue("Ground Brake");
				defaultMotionData.groundHandling = base.character.GetAttributeValue("Ground Handling");
				defaultMotionData.groundYawSpeed = base.character.GetAttributeValue("Ground Yaw Speed");
				defaultMotionData.groundYawAcceleration = base.character.GetAttributeValue("Ground Yaw Acceleration");
				defaultMotionData.groundPitchRollSpeed = base.character.GetAttributeValue("Ground Pitch Roll Speed");
				defaultMotionData.friction = base.character.GetAttributeValue("Ground Friction");
				defaultMotionData.strength = 0f;
				defaultMotionData.downforceIntrinsic = base.character.GetAttributeValue("Downforce Intrinsic");
				defaultMotionData.downforcePower = base.character.GetAttributeValue("Downforce Power");
				defaultMotionData.downforceMax = base.character.GetAttributeValue("Downforce Maximum");
				defaultMotionData.tractionIntrinsic = base.character.GetAttributeValue("Traction Intrinsic");
				defaultMotionData.tractionPower = base.character.GetAttributeValue("Traction Power");
				defaultMotionData.tractionMax = base.character.GetAttributeValue("Traction Maximum");
				defaultMotionData.buoyancyIntrinsic = base.character.GetAttributeValue("Buoyancy Intrinsic");
				defaultMotionData.buoyancyPower = base.character.GetAttributeValue("Buoyancy Power");
				defaultMotionData.buoyancyMax = base.character.GetAttributeValue("Buoyancy Maximum");
				defaultMotionData.gravityPowerBoost = gravityPowerBoost;
				defaultMotionData.freeBrakeThreshold = freeBrakeThreshold;
				defaultMotionData.groundBrakeThreshold = groundBrakeThreshold;
				defaultMotionData.groundNormalAdherence = 0f;
				defaultMotionData.adhereToGroundHeight = true;
				defaultMotionData.adhereToGroundMotion = true;
				DefaultMotionData defaultMotionData2 = defaultMotionData;
				if (base.motor.grounded)
				{
					switch (substate)
					{
					case Substate.Crouch:
						if (!sustained)
						{
							base.motor.SetStateSelection(CharacterStateSelection.Default);
							return;
						}
						if (base.motor.drive.magnitude > 0.05f)
						{
							substate = Substate.Roll;
						}
						break;
					case Substate.Charge:
						if (base.motor.drive.magnitude > 0.05f)
						{
							substate = Substate.Roll;
						}
						break;
					case Substate.Roll:
						if (base.motor.drive.magnitude < 0.05f)
						{
							substate = Substate.Crouch;
						}
						break;
					default:
						if (!sustained)
						{
							base.motor.SetStateSelection(CharacterStateSelection.Default);
							return;
						}
						if (base.motor.drive.magnitude > 0.05f)
						{
							substate = Substate.Roll;
						}
						else
						{
							substate = Substate.Charge;
						}
						break;
					}
					switch (substate)
					{
					case Substate.Charge:
						defaultMotionData2.desiredDrive = Vector3.zero;
						break;
					case Substate.Crouch:
						defaultMotionData2.desiredDrive = defaultMotionData2.desiredDrive.normalized * Mathf.Clamp(Vector3.Dot(defaultMotionData2.desiredDrive, Quaternion.Inverse(base.transform.rotation) * base.motor.velocity), 0f, defaultMotionData2.desiredDrive.magnitude);
						break;
					}
				}
				else if ((substate != Substate.Free) & (substate != Substate.Jump) & (substate != Substate.HomingAttack))
				{
					if (!sustained)
					{
						base.motor.SetStateSelection(CharacterStateSelection.Default);
						return;
					}
					substate = Substate.Free;
				}
				if (base.motor.grounded && base.motor.velocity.magnitude < rollingThreshold)
				{
					defaultMotionData2.groundTopSpeed = base.motor.velocity.magnitude - defaultMotionData2.groundBrakePower * Time.deltaTime;
					defaultMotionData2.groundBrakeThreshold = 1f;
					defaultMotionData2.desiredDrive = Vector3.zero;
					if (substate != Substate.Charge)
					{
						substate = Substate.Crouch;
					}
				}
				ApplyDefaultMotion(defaultMotionData2);
			}
			if ((bool)base.character.avatar)
			{
				base.character.avatar.SetState(GetStateName());
				if (substate == Substate.Charge)
				{
					base.character.avatar.drive = Vector3.forward * charge;
				}
				else if (substate == Substate.Crouch)
				{
					base.character.avatar.drive = Vector3.zero;
				}
				else
				{
					base.character.avatar.drive = base.motor.drive;
				}
				base.character.avatar.angularDrive = base.motor.angularDrive;
				base.character.avatar.grounded = base.motor.grounded;
				base.character.avatar.brake = base.motor.brake;
			}
			RaycastHit hitInfo;
			if ((bool)base.motor && base.motor.drive.magnitude > 0.1f && substate != 0 && base.motor.rigidbody.SweepTest(base.motor.velocity, out hitInfo, base.motor.velocity.magnitude * Time.fixedDeltaTime * impactDetectionBuffer))
			{
				Actor componentInParent = hitInfo.collider.GetComponentInParent<Actor>();
				ApplyImpact(componentInParent);
			}
		}

		private void OnCollisionEnter(Collision oCollision)
		{
			if ((bool)base.motor && !((base.motor.state != this) | (substate == Substate.Crouch)))
			{
				Actor componentInParent = oCollision.collider.GetComponentInParent<Actor>();
				ApplyImpact(componentInParent);
				if (substate == Substate.HomingAttack)
				{
					substate = Substate.Jump;
				}
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if ((bool)base.motor && !((base.motor.state != this) | (substate == Substate.Crouch)))
			{
				Actor componentInParent = oCollider.GetComponentInParent<Actor>();
				ApplyImpact(componentInParent);
				if (substate == Substate.HomingAttack)
				{
					substate = Substate.Jump;
				}
			}
		}

		private void Awake()
		{
			if ((bool)base.character)
			{
				base.character.ImpactIncoming += HandleImpactIncoming;
			}
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class HomingAttack : AbilityBehaviour
	{
		[SerializeField]
		private LayerMask _enemyLayers = default(LayerMask);

		[SerializeField]
		private LayerMask _blockingLayers = default(LayerMask);

		[SerializeField]
		private string _jumpEffectName = string.Empty;

		[SerializeField]
		private string _homingEffectName = string.Empty;

		[SerializeField]
		private float _homingAttackRange;

		[SerializeField]
		private float _homingDotProduct = 0.5f;

		[SerializeField]
		private bool _scaleHomingRange = true;

		public LayerMask enemyLayers
		{
			get
			{
				return _enemyLayers;
			}
		}

		public LayerMask blockingLayers
		{
			get
			{
				return _blockingLayers;
			}
		}

		public string jumpEffectName
		{
			get
			{
				return _jumpEffectName;
			}
		}

		public string homingEffectName
		{
			get
			{
				return _homingEffectName;
			}
		}

		public float homingAttackRange
		{
			get
			{
				return Mathf.Clamp(_homingAttackRange, 0f, float.MaxValue);
			}
		}

		public float homingDotProduct
		{
			get
			{
				return Mathf.Clamp(_homingDotProduct, -1f, 1f);
			}
		}

		public bool scaleHomingRange
		{
			get
			{
				return _scaleHomingRange;
			}
		}

		public override bool HasCustomRequirements(Ability o)
		{
			if ((bool)o.motor.state && ((o.motor.state is CharacterStateDefault) | (o.motor.state is CharacterStateRoll)))
			{
				return true;
			}
			return false;
		}

		public override void Engage(Ability o)
		{
			if (!o.actor | !o.motor)
			{
				return;
			}
			CharacterStateRoll component = o.actor.GetComponent<CharacterStateRoll>();
			Ability ability = o.actorAbilityComponent.FindAbility("Spindash");
			Spindash spindash = ((!ability.behaviour || !(ability.behaviour is Spindash)) ? null : (ability.behaviour as Spindash));
			bool flag = (bool)spindash && ability.active;
			bool flag2 = (bool)component && ((component.substate == CharacterStateRoll.Substate.Crouch) | (component.substate == CharacterStateRoll.Substate.Charge));
			if (flag && flag2 && o.motor.state == component)
			{
				spindash.ChargeSpindash(ability, spindash.chargeRate);
			}
			else if (o.CanActivate())
			{
				Activate(o);
				if (o.active)
				{
					component = o.motor.AddOrActivateState<CharacterStateRoll>();
					if (o.motor.grounded)
					{
						component.substate = CharacterStateRoll.Substate.Jump;
						if (jumpEffectName != string.Empty && (bool)o.character.avatar)
						{
							o.character.avatar.SpawnEffect(jumpEffectName);
						}
						o.motor.ApplyJumpForce(o.motor.ground.normal, o.character.GetAttributeValue("Jump Power"), o.character.GetAttributeValue("Jump Height"));
						o.motor.TriggerJumped();
					}
					else if (component.substate != CharacterStateRoll.Substate.HomingAttack)
					{
						HomingAttackTarget oTarget = null;
						Vector3 worldCenter = o.motor.worldCenter;
						Vector3 velocity = o.motor.velocity;
						float num = Vector3.Dot(velocity, o.motor.up);
						if (num < 0f)
						{
							num *= 2f;
						}
						velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, component.homingBoostSpeed, float.MaxValue);
						Vector3 vector = Utility.RelativeVector(velocity, o.motor.up);
						vector = ((!(vector.magnitude > 0f)) ? Vector3.forward : vector).normalized;
						vector *= Mathf.Clamp(velocity.magnitude, component.homingBoostSpeed, velocity.magnitude);
						velocity = (velocity + vector).normalized * velocity.magnitude;
						float radius = ((!scaleHomingRange) ? homingAttackRange : (homingAttackRange * velocity.magnitude));
						float num2 = homingDotProduct;
						Collider[] array = Physics.OverlapSphere(worldCenter, radius, enemyLayers, QueryTriggerInteraction.Collide);
						for (int i = 0; i < array.Length; i++)
						{
							HomingAttackTarget component2 = array[i].GetComponent<HomingAttackTarget>();
							if ((bool)component2 && component2.gameObject.activeInHierarchy)
							{
								Vector3 worldOffset = component2.worldOffset;
								float num3 = Vector3.Dot(velocity.normalized, (worldOffset - worldCenter).normalized);
								if (num3 > num2 && !Physics.Linecast(worldCenter, worldOffset, blockingLayers, QueryTriggerInteraction.Ignore))
								{
									oTarget = component2;
									num2 = num3;
								}
							}
						}
						component.HomingAttack(oTarget);
					}
				}
			}
			Deactivate(o);
		}

		public override void Disengage(Ability o)
		{
			if ((bool)o.motor && (bool)o.motor.state)
			{
				o.motor.state.DisengageJump();
			}
		}
	}
}

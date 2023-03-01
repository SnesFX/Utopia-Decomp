using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Flinch : AbilityBehaviour
	{
		[SerializeField]
		private ImpactEffectSetupData _invulnerableEffect;

		[SerializeField]
		private string _damageEffectName = string.Empty;

		[SerializeField]
		private string _deathEffectName = string.Empty;

		[SerializeField]
		private string _drownEffectName = string.Empty;

		[SerializeField]
		private float _recoilAngle = 5f;

		[SerializeField]
		private float _recoilSpeed = 5f;

		[SerializeField]
		private float _deathRecoilSpeed = 30f;

		public ImpactEffectSetupData invulnerableEffect
		{
			get
			{
				return _invulnerableEffect;
			}
		}

		public string damageEffectName
		{
			get
			{
				return _damageEffectName;
			}
		}

		public string deathEffectName
		{
			get
			{
				return _deathEffectName;
			}
		}

		public string drownEffectName
		{
			get
			{
				return _drownEffectName;
			}
		}

		public float recoilAngle
		{
			get
			{
				return Mathf.Clamp(_recoilAngle, 0f, float.MaxValue);
			}
		}

		public float recoilSpeed
		{
			get
			{
				return Mathf.Clamp(_recoilSpeed, 0f, float.MaxValue);
			}
		}

		public float deathRecoilSpeed
		{
			get
			{
				return Mathf.Clamp(_deathRecoilSpeed, 0f, float.MaxValue);
			}
		}

		public override void OnInitialize(Ability oAbility)
		{
			oAbility.actor.Death += oAbility.HandleTrigger;
		}

		public override void OnTrigger(Ability oAbility, ActorEventArgs e)
		{
			if (e.context is ContextImpactApply)
			{
				ActorImpactEventArgs actorImpactEventArgs = e as ActorImpactEventArgs;
				ImpactApplicationData impactData = actorImpactEventArgs.impactData;
				if (impactData == null || impactData.prevented || !oAbility.actor.alive)
				{
					return;
				}
				Activate(oAbility);
				if (oAbility.active)
				{
					oAbility.actor.ImpactEffect(new ImpactEffectApplicationData(invulnerableEffect));
					if ((bool)oAbility.motor)
					{
						CharacterStateDamage characterStateDamage = oAbility.motor.AddOrActivateState<CharacterStateDamage>();
						Vector3 vVector = oAbility.actor.transform.position - impactData.origin;
						vVector = Utility.RelativeVector(vVector, oAbility.motor.up).normalized;
						vVector = Quaternion.Euler(Vector3.Cross(vVector, oAbility.motor.up) * recoilAngle) * vVector * recoilSpeed;
						oAbility.motor.grounded = false;
						oAbility.motor.velocity = vVector;
						if (damageEffectName != string.Empty && (bool)oAbility.avatar)
						{
							oAbility.avatar.SpawnEffect(damageEffectName);
						}
					}
				}
				Deactivate(oAbility);
			}
			else if (e.context is ContextDeath && (bool)oAbility.motor)
			{
				ActorDeathEventArgs actorDeathEventArgs = e as ActorDeathEventArgs;
				CharacterStateDead characterStateDead = oAbility.motor.AddOrActivateState<CharacterStateDead>();
				oAbility.motor.grounded = false;
				oAbility.motor.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - oAbility.motor.transform.position, oAbility.motor.up);
				oAbility.motor.velocity = oAbility.motor.up * deathRecoilSpeed;
				if (actorDeathEventArgs.deathType is Drown && drownEffectName != string.Empty && (bool)oAbility.avatar)
				{
					oAbility.avatar.SpawnEffect(drownEffectName);
				}
				else if (damageEffectName != string.Empty && (bool)oAbility.avatar)
				{
					oAbility.avatar.SpawnEffect(deathEffectName);
				}
			}
		}
	}
}

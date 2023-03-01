using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Barrier : ImpactEffectBehaviour
	{
		[SerializeField]
		private GameObject _barrierEffect;

		[SerializeField]
		private ImpactEffectSetupData _invulnerableEffect;

		[SerializeField]
		private string _damageEffectName = string.Empty;

		[SerializeField]
		private float _recoilAngle = 5f;

		[SerializeField]
		private float _recoilSpeed = 5f;

		public GameObject barrierEffect
		{
			get
			{
				return _barrierEffect;
			}
		}

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

		public override void OnAdd(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming += oEffect.HandleTrigger;
			GameObject gameObject = UnityEngine.Object.Instantiate(barrierEffect);
			gameObject.transform.parent = oEffect.actor.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			if (!oEffect.variables.ContainsKey("barrier"))
			{
				oEffect.variables.Add("barrier", null);
			}
			oEffect.variables["barrier"] = gameObject;
		}

		public override void OnRemove(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming -= oEffect.HandleTrigger;
			if (oEffect.variables.ContainsKey("barrier"))
			{
				GameObject gameObject = oEffect.variables["barrier"] as GameObject;
				if ((bool)gameObject)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			oEffect.variables.Remove("barrier");
		}

		public override void OnTrigger(ImpactEffect oEffect, ActorEventArgs e)
		{
			if (!(e.context is ContextImpactIncoming))
			{
				return;
			}
			ActorImpactEventArgs actorImpactEventArgs = e as ActorImpactEventArgs;
			ImpactApplicationData impactData = actorImpactEventArgs.impactData;
			if (impactData == null)
			{
				return;
			}
			impactData.prevented = true;
			oEffect.actor.ImpactEffect(new ImpactEffectApplicationData(invulnerableEffect));
			CharacterMotor characterMotor = ((!oEffect.character) ? null : oEffect.character.motor);
			if ((bool)characterMotor)
			{
				CharacterStateDamage characterStateDamage = characterMotor.AddOrActivateState<CharacterStateDamage>();
				Vector3 vVector = characterMotor.transform.position - impactData.origin;
				vVector = Utility.RelativeVector(vVector, characterMotor.up).normalized;
				vVector = Quaternion.Euler(Vector3.Cross(vVector, characterMotor.up) * recoilAngle) * vVector * recoilSpeed;
				characterMotor.grounded = false;
				characterMotor.velocity = vVector;
				if (damageEffectName != string.Empty && (bool)oEffect.character.avatar)
				{
					oEffect.character.avatar.SpawnEffect(damageEffectName);
				}
			}
			oEffect.actor.statistics.RemoveImpactEffect(oEffect);
		}
	}
}

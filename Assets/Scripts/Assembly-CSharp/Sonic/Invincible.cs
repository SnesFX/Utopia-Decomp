using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Invincible : ImpactEffectBehaviour
	{
		[SerializeField]
		private GameObject _invincibilityEffect;

		[SerializeField]
		private ImpactSetupData _impact = new ImpactSetupData();

		[SerializeField]
		private float _impactDetectionBuffer = 2f;

		public GameObject invincibilityEffect
		{
			get
			{
				return _invincibilityEffect;
			}
		}

		public ImpactSetupData impact
		{
			get
			{
				return _impact;
			}
		}

		public float impactDetectionBuffer
		{
			get
			{
				return Mathf.Clamp(_impactDetectionBuffer, 0f, float.MaxValue);
			}
		}

		public override void OnUpdate(ImpactEffect oEffect)
		{
			CharacterMotor characterMotor = ((!oEffect.character) ? null : oEffect.character.motor);
			RaycastHit hitInfo;
			if ((bool)characterMotor && characterMotor.rigidbody.SweepTest(characterMotor.velocity, out hitInfo, characterMotor.velocity.magnitude * Time.fixedDeltaTime * impactDetectionBuffer))
			{
				Actor componentInParent = hitInfo.collider.GetComponentInParent<Actor>();
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = componentInParent;
				impactApplicationData.instigator = oEffect.character;
				impactApplicationData.source = oEffect.character;
				ImpactApplicationData oData = impactApplicationData;
				componentInParent.Impact(oData);
			}
		}

		public override void OnAdd(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming += oEffect.HandleTrigger;
			GameObject gameObject = UnityEngine.Object.Instantiate(invincibilityEffect);
			gameObject.transform.parent = oEffect.actor.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			if (!oEffect.variables.ContainsKey("invincibilityEffect"))
			{
				oEffect.variables.Add("invincibilityEffect", null);
			}
			oEffect.variables["invincibilityEffect"] = gameObject;
		}

		public override void OnRemove(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming -= oEffect.HandleTrigger;
			if (oEffect.variables.ContainsKey("invincibilityEffect"))
			{
				GameObject gameObject = oEffect.variables["invincibilityEffect"] as GameObject;
				if ((bool)gameObject)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			oEffect.variables.Remove("invincibilityEffect");
		}

		public override void OnTrigger(ImpactEffect oEffect, ActorEventArgs e)
		{
			if (e.context is ContextImpactIncoming)
			{
				ActorImpactEventArgs actorImpactEventArgs = e as ActorImpactEventArgs;
				ImpactApplicationData impactData = actorImpactEventArgs.impactData;
				if (impactData != null)
				{
					impactData.prevented = true;
				}
			}
		}
	}
}

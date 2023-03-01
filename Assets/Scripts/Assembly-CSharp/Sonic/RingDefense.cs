using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class RingDefense : AbilityBehaviour
	{
		[SerializeField]
		private ImpactEffectSetupData _invulnerableEffect;

		[SerializeField]
		private string _damageEffectName = string.Empty;

		[SerializeField]
		private float _recoilAngle = 45f;

		[SerializeField]
		private float _recoilSpeed = 5f;

		[SerializeField]
		private Currency _rings;

		[SerializeField]
		private RingDropper _ringDropper;

		[SerializeField]
		private Vector3 _ringDropperOffset = Vector3.up;

		[SerializeField]
		private int _maxRingsDropped = 20;

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

		public Currency rings
		{
			get
			{
				return _rings;
			}
		}

		public RingDropper ringDropper
		{
			get
			{
				return _ringDropper;
			}
		}

		public Vector3 ringDropperOffset
		{
			get
			{
				return _ringDropperOffset;
			}
		}

		public int maxRingsDropped
		{
			get
			{
				return Mathf.Clamp(_maxRingsDropped, 0, int.MaxValue);
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

		public override void OnInitialize(Ability oAbility)
		{
			oAbility.actor.ImpactHit += oAbility.HandleTrigger;
		}

		public override void OnTrigger(Ability oAbility, ActorEventArgs e)
		{
			if (!(e.context is ContextImpactHit))
			{
				return;
			}
			ActorImpactEventArgs actorImpactEventArgs = e as ActorImpactEventArgs;
			ImpactApplicationData impactData = actorImpactEventArgs.impactData;
			if (impactData == null || impactData.prevented || !oAbility.actor.alive)
			{
				return;
			}
			Activate(oAbility);
			if (oAbility.active && (bool)oAbility.actor && (bool)oAbility.actor.inventory)
			{
				int itemTotalQuantity = oAbility.actor.inventory.GetItemTotalQuantity(rings);
				if (itemTotalQuantity > 0)
				{
					oAbility.actor.inventory.RemoveItem(rings, itemTotalQuantity);
					oAbility.actor.ImpactEffect(new ImpactEffectApplicationData(invulnerableEffect));
					impactData.prevented = true;
					RingDropper ringDropper = UnityEngine.Object.Instantiate(this.ringDropper);
					ringDropper.startingRings = Mathf.Clamp(itemTotalQuantity, 0, maxRingsDropped);
					ringDropper.transform.parent = oAbility.actor.transform;
					ringDropper.transform.localPosition = ringDropperOffset;
					ringDropper.transform.localRotation = Quaternion.identity;
					if ((bool)oAbility.motor)
					{
						CharacterStateDamage characterStateDamage = oAbility.motor.AddOrActivateState<CharacterStateDamage>();
						Vector3 vVector = oAbility.actor.transform.position - impactData.origin;
						vVector = Utility.RelativeVector(vVector, oAbility.motor.up).normalized;
						vVector = Quaternion.Euler(Vector3.Cross(vVector, oAbility.motor.up) * recoilAngle) * vVector * recoilSpeed;
						if ((bool)oAbility.motor.defaultState)
						{
						}
						oAbility.motor.grounded = false;
						oAbility.motor.velocity = vVector;
						if (damageEffectName != string.Empty && (bool)oAbility.avatar)
						{
							oAbility.avatar.SpawnEffect(damageEffectName);
						}
					}
				}
			}
			Deactivate(oAbility);
		}
	}
}

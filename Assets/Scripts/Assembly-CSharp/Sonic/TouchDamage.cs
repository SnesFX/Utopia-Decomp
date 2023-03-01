using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class TouchDamage : ActorComponent
	{
		[SerializeField]
		private LayerMask _damageLayers = default(LayerMask);

		[SerializeField]
		private ImpactSetupData _impact = new ImpactSetupData();

		[SerializeField]
		private SpecialEffectHandler _effect = new SpecialEffectHandler();

		[SerializeField]
		private float _speedRequirement = 5f;

		public LayerMask damageLayers
		{
			get
			{
				return _damageLayers;
			}
		}

		public ImpactSetupData impact
		{
			get
			{
				return _impact;
			}
			set
			{
				_impact = value;
			}
		}

		public SpecialEffectHandler effect
		{
			get
			{
				return _effect;
			}
			set
			{
				_effect = value;
			}
		}

		public float speedRequirement
		{
			get
			{
				return _speedRequirement;
			}
		}

		private void OnCollisionEnter(Collision oCollision)
		{
			if ((int)damageLayers != ((int)damageLayers | (1 << oCollision.gameObject.layer)))
			{
				return;
			}
			Actor component = oCollision.gameObject.GetComponent<Actor>();
			ActorPhysics physics = component.physics;
			if ((bool)component && component.alive && (bool)component.impactable)
			{
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = component;
				impactApplicationData.instigator = base.actor;
				impactApplicationData.source = base.actor;
				impactApplicationData.origin = base.transform.position;
				impactApplicationData.velocity = ((!physics) ? Vector3.zero : physics.velocity);
				impactApplicationData.force = ((!physics) ? 0f : physics.velocity.magnitude);
				ImpactApplicationData impactApplicationData2 = impactApplicationData;
				component.Impact(impactApplicationData2);
				if (!impactApplicationData2.prevented)
				{
					effect.Spawn();
				}
			}
		}

		private void OnCollisionStay(Collision oCollision)
		{
			if ((int)damageLayers != ((int)damageLayers | (1 << oCollision.gameObject.layer)))
			{
				return;
			}
			Actor component = oCollision.gameObject.GetComponent<Actor>();
			ActorPhysics physics = component.physics;
			if ((bool)component && component.alive && (bool)component.impactable)
			{
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = component;
				impactApplicationData.instigator = base.actor;
				impactApplicationData.source = base.actor;
				impactApplicationData.origin = base.transform.position;
				impactApplicationData.velocity = ((!physics) ? Vector3.zero : physics.velocity);
				impactApplicationData.force = ((!physics) ? 0f : physics.velocity.magnitude);
				ImpactApplicationData impactApplicationData2 = impactApplicationData;
				component.Impact(impactApplicationData2);
				if (!impactApplicationData2.prevented)
				{
					effect.Spawn();
				}
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if ((int)damageLayers != ((int)damageLayers | (1 << oCollider.gameObject.layer)))
			{
				return;
			}
			Actor component = oCollider.gameObject.GetComponent<Actor>();
			ActorPhysics physics = component.physics;
			if ((bool)component && component.alive && (bool)component.impactable)
			{
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = component;
				impactApplicationData.instigator = base.actor;
				impactApplicationData.source = base.actor;
				impactApplicationData.origin = base.transform.position;
				impactApplicationData.velocity = ((!physics) ? Vector3.zero : physics.velocity);
				impactApplicationData.force = ((!physics) ? 0f : physics.velocity.magnitude);
				ImpactApplicationData impactApplicationData2 = impactApplicationData;
				component.Impact(impactApplicationData2);
				if (!impactApplicationData2.prevented)
				{
					effect.Spawn();
				}
			}
		}

		private void OnTriggerStay(Collider oCollider)
		{
			if ((int)damageLayers != ((int)damageLayers | (1 << oCollider.gameObject.layer)))
			{
				return;
			}
			Actor component = oCollider.gameObject.GetComponent<Actor>();
			ActorPhysics physics = component.physics;
			if ((bool)component && component.alive && (bool)component.impactable)
			{
				ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
				impactApplicationData.target = component;
				impactApplicationData.instigator = base.actor;
				impactApplicationData.source = base.actor;
				impactApplicationData.origin = base.transform.position;
				impactApplicationData.velocity = ((!physics) ? Vector3.zero : physics.velocity);
				impactApplicationData.force = ((!physics) ? 0f : physics.velocity.magnitude);
				ImpactApplicationData impactApplicationData2 = impactApplicationData;
				component.Impact(impactApplicationData2);
				if (!impactApplicationData2.prevented)
				{
					effect.Spawn();
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class BadnikProjectile : SpecialEffect
	{
		[SerializeField]
		private LayerMask _damageLayers = default(LayerMask);

		[SerializeField]
		private Vector3 _initialVelocity = Vector3.forward * 5f;

		[SerializeField]
		private ImpactSetupData _impact = new ImpactSetupData();

		[SerializeField]
		private SpecialEffectHandler _explosion = new SpecialEffectHandler();

		public LayerMask damageLayers
		{
			get
			{
				return _damageLayers;
			}
		}

		public Vector3 initialVelocity
		{
			get
			{
				return _initialVelocity;
			}
		}

		public ImpactSetupData impact
		{
			get
			{
				return _impact;
			}
		}

		public SpecialEffectHandler explosion
		{
			get
			{
				return _explosion;
			}
		}

		private void Start()
		{
			if (base.duration > 0f)
			{
				base.durationTimer = base.duration;
			}
			if ((bool)base.rigidbody)
			{
				base.rigidbody.velocity = base.transform.rotation * initialVelocity;
			}
		}

		private void OnCollisionEnter(Collision oCollision)
		{
			if ((int)damageLayers == ((int)damageLayers | (1 << oCollision.gameObject.layer)))
			{
				Character component = oCollision.gameObject.GetComponent<Character>();
				if ((bool)component && component.alive)
				{
					ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
					impactApplicationData.target = component;
					impactApplicationData.instigator = this;
					impactApplicationData.source = this;
					impactApplicationData.origin = oCollision.contacts[0].point;
					impactApplicationData.velocity = oCollision.relativeVelocity;
					impactApplicationData.force = oCollision.relativeVelocity.magnitude;
					ImpactApplicationData oData = impactApplicationData;
					component.Impact(oData);
				}
			}
			explosion.Spawn();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnCollisionStay(Collision oCollision)
		{
			if ((int)damageLayers == ((int)damageLayers | (1 << oCollision.gameObject.layer)))
			{
				Character component = oCollision.gameObject.GetComponent<Character>();
				if ((bool)component && component.alive)
				{
					ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
					impactApplicationData.target = component;
					impactApplicationData.instigator = this;
					impactApplicationData.source = this;
					impactApplicationData.origin = oCollision.contacts[0].point;
					impactApplicationData.velocity = oCollision.relativeVelocity;
					impactApplicationData.force = oCollision.relativeVelocity.magnitude;
					ImpactApplicationData oData = impactApplicationData;
					component.Impact(oData);
				}
			}
			explosion.Spawn();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

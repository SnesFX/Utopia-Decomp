using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Explosion : SpecialEffect
	{
		[SerializeField]
		private LayerMask _damageLayers = default(LayerMask);

		[SerializeField]
		private ImpactSetupData _impact = new ImpactSetupData();

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
		}

		public override void OnDestroyEffect()
		{
			Collider component = GetComponent<Collider>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if ((int)damageLayers == ((int)damageLayers | (1 << oCollider.gameObject.layer)))
			{
				Character component = oCollider.gameObject.GetComponent<Character>();
				if ((bool)component && component.alive)
				{
					ImpactApplicationData impactApplicationData = new ImpactApplicationData(impact);
					impactApplicationData.target = component;
					impactApplicationData.instigator = this;
					impactApplicationData.source = this;
					impactApplicationData.origin = base.transform.position;
					impactApplicationData.velocity = ((!oCollider.attachedRigidbody) ? Vector3.zero : oCollider.attachedRigidbody.velocity);
					impactApplicationData.force = ((!oCollider.attachedRigidbody) ? 0f : (oCollider.attachedRigidbody.velocity.magnitude * oCollider.attachedRigidbody.mass));
					ImpactApplicationData oData = impactApplicationData;
					component.Impact(oData);
				}
			}
		}
	}
}

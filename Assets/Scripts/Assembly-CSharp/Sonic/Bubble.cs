using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Bubble : SpecialEffect
	{
		[SerializeField]
		private SpecialEffectHandler _breathEffect = new SpecialEffectHandler();

		public SpecialEffectHandler breathEffect
		{
			get
			{
				return _breathEffect;
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			Breath component = oCollider.GetComponent<Breath>();
			if ((bool)component)
			{
				component.RestoreBreath();
				breathEffect.Spawn();
				ActorPhysics component2 = oCollider.GetComponent<ActorPhysics>();
				if ((bool)component2)
				{
					component2.velocity = Utility.RelativeVector(component2.velocity, component2.up);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}

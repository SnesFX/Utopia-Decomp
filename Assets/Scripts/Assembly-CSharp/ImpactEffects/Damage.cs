using UnityEngine;

namespace ImpactEffects
{
	public class Damage : ImpactEffectBehaviour
	{
		[SerializeField]
		private Element _damageType;

		public Element damageType
		{
			get
			{
				return _damageType;
			}
		}

		public override void OnApply(ImpactEffect oEffect)
		{
			if ((bool)oEffect.actor && oEffect.actor.alive)
			{
				Vital health = oEffect.actor.health;
				if (health != null)
				{
					health.currentValue -= oEffect.magnitude;
				}
			}
		}
	}
}

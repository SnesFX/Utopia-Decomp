using System;

namespace Sonic
{
	[Serializable]
	public class Invulnerable : ImpactEffectBehaviour
	{
		public override void OnAdd(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming += oEffect.HandleTrigger;
		}

		public override void OnRemove(ImpactEffect oEffect)
		{
			oEffect.actor.ImpactIncoming -= oEffect.HandleTrigger;
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

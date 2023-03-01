using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifierEffect : ImpactEffectBehaviour
{
	[SerializeField]
	private List<StatisticModifierSetupData> _modifiers = new List<StatisticModifierSetupData>();

	public List<StatisticModifierSetupData> modifiers
	{
		get
		{
			return _modifiers;
		}
	}

	public override void OnAdd(ImpactEffect oEffect)
	{
		for (int i = 0; i < modifiers.Count; i++)
		{
			if ((bool)oEffect.actor)
			{
				Statistic attribute = oEffect.actor.GetAttribute(modifiers[i].name);
				if (attribute != null)
				{
					attribute.AddModifier(new StatisticModifier(modifiers[i])
					{
						source = this
					});
				}
			}
		}
	}

	public override void OnRemove(ImpactEffect oEffect)
	{
		for (int i = 0; i < modifiers.Count; i++)
		{
			if ((bool)oEffect.actor)
			{
				Statistic attribute = oEffect.actor.GetAttribute(modifiers[i].name);
				if (attribute != null)
				{
					attribute.RemoveModifier(this);
				}
			}
		}
	}
}

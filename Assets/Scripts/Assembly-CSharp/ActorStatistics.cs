using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Actor))]
public class ActorStatistics : ActorComponent
{
	[SerializeField]
	private List<Statistic> _attributes = new List<Statistic>();

	[SerializeField]
	private List<ImpactEffect> _impactEffects = new List<ImpactEffect>();

	public List<Statistic> attributes
	{
		get
		{
			return _attributes;
		}
		set
		{
			_attributes = value;
		}
	}

	public List<ImpactEffect> impactEffects
	{
		get
		{
			return _impactEffects;
		}
		set
		{
			_impactEffects = value;
		}
	}

	public Statistic GetAttribute(string sName)
	{
		return _attributes.Find((Statistic o) => o.name == sName);
	}

	public float GetAttributeValue(string sName)
	{
		Statistic statistic = _attributes.Find((Statistic o) => o.name == sName);
		return (statistic == null) ? 0f : statistic.totalValue;
	}

	public void ApplyImpactEffect(ImpactEffectApplicationData oData)
	{
		if (oData.behaviour == null)
		{
			return;
		}
		ImpactEffect impactEffect = impactEffects.Find((ImpactEffect o) => o.behaviour == oData.behaviour && o.instigator == oData.instigator);
		if (impactEffect != null)
		{
			impactEffect.OnApply();
		}
		else
		{
			impactEffect = new ImpactEffect(oData);
			impactEffect.OnApply();
			if (impactEffect.duration > 0f)
			{
				impactEffects.Add(impactEffect);
				impactEffect.Initialize(base.actor);
				impactEffect.Expired += HandleImpactEffectExpired;
				impactEffect.OnAdd();
			}
		}
		if (impactEffect != null)
		{
			impactEffect.instigator = oData.instigator;
			impactEffect.magnitude = oData.magnitude;
			impactEffect.duration = oData.behaviour.duration;
			impactEffect.remainingDuration = oData.behaviour.duration;
			impactEffect.periodDuration = oData.behaviour.periodDuration;
			impactEffect.periodRemainingDuration = oData.behaviour.periodDuration;
			impactEffect.period = 0;
			impactEffect.quantity += oData.quantity;
		}
	}

	public void RemoveImpactEffect(ImpactEffect oImpactEffect)
	{
		oImpactEffect.OnRemove();
		impactEffects.Remove(oImpactEffect);
	}

	public void RemoveImpactEffect(ImpactEffectBehaviour oBehaviour)
	{
		List<ImpactEffect> list = impactEffects.FindAll((ImpactEffect o) => o.behaviour == oBehaviour);
		foreach (ImpactEffect item in list)
		{
			item.OnRemove();
			impactEffects.Remove(item);
		}
	}

	public void HandleImpactEffectExpired(object sender, ImpactEffectExpiredEventArgs e)
	{
		ImpactEffect impactEffect = ((!(sender is ImpactEffect)) ? null : (sender as ImpactEffect));
		if (impactEffect != null)
		{
			RemoveImpactEffect(impactEffect);
		}
	}

	private void Awake()
	{
		if (!base.actor)
		{
			return;
		}
		for (int i = 0; i < impactEffects.Count; i++)
		{
			impactEffects[i].Initialize(base.actor);
			if (impactEffects[i].behaviour != null)
			{
				impactEffects[i].duration = impactEffects[i].behaviour.duration;
				impactEffects[i].periodDuration = impactEffects[i].behaviour.periodDuration;
				impactEffects[i].remainingDuration = Mathf.Clamp(impactEffects[i].remainingDuration, 0f, impactEffects[i].duration);
				impactEffects[i].periodRemainingDuration = Mathf.Clamp(impactEffects[i].periodRemainingDuration, 0f, impactEffects[i].periodDuration);
			}
			impactEffects[i].OnAdd();
			impactEffects[i].Expired += HandleImpactEffectExpired;
		}
	}
}

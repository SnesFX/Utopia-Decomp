using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Actor))]
public class ActorAbilityComponent : ActorComponent
{
	[SerializeField]
	protected TagContainer _tags = new TagContainer();

	[SerializeField]
	protected List<Ability> _abilities = new List<Ability>();

	public TagContainer tags
	{
		get
		{
			return _tags;
		}
	}

	public List<Ability> abilities
	{
		get
		{
			return _abilities;
		}
	}

	public event ActorAbilityEventHandler AbilityEngaged;

	public event ActorAbilityEventHandler AbilityDisengaged;

	public Ability FindAbility(string sName)
	{
		return abilities.Find((Ability o) => o != null && o.behaviour != null && o.behaviour.displayName == sName);
	}

	public void EngageAbility(string sName)
	{
		Ability ability = abilities.Find((Ability o) => o != null && o.behaviour != null && o.behaviour.displayName == sName);
		if (ability != null)
		{
			ability.Engage();
		}
	}

	public void DisengageAbility(string sName)
	{
		Ability ability = abilities.Find((Ability o) => o != null && o.behaviour != null && o.behaviour.displayName == sName);
		if (ability != null)
		{
			ability.Disengage();
		}
	}

	public void CancelAbility(string sName)
	{
		Ability ability = abilities.Find((Ability o) => o != null && o.behaviour != null && o.behaviour.displayName == sName);
		if (ability != null)
		{
			ability.Cancel();
		}
	}

	public void CancelWithTags(TagContainer oTags)
	{
		List<Ability> list = abilities.FindAll((Ability o) => o.behaviour != null && o.active && o.behaviour.canCancel && o.behaviour.tags.AnyTagsMatch(oTags));
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Cancel();
		}
	}

	private void Awake()
	{
		if ((bool)base.actor)
		{
			for (int i = 0; i < abilities.Count; i++)
			{
				abilities[i].Initialize(base.actor);
			}
		}
	}
}

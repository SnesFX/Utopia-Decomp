using System;
using UnityEngine;

[Serializable]
public sealed class Cooldown
{
	private Actor _actor;

	[SerializeField]
	private string _name = "Global";

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _remainingDuration;

	[SerializeField]
	private TagContainer _tags = new TagContainer();

	public Actor actor
	{
		get
		{
			return _actor;
		}
	}

	public ActorAbilityComponent abilityComponent
	{
		get
		{
			return (!actor) ? null : actor.abilityComponent;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
	}

	public float duration
	{
		get
		{
			return Mathf.Clamp(_duration, 0f, float.MaxValue);
		}
	}

	public float remainingDuration
	{
		get
		{
			return Mathf.Clamp(_remainingDuration, 0f, duration);
		}
	}

	public TagContainer tags
	{
		get
		{
			return _tags;
		}
	}

	public event CooldownExpiredEventHandler Expired;

	public Cooldown(string sName, float fDuration, TagContainer oTags)
	{
		_tags = oTags;
		_name = sName;
		_duration = fDuration;
		_remainingDuration = fDuration;
	}

	private void TriggerExpired()
	{
		if (this.Expired != null)
		{
			this.Expired(this, new CooldownExpiredEventArgs());
		}
	}

	private void HandleTick(object sender, ActorEventArgs e)
	{
		_remainingDuration -= Mathf.Clamp(Time.deltaTime, 0f, float.MaxValue);
		_remainingDuration = Mathf.Clamp(_remainingDuration, 0f, duration);
		if (remainingDuration <= 0f)
		{
			TriggerExpired();
		}
	}

	public void Initialize(Actor oActor)
	{
		if (!_actor)
		{
			_actor = oActor;
		}
	}

	public void OnAdded()
	{
		if ((bool)actor)
		{
			actor.Tick += HandleTick;
			if ((bool)abilityComponent)
			{
				abilityComponent.tags.AppendTagsFromContainer(tags);
			}
		}
	}

	public void OnRemoved()
	{
		if ((bool)actor)
		{
			actor.Tick -= HandleTick;
			if ((bool)abilityComponent)
			{
				abilityComponent.tags.RemoveTagsFromContainer(tags);
			}
		}
	}
}

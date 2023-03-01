using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Ability
{
	[SerializeField]
	private AbilityBehaviour _behaviour;

	[SerializeField]
	private Hashtable _variables = new Hashtable();

	private Actor _actor;

	private bool _engaged;

	[SerializeField]
	private bool _active;

	public AbilityBehaviour behaviour
	{
		get
		{
			return _behaviour;
		}
		protected set
		{
			_behaviour = value;
		}
	}

	public Hashtable variables
	{
		get
		{
			return _variables;
		}
		protected set
		{
			_variables = value;
		}
	}

	public bool active
	{
		get
		{
			return _active;
		}
		protected set
		{
			_active = value;
		}
	}

	public bool engaged
	{
		get
		{
			return _engaged;
		}
		set
		{
			_engaged = value;
		}
	}

	public Actor actor
	{
		get
		{
			return _actor;
		}
		protected set
		{
			_actor = value;
		}
	}

	public ActorPhysics actorPhysics
	{
		get
		{
			return (!actor) ? null : actor.physics;
		}
	}

	public ActorStatistics actorStatistics
	{
		get
		{
			return (!actor) ? null : actor.statistics;
		}
	}

	public ActorAbilityComponent actorAbilityComponent
	{
		get
		{
			return (!actor) ? null : actor.abilityComponent;
		}
	}

	public Character character
	{
		get
		{
			return (!(_actor is Character)) ? null : (_actor as Character);
		}
	}

	public CharacterMotor motor
	{
		get
		{
			return (!actorPhysics || !(actorPhysics is CharacterMotor)) ? null : (actorPhysics as CharacterMotor);
		}
	}

	public CharacterAvatar avatar
	{
		get
		{
			return (!character) ? null : character.avatar;
		}
	}

	public bool IsBlocked()
	{
		if (!actor | !behaviour)
		{
			return false;
		}
		if (actorAbilityComponent.tags.AnyTagsMatch(behaviour.blockedByTags))
		{
			return true;
		}
		return false;
	}

	public bool IsOnCooldown()
	{
		return false;
	}

	public bool HasRequiredVitals()
	{
		return (bool)behaviour && behaviour.HasRequiredVitals(this);
	}

	public bool HasRequiredItems()
	{
		return (bool)behaviour && behaviour.HasRequiredItems(this);
	}

	public bool CanPayRequirement()
	{
		if (!actor | !behaviour)
		{
			return false;
		}
		if (!HasRequiredVitals())
		{
			return false;
		}
		if (!HasRequiredVitals())
		{
			return false;
		}
		return true;
	}

	public bool HasRequiredSpeed()
	{
		return (bool)behaviour && behaviour.HasRequiredSpeed(this);
	}

	public bool HasRequiredGround()
	{
		return (bool)behaviour && behaviour.HasRequiredGround(this);
	}

	public bool HasCustomRequirements()
	{
		return (bool)behaviour && behaviour.HasCustomRequirements(this);
	}

	public bool CanActivate()
	{
		if (active | !actor | !behaviour)
		{
			return false;
		}
		return !IsBlocked() && !IsOnCooldown() && HasRequiredVitals() && HasRequiredItems() && HasRequiredSpeed() && HasRequiredGround();
	}

	public void HandleTick(object sender, ActorEventArgs e)
	{
		OnUpdate();
	}

	public void HandleTrigger(object sender, ActorEventArgs e)
	{
		OnTrigger(e);
	}

	public void Initialize(Actor oActor)
	{
		if (!((bool)actor | !behaviour))
		{
			_actor = oActor;
			_variables = new Hashtable();
			behaviour.OnInitialize(this);
		}
	}

	public void SetActive(bool bActive)
	{
		if ((bool)actor)
		{
			if (!active && bActive)
			{
				actor.Tick += HandleTick;
			}
			else if (active && !bActive)
			{
				actor.Tick -= HandleTick;
			}
			active = bActive;
		}
	}

	public void Engage()
	{
		if ((bool)behaviour)
		{
			behaviour.Engage(this);
		}
	}

	public void Disengage()
	{
		if ((bool)behaviour)
		{
			behaviour.Disengage(this);
		}
	}

	public void Cancel()
	{
		if (!(!active | !behaviour))
		{
			behaviour.Cancel(this);
		}
	}

	public void Interrupt()
	{
		if (!(!active | !behaviour | !actorAbilityComponent))
		{
			behaviour.Interrupt(this);
		}
	}

	public void OnUpdate()
	{
		if ((bool)behaviour)
		{
			behaviour.OnUpdate(this);
		}
	}

	public void OnTrigger(ActorEventArgs e)
	{
		if ((bool)behaviour)
		{
			behaviour.OnTrigger(this, e);
		}
	}
}

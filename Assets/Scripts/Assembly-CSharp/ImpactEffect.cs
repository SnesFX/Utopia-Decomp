using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ImpactEffect
{
	[SerializeField]
	private ImpactEffectBehaviour _behaviour;

	[SerializeField]
	private Hashtable _variables = new Hashtable();

	private Actor _actor;

	[SerializeField]
	private Actor _instigator;

	[SerializeField]
	private int _quantity;

	[SerializeField]
	private float _magnitude;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _remainingDuration;

	[SerializeField]
	private int _period;

	[SerializeField]
	private float _periodDuration;

	[SerializeField]
	private float _periodRemainingDuration;

	public ImpactEffectBehaviour behaviour
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

	public Actor actor
	{
		get
		{
			return _actor;
		}
	}

	public Character character
	{
		get
		{
			return (!(_actor is Character)) ? null : (_actor as Character);
		}
	}

	public Actor instigator
	{
		get
		{
			return _instigator;
		}
		set
		{
			_instigator = value;
		}
	}

	public int quantity
	{
		get
		{
			return (_behaviour != null) ? Mathf.Clamp(_quantity, 0, _behaviour.maximumQuantity) : 0;
		}
		set
		{
			_quantity = ((_behaviour != null) ? Mathf.Clamp(value, 0, _behaviour.maximumQuantity) : 0);
		}
	}

	public float magnitude
	{
		get
		{
			return _magnitude;
		}
		set
		{
			_magnitude = value;
		}
	}

	public float duration
	{
		get
		{
			return Mathf.Clamp(_duration, 0f, float.MaxValue);
		}
		set
		{
			_duration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float remainingDuration
	{
		get
		{
			return Mathf.Clamp(_remainingDuration, 0f, duration);
		}
		set
		{
			_remainingDuration = Mathf.Clamp(value, 0f, duration);
		}
	}

	public int period
	{
		get
		{
			return _period;
		}
		set
		{
			_period = value;
		}
	}

	public float periodDuration
	{
		get
		{
			return Mathf.Clamp(_periodDuration, 0f, float.MaxValue);
		}
		set
		{
			_periodDuration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float periodRemainingDuration
	{
		get
		{
			return Mathf.Clamp(_periodRemainingDuration, 0f, periodDuration);
		}
		set
		{
			_periodRemainingDuration = Mathf.Clamp(value, 0f, periodDuration);
		}
	}

	public event ImpactEffectExpiredEventHandler Expired;

	public ImpactEffect(ImpactEffectApplicationData oData)
	{
		behaviour = oData.behaviour;
		instigator = oData.instigator;
		magnitude = oData.magnitude;
		quantity = oData.quantity;
		duration = behaviour.duration;
		remainingDuration = behaviour.duration;
		periodDuration = behaviour.periodDuration;
		periodRemainingDuration = behaviour.periodDuration;
	}

	private void TriggerExpired()
	{
		if (this.Expired != null)
		{
			this.Expired(this, new ImpactEffectExpiredEventArgs());
		}
	}

	private void HandleTick(object sender, ActorEventArgs e)
	{
		OnUpdate();
		if (behaviour.persistent)
		{
			return;
		}
		if (periodDuration > 0f)
		{
			periodRemainingDuration = _periodRemainingDuration - Time.deltaTime;
			if (periodRemainingDuration <= 0f && behaviour != null)
			{
				OnPeriodTick();
				periodRemainingDuration = periodDuration;
				period++;
			}
		}
		if (duration > -1f)
		{
			remainingDuration = _remainingDuration - Time.deltaTime;
			if ((remainingDuration <= 0f) & (duration > -1f))
			{
				TriggerExpired();
			}
		}
	}

	public void HandleTrigger(object sender, ActorEventArgs e)
	{
		OnTrigger(e);
	}

	public void Initialize(Actor oActor)
	{
		if (!_actor)
		{
			_actor = oActor;
		}
	}

	public void OnApply()
	{
		if ((bool)behaviour)
		{
			behaviour.OnApply(this);
		}
	}

	public void OnAdd()
	{
		if (actor != null)
		{
			actor.Tick += HandleTick;
		}
		if ((bool)behaviour)
		{
			behaviour.OnAdd(this);
		}
	}

	public void OnRemove()
	{
		if ((bool)behaviour)
		{
			behaviour.OnRemove(this);
		}
		if ((bool)actor)
		{
			actor.Tick -= HandleTick;
		}
	}

	public void OnUpdate()
	{
		if ((bool)behaviour)
		{
			behaviour.OnUpdate(this);
		}
	}

	public void OnPeriodTick()
	{
		if ((bool)behaviour)
		{
			behaviour.OnPeriodTick(this);
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

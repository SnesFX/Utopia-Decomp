using System;
using UnityEngine;

[Serializable]
public class ImpactEffectApplicationData
{
	private ImpactEffectBehaviour _behaviour;

	private Actor _target;

	private Actor _instigator;

	private Actor _source;

	private bool _prevented;

	private float _magnitude;

	private int _quantity;

	private float _duration = 1f;

	public ImpactEffectBehaviour behaviour
	{
		get
		{
			return _behaviour;
		}
		set
		{
			_behaviour = value;
		}
	}

	public Actor target
	{
		get
		{
			return _target;
		}
		set
		{
			_target = value;
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

	public Actor source
	{
		get
		{
			return _source;
		}
		set
		{
			_source = value;
		}
	}

	public bool prevented
	{
		get
		{
			return _prevented;
		}
		set
		{
			_prevented = value;
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

	public ImpactEffectApplicationData(ImpactEffectSetupData oSetup)
	{
		behaviour = oSetup.behaviour;
		magnitude = oSetup.magnitude;
		quantity = oSetup.quantity;
		duration = oSetup.duration;
	}
}

using System;
using UnityEngine;

[Serializable]
public class ImpactEffectSetupData
{
	[SerializeField]
	private ImpactEffectBehaviour _behaviour;

	[SerializeField]
	private float _magnitude;

	[SerializeField]
	private int _quantity;

	[SerializeField]
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
}

using System;
using UnityEngine;

[Serializable]
public class ImpactEffectBehaviour : ScriptableObject
{
	[SerializeField]
	private string _displayName = "Impact Effect";

	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private string _description = "Description.";

	[SerializeField]
	private TagContainer _tags = new TagContainer();

	[SerializeField]
	private bool _display = true;

	[SerializeField]
	private bool _persistent;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _periodDuration;

	[SerializeField]
	private int _maximumQuantity;

	public string displayName
	{
		get
		{
			return _displayName;
		}
	}

	public Sprite icon
	{
		get
		{
			return _icon;
		}
	}

	public string description
	{
		get
		{
			return _description;
		}
	}

	public TagContainer tags
	{
		get
		{
			return _tags;
		}
	}

	public bool display
	{
		get
		{
			return _display;
		}
	}

	public bool persistent
	{
		get
		{
			return _persistent;
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

	public int maximumQuantity
	{
		get
		{
			return Mathf.Clamp(_maximumQuantity, 0, int.MaxValue);
		}
		set
		{
			_maximumQuantity = Mathf.Clamp(value, 0, int.MaxValue);
		}
	}

	public virtual void OnApply(ImpactEffect oEffect)
	{
	}

	public virtual void OnAdd(ImpactEffect oEffect)
	{
	}

	public virtual void OnRemove(ImpactEffect oEffect)
	{
	}

	public virtual void OnUpdate(ImpactEffect oEffect)
	{
	}

	public virtual void OnPeriodTick(ImpactEffect oEffect)
	{
	}

	public virtual void OnTrigger(ImpactEffect oEffect, ActorEventArgs e)
	{
	}
}

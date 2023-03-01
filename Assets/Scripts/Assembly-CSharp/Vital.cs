using System;
using UnityEngine;

[Serializable]
public class Vital : ActorComponent
{
	[SerializeField]
	private float _currentValue = 100f;

	[SerializeField]
	private Statistic _maximum = new Statistic("Maximum Vital", true, 100f);

	[SerializeField]
	private Statistic _regeneration = new Statistic("Vital Regeneration", true, 10f);

	public float currentValue
	{
		get
		{
			return Mathf.Clamp(_currentValue, 0f, maximum.totalValue);
		}
		set
		{
			_currentValue = Mathf.Clamp(value, 0f, maximum.totalValue);
		}
	}

	public Statistic maximum
	{
		get
		{
			return _maximum;
		}
	}

	public Statistic regeneration
	{
		get
		{
			return _regeneration;
		}
	}

	protected void HandleTick(object sender, ActorEventArgs e)
	{
		if (e.context is ContextTick)
		{
			Regenerate(regeneration.totalValue * (e as ActorTickEventArgs).deltaTime);
		}
	}

	public virtual void Regenerate(float fAmount)
	{
		if (base.actor.alive)
		{
			currentValue += regeneration.totalValue * fAmount;
		}
	}

	private void Awake()
	{
		if ((bool)base.actor && regeneration.totalValue > 0f)
		{
			base.actor.Tick += HandleTick;
		}
	}
}

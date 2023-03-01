using System;
using UnityEngine;

[Serializable]
public class Breath : Vital
{
	[Range(0.1f, 1f)]
	[SerializeField]
	private float _submersionThreshold = 0.8f;

	[SerializeField]
	private float _drowningRate = 10f;

	private bool _submerged;

	public float submersionThreshold
	{
		get
		{
			return Mathf.Clamp01(_submersionThreshold);
		}
	}

	public float drowningRate
	{
		get
		{
			return Mathf.Clamp(_drowningRate, 0f, float.MaxValue);
		}
		set
		{
			_drowningRate = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public bool submerged
	{
		get
		{
			return _submerged;
		}
		protected set
		{
			_submerged = value;
		}
	}

	public event ActorEventHandler Tick;

	public event ActorEventHandler Restored;

	public event ActorEventHandler Submerged;

	public event ActorEventHandler Desubmerged;

	protected void TriggerTick(float fDeltaTime)
	{
		if (this.Tick != null)
		{
			this.Tick(this, new ActorTickEventArgs(new ContextTick(), fDeltaTime));
		}
	}

	protected void TriggerRestored()
	{
		if (this.Restored != null)
		{
			this.Restored(this, new ActorGenericEventArgs(new ContextGeneric()));
		}
	}

	protected void TriggerSubmerged()
	{
		if (this.Submerged != null)
		{
			this.Submerged(this, new ActorGenericEventArgs(new ContextGeneric()));
		}
	}

	protected void TriggerDesubmerged()
	{
		if (this.Desubmerged != null)
		{
			this.Desubmerged(this, new ActorGenericEventArgs(new ContextGeneric()));
		}
	}

	protected void HandleRespawn(object sender, ActorEventArgs e)
	{
		if (e.context is ContextRespawn)
		{
			RestoreBreath();
		}
	}

	public override void Regenerate(float fAmount)
	{
		if (base.actor.alive && !submerged)
		{
			base.currentValue += base.regeneration.totalValue * fAmount;
		}
	}

	public void RestoreBreath()
	{
		base.currentValue = base.maximum.totalValue;
		TriggerRestored();
	}

	private void Update()
	{
		if ((bool)base.actor && base.actor.alive && (bool)base.actor.physics)
		{
			if (base.actor.physics.physicsVolumeData.submersion > submersionThreshold)
			{
				if (!submerged)
				{
					submerged = true;
					TriggerSubmerged();
				}
			}
			else if (submerged)
			{
				submerged = false;
				TriggerDesubmerged();
			}
			if (submerged)
			{
				base.currentValue -= drowningRate * Time.deltaTime;
				TriggerTick(Time.deltaTime);
				if (base.currentValue == 0f)
				{
					base.actor.Die(new Drown());
				}
			}
		}
		else
		{
			submerged = false;
		}
	}

	private void Awake()
	{
		if ((bool)base.actor && base.regeneration.totalValue > 0f)
		{
			base.actor.Tick += base.HandleTick;
			base.actor.Respawned += HandleRespawn;
		}
	}
}

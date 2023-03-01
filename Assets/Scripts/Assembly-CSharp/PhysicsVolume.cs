using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class PhysicsVolume : Actor
{
	[SerializeField]
	private PhysicsSubstance _substance;

	[SerializeField]
	private float _density;

	public float density
	{
		get
		{
			return (!substance) ? 0f : substance.density;
		}
	}

	public PhysicsSubstance substance
	{
		get
		{
			if (!_substance)
			{
				_substance = new PhysicsSubstance(PhysicsSubstanceType.Gas);
				_substance.name = "Default Gas";
			}
			return _substance;
		}
		set
		{
			_substance = value;
		}
	}

	private void OnTriggerEnter(Collider oCollider)
	{
		ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			component.EnteredPhysicsVolume(this);
		}
	}

	private void OnTriggerExit(Collider oCollider)
	{
		ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			component.ExitedPhysicsVolume(this);
		}
	}
}

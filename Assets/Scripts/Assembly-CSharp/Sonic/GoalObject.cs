using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class GoalObject : Actor
	{
		private bool _activated;

		private Animation _animation;

		[SerializeField]
		private AnimationClip _goalSpin;

		[SerializeField]
		private SpecialEffectHandler _goalEffect = new SpecialEffectHandler();

		public bool activated
		{
			get
			{
				return _activated;
			}
			protected set
			{
				_activated = value;
			}
		}

		public Animation animation
		{
			get
			{
				return (!_animation) ? (_animation = GetComponentInChildren<Animation>()) : _animation;
			}
		}

		public AnimationClip goalSpin
		{
			get
			{
				return _goalSpin;
			}
		}

		public SpecialEffectHandler goalEffect
		{
			get
			{
				return _goalEffect;
			}
		}

		public event ActorEventHandler Triggered;

		private void OnTriggerEnter(Collider oCollider)
		{
			if (activated)
			{
				return;
			}
			Character component = oCollider.GetComponent<Character>();
			if ((bool)component && (bool)component.user)
			{
				if (this.Triggered != null)
				{
					this.Triggered(this, new GoalTriggeredArgs(new ContextGoal(), component));
				}
				goalEffect.Spawn();
				if ((bool)animation)
				{
					animation.clip = goalSpin;
					animation.Play();
				}
				activated = true;
			}
		}
	}
}

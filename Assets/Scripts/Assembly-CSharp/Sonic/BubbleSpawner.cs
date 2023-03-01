using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class BubbleSpawner : MonoBehaviour
	{
		[SerializeField]
		private SpecialEffectHandler _bubble = new SpecialEffectHandler();

		[SerializeField]
		private float _bubbleSpawnDelay = 5f;

		[SerializeField]
		private float _bubbleDuration = 5f;

		[SerializeField]
		private float _bubbleSpeed = 0.5f;

		private float _timer;

		public SpecialEffectHandler bubble
		{
			get
			{
				return _bubble;
			}
		}

		public float bubbleSpawnDelay
		{
			get
			{
				return Mathf.Clamp(_bubbleSpawnDelay, 0f, float.MaxValue);
			}
		}

		public float bubbleDuration
		{
			get
			{
				return Mathf.Clamp(_bubbleDuration, 0f, float.MaxValue);
			}
		}

		public float bubbleSpeed
		{
			get
			{
				return Mathf.Clamp(_bubbleSpeed, 0f, float.MaxValue);
			}
		}

		public float timer
		{
			get
			{
				return Mathf.Clamp(_timer, 0f, float.MaxValue);
			}
			protected set
			{
				_timer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		private void Update()
		{
			timer -= Time.deltaTime;
			if (timer == 0f)
			{
				bubble.Spawn();
				bubble.activeObject.duration = bubbleDuration;
				bubble.activeObject.durationTimer = bubbleDuration;
				if ((bool)bubble.activeObject.rigidbody)
				{
					bubble.activeObject.rigidbody.velocity = -Physics.gravity.normalized * bubbleSpeed;
				}
				timer = bubbleSpawnDelay;
			}
		}
	}
}

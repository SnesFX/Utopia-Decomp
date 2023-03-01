using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class RingDropper : MonoBehaviour
	{
		private ActorPhysics _physics;

		[SerializeField]
		private SceneRing _ringPrefab;

		[SerializeField]
		private float _dropTimeBuffer = 0.1f;

		private float _dropTimer;

		[SerializeField]
		private int _startingRings = 20;

		private int _initialRings = 20;

		private int _remainingRings;

		[SerializeField]
		private bool _inheritVelocity;

		private float _dropYaw;

		[SerializeField]
		private float _dropSpeed = 5f;

		[SerializeField]
		private float _dropAngle = 45f;

		[SerializeField]
		private List<RingDropperThresholdEffect> _effects = new List<RingDropperThresholdEffect>
		{
			new RingDropperThresholdEffect
			{
				threshold = 0,
				effect = new SpecialEffectHandler()
			}
		};

		public ActorPhysics physics
		{
			get
			{
				return (!_physics) ? (_physics = GetComponentInParent<ActorPhysics>()) : _physics;
			}
		}

		public SceneRing ringPrefab
		{
			get
			{
				return _ringPrefab;
			}
		}

		public float dropTimeBuffer
		{
			get
			{
				return Mathf.Clamp(_dropTimeBuffer, 0f, float.MaxValue);
			}
		}

		protected float dropTimer
		{
			get
			{
				return _dropTimer;
			}
			set
			{
				_dropTimer = value;
			}
		}

		public int startingRings
		{
			get
			{
				return Mathf.Clamp(_startingRings, 0, int.MaxValue);
			}
			set
			{
				_startingRings = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		protected int initialRings
		{
			get
			{
				return Mathf.Clamp(_initialRings, 0, int.MaxValue);
			}
			set
			{
				_initialRings = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		public int remainingRings
		{
			get
			{
				return Mathf.Clamp(_remainingRings, 0, int.MaxValue);
			}
			protected set
			{
				_remainingRings = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		public bool inheritVelocity
		{
			get
			{
				return _inheritVelocity;
			}
			set
			{
				_inheritVelocity = value;
			}
		}

		public float dropYaw
		{
			get
			{
				return Mathf.Clamp(_dropYaw, 0f, float.MaxValue);
			}
			protected set
			{
				_dropYaw = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float dropSpeed
		{
			get
			{
				return Mathf.Clamp(_dropSpeed, 0f, float.MaxValue);
			}
			set
			{
				_dropSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float dropAngle
		{
			get
			{
				return Mathf.Clamp(_dropAngle, -90f, 90f);
			}
			set
			{
				_dropAngle = Mathf.Clamp(value, -90f, 90f);
			}
		}

		public List<RingDropperThresholdEffect> effects
		{
			get
			{
				return _effects;
			}
			set
			{
				_effects = value;
			}
		}

		private IEnumerator Delay(float fDelay)
		{
			yield return new WaitForSeconds(fDelay);
		}

		public void SpawnEffect(int iThreshold)
		{
			for (int num = effects.Count - 1; num >= 0; num--)
			{
				if (iThreshold >= effects[num].threshold)
				{
					effects[num].effect.Spawn();
					break;
				}
			}
		}

		private void Start()
		{
			initialRings = startingRings;
			remainingRings = startingRings;
			dropTimer = dropTimeBuffer;
			if (initialRings > 0)
			{
				dropYaw = 180f / (float)initialRings;
			}
			SpawnEffect(initialRings);
		}

		private void Update()
		{
			if (dropTimer <= 0f)
			{
				if (remainingRings > 0)
				{
					Vector3 vector = ((!inheritVelocity || !physics) ? Vector3.zero : physics.velocity);
					Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, base.transform.up);
					Vector3 vector2 = Quaternion.Euler(Vector3.left * dropAngle) * Vector3.forward * dropSpeed;
					vector2 = Quaternion.Euler(Vector3.up * ((float)remainingRings * dropYaw)) * vector2;
					SceneRing sceneRing = UnityEngine.Object.Instantiate(ringPrefab, base.transform.position, quaternion, null) as SceneRing;
					sceneRing.Drop(quaternion * vector2 + vector);
					if (remainingRings > 1)
					{
						SceneRing sceneRing2 = UnityEngine.Object.Instantiate(ringPrefab, base.transform.position, quaternion, null) as SceneRing;
						sceneRing2.Drop(quaternion * (Quaternion.Euler(Vector3.up * 180f) * vector2 + vector));
						remainingRings--;
					}
					remainingRings--;
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				dropTimer += dropTimeBuffer;
			}
			dropTimer -= Time.deltaTime;
		}
	}
}

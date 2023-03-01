using System;
using System.Collections;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class SceneRing : SceneItem
	{
		private static bool _switchRingEffect;

		private static SpecialEffect _ringEffectOne;

		private static SpecialEffect _ringEffectTwo;

		private static float _globalAngle;

		private bool _dropped;

		[SerializeField]
		private GameObject _displayObject;

		[SerializeField]
		private Collider _bounceCollider;

		[SerializeField]
		private bool _active = true;

		private bool _canPickUp;

		[SerializeField]
		private float _dropPickupDelay = 2f;

		[SerializeField]
		private float _dropDestroyDelay = 10f;

		[SerializeField]
		private float _respawnDelay = 60f;

		[SerializeField]
		private SpecialEffectHandler _specialEffect = new SpecialEffectHandler();

		public static bool switchRingEffect
		{
			get
			{
				return _switchRingEffect;
			}
			set
			{
				_switchRingEffect = value;
			}
		}

		public static SpecialEffect ringEffectOne
		{
			get
			{
				return _ringEffectOne;
			}
			set
			{
				_ringEffectOne = value;
			}
		}

		public static SpecialEffect ringEffectTwo
		{
			get
			{
				return _ringEffectTwo;
			}
			set
			{
				_ringEffectTwo = value;
			}
		}

		public static float globalAngle
		{
			get
			{
				return _globalAngle;
			}
			set
			{
				_globalAngle = value;
			}
		}

		public bool dropped
		{
			get
			{
				return _dropped;
			}
			protected set
			{
				_dropped = value;
			}
		}

		public GameObject displayObject
		{
			get
			{
				return _displayObject;
			}
			protected set
			{
				_displayObject = value;
			}
		}

		public Collider bounceCollider
		{
			get
			{
				return _bounceCollider;
			}
		}

		public bool active
		{
			get
			{
				return _active;
			}
			protected set
			{
				_active = value;
			}
		}

		public bool canPickUp
		{
			get
			{
				return _canPickUp;
			}
			protected set
			{
				_canPickUp = value;
			}
		}

		public float dropPickupDelay
		{
			get
			{
				return Mathf.Clamp(_dropPickupDelay, 0f, float.MaxValue);
			}
		}

		public float dropDestroyDelay
		{
			get
			{
				return Mathf.Clamp(_dropDestroyDelay, 0f, float.MaxValue);
			}
		}

		public float respawnDelay
		{
			get
			{
				return Mathf.Clamp(_respawnDelay, 0f, float.MaxValue);
			}
		}

		public SpecialEffectHandler specialEffect
		{
			get
			{
				return _specialEffect;
			}
		}

		private IEnumerator Delay(float fDelay)
		{
			yield return new WaitForSeconds(fDelay);
		}

		private IEnumerator DropSequence()
		{
			yield return new WaitForSeconds(dropPickupDelay);
			canPickUp = true;
			yield return new WaitForSeconds(dropDestroyDelay);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private IEnumerator RespawnSequence()
		{
			yield return new WaitForSeconds(respawnDelay);
			Respawn();
		}

		public void Drop(Vector3 vVelocity)
		{
			if ((bool)base.physics && (bool)base.physics.rigidbody && (bool)bounceCollider)
			{
				active = true;
				base.physics.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				base.physics.velocity = vVelocity;
				base.physics.useGravity = true;
				bounceCollider.gameObject.SetActive(true);
				dropped = true;
			}
		}

		public override void OnRespawn()
		{
			if (dropped)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			_active = true;
			_canPickUp = true;
			base.transform.position = base.spawnPosition;
			base.transform.rotation = base.spawnRotation;
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			if ((bool)displayObject)
			{
				displayObject.SetActive(true);
			}
		}

		public override void OnDespawn()
		{
			_active = false;
			if ((bool)displayObject)
			{
				displayObject.SetActive(false);
			}
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
			if (respawnDelay > 0f)
			{
				StartCoroutine(RespawnSequence());
			}
		}

		private void Awake()
		{
			GameMode.AddActorRegister(this);
			base.spawnPosition = base.transform.position;
			base.spawnRotation = base.transform.rotation;
			if (active)
			{
				Respawn();
			}
			else
			{
				Despawn();
			}
		}

		private void Start()
		{
			if (!dropped)
			{
				if ((bool)bounceCollider)
				{
					bounceCollider.gameObject.SetActive(false);
				}
				if ((bool)base.rigidbody)
				{
					base.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				}
			}
			else
			{
				canPickUp = false;
				StartCoroutine(DropSequence());
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if (!(active & canPickUp & (bool)oCollider.attachedRigidbody))
			{
				return;
			}
			ActorInventory component = oCollider.GetComponent<ActorInventory>();
			if (!component || !component.CanAddItem(base.item))
			{
				return;
			}
			component.AddItem(base.item);
			if (switchRingEffect)
			{
				if ((bool)ringEffectOne)
				{
					AudioSource componentInChildren = ringEffectOne.GetComponentInChildren<AudioSource>();
					if ((bool)componentInChildren)
					{
						componentInChildren.Stop();
					}
					ringEffectOne.Destroy();
				}
				ringEffectOne = specialEffect.Spawn();
			}
			else
			{
				if ((bool)ringEffectTwo)
				{
					AudioSource componentInChildren2 = ringEffectTwo.GetComponentInChildren<AudioSource>();
					if ((bool)componentInChildren2)
					{
						componentInChildren2.Stop();
					}
					ringEffectTwo.Destroy();
				}
				ringEffectTwo = specialEffect.Spawn();
			}
			switchRingEffect = !switchRingEffect;
			active = false;
			if (dropped)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				Despawn();
			}
		}

		private void OnTriggerStay(Collider oCollider)
		{
			if (!(active & canPickUp & (bool)oCollider.attachedRigidbody))
			{
				return;
			}
			ActorInventory component = oCollider.GetComponent<ActorInventory>();
			if (!component || !component.CanAddItem(base.item))
			{
				return;
			}
			component.AddItem(base.item);
			if (switchRingEffect)
			{
				if ((bool)ringEffectOne)
				{
					AudioSource componentInChildren = ringEffectOne.GetComponentInChildren<AudioSource>();
					if ((bool)componentInChildren)
					{
						componentInChildren.Stop();
					}
					ringEffectOne.Destroy();
				}
				ringEffectOne = specialEffect.Spawn();
			}
			else
			{
				if ((bool)ringEffectTwo)
				{
					AudioSource componentInChildren2 = ringEffectTwo.GetComponentInChildren<AudioSource>();
					if ((bool)componentInChildren2)
					{
						componentInChildren2.Stop();
					}
					ringEffectTwo.Destroy();
				}
				ringEffectTwo = specialEffect.Spawn();
			}
			switchRingEffect = !switchRingEffect;
			active = false;
			if (dropped)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				Despawn();
			}
		}

		private void Update()
		{
			if ((bool)displayObject && displayObject.activeSelf)
			{
				displayObject.transform.localRotation = Quaternion.Euler(Vector3.up * globalAngle);
			}
		}
	}
}

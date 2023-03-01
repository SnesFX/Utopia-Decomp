using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Newtron : Badnik
	{
		public enum Type
		{
			Newtron = 0,
			Leon = 1
		}

		public enum State
		{
			Stationary = 0,
			Alarm = 1,
			Prepare = 2,
			Jump = 3,
			Rocket = 4,
			Fire = 5,
			Rest = 6
		}

		[SerializeField]
		private Type _type;

		[SerializeField]
		private State _state;

		[SerializeField]
		private SkinnedMeshRenderer _meshRenderer;

		[SerializeField]
		private Material _newtronMaterial;

		[SerializeField]
		private Material _leonMaterial;

		private float _timer;

		[SerializeField]
		private float _prepareDuration = 1.5f;

		[SerializeField]
		private float _jumpDuration = 0.5f;

		[SerializeField]
		private float _jumpVelocity = 35f;

		[SerializeField]
		private Collider _explosionCollider;

		[SerializeField]
		private SpecialEffectHandler _explosionEffect = new SpecialEffectHandler();

		[SerializeField]
		private SpecialEffectHandler _fireEffect = new SpecialEffectHandler();

		[SerializeField]
		private float _bulletSpeed = 15f;

		public Type type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public State state
		{
			get
			{
				return _state;
			}
			protected set
			{
				_state = value;
			}
		}

		public SkinnedMeshRenderer meshRenderer
		{
			get
			{
				return _meshRenderer;
			}
		}

		public Material newtronMaterial
		{
			get
			{
				return _newtronMaterial;
			}
		}

		public Material leonMaterial
		{
			get
			{
				return _leonMaterial;
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

		public float prepareDuration
		{
			get
			{
				return Mathf.Clamp(_prepareDuration, 0f, float.MaxValue);
			}
		}

		public float jumpDuration
		{
			get
			{
				return Mathf.Clamp(_jumpDuration, 0f, float.MaxValue);
			}
		}

		public float jumpVelocity
		{
			get
			{
				return Mathf.Clamp(_jumpVelocity, 0f, float.MaxValue);
			}
		}

		public Collider explosionCollider
		{
			get
			{
				return _explosionCollider;
			}
		}

		public SpecialEffectHandler explosionEffect
		{
			get
			{
				return _explosionEffect;
			}
		}

		public SpecialEffectHandler fireEffect
		{
			get
			{
				return _fireEffect;
			}
		}

		public float bulletSpeed
		{
			get
			{
				return Mathf.Clamp(_bulletSpeed, 0f, float.MaxValue);
			}
		}

		public void SetType(Type oType)
		{
			type = oType;
			if ((bool)meshRenderer)
			{
				if (oType == Type.Newtron)
				{
					meshRenderer.material = newtronMaterial;
				}
				else
				{
					meshRenderer.material = leonMaterial;
				}
			}
		}

		public void SetState(State oState)
		{
			if (state == oState)
			{
				return;
			}
			switch (oState)
			{
			case State.Stationary:
				if ((bool)base.rigidbody)
				{
					base.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				}
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Stationary");
				}
				break;
			case State.Prepare:
				timer = prepareDuration;
				if ((bool)base.avatar && type == Type.Newtron)
				{
					base.avatar.SpawnEffect("Transform");
				}
				break;
			case State.Jump:
				timer = jumpDuration;
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Rocket");
				}
				break;
			case State.Rocket:
				if ((bool)base.rigidbody)
				{
					base.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				}
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Rocket");
				}
				break;
			case State.Fire:
				if ((bool)base.avatar)
				{
					if ((bool)base.rigidbody)
					{
						base.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					}
					base.avatar.SetState("Fire");
				}
				break;
			default:
				if ((bool)base.rigidbody)
				{
					base.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				}
				if ((bool)base.avatar)
				{
					base.avatar.SetState("Stationary");
				}
				break;
			}
			state = oState;
		}

		public void FireBullet(Vector3 vTarget)
		{
			fireEffect.Spawn();
			GameObject gameObject = fireEffect.activeObject.gameObject;
			if ((bool)gameObject)
			{
				gameObject.transform.rotation = Quaternion.FromToRotation(gameObject.transform.forward, vTarget - gameObject.transform.position) * gameObject.transform.rotation;
			}
		}

		public override void OnRespawn()
		{
			base.alive = true;
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			base.transform.position = base.spawnPosition;
			base.transform.rotation = base.spawnRotation;
			SetState(State.Stationary);
			base.gameObject.SetActive(true);
		}

		private void Start()
		{
			SetType(type);
			SetState(state);
		}

		private void FixedUpdate()
		{
			if (state == State.Prepare)
			{
				timer -= Time.deltaTime;
				if (timer == 0f)
				{
					SetState(State.Jump);
				}
			}
			else if (state == State.Jump)
			{
				timer -= Time.deltaTime;
				base.velocity = base.transform.forward * jumpVelocity;
				if (timer == 0f)
				{
					SetState(State.Rocket);
				}
			}
			if (state == State.Rocket && (bool)base.rigidbody)
			{
				Vector3 vector = base.velocity;
				Vector3 vector2 = base.transform.rotation * base.desiredDrive;
				Vector3 vector3 = base.transform.rotation * base.desiredAngle;
				if (vector3 == Vector3.zero)
				{
					vector3 = base.transform.forward;
				}
				Vector3 axis;
				float angle;
				Quaternion.FromToRotation(base.transform.forward, vector3).ToAngleAxis(out angle, out axis);
				angle = Mathf.Clamp(angle, (0f - base.rotateSpeed) * Time.deltaTime, base.rotateSpeed * Time.deltaTime);
				Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
				float num = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), 0f, vector.magnitude);
				vector = quaternion * vector.normalized * num + (vector - vector.normalized * num);
				vector = vector.normalized * Mathf.Clamp(vector.magnitude, base.speed, vector.magnitude);
				base.transform.rotation = quaternion * base.transform.rotation;
				Vector3 vector4 = Utility.RelativeVector(Vector3.up, base.transform.forward);
				base.transform.rotation = Quaternion.FromToRotation(base.transform.up, (!(vector4 != Vector3.zero)) ? base.transform.up : vector4) * base.transform.rotation;
				base.rigidbody.velocity = base.transform.forward * vector2.magnitude;
			}
		}

		private void OnCollisionEnter(Collision oCollision)
		{
			if (state != State.Rocket)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < oCollision.contacts.Length; i++)
			{
				if (oCollision.contacts[i].thisCollider == explosionCollider)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				explosionEffect.Spawn();
				Die();
				SetState(State.Stationary);
				base.transform.position = base.spawnPosition;
				base.transform.rotation = base.spawnRotation;
			}
		}
	}
}

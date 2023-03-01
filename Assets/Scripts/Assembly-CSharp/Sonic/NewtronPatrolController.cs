using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class NewtronPatrolController : BadnikPatrolController
	{
		public enum State
		{
			Stationary = 0,
			Alarm = 1,
			Rocket = 2,
			Fire = 3,
			Rest = 4
		}

		private Newtron _newtron;

		private Character _targetCharacter;

		[SerializeField]
		private LayerMask _characterLayers = default(LayerMask);

		[SerializeField]
		private State _state;

		[SerializeField]
		private float _detectionRadius;

		[SerializeField]
		private float _alarmDuration;

		[SerializeField]
		private float _fireDuration;

		[SerializeField]
		private float _restDuration;

		[SerializeField]
		private float _idleChirpDelay = 5f;

		[SerializeField]
		private float _tauntChirpDelay = 1f;

		private float _timer;

		public Newtron newtron
		{
			get
			{
				return (!_newtron) ? (_newtron = ((!base.badnik || !(base.badnik is Newtron)) ? null : (base.badnik as Newtron))) : _newtron;
			}
		}

		public Character targetCharacter
		{
			get
			{
				return _targetCharacter;
			}
			set
			{
				_targetCharacter = value;
			}
		}

		public LayerMask characterLayers
		{
			get
			{
				return _characterLayers;
			}
		}

		public State state
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		public float detectionRadius
		{
			get
			{
				return Mathf.Clamp(_detectionRadius, 0f, float.MaxValue);
			}
			set
			{
				_detectionRadius = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float alarmDuration
		{
			get
			{
				return Mathf.Clamp(_alarmDuration, 0f, float.MaxValue);
			}
		}

		public float fireDuration
		{
			get
			{
				return Mathf.Clamp(_fireDuration, 0f, float.MaxValue);
			}
		}

		public float restDuration
		{
			get
			{
				return Mathf.Clamp(_restDuration, 0f, float.MaxValue);
			}
		}

		public float idleChirpDelay
		{
			get
			{
				return Mathf.Clamp(_idleChirpDelay, 0f, float.MaxValue);
			}
		}

		public float tauntChirpDelay
		{
			get
			{
				return Mathf.Clamp(_tauntChirpDelay, 0f, float.MaxValue);
			}
		}

		public float timer
		{
			get
			{
				return Mathf.Clamp(_timer, 0f, float.MaxValue);
			}
			set
			{
				_timer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if ((bool)newtron)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(newtron.transform.position, detectionRadius);
			}
		}

		private void Start()
		{
			if ((bool)newtron)
			{
				newtron.spawnPosition = newtron.transform.position;
				newtron.spawnRotation = newtron.transform.rotation;
				newtron.SetState(Newtron.State.Stationary);
				timer = idleChirpDelay;
			}
		}

		private void FixedUpdate()
		{
			if (!newtron)
			{
				return;
			}
			if (!newtron.alive | !newtron.gameObject.activeInHierarchy)
			{
				newtron.SetState(Newtron.State.Stationary);
				newtron.transform.position = newtron.spawnPosition;
				newtron.transform.rotation = newtron.spawnRotation;
				state = State.Stationary;
				targetCharacter = null;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float speed = newtron.speed;
			if (state == State.Stationary)
			{
				timer -= Time.deltaTime;
				if (timer == 0f)
				{
					timer = idleChirpDelay;
					if ((bool)newtron.avatar)
					{
						newtron.avatar.SpawnEffect("Idle");
					}
				}
				Collider[] array = Physics.OverlapSphere(base.possessedPawn.transform.position, detectionRadius, characterLayers, QueryTriggerInteraction.Collide);
				targetCharacter = null;
				for (int i = 0; i < array.Length; i++)
				{
					targetCharacter = array[i].GetComponent<Character>();
					if ((bool)targetCharacter && (bool)targetCharacter.user && !Physics.Linecast(newtron.transform.position, array[i].bounds.center, base.physicalLayers, QueryTriggerInteraction.Ignore))
					{
						if ((bool)newtron.avatar && newtron.type == Newtron.Type.Leon)
						{
							newtron.avatar.SpawnEffect("Alarm");
						}
						state = State.Alarm;
						timer = alarmDuration;
						break;
					}
				}
			}
			else if (state == State.Alarm)
			{
				timer -= Time.deltaTime;
				if (timer == 0f)
				{
					if (newtron.type == Newtron.Type.Newtron)
					{
						state = State.Rocket;
						newtron.SetState(Newtron.State.Prepare);
					}
					else
					{
						state = State.Fire;
						timer = fireDuration;
						newtron.SetState(Newtron.State.Fire);
					}
				}
			}
			else if (state == State.Fire)
			{
				if (newtron.state == Newtron.State.Stationary)
				{
					targetCharacter = null;
					timer = idleChirpDelay;
					state = State.Stationary;
				}
				else
				{
					timer -= Time.deltaTime;
					if (timer == 0f)
					{
						newtron.SetState(Newtron.State.Rest);
						timer = restDuration;
						state = State.Rest;
						if ((bool)targetCharacter)
						{
							Vector3 vTarget = ((!targetCharacter.motor) ? targetCharacter.transform.position : targetCharacter.motor.worldCenter);
							newtron.FireBullet(vTarget);
						}
					}
				}
			}
			else if (state == State.Rest)
			{
				bool flag = timer < restDuration - tauntChirpDelay;
				timer -= Time.deltaTime;
				if (!flag && timer < restDuration - tauntChirpDelay && (bool)newtron.avatar)
				{
					newtron.avatar.SpawnEffect("Taunt");
				}
				if (timer == 0f)
				{
					newtron.SetState(Newtron.State.Stationary);
					state = State.Stationary;
				}
			}
			else if (state == State.Rocket && newtron.state == Newtron.State.Stationary)
			{
				targetCharacter = null;
				timer = idleChirpDelay;
				state = State.Stationary;
			}
			if (state == State.Stationary)
			{
				zero = Vector3.zero;
				zero2 = Vector3.zero;
			}
			else if (state == State.Rocket)
			{
				Vector3 lhs = ((!targetCharacter.motor) ? Vector3.zero : targetCharacter.motor.velocity);
				Vector3 vector = ((!targetCharacter.motor) ? (targetCharacter.transform.position + targetCharacter.transform.up) : targetCharacter.motor.worldTopPoint);
				zero = Vector3.ClampMagnitude(vector - newtron.transform.position, 1f) * Mathf.Clamp(speed, Vector3.Dot(lhs, newtron.transform.forward), float.MaxValue);
				zero2 = zero;
			}
			else
			{
				zero = Vector3.zero;
				zero2 = Vector3.zero;
			}
			newtron.desiredDrive = Quaternion.Inverse(newtron.transform.rotation) * zero;
			newtron.desiredAngle = Quaternion.Inverse(newtron.transform.rotation) * zero2;
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class MotobugPatrolController : BadnikPatrolController
	{
		public enum State
		{
			Idle = 0,
			Patrol = 1,
			Alarm = 2,
			Prepare = 3,
			Attack = 4,
			Retreat = 5
		}

		private Motobug _motobug;

		private Character _targetCharacter;

		[SerializeField]
		private LayerMask _characterLayers = default(LayerMask);

		[SerializeField]
		private State _state = State.Patrol;

		private Vector3 _destination = Vector3.zero;

		[SerializeField]
		private float _destinationBuffer = 1f;

		[SerializeField]
		private float _detectionRadius;

		[SerializeField]
		private float _chaseRadius;

		[SerializeField]
		private float _patrolRadius;

		[SerializeField]
		private float _patrolSpeed;

		[SerializeField]
		private float _minimumPatrolIdle;

		[SerializeField]
		private float _maximumPatrolIdle;

		[SerializeField]
		private float _minimumPatrolDuration;

		[SerializeField]
		private float _maximumPatrolDuration;

		[SerializeField]
		private float _minimumPatrolAngleDelay;

		[SerializeField]
		private float _maximumPatrolAngleDelay;

		[SerializeField]
		private float _minimumChirpDelay;

		[SerializeField]
		private float _maximumChirpDelay;

		[SerializeField]
		private float _alarmDuration;

		[SerializeField]
		private float _prepareDuration;

		private float _patrolAngle;

		private float _timer;

		private float _idleTimer;

		private float _angleTimer;

		public Motobug motobug
		{
			get
			{
				return (!_motobug) ? (_motobug = ((!base.badnik || !(base.badnik is Motobug)) ? null : (base.badnik as Motobug))) : _motobug;
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

		public Vector3 destination
		{
			get
			{
				return _destination;
			}
			set
			{
				_destination = value;
			}
		}

		public float destinationBuffer
		{
			get
			{
				return Mathf.Clamp(_destinationBuffer, 0f, float.MaxValue);
			}
		}

		public float detectionRadius
		{
			get
			{
				return Mathf.Clamp(_detectionRadius, 0f, float.MaxValue);
			}
		}

		public float chaseRadius
		{
			get
			{
				return Mathf.Clamp(_chaseRadius, patrolRadius, float.MaxValue);
			}
		}

		public float patrolRadius
		{
			get
			{
				return Mathf.Clamp(_patrolRadius, 0f, float.MaxValue);
			}
		}

		public float patrolSpeed
		{
			get
			{
				return Mathf.Clamp(_patrolSpeed, 0f, float.MaxValue);
			}
		}

		public float minimumPatrolIdle
		{
			get
			{
				return Mathf.Clamp(_minimumPatrolIdle, 0f, float.MaxValue);
			}
		}

		public float maximumPatrolIdle
		{
			get
			{
				return Mathf.Clamp(_maximumPatrolIdle, 0f, float.MaxValue);
			}
		}

		public float minimumPatrolDuration
		{
			get
			{
				return Mathf.Clamp(_minimumPatrolDuration, 0f, float.MaxValue);
			}
		}

		public float maximumPatrolDuration
		{
			get
			{
				return Mathf.Clamp(_maximumPatrolDuration, 0f, float.MaxValue);
			}
		}

		public float minimumPatrolAngleDelay
		{
			get
			{
				return Mathf.Clamp(_minimumPatrolAngleDelay, 0f, float.MaxValue);
			}
		}

		public float maximumPatrolAngleDelay
		{
			get
			{
				return Mathf.Clamp(_maximumPatrolAngleDelay, 0f, float.MaxValue);
			}
		}

		public float minimumChirpDelay
		{
			get
			{
				return Mathf.Clamp(_minimumChirpDelay, 0f, float.MaxValue);
			}
		}

		public float maximumChirpDelay
		{
			get
			{
				return Mathf.Clamp(_maximumChirpDelay, 0f, float.MaxValue);
			}
		}

		public float alarmDuration
		{
			get
			{
				return Mathf.Clamp(_alarmDuration, 0f, float.MaxValue);
			}
		}

		public float prepareDuration
		{
			get
			{
				return Mathf.Clamp(_prepareDuration, 0f, float.MaxValue);
			}
		}

		public float patrolAngle
		{
			get
			{
				return Mathf.Clamp(_patrolAngle, -180f, 180f);
			}
			set
			{
				_patrolAngle = Mathf.Clamp(value, -180f, 180f);
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

		public float idleTimer
		{
			get
			{
				return Mathf.Clamp(_idleTimer, 0f, float.MaxValue);
			}
			set
			{
				_idleTimer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float angleTimer
		{
			get
			{
				return Mathf.Clamp(_angleTimer, 0f, float.MaxValue);
			}
			set
			{
				_angleTimer = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.transform.position, patrolRadius);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.transform.position, chaseRadius);
			if ((bool)base.possessedPawn)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.possessedPawn.transform.position, detectionRadius);
			}
		}

		private void Start()
		{
			state = State.Idle;
			timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
		}

		private void FixedUpdate()
		{
			if (!motobug || (!motobug.alive | !motobug.gameObject.activeInHierarchy))
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			float speed = motobug.speed;
			if (state == State.Patrol || state == State.Idle)
			{
				idleTimer -= Time.deltaTime;
				if (idleTimer == 0f)
				{
					if ((bool)motobug.avatar)
					{
						motobug.avatar.SpawnEffect("Idle");
					}
					idleTimer = UnityEngine.Random.Range(minimumChirpDelay, maximumChirpDelay);
				}
			}
			timer -= Time.deltaTime;
			if (timer == 0f)
			{
				if (state == State.Idle)
				{
					timer = UnityEngine.Random.Range(minimumPatrolDuration, maximumPatrolDuration);
					state = State.Patrol;
				}
				else if (state == State.Patrol)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					state = State.Idle;
				}
				else if (state == State.Alarm)
				{
					motobug.SetState(Motobug.State.Attack);
					timer = prepareDuration;
					state = State.Prepare;
				}
				else if (state == State.Prepare)
				{
					motobug.SetState(Motobug.State.Attack);
					state = State.Attack;
				}
			}
			if ((state == State.Idle) | (state == State.Patrol))
			{
				Collider[] array = Physics.OverlapSphere(base.possessedPawn.transform.position, detectionRadius, characterLayers, QueryTriggerInteraction.Collide);
				targetCharacter = null;
				for (int i = 0; i < array.Length; i++)
				{
					targetCharacter = array[i].GetComponent<Character>();
					if ((bool)targetCharacter && (bool)targetCharacter.user && (targetCharacter.transform.position - base.transform.position).magnitude < chaseRadius && !Physics.Linecast(motobug.transform.position, array[i].bounds.center, base.physicalLayers, QueryTriggerInteraction.Ignore))
					{
						motobug.SetState(Motobug.State.Alarm);
						state = State.Alarm;
						timer = alarmDuration;
						break;
					}
				}
			}
			else if (state == State.Attack && (targetCharacter.transform.position - base.transform.position).magnitude > chaseRadius)
			{
				motobug.SetState(Motobug.State.Default);
				state = State.Retreat;
			}
			if (!targetCharacter && ((state == State.Alarm) | (state == State.Prepare) | (state == State.Attack)))
			{
				state = State.Retreat;
			}
			if (state == State.Idle)
			{
				vector = Vector3.zero;
			}
			else if (state == State.Patrol)
			{
				angleTimer -= Time.deltaTime;
				if (angleTimer == 0f)
				{
					patrolAngle = UnityEngine.Random.Range(0f - motobug.rotateSpeed, motobug.rotateSpeed);
				}
				vector = Quaternion.AngleAxis(patrolAngle * Time.deltaTime, motobug.transform.up) * motobug.transform.forward * speed;
				vector = Vector3.ClampMagnitude(motobug.transform.position + vector * Time.deltaTime - base.transform.position, chaseRadius) + base.transform.position;
				vector = (vector - motobug.transform.position).normalized * speed;
				vector2 = vector;
				if (vector.magnitude <= destinationBuffer)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					vector = Vector3.zero;
					state = State.Idle;
				}
			}
			else if (state == State.Alarm)
			{
				vector = Vector3.zero;
				vector2 = targetCharacter.transform.position - motobug.transform.position;
			}
			else if (state == State.Prepare)
			{
				vector = Vector3.zero;
				vector2 = targetCharacter.transform.position - motobug.transform.position;
			}
			else if (state == State.Attack)
			{
				vector = Utility.RelativeVector(targetCharacter.transform.position - motobug.transform.position, motobug.up).normalized * speed;
				vector2 = vector;
			}
			else if (state == State.Retreat)
			{
				vector = Utility.RelativeVector(base.transform.position - motobug.transform.position, motobug.up);
				vector = Vector3.ClampMagnitude(vector, 1f) * Mathf.Clamp(speed, 0f, patrolSpeed);
				vector2 = vector;
				if (vector.magnitude <= destinationBuffer)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					vector = Vector3.zero;
					state = State.Idle;
				}
			}
			motobug.desiredDrive = Quaternion.Inverse(motobug.transform.rotation) * vector;
			motobug.desiredAngle = Quaternion.Inverse(motobug.transform.rotation) * vector2;
		}
	}
}

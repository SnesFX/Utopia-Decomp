using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class BuzzbomberPatrolController : BadnikPatrolController
	{
		public enum State
		{
			Idle = 0,
			Patrol = 1,
			Alarm = 2,
			Chase = 3,
			Attack = 4,
			Rest = 5,
			Retreat = 6
		}

		private Buzzbomber _buzzbomber;

		private Character _targetCharacter;

		[SerializeField]
		private LayerMask _characterLayers = default(LayerMask);

		[SerializeField]
		private State _state = State.Patrol;

		[SerializeField]
		private GameObject _projectile;

		[SerializeField]
		private Vector3 _projectileSpawnPoint = Vector3.zero;

		private Vector3 _destination = Vector3.zero;

		[SerializeField]
		private float _destinationBuffer = 1f;

		[SerializeField]
		private float _detectionRadius;

		[SerializeField]
		private Bounds _patrolBounds = new Bounds(Vector3.zero, Vector3.one * 10f);

		[SerializeField]
		private float _minimumPatrolIdle;

		[SerializeField]
		private float _maximumPatrolIdle;

		[SerializeField]
		private float _patrolDuration;

		[SerializeField]
		private float _alarmDuration;

		[SerializeField]
		private float _attackDuration;

		[SerializeField]
		private float _restDuration;

		[SerializeField]
		private float _chaseHeight;

		[SerializeField]
		private float _chaseDistance;

		private float _timer;

		public Buzzbomber buzzbomber
		{
			get
			{
				return (!_buzzbomber) ? (_buzzbomber = ((!base.badnik || !(base.badnik is Buzzbomber)) ? null : (base.badnik as Buzzbomber))) : _buzzbomber;
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

		public GameObject projectile
		{
			get
			{
				return _projectile;
			}
			set
			{
				_projectile = value;
			}
		}

		public Vector3 projectileSpawnPoint
		{
			get
			{
				return _projectileSpawnPoint;
			}
			set
			{
				_projectileSpawnPoint = value;
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

		public Bounds patrolBounds
		{
			get
			{
				return _patrolBounds;
			}
			set
			{
				_patrolBounds = value;
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

		public float patrolDuration
		{
			get
			{
				return Mathf.Clamp(_patrolDuration, 0f, float.MaxValue);
			}
		}

		public float alarmDuration
		{
			get
			{
				return Mathf.Clamp(_alarmDuration, 0f, float.MaxValue);
			}
		}

		public float attackDuration
		{
			get
			{
				return Mathf.Clamp(_attackDuration, 0f, float.MaxValue);
			}
		}

		public float restDuration
		{
			get
			{
				return Mathf.Clamp(_restDuration, 0f, float.MaxValue);
			}
		}

		public float chaseHeight
		{
			get
			{
				return Mathf.Clamp(_chaseHeight, 0f, float.MaxValue);
			}
		}

		public float chaseDistance
		{
			get
			{
				return Mathf.Clamp(_chaseDistance, 0f, float.MaxValue);
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
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(base.transform.position + patrolBounds.center, patrolBounds.size);
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
			destination = base.transform.position + patrolBounds.center;
		}

		private void FixedUpdate()
		{
			if (!buzzbomber || (!buzzbomber.alive | !buzzbomber.gameObject.activeInHierarchy))
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			float speed = buzzbomber.speed;
			timer -= Time.deltaTime;
			if (timer == 0f)
			{
				if (state == State.Idle)
				{
					destination = base.transform.position + patrolBounds.center + new Vector3(UnityEngine.Random.Range(0f - patrolBounds.extents.x, patrolBounds.extents.x), 0f, UnityEngine.Random.Range(0f - patrolBounds.extents.z, patrolBounds.extents.z));
					RaycastHit hitInfo;
					if (buzzbomber.rigidbody.SweepTest(destination - buzzbomber.transform.position, out hitInfo, (destination - buzzbomber.transform.position).magnitude))
					{
						destination = (destination - buzzbomber.transform.position).normalized * hitInfo.distance + buzzbomber.transform.position;
					}
					timer = patrolDuration;
					state = State.Patrol;
				}
				else if (state == State.Patrol)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					state = State.Idle;
				}
				else if (state == State.Alarm)
				{
					state = State.Chase;
					buzzbomber.SetState(Buzzbomber.State.Fly);
				}
				else if (state == State.Attack)
				{
					if ((bool)buzzbomber.avatar)
					{
						buzzbomber.avatar.SpawnEffect("Shoot");
					}
					timer = restDuration;
					state = State.Rest;
				}
				else if (state == State.Rest)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					state = State.Idle;
					buzzbomber.SetState(Buzzbomber.State.Fly);
				}
			}
			if ((state == State.Idle) | (state == State.Patrol))
			{
				Collider[] array = Physics.OverlapSphere(base.possessedPawn.transform.position, detectionRadius, characterLayers, QueryTriggerInteraction.Collide);
				targetCharacter = null;
				for (int i = 0; i < array.Length; i++)
				{
					if (!Physics.Linecast(buzzbomber.transform.position, array[i].bounds.center, base.physicalLayers, QueryTriggerInteraction.Ignore))
					{
						targetCharacter = array[i].GetComponent<Character>();
						if ((bool)targetCharacter && (bool)targetCharacter.user)
						{
							state = State.Alarm;
							timer = alarmDuration;
							break;
						}
					}
				}
			}
			Bounds bounds = patrolBounds;
			bounds.Expand(detectionRadius * 2f);
			bounds.center += base.transform.position;
			if (((state == State.Chase) | (state == State.Alarm) | (state == State.Attack)) && (!targetCharacter || !bounds.Contains(targetCharacter.transform.position)))
			{
				targetCharacter = null;
				state = State.Retreat;
				buzzbomber.SetState(Buzzbomber.State.Fly);
				destination = base.transform.position + patrolBounds.center;
			}
			if (state == State.Idle)
			{
				vector = Vector3.zero;
				vector2 = base.transform.forward;
			}
			else if (state == State.Patrol)
			{
				vector = Vector3.ClampMagnitude(destination - buzzbomber.transform.position, 1f) * speed;
				vector2 = Utility.RelativeVector(vector, base.transform.up);
				if (vector2.magnitude < 0.1f)
				{
					vector2 = base.transform.forward;
				}
				if ((destination - buzzbomber.transform.position).magnitude <= destinationBuffer)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					vector = Vector3.zero;
					state = State.Idle;
				}
			}
			else if (state == State.Alarm)
			{
				vector = Vector3.zero;
				vector2 = targetCharacter.transform.position - buzzbomber.transform.position;
			}
			else if (state == State.Chase)
			{
				Vector3 vector3 = Utility.RelativeVector(targetCharacter.transform.position - buzzbomber.transform.position, Vector3.up);
				if (vector3 == Vector3.zero)
				{
					vector3 = buzzbomber.transform.position;
				}
				vector3 = vector3.normalized;
				destination = targetCharacter.transform.position - vector3.normalized * chaseDistance - Physics.gravity.normalized * chaseHeight;
				destination -= Vector3.up * Mathf.Clamp(Vector3.Dot(destination - bounds.center, Vector3.up), 0f, float.MaxValue);
				vector = Vector3.ClampMagnitude(destination - buzzbomber.transform.position, 1f) * speed;
				vector2 = targetCharacter.transform.position - buzzbomber.transform.position;
				if ((destination - buzzbomber.transform.position).magnitude <= destinationBuffer)
				{
					buzzbomber.SetState(Buzzbomber.State.Attack);
					state = State.Attack;
					timer = attackDuration;
				}
			}
			else if (state == State.Attack)
			{
				vector = Vector3.ClampMagnitude(targetCharacter.transform.position - buzzbomber.transform.position, 1f) * speed;
				vector2 = vector;
				vector = Vector3.zero;
			}
			else if (state == State.Rest)
			{
				vector = Vector3.zero;
				vector2 = targetCharacter.transform.position - buzzbomber.transform.position;
			}
			else if (state == State.Retreat)
			{
				vector = base.transform.position + patrolBounds.center - buzzbomber.transform.position;
				vector = Vector3.ClampMagnitude(vector, 1f) * speed;
				vector2 = vector;
				if ((destination - buzzbomber.transform.position).magnitude <= destinationBuffer)
				{
					timer = UnityEngine.Random.Range(minimumPatrolIdle, maximumPatrolIdle);
					vector = Vector3.zero;
					state = State.Idle;
				}
			}
			buzzbomber.desiredDrive = Quaternion.Inverse(buzzbomber.transform.rotation) * vector;
			buzzbomber.desiredAngle = Quaternion.Inverse(buzzbomber.transform.rotation) * vector2;
		}
	}
}

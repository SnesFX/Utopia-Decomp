using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CrabmeatPatrolController : BadnikPatrolController
	{
		public enum Substate
		{
			Scuttle = 0,
			Fire = 1
		}

		private Crabmeat _crabmeat;

		[SerializeField]
		private Substate _substate;

		private Vector3 _destination = Vector3.zero;

		[SerializeField]
		private float _destinationBuffer = 1f;

		private bool _scuttleSide = true;

		[SerializeField]
		private float _scuttleRange;

		[SerializeField]
		private float _minimumScuttleTime;

		[SerializeField]
		private float _maximumScuttleTime;

		[SerializeField]
		private float _fireDuration;

		private float _idleEffectTimer;

		private float _timer;

		public Crabmeat crabmeat
		{
			get
			{
				return (!_crabmeat) ? (_crabmeat = ((!base.badnik || !(base.badnik is Crabmeat)) ? null : (base.badnik as Crabmeat))) : _crabmeat;
			}
		}

		public Substate substate
		{
			get
			{
				return _substate;
			}
			set
			{
				_substate = value;
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

		public bool scuttleSide
		{
			get
			{
				return _scuttleSide;
			}
			set
			{
				_scuttleSide = value;
			}
		}

		public float scuttleRange
		{
			get
			{
				return Mathf.Clamp(_scuttleRange, 0f, float.MaxValue);
			}
		}

		public float minimumScuttleIdle
		{
			get
			{
				return Mathf.Clamp(_minimumScuttleTime, 0f, float.MaxValue);
			}
		}

		public float maximumScuttleIdle
		{
			get
			{
				return Mathf.Clamp(_maximumScuttleTime, 0f, float.MaxValue);
			}
		}

		public float fireDuration
		{
			get
			{
				return Mathf.Clamp(_fireDuration, 0f, float.MaxValue);
			}
		}

		public float idleEffectTimer
		{
			get
			{
				return Mathf.Clamp(_idleEffectTimer, 0f, float.MaxValue);
			}
			protected set
			{
				_idleEffectTimer = Mathf.Clamp(value, 0f, float.MaxValue);
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
			Vector3 vector = base.transform.position - base.transform.right * (scuttleRange * 0.5f);
			Vector3 vector2 = base.transform.position + base.transform.right * (scuttleRange * 0.5f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(vector - base.transform.right * destinationBuffer, base.transform.right * destinationBuffer * 2f);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(vector - base.transform.up * destinationBuffer, base.transform.up * destinationBuffer * 2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(vector - base.transform.forward * destinationBuffer, base.transform.forward * destinationBuffer * 2f);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(vector, destinationBuffer);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(vector2 - base.transform.right * destinationBuffer, base.transform.right * destinationBuffer * 2f);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(vector2 - base.transform.up * destinationBuffer, base.transform.up * destinationBuffer * 2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(vector2 - base.transform.forward * destinationBuffer, base.transform.forward * destinationBuffer * 2f);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(vector2, destinationBuffer);
		}

		private void Start()
		{
			substate = Substate.Scuttle;
			timer = UnityEngine.Random.Range(minimumScuttleIdle, maximumScuttleIdle);
			idleEffectTimer = timer * 0.5f;
			if ((bool)crabmeat)
			{
				crabmeat.spawnPosition = base.transform.position;
				crabmeat.spawnRotation = base.transform.rotation;
			}
		}

		private void FixedUpdate()
		{
			if (!crabmeat || (!crabmeat.alive | !crabmeat.gameObject.activeInHierarchy))
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 forward = base.transform.forward;
			float speed = crabmeat.speed;
			if (timer > 0f)
			{
				bool flag = timer > idleEffectTimer;
				timer -= Time.deltaTime;
				if (substate == Substate.Scuttle && (bool)crabmeat.avatar && flag && timer <= idleEffectTimer)
				{
					crabmeat.avatar.SpawnEffect("Idle");
				}
				if (timer == 0f)
				{
					if (substate == Substate.Scuttle)
					{
						timer = fireDuration;
						substate = Substate.Fire;
						crabmeat.state = Crabmeat.State.Fire;
					}
					else if (substate == Substate.Fire)
					{
						timer = UnityEngine.Random.Range(minimumScuttleIdle, maximumScuttleIdle);
						idleEffectTimer = timer * 0.5f;
						substate = Substate.Scuttle;
						crabmeat.state = Crabmeat.State.Scuttle;
					}
				}
			}
			if (substate == Substate.Scuttle)
			{
				Vector3 vector2 = base.transform.position - base.transform.right * scuttleRange * 0.5f;
				Vector3 vector3 = base.transform.position + base.transform.right * scuttleRange * 0.5f;
				if (scuttleSide && Utility.RelativeVector(vector2 - crabmeat.transform.position, -Physics.gravity.normalized).magnitude <= destinationBuffer)
				{
					scuttleSide = false;
				}
				else if (Utility.RelativeVector(vector3 - crabmeat.transform.position, -Physics.gravity.normalized).magnitude <= destinationBuffer)
				{
					scuttleSide = true;
				}
				vector = ((!scuttleSide) ? (Vector3.ClampMagnitude(vector3 - crabmeat.transform.position, 1f) * speed) : (Vector3.ClampMagnitude(vector2 - crabmeat.transform.position, 1f) * speed));
			}
			else if (substate == Substate.Fire)
			{
				vector = Vector3.zero;
			}
			crabmeat.desiredDrive = Quaternion.Inverse(crabmeat.transform.rotation) * vector;
			crabmeat.desiredAngle = Quaternion.Inverse(crabmeat.transform.rotation) * forward;
		}
	}
}

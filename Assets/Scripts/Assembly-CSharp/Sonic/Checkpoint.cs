using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Checkpoint : Actor
	{
		public enum Status
		{
			Inactive = 0,
			Active = 1,
			Passive = 2
		}

		[SerializeField]
		private Status _status;

		[SerializeField]
		private Transform _spawnPoint;

		private AudioSource _audioSource;

		private Animation _animation;

		[SerializeField]
		private AnimationClip _inactiveClip;

		[SerializeField]
		private AnimationClip _activeClip;

		[SerializeField]
		private SkinnedMeshRenderer _lampMeshRenderer;

		[SerializeField]
		private ParticleSystem _particleSystem;

		[SerializeField]
		private GameObject _inactiveObject;

		[SerializeField]
		private GameObject _activeObject;

		[SerializeField]
		private GameObject _passiveObject;

		public GameObject inactiveObject
		{
			get
			{
				return _inactiveObject;
			}
		}

		public GameObject activeObject
		{
			get
			{
				return _activeObject;
			}
		}

		public GameObject passiveObject
		{
			get
			{
				return _passiveObject;
			}
		}

		public Status status
		{
			get
			{
				return _status;
			}
			protected set
			{
				_status = value;
			}
		}

		public Transform spawnPoint
		{
			get
			{
				return _spawnPoint;
			}
		}

		public AudioSource audioSource
		{
			get
			{
				return (!_audioSource) ? (_audioSource = GetComponentInChildren<AudioSource>()) : _audioSource;
			}
		}

		public Animation animation
		{
			get
			{
				return (!_animation) ? (_animation = GetComponentInChildren<Animation>()) : _animation;
			}
		}

		public AnimationClip inactiveClip
		{
			get
			{
				return _inactiveClip;
			}
		}

		public AnimationClip activeClip
		{
			get
			{
				return _activeClip;
			}
		}

		public SkinnedMeshRenderer lampMeshRenderer
		{
			get
			{
				return _lampMeshRenderer;
			}
			set
			{
				_lampMeshRenderer = value;
			}
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return _particleSystem;
			}
			set
			{
				_particleSystem = value;
			}
		}

		public event ActorEventHandler Activated;

		public void TriggerActivatedEvent()
		{
			if (this.Activated != null)
			{
				this.Activated(this, new ActorGenericEventArgs(new ContextCheckpointActivated()));
			}
		}

		public void SetLampColor()
		{
			if ((bool)inactiveObject)
			{
				inactiveObject.SetActive(status == Status.Inactive);
			}
			if ((bool)activeObject)
			{
				activeObject.SetActive(status == Status.Active);
			}
			if ((bool)passiveObject)
			{
				passiveObject.SetActive(status == Status.Passive);
			}
		}

		public void Active()
		{
			if (status != Status.Active)
			{
				status = Status.Active;
				TriggerActivatedEvent();
				SetLampColor();
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
				if ((bool)animation)
				{
					animation.clip = activeClip;
					animation.Play();
				}
			}
		}

		public void Inactive()
		{
			status = Status.Inactive;
			SetLampColor();
			if ((bool)animation)
			{
				animation.clip = inactiveClip;
				animation.Play();
			}
		}

		public void Passive()
		{
			status = Status.Passive;
			SetLampColor();
		}

		public override void OnRespawn()
		{
		}

		public override void OnDespawn()
		{
		}

		private void Start()
		{
			SetLampColor();
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if (status == Status.Inactive)
			{
				Character component = oCollider.gameObject.GetComponent<Character>();
				if ((bool)component && (bool)component.user)
				{
					Active();
				}
			}
		}
	}
}

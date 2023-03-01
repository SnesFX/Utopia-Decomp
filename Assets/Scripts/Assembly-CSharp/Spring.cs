using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Spring : MonoBehaviour
{
	public enum SpringType
	{
		Yellow = 0,
		Red = 1
	}

	[SerializeField]
	private SpringType _type;

	[SerializeField]
	private SkinnedMeshRenderer _mesh;

	[SerializeField]
	private Material _yellowMaterial;

	[SerializeField]
	private Material _redMaterial;

	[SerializeField]
	private AudioClip _yellowSound;

	[SerializeField]
	private AudioClip _redSound;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private string _animatorTrigger = string.Empty;

	private AudioSource _audioSource;

	[SerializeField]
	private Vector3 _direction = Vector3.up;

	[SerializeField]
	private float _speed = 50f;

	[SerializeField]
	private float _requiredSpeed;

	[SerializeField]
	private bool _redirectMomentum = true;

	private bool _ready = true;

	[SerializeField]
	private float _resetDelay = 1f;

	public SpringType type
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

	public SkinnedMeshRenderer mesh
	{
		get
		{
			return _mesh;
		}
	}

	public Material yellowMaterial
	{
		get
		{
			return _yellowMaterial;
		}
	}

	public Material redMaterial
	{
		get
		{
			return _redMaterial;
		}
	}

	public AudioClip yellowSound
	{
		get
		{
			return _yellowSound;
		}
	}

	public AudioClip redSound
	{
		get
		{
			return _redSound;
		}
	}

	public Animator animator
	{
		get
		{
			return (!_animator) ? (_animator = GetComponentInChildren<Animator>()) : _animator;
		}
	}

	public string animatorTrigger
	{
		get
		{
			return _animatorTrigger;
		}
	}

	public AudioSource audioSource
	{
		get
		{
			return (!_audioSource) ? (_audioSource = GetComponent<AudioSource>()) : _audioSource;
		}
	}

	public Vector3 direction
	{
		get
		{
			return _direction.normalized;
		}
		set
		{
			_direction = value.normalized;
		}
	}

	public float speed
	{
		get
		{
			return Mathf.Clamp(_speed, 0f, float.MaxValue);
		}
		set
		{
			_speed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float requiredSpeed
	{
		get
		{
			return Mathf.Clamp(_requiredSpeed, 0f, float.MaxValue);
		}
		set
		{
			_requiredSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public bool redirectMomentum
	{
		get
		{
			return _redirectMomentum;
		}
		set
		{
			_redirectMomentum = value;
		}
	}

	public bool ready
	{
		get
		{
			return _ready;
		}
		protected set
		{
			_ready = value;
		}
	}

	public float resetDelay
	{
		get
		{
			return Mathf.Clamp(_resetDelay, 0f, float.MaxValue);
		}
		set
		{
			_resetDelay = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public void SetType(SpringType oType)
	{
		type = oType;
		if ((bool)mesh)
		{
			if (type == SpringType.Yellow)
			{
				mesh.material = yellowMaterial;
			}
			else
			{
				mesh.material = redMaterial;
			}
		}
		if ((bool)audioSource)
		{
			if (type == SpringType.Yellow)
			{
				audioSource.clip = yellowSound;
			}
			else
			{
				audioSource.clip = redSound;
			}
		}
	}

	private IEnumerator DelayReset(float fDelay)
	{
		yield return new WaitForSeconds(fDelay);
		ready = true;
	}

	private void DoSpring(GameObject oGameObject)
	{
		if (!ready)
		{
			return;
		}
		ActorPhysics component = oGameObject.GetComponent<ActorPhysics>();
		if ((bool)component)
		{
			Vector3 vector = base.transform.rotation * direction * speed;
			vector -= ((!redirectMomentum) ? Utility.VectorInDirection(component.velocity, vector) : component.velocity);
			component.grounded = false;
			component.velocity += vector;
			if ((bool)audioSource)
			{
				audioSource.Play();
			}
			if ((bool)animator && animatorTrigger != string.Empty)
			{
				animator.SetTrigger(animatorTrigger);
			}
			CharacterMotor characterMotor = ((!(component is CharacterMotor)) ? null : (component as CharacterMotor));
			if ((bool)characterMotor && characterMotor.character.alive)
			{
				characterMotor.SetStateSelection(CharacterStateSelection.Default);
			}
			ready = false;
			StartCoroutine(DelayReset(resetDelay));
			return;
		}
		Rigidbody component2 = oGameObject.GetComponent<Rigidbody>();
		if ((bool)component2)
		{
			Vector3 vector2 = base.transform.rotation * direction * speed;
			vector2 -= ((!redirectMomentum) ? Utility.VectorInDirection(component2.velocity, vector2) : component2.velocity);
			component2.velocity += vector2;
			if ((bool)audioSource)
			{
				audioSource.Play();
			}
			if ((bool)animator && animatorTrigger != string.Empty)
			{
				animator.SetTrigger(animatorTrigger);
			}
			ready = false;
			StartCoroutine(DelayReset(resetDelay));
		}
	}

	private void OnCollisionEnter(Collision oCollision)
	{
		if (ready && (bool)oCollision.rigidbody && oCollision.rigidbody.velocity.magnitude > requiredSpeed)
		{
			DoSpring(oCollision.gameObject);
		}
	}

	private void OnCollisionStay(Collision oCollision)
	{
		if (ready && (bool)oCollision.rigidbody && oCollision.rigidbody.velocity.magnitude > requiredSpeed)
		{
			DoSpring(oCollision.gameObject);
		}
	}

	private void OnTriggerEnter(Collider oCollider)
	{
		if (ready && (bool)oCollider.attachedRigidbody)
		{
			DoSpring(oCollider.gameObject);
		}
	}

	private void OnTriggerStay(Collider oCollider)
	{
		if (ready && (bool)oCollider.attachedRigidbody)
		{
			DoSpring(oCollider.gameObject);
		}
	}

	private void Awake()
	{
		SetType(type);
	}
}

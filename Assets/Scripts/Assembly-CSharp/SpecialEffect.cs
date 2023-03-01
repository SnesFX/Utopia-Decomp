using System;
using UnityEngine;

[Serializable]
public class SpecialEffect : Actor
{
	public enum AttachmentType
	{
		None = 0,
		AttachToSource = 1,
		AttachToGround = 2
	}

	private bool _destroying;

	[SerializeField]
	private float _duration;

	private float _durationTimer;

	[SerializeField]
	private float _destroyDelay;

	[SerializeField]
	private AttachmentType _attachmentType;

	[SerializeField]
	private LayerMask _attachmentMask = default(LayerMask);

	[SerializeField]
	private float _attachStartHeight = 0.1f;

	[SerializeField]
	private float _attachDistance = 0.5f;

	public bool destroying
	{
		get
		{
			return _destroying;
		}
		protected set
		{
			_destroying = value;
		}
	}

	public float duration
	{
		get
		{
			return Mathf.Clamp(_duration, 0f, float.MaxValue);
		}
		set
		{
			_duration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float durationTimer
	{
		get
		{
			return Mathf.Clamp(_durationTimer, 0f, float.MaxValue);
		}
		set
		{
			_durationTimer = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float destroyDelay
	{
		get
		{
			return Mathf.Clamp(_destroyDelay, 0f, float.MaxValue);
		}
	}

	public AttachmentType attachmentType
	{
		get
		{
			return _attachmentType;
		}
	}

	public LayerMask attachmentMask
	{
		get
		{
			return _attachmentMask;
		}
	}

	public float attachStartHeight
	{
		get
		{
			return Mathf.Clamp(_attachStartHeight, 0f, float.MaxValue);
		}
	}

	public float attachDistance
	{
		get
		{
			return Mathf.Clamp(_attachDistance, 0f, float.MaxValue);
		}
	}

	public SpecialEffect Spawn(Transform oSource)
	{
		if (!oSource)
		{
			return null;
		}
		SpecialEffect specialEffect = null;
		RaycastHit hitInfo;
		if (attachmentType == AttachmentType.None)
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = oSource.position;
			specialEffect.transform.rotation = oSource.rotation;
		}
		else if (attachmentType == AttachmentType.AttachToSource)
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = oSource.position;
			specialEffect.transform.rotation = oSource.rotation;
			specialEffect.transform.parent = oSource;
		}
		else if (attachmentType == AttachmentType.AttachToGround && Physics.Raycast(oSource.position + oSource.up * attachStartHeight, -oSource.up, out hitInfo, attachDistance, attachmentMask))
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = hitInfo.point;
			specialEffect.transform.rotation = Quaternion.FromToRotation(specialEffect.transform.up, hitInfo.normal) * oSource.rotation;
			specialEffect.transform.parent = hitInfo.transform;
		}
		specialEffect.OnSpawn();
		return specialEffect;
	}

	public SpecialEffect Spawn(Transform oSource, Vector3 vPosition, Quaternion qRotation)
	{
		if (!oSource)
		{
			return null;
		}
		SpecialEffect specialEffect = null;
		RaycastHit hitInfo;
		if (attachmentType == AttachmentType.None)
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = oSource.position + vPosition;
			specialEffect.transform.rotation = oSource.rotation * qRotation;
		}
		else if (attachmentType == AttachmentType.AttachToSource)
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = oSource.position + vPosition;
			specialEffect.transform.rotation = oSource.rotation * qRotation;
			specialEffect.transform.parent = oSource;
		}
		else if (attachmentType == AttachmentType.AttachToGround && Physics.Raycast(oSource.position + oSource.up * attachStartHeight, -oSource.up, out hitInfo, attachDistance, attachmentMask))
		{
			specialEffect = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<SpecialEffect>();
			specialEffect.transform.position = hitInfo.point;
			specialEffect.transform.rotation = Quaternion.FromToRotation(specialEffect.transform.up, hitInfo.normal) * oSource.rotation;
			specialEffect.transform.parent = hitInfo.transform;
		}
		if ((bool)specialEffect)
		{
			specialEffect.OnSpawn();
		}
		return specialEffect;
	}

	public void Refresh()
	{
		durationTimer = duration;
		OnRefresh();
	}

	public void Destroy()
	{
		destroying = true;
		durationTimer = 0f;
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Stop();
		}
		UnityEngine.Object.Destroy(base.gameObject, destroyDelay);
		OnDestroyEffect();
	}

	public virtual void OnSpawn()
	{
	}

	public virtual void OnRefresh()
	{
	}

	public virtual void OnDestroyEffect()
	{
	}

	private void Start()
	{
		if (duration > 0f)
		{
			durationTimer = duration;
		}
	}

	private void Update()
	{
		durationTimer -= Time.deltaTime;
		if (durationTimer == 0f)
		{
			Destroy();
		}
	}
}

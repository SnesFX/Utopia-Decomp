using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class StdItemGiver : SceneItem
{
	[SerializeField]
	private GameObject _displayPrefab;

	[SerializeField]
	private GameObject _displayObject;

	[SerializeField]
	private bool _active = true;

	[SerializeField]
	private float _respawnDelay = 60f;

	[SerializeField]
	private SpecialEffectHandler _specialEffect = new SpecialEffectHandler();

	public GameObject displayPrefab
	{
		get
		{
			return _displayPrefab;
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

	public bool active
	{
		get
		{
			return _active;
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

	public new void Respawn()
	{
		_active = true;
		Collider[] components = GetComponents<Collider>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = true;
		}
		if (!displayObject)
		{
			displayObject = UnityEngine.Object.Instantiate(displayPrefab);
		}
	}

	public new void Despawn()
	{
		_active = false;
		if ((bool)displayObject)
		{
			UnityEngine.Object.Destroy(displayObject);
		}
		Collider[] components = GetComponents<Collider>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
		specialEffect.Spawn();
		if (respawnDelay > 0f)
		{
			StartCoroutine(StartRespawnTimer());
		}
	}

	private IEnumerator StartRespawnTimer()
	{
		yield return new WaitForSeconds(respawnDelay);
		Respawn();
	}

	private void Awake()
	{
		if (active)
		{
			Respawn();
		}
		else
		{
			Despawn();
		}
	}

	private void OnTriggerEnter(Collider oCollider)
	{
		if (active)
		{
			ActorInventory component = oCollider.GetComponent<ActorInventory>();
			if ((bool)component && component.CanAddItem(base.item))
			{
				component.AddItem(base.item);
				Despawn();
			}
		}
	}
}

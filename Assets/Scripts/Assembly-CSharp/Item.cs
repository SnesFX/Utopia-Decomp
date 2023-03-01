using System;
using UnityEngine;

[Serializable]
public class Item : ScriptableObject
{
	[SerializeField]
	private string _displayName = "Item";

	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private SceneItem _sceneObject;

	[SerializeField]
	private int _maximumQuantity = 99;

	[SerializeField]
	private int _value;

	[SerializeField]
	private float _weight;

	[SerializeField]
	private bool _tradeable = true;

	[SerializeField]
	private bool _destroyable = true;

	public string displayName
	{
		get
		{
			return _displayName;
		}
	}

	public Sprite icon
	{
		get
		{
			return _icon;
		}
	}

	public SceneItem sceneObject
	{
		get
		{
			return _sceneObject;
		}
	}

	public int maximumQuantity
	{
		get
		{
			return Mathf.Clamp(_maximumQuantity, 1, int.MaxValue);
		}
	}

	public int value
	{
		get
		{
			return _value;
		}
	}

	public float weight
	{
		get
		{
			return _weight;
		}
	}

	public bool tradeable
	{
		get
		{
			return _tradeable;
		}
	}

	public bool destroyable
	{
		get
		{
			return _destroyable;
		}
	}

	public virtual void OnAdded(InventoryItemEntry oEntry)
	{
	}

	public virtual void OnAddedQuantity(InventoryItemEntry oEntry, int iQuantity)
	{
	}

	public virtual void OnRemoved(InventoryItemEntry oEntry)
	{
	}

	public virtual void OnRemovedQuantity(InventoryItemEntry oEntry, int iQuantity)
	{
	}
}

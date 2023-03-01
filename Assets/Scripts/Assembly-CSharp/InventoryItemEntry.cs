using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItemEntry
{
	private ActorInventory _inventory;

	[SerializeField]
	private Item _item;

	[SerializeField]
	private ItemQuality _quality;

	[SerializeField]
	private int _quantity;

	[SerializeField]
	private List<ItemProperty> _properties = new List<ItemProperty>();

	public ActorInventory inventory
	{
		get
		{
			return _inventory;
		}
		private set
		{
			_inventory = value;
		}
	}

	public Item item
	{
		get
		{
			return _item;
		}
		set
		{
			_item = value;
		}
	}

	public ItemQuality quality
	{
		get
		{
			return _quality;
		}
		set
		{
			_quality = value;
		}
	}

	public int quantity
	{
		get
		{
			return Mathf.Clamp(_quantity, 0, (!_item) ? 1 : _item.maximumQuantity);
		}
		set
		{
			_quantity = Mathf.Clamp(value, 0, (!_item) ? 1 : _item.maximumQuantity);
		}
	}

	public List<ItemProperty> properties
	{
		get
		{
			return _properties;
		}
		set
		{
			_properties = value;
		}
	}

	public void Initialize(ActorInventory oInventory)
	{
		if (!inventory)
		{
			inventory = oInventory;
		}
	}

	public void OnAdded()
	{
		if ((bool)item)
		{
			item.OnAdded(this);
		}
	}

	public void OnAddedQuantity(int iQuantity)
	{
		if ((bool)item)
		{
			item.OnAddedQuantity(this, iQuantity);
		}
	}

	public void OnRemoved()
	{
		if ((bool)item)
		{
			item.OnRemoved(this);
		}
	}

	public void OnRemovedQuantity(int iQuantity)
	{
		if ((bool)item)
		{
			item.OnRemovedQuantity(this, iQuantity);
		}
	}
}

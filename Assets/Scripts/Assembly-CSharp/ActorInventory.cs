using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Actor))]
public class ActorInventory : ActorComponent
{
	[SerializeField]
	protected List<InventoryItemEntry> _items = new List<InventoryItemEntry>();

	[SerializeField]
	protected bool _isFixedSize;

	[SerializeField]
	protected int _maximumEntries = -1;

	public List<InventoryItemEntry> items
	{
		get
		{
			return _items;
		}
	}

	public bool isFixedSize
	{
		get
		{
			return _isFixedSize;
		}
	}

	public int maximumEntries
	{
		get
		{
			return _maximumEntries;
		}
	}

	public bool CanAddItem(InventoryItemEntry oEntry)
	{
		if (oEntry.item == null)
		{
			return false;
		}
		InventoryItemEntry inventoryItemEntry = items.Find((InventoryItemEntry o) => (o.item == oEntry.item) & (o.quantity + oEntry.quantity <= oEntry.item.maximumQuantity));
		if (inventoryItemEntry != null)
		{
			return true;
		}
		if ((maximumEntries == -1) | (GetValidEntryCount() + 1 <= maximumEntries))
		{
			return true;
		}
		return false;
	}

	public bool CanAddItem(Item oItem, int iQuantity)
	{
		if (oItem == null)
		{
			return false;
		}
		InventoryItemEntry inventoryItemEntry = items.Find((InventoryItemEntry o) => o.item == oItem && o.quantity + iQuantity <= oItem.maximumQuantity);
		if (inventoryItemEntry != null)
		{
			return true;
		}
		if ((maximumEntries == -1) | (GetValidEntryCount() + 1 <= maximumEntries))
		{
			return true;
		}
		return false;
	}

	public int GetItemTotalQuantity(Item oItem)
	{
		if (oItem == null)
		{
			return -1;
		}
		int num = 0;
		List<InventoryItemEntry> list = items.FindAll((InventoryItemEntry o) => o.item == oItem);
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].quantity;
		}
		return num;
	}

	public int GetValidEntryCount()
	{
		List<InventoryItemEntry> list = items.FindAll((InventoryItemEntry o) => o.item != null);
		return list.Count;
	}

	public void AddItem(InventoryItemEntry oEntry)
	{
		if (!(oEntry.item == null))
		{
			InventoryItemEntry inventoryItemEntry = items.Find((InventoryItemEntry o) => (o.item == oEntry.item) & (o.quantity + oEntry.quantity <= oEntry.item.maximumQuantity));
			if (inventoryItemEntry != null)
			{
				inventoryItemEntry.quantity += oEntry.quantity;
				oEntry.OnAddedQuantity(oEntry.quantity);
			}
			else if ((maximumEntries == -1) | (GetValidEntryCount() + 1 <= maximumEntries))
			{
				InventoryItemEntry inventoryItemEntry2 = new InventoryItemEntry();
				inventoryItemEntry2.Initialize(this);
				inventoryItemEntry2.item = oEntry.item;
				inventoryItemEntry2.quantity = oEntry.quantity;
				inventoryItemEntry2.quality = oEntry.quality;
				items.Add(inventoryItemEntry2);
				inventoryItemEntry2.OnAdded();
			}
		}
	}

	public void AddItem(Item oItem, int iQuantity)
	{
		if (!(oItem == null))
		{
			InventoryItemEntry inventoryItemEntry = items.Find((InventoryItemEntry o) => o.item == oItem && o.quantity + iQuantity <= oItem.maximumQuantity);
			if (inventoryItemEntry != null)
			{
				inventoryItemEntry.quantity += iQuantity;
				inventoryItemEntry.OnAddedQuantity(iQuantity);
			}
			else if (((maximumEntries == -1) | (GetValidEntryCount() + 1 <= maximumEntries)) && isFixedSize)
			{
				InventoryItemEntry inventoryItemEntry2 = new InventoryItemEntry();
				inventoryItemEntry2.Initialize(this);
				inventoryItemEntry2.item = oItem;
				inventoryItemEntry2.quantity = iQuantity;
				items.Add(inventoryItemEntry2);
				inventoryItemEntry2.OnAdded();
			}
		}
	}

	public void RemoveItem(Item oItem, int iQuantity)
	{
		if (oItem == null)
		{
			return;
		}
		List<InventoryItemEntry> list = items.FindAll((InventoryItemEntry o) => o.item == oItem);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			int num2 = Mathf.Clamp(list[i].quantity, 0, iQuantity - num);
			list[i].OnRemovedQuantity(num2);
			list[i].quantity -= num2;
			num += num2;
			if (num >= iQuantity)
			{
				break;
			}
		}
		list = items.FindAll((InventoryItemEntry o) => o.item == oItem && o.quantity == 0);
		for (int j = 0; j < list.Count; j++)
		{
			list[j].OnRemoved();
		}
		items.RemoveAll((InventoryItemEntry o) => o.item == oItem && o.quantity == 0);
	}

	public void RemoveAllOfItem(Item oItem)
	{
		List<InventoryItemEntry> list = items.FindAll((InventoryItemEntry o) => o.item == oItem);
		foreach (InventoryItemEntry item in items)
		{
			item.OnRemoved();
		}
		items.RemoveAll((InventoryItemEntry o) => o.item == oItem);
	}

	public void RemoveNullEntries()
	{
		if (!isFixedSize)
		{
			items.RemoveAll((InventoryItemEntry o) => o.item == null);
		}
	}

	private void Awake()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].Initialize(this);
		}
	}
}

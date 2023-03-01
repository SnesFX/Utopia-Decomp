using System;
using UnityEngine;

[Serializable]
public class SceneItem : Actor
{
	[SerializeField]
	protected InventoryItemEntry _item;

	public InventoryItemEntry item
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
}

using System;
using UnityEngine;

[Serializable]
public sealed class ItemRequirement
{
	[SerializeField]
	private AbilityRequirementType _requirement = AbilityRequirementType.Full;

	[SerializeField]
	private Item _item;

	[SerializeField]
	private int _quantity = 1;

	public AbilityRequirementType requirement
	{
		get
		{
			return _requirement;
		}
	}

	public Item item
	{
		get
		{
			return _item;
		}
	}

	public int quantity
	{
		get
		{
			return _quantity;
		}
	}
}

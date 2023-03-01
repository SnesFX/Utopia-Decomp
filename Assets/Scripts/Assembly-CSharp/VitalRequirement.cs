using System;
using UnityEngine;

[Serializable]
public sealed class VitalRequirement
{
	[SerializeField]
	private AbilityRequirementType _requirement = AbilityRequirementType.Full;

	[SerializeField]
	private string _name = "Energy";

	[SerializeField]
	private float _quantity = 1f;

	public AbilityRequirementType requirement
	{
		get
		{
			return _requirement;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
	}

	public float quantity
	{
		get
		{
			return _quantity;
		}
	}
}

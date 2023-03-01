using System;
using UnityEngine;

[Serializable]
public class StatisticModifierSetupData
{
	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private StatisticModifierType _type;

	[SerializeField]
	private float _value;

	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public StatisticModifierType type
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

	public float value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}
}

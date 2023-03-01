using System;
using UnityEngine;

[Serializable]
public class StatisticModifier
{
	[SerializeField]
	private StatisticModifierType _type;

	[SerializeField]
	private UnityEngine.Object _source;

	[SerializeField]
	private float _value;

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

	public UnityEngine.Object source
	{
		get
		{
			return _source;
		}
		set
		{
			_source = value;
		}
	}

	public float value
	{
		get
		{
			return (!(_source != null)) ? 0f : _value;
		}
		set
		{
			_value = ((!(_source != null)) ? 0f : value);
		}
	}

	public StatisticModifier(StatisticModifierType oType, UnityEngine.Object oSource, float fValue)
	{
		_type = oType;
		_source = oSource;
		_value = fValue;
	}

	public StatisticModifier(StatisticModifierSetupData oData)
	{
		_type = oData.type;
		_value = oData.value;
	}
}

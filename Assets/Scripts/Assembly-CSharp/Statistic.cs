using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Statistic
{
	[SerializeField]
	private string _name = "Statistic";

	[SerializeField]
	[HideInInspector]
	private bool _integer;

	[SerializeField]
	private float _baseValue;

	[HideInInspector]
	[SerializeField]
	private float _numericalModifier;

	[SerializeField]
	[HideInInspector]
	private float _percentileModifier;

	private List<StatisticModifier> _modifiers = new List<StatisticModifier>();

	public string name
	{
		get
		{
			return _name;
		}
		private set
		{
			_name = value;
		}
	}

	public bool integer
	{
		get
		{
			return _integer;
		}
		private set
		{
			_integer = value;
		}
	}

	public float baseValue
	{
		get
		{
			return _baseValue;
		}
		set
		{
			_baseValue = value;
		}
	}

	public float numericalModifier
	{
		get
		{
			return _numericalModifier;
		}
		private set
		{
			_numericalModifier = value;
		}
	}

	public float percentileModifier
	{
		get
		{
			return _percentileModifier;
		}
		private set
		{
			_percentileModifier = value;
		}
	}

	public float totalValue
	{
		get
		{
			return (baseValue + numericalModifier) * (1f + percentileModifier);
		}
	}

	public List<StatisticModifier> modifiers
	{
		get
		{
			return _modifiers;
		}
	}

	public Statistic()
	{
		_name = "Statistic";
		_integer = false;
		_baseValue = 0f;
		_numericalModifier = 0f;
		_percentileModifier = 0f;
	}

	public Statistic(string sName)
	{
		_name = sName;
		_integer = false;
		_baseValue = 0f;
		_numericalModifier = 0f;
		_percentileModifier = 0f;
	}

	public Statistic(string sName, bool bInteger)
	{
		_name = sName;
		_integer = bInteger;
		_baseValue = 0f;
		_numericalModifier = 0f;
		_percentileModifier = 0f;
	}

	public Statistic(string sName, bool bInteger, float fValue)
	{
		_name = sName;
		_integer = bInteger;
		_baseValue = fValue;
		_numericalModifier = 0f;
		_percentileModifier = 0f;
	}

	public virtual void CalculateValue()
	{
		_numericalModifier = 0f;
		_percentileModifier = 0f;
		_modifiers.RemoveAll((StatisticModifier o) => o.source == null);
		for (int i = 0; i < _modifiers.Count; i++)
		{
			switch (_modifiers[i].type)
			{
			case StatisticModifierType.Numerical:
				_numericalModifier += _modifiers[i].value;
				break;
			case StatisticModifierType.Percentile:
				_percentileModifier += _modifiers[i].value;
				break;
			}
		}
	}

	public void AddModifier(StatisticModifier oModifier)
	{
		float num = _modifiers.FindIndex((StatisticModifier o) => o.source == oModifier.source);
		if (oModifier.source != null && num < 0f)
		{
			_modifiers.Add(oModifier);
		}
		CalculateValue();
	}

	public void AddModifier(StatisticModifierType oType, UnityEngine.Object oSource, float fValue)
	{
		float num = _modifiers.FindIndex((StatisticModifier o) => o.source == oSource);
		if (oSource != null && num < 0f)
		{
			_modifiers.Add(new StatisticModifier(oType, oSource, fValue));
		}
		CalculateValue();
	}

	public void RemoveModifier(UnityEngine.Object oSource)
	{
		_modifiers.RemoveAll((StatisticModifier o) => o.source == oSource);
		CalculateValue();
	}
}

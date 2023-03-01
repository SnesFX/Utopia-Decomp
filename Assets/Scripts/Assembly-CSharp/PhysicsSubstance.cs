using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class PhysicsSubstance : ScriptableObject
{
	[SerializeField]
	protected internal float _density = 1.225f;

	[SerializeField]
	protected internal PhysicsSubstanceType _type = PhysicsSubstanceType.Gas;

	public float density
	{
		get
		{
			return _density;
		}
		set
		{
			_density = value;
		}
	}

	public PhysicsSubstanceType type
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

	public PhysicsSubstance()
	{
		_density = 1.225f;
		_type = PhysicsSubstanceType.Gas;
	}

	public PhysicsSubstance(PhysicsSubstanceType oType)
	{
		_type = oType;
		switch (oType)
		{
		case PhysicsSubstanceType.Void:
			_density = 0f;
			break;
		case PhysicsSubstanceType.Gas:
			_density = 1.225f;
			break;
		case PhysicsSubstanceType.Liquid:
			_density = 1000f;
			break;
		}
	}
}

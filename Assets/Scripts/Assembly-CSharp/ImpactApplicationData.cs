using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImpactApplicationData
{
	private List<ImpactEffectSetupData> _effects = new List<ImpactEffectSetupData>();

	private TagContainer _requiredTags = new TagContainer();

	private TagContainer _blockedByTags = new TagContainer();

	private Actor _target;

	private Actor _instigator;

	private Actor _source;

	private ImpactResult _result;

	private bool _prevented;

	private Vector3 _origin = Vector3.zero;

	private Vector3 _velocity = Vector3.zero;

	private float _force;

	private float _surfaceArea;

	private float _magnitude;

	public TagContainer requiredTags
	{
		get
		{
			return _requiredTags;
		}
		set
		{
			_requiredTags = value;
		}
	}

	public TagContainer blockedByTags
	{
		get
		{
			return _blockedByTags;
		}
		set
		{
			_requiredTags = value;
		}
	}

	public List<ImpactEffectSetupData> effects
	{
		get
		{
			return _effects;
		}
		set
		{
			_effects = value;
		}
	}

	public Actor target
	{
		get
		{
			return _target;
		}
		set
		{
			_target = value;
		}
	}

	public Actor instigator
	{
		get
		{
			return _instigator;
		}
		set
		{
			_instigator = value;
		}
	}

	public Actor source
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

	public bool prevented
	{
		get
		{
			return _prevented;
		}
		set
		{
			_prevented = value;
		}
	}

	public ImpactResult result
	{
		get
		{
			return _result;
		}
		set
		{
			_result = value;
		}
	}

	public Vector3 origin
	{
		get
		{
			return _origin;
		}
		set
		{
			_origin = value;
		}
	}

	public Vector3 velocity
	{
		get
		{
			return _velocity;
		}
		set
		{
			_velocity = value;
		}
	}

	public float force
	{
		get
		{
			return Mathf.Clamp(_force, 0f, float.MaxValue);
		}
		set
		{
			_force = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float surfaceArea
	{
		get
		{
			return Mathf.Clamp(surfaceArea, 0f, float.MaxValue);
		}
		set
		{
			_surfaceArea = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float magnitude
	{
		get
		{
			return Mathf.Clamp(_magnitude, 0f, float.MaxValue);
		}
		set
		{
			_magnitude = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public ImpactApplicationData(ImpactSetupData oSetup)
	{
		requiredTags.AppendTagsFromContainer(oSetup.requiredTags);
		blockedByTags.AppendTagsFromContainer(oSetup.blockedByTags);
		effects = oSetup.effects;
	}
}

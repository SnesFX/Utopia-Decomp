using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImpactSetupData
{
	[SerializeField]
	private TagContainer _requiredTags = new TagContainer();

	[SerializeField]
	private TagContainer _blockedByTags = new TagContainer();

	[SerializeField]
	private List<ImpactEffectSetupData> _effects = new List<ImpactEffectSetupData>();

	public TagContainer requiredTags
	{
		get
		{
			return _requiredTags;
		}
	}

	public TagContainer blockedByTags
	{
		get
		{
			return _blockedByTags;
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
}

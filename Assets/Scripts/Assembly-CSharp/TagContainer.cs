using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TagContainer
{
	[SerializeField]
	private List<string> _tags = new List<string>();

	public List<string> tags
	{
		get
		{
			return _tags;
		}
	}

	public bool ContainsTag(string sTag)
	{
		return _tags.Contains(sTag);
	}

	public int GetTagQuantity(string sTag)
	{
		return _tags.FindAll((string o) => o == sTag).Count;
	}

	public bool AnyTagsMatch(TagContainer oContainer)
	{
		foreach (string tag in oContainer.tags)
		{
			if (ContainsTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool AllTagsMatch(TagContainer oContainer)
	{
		foreach (string tag in oContainer.tags)
		{
			if (!ContainsTag(tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool NoTagsMatch(TagContainer oContainer)
	{
		foreach (string tag in oContainer.tags)
		{
			if (ContainsTag(tag))
			{
				return false;
			}
		}
		return true;
	}

	public void AddTag(string sTag)
	{
		_tags.Add(sTag);
	}

	public void AddUniqueTag(string sTag)
	{
		if (!_tags.Contains(sTag))
		{
			_tags.Add(sTag);
		}
	}

	public void ClearTags()
	{
		_tags.Clear();
	}

	public void RemoveTag(string sTag)
	{
		_tags.Remove(sTag);
	}

	public void RemoveAllOfTag(string sTag)
	{
		_tags.RemoveAll((string o) => o == sTag);
	}

	public void AppendTagsFromContainer(TagContainer oContainer)
	{
		for (int i = 0; i < oContainer.tags.Count; i++)
		{
			AddTag(oContainer.tags[i]);
		}
	}

	public void RemoveTagsFromContainer(TagContainer oContainer)
	{
		for (int i = 0; i < oContainer.tags.Count; i++)
		{
			RemoveAllOfTag(oContainer.tags[i]);
		}
	}
}

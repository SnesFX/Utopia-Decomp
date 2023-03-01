using System;
using UnityEngine;

[Serializable]
public class SpecialEffectHandler
{
	public enum SpawnMethod
	{
		Refresh = 0,
		Replace = 1,
		ReplaceAndDestroy = 2,
		NoReplace = 3
	}

	[SerializeField]
	private string _name = "Effect";

	[SerializeField]
	private SpawnMethod _spawnMethod = SpawnMethod.ReplaceAndDestroy;

	[SerializeField]
	private SpecialEffect _effectObject;

	private SpecialEffect _activeObject;

	[SerializeField]
	private Transform _effectSource;

	[SerializeField]
	private Vector3 _offsetPosition = Vector3.zero;

	[SerializeField]
	private Vector3 _offsetRotation = Vector3.zero;

	public SpawnMethod spawnMethod
	{
		get
		{
			return _spawnMethod;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
	}

	public SpecialEffect effectObject
	{
		get
		{
			return _effectObject;
		}
	}

	public SpecialEffect activeObject
	{
		get
		{
			return _activeObject;
		}
		private set
		{
			_activeObject = value;
		}
	}

	public Transform effectSource
	{
		get
		{
			return _effectSource;
		}
	}

	public Vector3 offsetPosition
	{
		get
		{
			return _offsetPosition;
		}
	}

	public Vector3 offsetRotation
	{
		get
		{
			return _offsetRotation;
		}
	}

	public SpecialEffect Spawn()
	{
		return Spawn(offsetPosition, Quaternion.Euler(offsetRotation));
	}

	public SpecialEffect Spawn(Vector3 position)
	{
		return Spawn(position, Quaternion.Euler(offsetRotation));
	}

	public SpecialEffect Spawn(Quaternion rotation)
	{
		return Spawn(offsetPosition, rotation);
	}

	public SpecialEffect Spawn(Vector3 position, Quaternion rotation)
	{
		if (!effectObject | !effectSource)
		{
			return null;
		}
		if ((bool)activeObject)
		{
			switch (spawnMethod)
			{
			case SpawnMethod.Refresh:
				if (activeObject.destroying)
				{
					activeObject = effectObject.Spawn(effectSource, position, rotation);
				}
				else
				{
					activeObject.Refresh();
				}
				break;
			case SpawnMethod.Replace:
				activeObject = effectObject.Spawn(effectSource, position, rotation);
				break;
			case SpawnMethod.ReplaceAndDestroy:
				activeObject.Destroy();
				activeObject = effectObject.Spawn(effectSource, position, rotation);
				break;
			case SpawnMethod.NoReplace:
				if (activeObject.durationTimer == 0f)
				{
					activeObject.Destroy();
					activeObject = effectObject.Spawn(effectSource, position, rotation);
				}
				break;
			}
		}
		else
		{
			activeObject = effectObject.Spawn(effectSource, position, rotation);
		}
		return activeObject;
	}

	public void Refresh()
	{
		if ((bool)activeObject)
		{
			activeObject.Refresh();
		}
	}

	public void Destroy()
	{
		if ((bool)activeObject)
		{
			activeObject.Destroy();
		}
	}
}

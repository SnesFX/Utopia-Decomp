using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class User : Controller
{
	[SerializeField]
	private UserInterface _userInterface;

	[SerializeField]
	private CameraController _possessedCamera;

	public UserInterface userInterface
	{
		get
		{
			return _userInterface;
		}
		set
		{
			_userInterface = value;
		}
	}

	public CameraController possessedCamera
	{
		get
		{
			return _possessedCamera;
		}
		set
		{
			_possessedCamera = value;
		}
	}

	private void Awake()
	{
		if ((bool)base.possessedPawn)
		{
			Possess(base.possessedPawn);
		}
	}
}

using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class UserInterface : MonoBehaviour
{
	[SerializeField]
	private User _user;

	[SerializeField]
	private bool _mouselookDefault = true;

	[SerializeField]
	private string _mouseToggle = string.Empty;

	[SerializeField]
	private string _mouseX = "Mouse X";

	[SerializeField]
	private string _mouseY = "Mouse Y";

	[SerializeField]
	private string _mouseScrollWheel = "Mouse ScrollWheel";

	[SerializeField]
	private Vector2 _mouseSensitivity = Vector2.one;

	[SerializeField]
	private bool _invertMouseX;

	[SerializeField]
	private bool _invertMouseY;

	public User user
	{
		get
		{
			return _user;
		}
		set
		{
			_user = value;
		}
	}

	public bool mouselookDefault
	{
		get
		{
			return _mouselookDefault;
		}
		set
		{
			_mouselookDefault = value;
		}
	}

	public string mouseToggle
	{
		get
		{
			return _mouseToggle;
		}
		set
		{
			_mouseToggle = value;
		}
	}

	public string mouseX
	{
		get
		{
			return _mouseX;
		}
		set
		{
			_mouseX = value;
		}
	}

	public string mouseY
	{
		get
		{
			return _mouseY;
		}
		set
		{
			_mouseY = value;
		}
	}

	public string mouseScrollWheel
	{
		get
		{
			return _mouseScrollWheel;
		}
		set
		{
			_mouseScrollWheel = value;
		}
	}

	public Vector2 mouseSensitivity
	{
		get
		{
			return _mouseSensitivity;
		}
		set
		{
			_mouseSensitivity = value;
		}
	}

	public bool invertMouseX
	{
		get
		{
			return _invertMouseX;
		}
		set
		{
			_invertMouseX = value;
		}
	}

	public bool invertMouseY
	{
		get
		{
			return _invertMouseY;
		}
		set
		{
			_invertMouseY = value;
		}
	}

	private void Start()
	{
		if (mouselookDefault)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (mouseToggle != string.Empty && Input.GetButtonDown(mouseToggle))
		{
			if (Cursor.lockState == CursorLockMode.None)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else if (Cursor.lockState == CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}
}

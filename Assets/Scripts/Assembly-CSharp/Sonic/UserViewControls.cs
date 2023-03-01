using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonic
{
	[Serializable]
	public class UserViewControls : MonoBehaviour
	{
		private User _user;

		private UserInterface _userInterface;

		[SerializeField]
		private bool _useMouseInput = true;

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

		[SerializeField]
		private string _pitch = "Mouse Y";

		[SerializeField]
		private string _yaw = "Mouse X";

		[SerializeField]
		private string _zoom = "Mouse ScrollWheel";

		[SerializeField]
		private Vector3 _viewSensitivity = Vector3.one;

		[SerializeField]
		private bool _invertPitch;

		[SerializeField]
		private bool _invertYaw;

		[SerializeField]
		private bool _invertZoom;

		public User user
		{
			get
			{
				return (!_user) ? (_user = GetComponent<User>()) : _user;
			}
		}

		public UserInterface userInterface
		{
			get
			{
				return (!_userInterface) ? (_userInterface = GetComponent<UserInterface>()) : _userInterface;
			}
		}

		public bool useMouseInput
		{
			get
			{
				return _useMouseInput;
			}
			set
			{
				_useMouseInput = value;
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

		public Vector3 viewSensitivity
		{
			get
			{
				return _viewSensitivity;
			}
			set
			{
				_viewSensitivity = value;
			}
		}

		public string pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
			}
		}

		public string yaw
		{
			get
			{
				return _yaw;
			}
			set
			{
				_yaw = value;
			}
		}

		public string zoom
		{
			get
			{
				return _zoom;
			}
			set
			{
				_zoom = value;
			}
		}

		public bool invertPitch
		{
			get
			{
				return _invertPitch;
			}
			set
			{
				_invertPitch = value;
			}
		}

		public bool invertYaw
		{
			get
			{
				return _invertYaw;
			}
			set
			{
				_invertYaw = value;
			}
		}

		public bool invertZoom
		{
			get
			{
				return _invertZoom;
			}
			set
			{
				_invertZoom = value;
			}
		}

		private void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			CameraController cameraController = null;
			Quaternion identity = Quaternion.identity;
			User user = this.user;
			if (!user)
			{
				user = userInterface.user;
			}
			if ((bool)user)
			{
				cameraController = user.possessedCamera;
			}
			if (!cameraController)
			{
				return;
			}
			identity = cameraController.viewRotation;
			Vector2 zero = Vector2.zero;
			Vector2 viewInput = Vector3.zero;
			float num = 0f;
			if (pitch != string.Empty)
			{
				viewInput.x += Input.GetAxis(pitch) * ((!invertPitch) ? 1f : (-1f)) * viewSensitivity.x;
			}
			if (yaw != string.Empty)
			{
				viewInput.y += Input.GetAxis(yaw) * ((!invertYaw) ? 1f : (-1f)) * viewSensitivity.y;
			}
			if (zoom != string.Empty)
			{
				num += Input.GetAxis(zoom) * ((!invertZoom) ? 1f : (-1f)) * viewSensitivity.z;
			}
			if (useMouseInput && Cursor.lockState == CursorLockMode.Locked)
			{
				if (mouseX != string.Empty)
				{
					zero.y += Input.GetAxis(mouseX) * ((!invertMouseX) ? 1f : (-1f)) * mouseSensitivity.x;
				}
				if (mouseY != string.Empty)
				{
					zero.x += Input.GetAxis(mouseY) * ((!invertMouseY) ? 1f : (-1f)) * mouseSensitivity.y;
				}
				if (mouseScrollWheel != string.Empty)
				{
					num += Input.GetAxis(mouseScrollWheel) * ((!invertZoom) ? 1f : (-1f));
				}
			}
			else if ((EventSystem.current == null) | ((bool)EventSystem.current && !EventSystem.current.IsPointerOverGameObject()))
			{
				num += ((!(_mouseScrollWheel == string.Empty)) ? (Input.GetAxis(mouseScrollWheel) * ((!invertZoom) ? 1f : (-1f))) : 0f);
			}
			if ((bool)cameraController.state)
			{
				cameraController.mouseInput = zero;
				cameraController.viewInput = viewInput;
				cameraController.zoomInput = num;
			}
		}
	}
}

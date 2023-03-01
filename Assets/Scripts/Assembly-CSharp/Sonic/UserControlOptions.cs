using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	[RequireComponent(typeof(User))]
	public class UserControlOptions : MonoBehaviour
	{
		private User _user;

		[SerializeField]
		private bool _invertMouseX;

		[SerializeField]
		private bool _invertMouseY;

		[SerializeField]
		private float _mouseViewSensitivity = 0.1f;

		[SerializeField]
		private float _zoomSensitivity = 1f;

		[SerializeField]
		private bool _invertViewPitch;

		[SerializeField]
		private bool _invertViewYaw;

		[SerializeField]
		private bool _invertViewZoom;

		[SerializeField]
		private float _joystickViewSensitivity = 0.1f;

		public User user
		{
			get
			{
				return (!_user) ? (_user = GetComponent<User>()) : _user;
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

		public float mouseViewSensitivity
		{
			get
			{
				return _mouseViewSensitivity;
			}
			set
			{
				mouseViewSensitivity = value;
			}
		}

		public float zoomSensitivity
		{
			get
			{
				return _zoomSensitivity;
			}
			set
			{
				_zoomSensitivity = value;
			}
		}

		public bool invertViewPitch
		{
			get
			{
				return _invertViewPitch;
			}
			set
			{
				_invertViewPitch = value;
			}
		}

		public bool invertViewYaw
		{
			get
			{
				return _invertViewYaw;
			}
			set
			{
				_invertViewYaw = value;
			}
		}

		public bool invertViewZoom
		{
			get
			{
				return _invertViewZoom;
			}
			set
			{
				_invertViewZoom = value;
			}
		}

		public float joystickViewSensitivity
		{
			get
			{
				return _joystickViewSensitivity;
			}
			set
			{
				joystickViewSensitivity = value;
			}
		}

		public void ApplyOptions()
		{
			if ((bool)user && (bool)user.userInterface)
			{
				UserViewControls component = user.userInterface.GetComponent<UserViewControls>();
				if ((bool)component)
				{
					component.invertMouseX = invertMouseX;
					component.invertMouseY = invertMouseY;
					component.mouseSensitivity = new Vector2(mouseViewSensitivity, mouseViewSensitivity);
					component.invertPitch = invertViewPitch;
					component.invertYaw = invertViewYaw;
					component.invertZoom = invertViewZoom;
					component.mouseSensitivity = new Vector3(joystickViewSensitivity, joystickViewSensitivity, zoomSensitivity);
				}
				UserCharacterControls component2 = user.userInterface.GetComponent<UserCharacterControls>();
			}
		}

		private void Start()
		{
			ApplyOptions();
		}
	}
}

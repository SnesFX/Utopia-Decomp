using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class CameraController : Actor
{
	[SerializeField]
	private Actor _targetActor;

	[SerializeField]
	private CameraState _state;

	private Camera _displayCamera;

	private Vector3 _viewPosition = new Vector3(0f, 0f, 3.5f);

	private Quaternion _viewRotation = Quaternion.identity;

	private Vector2 _mouseInput = Vector2.zero;

	private Vector2 _viewInput = Vector2.zero;

	private float _zoomInput;

	public Actor targetActor
	{
		get
		{
			return _targetActor;
		}
		set
		{
			_targetActor = value;
		}
	}

	public CameraState state
	{
		get
		{
			return _state;
		}
		protected set
		{
			state = value;
		}
	}

	public Camera displayCamera
	{
		get
		{
			if (!_displayCamera)
			{
				_displayCamera = GetComponentInChildren<Camera>();
			}
			return _displayCamera;
		}
	}

	public Vector3 viewPosition
	{
		get
		{
			return _viewPosition;
		}
		set
		{
			_viewPosition = value;
		}
	}

	public Quaternion viewRotation
	{
		get
		{
			return _viewRotation;
		}
		set
		{
			_viewRotation = value;
		}
	}

	public Vector2 mouseInput
	{
		get
		{
			return _mouseInput;
		}
		set
		{
			_mouseInput = value;
		}
	}

	public Vector2 viewInput
	{
		get
		{
			return _viewInput;
		}
		set
		{
			_viewInput = value;
		}
	}

	public float zoomInput
	{
		get
		{
			return _zoomInput;
		}
		set
		{
			_zoomInput = value;
		}
	}

	public Vector3[] GetClipPoints()
	{
		Vector3[] array = new Vector3[4];
		if ((bool)displayCamera)
		{
			float nearClipPlane = displayCamera.nearClipPlane;
			float num = nearClipPlane * Mathf.Tan(displayCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
			float num2 = num * displayCamera.aspect;
			array[0] = new Vector3(0f - num2, num, 0f);
			array[1] = new Vector3(num2, num, 0f);
			array[2] = new Vector3(0f - num2, 0f - num, 0f);
			array[3] = new Vector3(num2, 0f - num, 0f);
		}
		return array;
	}

	public void ActivateState(CameraState oState)
	{
		if ((bool)oState && oState.gameObject == base.gameObject)
		{
			if ((bool)state)
			{
				state.OnStateExit();
			}
			state = oState;
			state.OnStateEnter();
		}
	}

	public void ActivateState<T>() where T : CameraState
	{
		CameraState component = GetComponent<T>();
		if ((bool)component)
		{
			if ((bool)state)
			{
				state.OnStateExit();
			}
			state = component;
			state.OnStateEnter();
		}
	}

	public T AddState<T>() where T : CameraState
	{
		return AddComponent<T>();
	}

	public T AddOrActivateState<T>() where T : CameraState
	{
		T val = GetComponent<T>();
		if (!(UnityEngine.Object)val)
		{
			val = AddState<T>();
		}
		ActivateState(val);
		return val;
	}

	private void Awake()
	{
		viewPosition = base.transform.position;
		viewRotation = base.transform.rotation;
	}

	private void Update()
	{
		if ((bool)state)
		{
			state.OnStateUpdate();
		}
	}
}

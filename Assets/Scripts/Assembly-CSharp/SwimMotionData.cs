using UnityEngine;

public class SwimMotionData
{
	private Vector3 _desiredDrive = Vector3.zero;

	private Vector3 _desiredFacing = Vector3.zero;

	private float _topSpeed = 8.6f;

	private float _speed = 6.4f;

	private float _acceleration = 4.905f;

	private float _deceleration = 9.81f;

	private float _brakePower = 35f;

	private float _handling;

	private float _yawSpeed = 360f;

	private float _pitchRollSpeed = 720f;

	private float _brakeThreshold = -0.707f;

	public Vector3 desiredDrive
	{
		get
		{
			return _desiredDrive;
		}
		set
		{
			_desiredDrive = value;
		}
	}

	public Vector3 desiredFacing
	{
		get
		{
			return _desiredFacing;
		}
		set
		{
			_desiredFacing = value;
		}
	}

	public float topSpeed
	{
		get
		{
			return Mathf.Clamp(_topSpeed, 0f, float.MaxValue);
		}
		set
		{
			_topSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float speed
	{
		get
		{
			return Mathf.Clamp(_speed, 0f, float.MaxValue);
		}
		set
		{
			_speed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float acceleration
	{
		get
		{
			return Mathf.Clamp(_acceleration, 0f, float.MaxValue);
		}
		set
		{
			_acceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float deceleration
	{
		get
		{
			return Mathf.Clamp(_deceleration, 0f, float.MaxValue);
		}
		set
		{
			_deceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float brakePower
	{
		get
		{
			return Mathf.Clamp(_brakePower, 0f, float.MaxValue);
		}
		set
		{
			_brakePower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float handling
	{
		get
		{
			return Mathf.Clamp(_handling, 0f, float.MaxValue);
		}
		set
		{
			_handling = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float yawSpeed
	{
		get
		{
			return Mathf.Clamp(_yawSpeed, 0f, float.MaxValue);
		}
		set
		{
			_yawSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float pitchRollSpeed
	{
		get
		{
			return Mathf.Clamp(_pitchRollSpeed, 0f, float.MaxValue);
		}
		set
		{
			_pitchRollSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float brakeThreshold
	{
		get
		{
			return Mathf.Clamp(_brakeThreshold, -1f, 0f);
		}
		set
		{
			_brakeThreshold = Mathf.Clamp(value, -1f, 0f);
		}
	}
}

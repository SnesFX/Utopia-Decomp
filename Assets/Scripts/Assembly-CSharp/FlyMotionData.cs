using UnityEngine;

public class FlyMotionData
{
	private Vector3 _desiredDrive = Vector3.zero;

	private Vector3 _desiredAngle = Vector3.zero;

	private float _antigravityIntrinsic = 9.81f;

	private float _antigravityPower = 1f;

	private float _antigravityMax = float.MaxValue;

	private float _topSpeed = 8.6f;

	private float _speed = 6.4f;

	private float _acceleration = 4.905f;

	private float _deceleration = 9.81f;

	private float _handling;

	private float _pitchSpeed = 360f;

	private float _yawSpeed = 360f;

	private float _rollSpeed = 360f;

	private float _strength;

	private float _gravityPowerBoost = 1f;

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

	public Vector3 desiredAngle
	{
		get
		{
			return _desiredAngle;
		}
		set
		{
			_desiredAngle = value;
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

	public float pitchSpeed
	{
		get
		{
			return Mathf.Clamp(_pitchSpeed, 0f, float.MaxValue);
		}
		set
		{
			_pitchSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
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

	public float rollSpeed
	{
		get
		{
			return Mathf.Clamp(_rollSpeed, 0f, float.MaxValue);
		}
		set
		{
			_rollSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float strength
	{
		get
		{
			return Mathf.Clamp(_strength, 0f, float.MaxValue);
		}
		set
		{
			_strength = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float gravityPowerBoost
	{
		get
		{
			return Mathf.Clamp(_gravityPowerBoost, 0f, float.MaxValue);
		}
		set
		{
			_gravityPowerBoost = Mathf.Clamp(value, 0f, float.MaxValue);
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

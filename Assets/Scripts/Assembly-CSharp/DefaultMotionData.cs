using UnityEngine;

public class DefaultMotionData
{
	private Vector3 _desiredDrive = Vector3.zero;

	private Vector3 _desiredAngle = Vector3.zero;

	private float _freeTopSpeed = 8.6f;

	private float _freeSpeed = 6.4f;

	private float _freeAcceleration = 4.905f;

	private float _freeDeceleration = 9.81f;

	private float _freeBrakePower = 35f;

	private float _freeHandling;

	private float _freeYawSpeed = 360f;

	private float _freeYawAcceleration = 720f;

	private float _freePitchRollSpeed = 720f;

	private float _groundTopSpeed = 8.6f;

	private float _groundSpeed = 6.4f;

	private float _groundAcceleration = 4.905f;

	private float _groundDeceleration = 9.81f;

	private float _groundBrakePower = 9.81f;

	private float _groundHandling;

	private float _groundYawSpeed = 360f;

	private float _groundYawAcceleration = 720f;

	private float _groundPitchRollSpeed = 720f;

	private float _friction = 1f;

	private float _strength;

	private float _downforceIntrinsic;

	private float _downforcePower;

	private float _downforceMax;

	private float _tractionIntrinsic;

	private float _tractionPower;

	private float _tractionMax;

	private float _buoyancyIntrinsic;

	private float _buoyancyPower;

	private float _buoyancyMax;

	private float _gravityPowerBoost = 1f;

	private float _freeBrakeThreshold = -0.707f;

	private float _groundBrakeThreshold = -0.707f;

	private float _groundNormalAdherence;

	private bool _adhereToGroundMotion = true;

	private bool _adhereToGroundHeight = true;

	private bool _faceBrakeDirection = true;

	private bool _steeringBasedFacing = true;

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

	public float freeTopSpeed
	{
		get
		{
			return Mathf.Clamp(_freeTopSpeed, 0f, float.MaxValue);
		}
		set
		{
			_freeTopSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeSpeed
	{
		get
		{
			return Mathf.Clamp(_freeSpeed, 0f, float.MaxValue);
		}
		set
		{
			_freeSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeAcceleration
	{
		get
		{
			return Mathf.Clamp(_freeAcceleration, 0f, float.MaxValue);
		}
		set
		{
			_freeAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeDeceleration
	{
		get
		{
			return Mathf.Clamp(_freeDeceleration, 0f, float.MaxValue);
		}
		set
		{
			_freeDeceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeBrakePower
	{
		get
		{
			return Mathf.Clamp(_freeBrakePower, 0f, float.MaxValue);
		}
		set
		{
			_freeBrakePower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeHandling
	{
		get
		{
			return Mathf.Clamp(_freeHandling, 0f, float.MaxValue);
		}
		set
		{
			_freeHandling = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeYawSpeed
	{
		get
		{
			return Mathf.Clamp(_freeYawSpeed, 0f, float.MaxValue);
		}
		set
		{
			_freeYawSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freeYawAcceleration
	{
		get
		{
			return Mathf.Clamp(_freeYawAcceleration, 0f, float.MaxValue);
		}
		set
		{
			_freeYawAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float freePitchRollSpeed
	{
		get
		{
			return Mathf.Clamp(_freePitchRollSpeed, 0f, float.MaxValue);
		}
		set
		{
			_freePitchRollSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundTopSpeed
	{
		get
		{
			return Mathf.Clamp(_groundTopSpeed, 0f, float.MaxValue);
		}
		set
		{
			_groundTopSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundSpeed
	{
		get
		{
			return Mathf.Clamp(_groundSpeed, 0f, float.MaxValue);
		}
		set
		{
			_groundSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundAcceleration
	{
		get
		{
			return Mathf.Clamp(_groundAcceleration, 0f, float.MaxValue);
		}
		set
		{
			_groundAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundDeceleration
	{
		get
		{
			return Mathf.Clamp(_groundDeceleration, 0f, float.MaxValue);
		}
		set
		{
			_groundDeceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundBrakePower
	{
		get
		{
			return Mathf.Clamp(_groundBrakePower, 0f, float.MaxValue);
		}
		set
		{
			_groundBrakePower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundHandling
	{
		get
		{
			return Mathf.Clamp(_groundHandling, 0f, float.MaxValue);
		}
		set
		{
			_groundHandling = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundYawSpeed
	{
		get
		{
			return Mathf.Clamp(_groundYawSpeed, 0f, float.MaxValue);
		}
		set
		{
			_groundYawSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundYawAcceleration
	{
		get
		{
			return Mathf.Clamp(_groundYawAcceleration, 0f, float.MaxValue);
		}
		set
		{
			_groundYawAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float groundPitchRollSpeed
	{
		get
		{
			return Mathf.Clamp(_groundPitchRollSpeed, 0f, float.MaxValue);
		}
		set
		{
			_groundPitchRollSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float friction
	{
		get
		{
			return Mathf.Clamp(_friction, 0f, float.MaxValue);
		}
		set
		{
			_friction = Mathf.Clamp(value, 0f, float.MaxValue);
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

	public float downforceIntrinsic
	{
		get
		{
			return Mathf.Clamp(_downforceIntrinsic, 0f, float.MaxValue);
		}
		set
		{
			_downforceIntrinsic = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float downforcePower
	{
		get
		{
			return Mathf.Clamp(_downforcePower, 0f, float.MaxValue);
		}
		set
		{
			_downforcePower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float downforceMax
	{
		get
		{
			return Mathf.Clamp(_downforceMax, 0f, float.MaxValue);
		}
		set
		{
			_downforceMax = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float tractionIntrinsic
	{
		get
		{
			return Mathf.Clamp(_tractionIntrinsic, 0f, float.MaxValue);
		}
		set
		{
			_tractionIntrinsic = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float tractionPower
	{
		get
		{
			return Mathf.Clamp(_tractionPower, 0f, float.MaxValue);
		}
		set
		{
			_tractionPower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float tractionMax
	{
		get
		{
			return Mathf.Clamp(_tractionMax, 0f, float.MaxValue);
		}
		set
		{
			_tractionMax = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float buoyancyIntrinsic
	{
		get
		{
			return Mathf.Clamp(_buoyancyIntrinsic, 0f, float.MaxValue);
		}
		set
		{
			_buoyancyIntrinsic = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float buoyancyPower
	{
		get
		{
			return Mathf.Clamp(_buoyancyPower, 0f, float.MaxValue);
		}
		set
		{
			_buoyancyPower = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public float buoyancyMax
	{
		get
		{
			return Mathf.Clamp(_tractionMax, 0f, float.MaxValue);
		}
		set
		{
			_tractionMax = Mathf.Clamp(value, 0f, float.MaxValue);
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

	public float freeBrakeThreshold
	{
		get
		{
			return Mathf.Clamp(_freeBrakeThreshold, -1f, 0f);
		}
		set
		{
			_freeBrakeThreshold = Mathf.Clamp(value, -1f, 0f);
		}
	}

	public float groundBrakeThreshold
	{
		get
		{
			return Mathf.Clamp(_groundBrakeThreshold, -1f, 0f);
		}
		set
		{
			_groundBrakeThreshold = Mathf.Clamp(value, -1f, 0f);
		}
	}

	public float groundNormalAdherence
	{
		get
		{
			return Mathf.Clamp01(_groundNormalAdherence);
		}
		set
		{
			_groundNormalAdherence = Mathf.Clamp01(value);
		}
	}

	public bool adhereToGroundMotion
	{
		get
		{
			return _adhereToGroundMotion;
		}
		set
		{
			_adhereToGroundMotion = value;
		}
	}

	public bool adhereToGroundHeight
	{
		get
		{
			return _adhereToGroundHeight;
		}
		set
		{
			_adhereToGroundHeight = value;
		}
	}

	public bool faceBrakeDirection
	{
		get
		{
			return _faceBrakeDirection;
		}
		set
		{
			_faceBrakeDirection = value;
		}
	}

	public bool steeringBasedFacing
	{
		get
		{
			return _steeringBasedFacing;
		}
		set
		{
			_steeringBasedFacing = value;
		}
	}
}

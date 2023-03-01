using System;
using UnityEngine;

[Serializable]
public class CameraStateThirdPerson : CameraState
{
	public enum OrientationMode
	{
		GlobalUp = 0,
		GravityUp = 1,
		TargetUp = 2,
		TargetUpNoRoll = 3
	}

	public enum PitchInputMode
	{
		Set = 0,
		Influence = 1
	}

	[SerializeField]
	private LayerMask _collisionLayers = default(LayerMask);

	[SerializeField]
	private LayerMask _portalLayers = default(LayerMask);

	private Quaternion _desiredRotation = Quaternion.identity;

	private Vector3 _desiredPosition = Vector3.forward * -3.5f;

	[SerializeField]
	private Vector3 _sensitivity = Vector3.one;

	[SerializeField]
	private Vector3 _targetOffset = Vector3.zero;

	[SerializeField]
	private float _minimumPitch = -45f;

	[SerializeField]
	private float _maximumPitch = 75f;

	[SerializeField]
	private float _minimumZoom = 0.5f;

	[SerializeField]
	private float _maximumZoom = 7.5f;

	[SerializeField]
	private float _desiredZoom = 4f;

	[SerializeField]
	private float _zoomSpeed = 10f;

	[SerializeField]
	private OrientationMode _orientationMode = OrientationMode.TargetUp;

	[SerializeField]
	private PitchInputMode _pitchInputMode = PitchInputMode.Influence;

	[SerializeField]
	[Range(0f, 1f)]
	private float _orientationSlerp = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float _rotationSlerp = 1f;

	[SerializeField]
	private bool _chase;

	[SerializeField]
	private bool _chaseToPitch = true;

	[SerializeField]
	private bool _chaseToPitchReverse = true;

	[SerializeField]
	private float _chasePitch = 17.5f;

	[SerializeField]
	private float _minimumChasePitch = -90f;

	[SerializeField]
	private float _maximumChasePitch = 90f;

	[SerializeField]
	private float _chaseSpeed = 90f;

	[SerializeField]
	private float _inputChaseDelay = 5f;

	[SerializeField]
	private AnimationCurve _chaseDelayCurve = new AnimationCurve();

	private Vector3 _upVector = Vector3.up;

	private Vector3 _lastUpVector = Vector3.up;

	private Vector3 _lastTargetPosition = Vector3.zero;

	private float _chaseDelayTimer;

	private bool _mouseMode;

	public LayerMask collisionLayers
	{
		get
		{
			return _collisionLayers;
		}
		set
		{
			_collisionLayers = value;
		}
	}

	public LayerMask portalLayers
	{
		get
		{
			return _portalLayers;
		}
		set
		{
			_portalLayers = value;
		}
	}

	public Quaternion desiredRotation
	{
		get
		{
			return _desiredRotation;
		}
		set
		{
			_desiredRotation = value;
		}
	}

	public Vector3 desiredPosition
	{
		get
		{
			return _desiredPosition;
		}
		set
		{
			_desiredPosition = value;
		}
	}

	public Vector3 sensitivity
	{
		get
		{
			return _sensitivity;
		}
		set
		{
			_sensitivity = value;
		}
	}

	public Vector3 targetOffset
	{
		get
		{
			return _targetOffset;
		}
		set
		{
			_targetOffset = value;
		}
	}

	public float minimumPitch
	{
		get
		{
			return Mathf.Clamp(_minimumPitch, -90f, 90f);
		}
		set
		{
			_minimumPitch = Mathf.Clamp(value, -90f, 90f);
		}
	}

	public float maximumPitch
	{
		get
		{
			return Mathf.Clamp(_maximumPitch, -90f, 90f);
		}
		set
		{
			_maximumPitch = Mathf.Clamp(value, -90f, 90f);
		}
	}

	public float minimumZoom
	{
		get
		{
			return Mathf.Clamp(_minimumZoom, 0f, float.PositiveInfinity);
		}
		set
		{
			_minimumZoom = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
	}

	public float maximumZoom
	{
		get
		{
			return Mathf.Clamp(_maximumZoom, 0f, float.PositiveInfinity);
		}
		set
		{
			_maximumZoom = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
	}

	public float desiredZoom
	{
		get
		{
			return Mathf.Clamp(_desiredZoom, 0f, float.PositiveInfinity);
		}
		set
		{
			_desiredZoom = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
	}

	public float zoomSpeed
	{
		get
		{
			return Mathf.Clamp(_zoomSpeed, 0f, float.PositiveInfinity);
		}
		set
		{
			_zoomSpeed = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
	}

	public OrientationMode orientationMode
	{
		get
		{
			return _orientationMode;
		}
		set
		{
			_orientationMode = value;
		}
	}

	public PitchInputMode pitchInputMode
	{
		get
		{
			return _pitchInputMode;
		}
		set
		{
			_pitchInputMode = value;
		}
	}

	public float orientationSlerp
	{
		get
		{
			return _orientationSlerp;
		}
		set
		{
			_orientationSlerp = value;
		}
	}

	public float rotationSlerp
	{
		get
		{
			return _rotationSlerp;
		}
		set
		{
			_rotationSlerp = value;
		}
	}

	public bool chase
	{
		get
		{
			return _chase;
		}
		set
		{
			_chase = value;
		}
	}

	public bool chaseToPitch
	{
		get
		{
			return _chaseToPitch;
		}
		set
		{
			_chaseToPitch = value;
		}
	}

	public bool chaseToPitchReverse
	{
		get
		{
			return _chaseToPitchReverse;
		}
		set
		{
			_chaseToPitchReverse = value;
		}
	}

	public float chasePitch
	{
		get
		{
			return _chasePitch;
		}
		set
		{
			_chasePitch = value;
		}
	}

	public float chaseSpeed
	{
		get
		{
			return _chaseSpeed;
		}
		set
		{
			_chaseSpeed = value;
		}
	}

	public float minimumChasePitch
	{
		get
		{
			return Mathf.Clamp(_minimumChasePitch, minimumPitch, 90f);
		}
		set
		{
			_minimumChasePitch = Mathf.Clamp(value, minimumPitch, 90f);
		}
	}

	public float maximumChasePitch
	{
		get
		{
			return Mathf.Clamp(_maximumChasePitch, -90f, maximumPitch);
		}
		set
		{
			_maximumChasePitch = Mathf.Clamp(value, -90f, maximumPitch);
		}
	}

	public float inputChaseDelay
	{
		get
		{
			return Mathf.Clamp(_inputChaseDelay, 0f, float.MaxValue);
		}
		set
		{
			_inputChaseDelay = Mathf.Clamp(value, 0f, float.MaxValue);
		}
	}

	public AnimationCurve chaseDelayCurve
	{
		get
		{
			return _chaseDelayCurve;
		}
		set
		{
			_chaseDelayCurve = value;
		}
	}

	public bool mouseMode
	{
		get
		{
			return _mouseMode;
		}
		protected set
		{
			_mouseMode = value;
		}
	}

	public float GetZoomDistance(Quaternion qRotation, Vector3 vTargetPos, Vector3 vPosition)
	{
		Vector3 direction = vPosition - vTargetPos;
		float result = direction.magnitude;
		if ((bool)base.cameraController && (bool)base.cameraController.displayCamera)
		{
			Vector3[] clipPoints = base.cameraController.GetClipPoints();
			bool flag = true;
			RaycastHit hitInfo;
			for (int i = 0; i < 4; i++)
			{
				if (Physics.Raycast(vTargetPos, qRotation * clipPoints[i], out hitInfo, clipPoints[i].magnitude, collisionLayers))
				{
					flag = false;
					result = 0f;
				}
			}
			if (flag && Physics.SphereCast(vTargetPos, clipPoints[0].magnitude, direction, out hitInfo, direction.magnitude, collisionLayers))
			{
				result = hitInfo.distance + base.cameraController.displayCamera.nearClipPlane;
			}
		}
		return result;
	}

	private void Awake()
	{
		desiredPosition = base.transform.position;
		desiredRotation = base.transform.rotation;
	}

	public override void OnStateUpdate()
	{
		CameraController cameraController = base.cameraController;
		if (!cameraController || !cameraController.targetActor)
		{
			return;
		}
		float num = Time.smoothDeltaTime / Time.fixedDeltaTime;
		Vector2 vector = new Vector2(cameraController.mouseInput.x * sensitivity.x, cameraController.mouseInput.y * sensitivity.y);
		Vector2 vector2 = new Vector2(cameraController.viewInput.x * sensitivity.x, cameraController.viewInput.y * sensitivity.y);
		float num2 = cameraController.zoomInput * sensitivity.z;
		Vector3 position = cameraController.targetActor.transform.position;
		position += cameraController.targetActor.transform.up * targetOffset.y;
		position += desiredRotation * Vector3.right * targetOffset.x;
		Vector3 lhs = position - _lastTargetPosition;
		Vector3 vector3 = ((!cameraController.targetActor.physics) ? Vector3.up : cameraController.targetActor.physics.up);
		switch (orientationMode)
		{
		case OrientationMode.GlobalUp:
			vector3 = Vector3.up;
			break;
		case OrientationMode.TargetUp:
			vector3 = cameraController.targetActor.transform.up;
			break;
		case OrientationMode.TargetUpNoRoll:
			vector3 = Utility.RelativeVector(cameraController.targetActor.transform.up, Utility.RelativeVector(cameraController.viewRotation * Vector3.right, vector3).normalized).normalized;
			break;
		}
		vector3 = Vector3.Slerp(_lastUpVector, vector3, orientationSlerp);
		Quaternion quaternion = Quaternion.FromToRotation(_lastUpVector, vector3);
		Quaternion quaternion2 = desiredRotation;
		Quaternion quaternion3 = quaternion * desiredRotation;
		Vector3 vector4 = quaternion * (desiredPosition - _lastTargetPosition);
		Quaternion quaternion4 = Quaternion.FromToRotation(quaternion3 * Vector3.up, vector3);
		Vector3 vector5 = quaternion4 * (quaternion3 * Vector3.forward);
		Quaternion quaternion5 = Quaternion.LookRotation(Utility.RelativeVector(vector5, vector3), vector3);
		Vector3 eulerAngles = (Quaternion.Inverse(quaternion5) * quaternion3).eulerAngles;
		Vector3 vector6 = ((!cameraController.targetActor.physics) ? Vector3.zero : cameraController.targetActor.physics.velocity);
		Vector3 vector7 = Utility.RelativeVector(vector6, vector3);
		eulerAngles.x = ((!(eulerAngles.x > 180f)) ? eulerAngles.x : (eulerAngles.x - 360f));
		eulerAngles.y = ((!(eulerAngles.y > 180f)) ? eulerAngles.y : (eulerAngles.y - 360f));
		if (_chaseDelayTimer > 0f)
		{
			_chaseDelayTimer = Mathf.Clamp(_chaseDelayTimer - Time.deltaTime, 0f, inputChaseDelay);
			if (_chaseDelayTimer == 0f)
			{
				_mouseMode = false;
			}
		}
		if ((vector2.magnitude > 0f) | (vector.magnitude > 0f))
		{
			_chaseDelayTimer = inputChaseDelay;
			if (vector.magnitude > 0f)
			{
				_mouseMode = true;
			}
			else if (vector2.magnitude > 0f)
			{
				_mouseMode = false;
			}
		}
		Vector3 fromDirection = Quaternion.Inverse(quaternion3) * (_lastTargetPosition - cameraController.transform.position);
		Vector3 toDirection = Quaternion.Inverse(quaternion3) * (position - cameraController.transform.position);
		Vector3 vector8 = Vector3.zero;
		Vector3 zero = Vector3.zero;
		Quaternion quaternion6 = Quaternion.FromToRotation(fromDirection, toDirection);
		Vector3 axis = Vector3.zero;
		float angle = 0f;
		float num3 = ((!((inputChaseDelay > 0f) & (_chaseDelayTimer > 0f))) ? 1f : chaseDelayCurve.Evaluate(1f - _chaseDelayTimer / inputChaseDelay));
		if (chase)
		{
			quaternion6.ToAngleAxis(out angle, out axis);
			vector8 = Quaternion.AngleAxis(angle, axis).eulerAngles;
			vector8.x = ((!(vector8.x > 180f)) ? vector8.x : (vector8.x - 360f));
			vector8.y = ((!(vector8.y > 180f)) ? vector8.y : (vector8.y - 360f));
			if (chaseToPitch)
			{
				float num4 = Vector3.Dot(vector6, vector5);
				num4 = Mathf.Clamp01((!chaseToPitchReverse) ? num4 : Mathf.Abs(num4));
				vector8.x += (chasePitch - (eulerAngles.x + vector8.x)) * num4;
			}
			Vector2 vector9 = Vector2.ClampMagnitude(vector8, chaseSpeed * Time.deltaTime);
			vector8 = new Vector3(vector9.x, vector9.y, vector8.z);
		}
		if ((pitchInputMode == PitchInputMode.Influence) & !mouseMode)
		{
			zero.x = Mathf.Clamp(cameraController.viewInput.x * (maximumPitch - minimumPitch) * 0.5f, minimumPitch, maximumPitch);
			zero.x = Mathf.Clamp(zero.x + chasePitch, minimumChasePitch, maximumChasePitch);
		}
		else
		{
			zero.x = Mathf.Clamp(vector.x + vector2.x + eulerAngles.x, minimumPitch, maximumPitch);
			zero.x = Mathf.Clamp(zero.x + vector8.x * num3, minimumChasePitch, maximumChasePitch);
		}
		zero.y = vector.y + vector2.y + eulerAngles.y + vector8.y * num3;
		desiredZoom = Mathf.Clamp(desiredZoom + num2, minimumZoom, maximumZoom);
		Quaternion quaternion7 = quaternion5 * Quaternion.Euler(new Vector3(zero.x, zero.y, 0f));
		desiredRotation = quaternion7;
		cameraController.viewRotation = Quaternion.Slerp(cameraController.viewRotation, desiredRotation, rotationSlerp);
		Vector3 vector10 = cameraController.viewRotation * Vector3.back * desiredZoom;
		desiredPosition = position + vector10;
		cameraController.viewPosition = cameraController.transform.position + Vector3.ClampMagnitude(desiredPosition - cameraController.viewPosition, zoomSpeed * Time.deltaTime);
		cameraController.viewPosition = Vector3.Lerp(cameraController.viewPosition, desiredPosition, 1f);
		cameraController.viewRotation = Quaternion.LookRotation(position - cameraController.viewPosition, vector3);
		quaternion7 = cameraController.viewRotation;
		vector10 = cameraController.viewPosition - position;
		cameraController.transform.rotation = quaternion7;
		float num5 = Mathf.Clamp(Vector3.Dot(lhs, cameraController.viewRotation * Vector3.back), 0f, lhs.magnitude);
		float zoomDistance = GetZoomDistance(quaternion7, position, position + vector10);
		zoomDistance = Mathf.Clamp((cameraController.transform.position - position).magnitude + num5 + zoomSpeed * Time.deltaTime, 0f, zoomDistance);
		cameraController.transform.position = position + Vector3.ClampMagnitude(vector10, zoomDistance);
		_lastTargetPosition = position;
		_lastUpVector = vector3;
	}
}

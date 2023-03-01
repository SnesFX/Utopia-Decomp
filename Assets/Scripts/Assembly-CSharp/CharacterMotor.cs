using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Character))]
[DisallowMultipleComponent]
public class CharacterMotor : ActorPhysics
{
	public const float GROUND_ANGLE = 40f;

	public const float SLOPE_ANGLE = 60f;

	public const float WALL_ANGLE = 100f;

	[SerializeField]
	private CharacterStateSelection _stateSelection = CharacterStateSelection.Default;

	[SerializeField]
	private CharacterState _state;

	[SerializeField]
	private CharacterState _defaultState;

	private Vector3 _angularDrive = Vector3.zero;

	private Vector3 _drive = Vector3.zero;

	private Vector3 _desiredDrive = Vector3.zero;

	private Vector3 _desiredAngle = Vector3.zero;

	private bool _brake;

	[SerializeField]
	private float _stepHeight = 0.3f;

	public Character character
	{
		get
		{
			return (!(base.actor is Character)) ? null : (base.actor as Character);
		}
	}

	public CharacterStateSelection stateSelection
	{
		get
		{
			return _stateSelection;
		}
		set
		{
			_stateSelection = value;
		}
	}

	public CharacterState state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
		}
	}

	public CharacterState defaultState
	{
		get
		{
			return _defaultState;
		}
		set
		{
			_defaultState = value;
		}
	}

	public Vector3 angularDrive
	{
		get
		{
			return _angularDrive;
		}
		set
		{
			_angularDrive = value;
		}
	}

	public Vector3 drive
	{
		get
		{
			return _drive;
		}
		set
		{
			_drive = value;
		}
	}

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

	public bool brake
	{
		get
		{
			return _brake;
		}
		set
		{
			_brake = value;
		}
	}

	public float stepHeight
	{
		get
		{
			return Mathf.Clamp(_stepHeight, 0f, float.PositiveInfinity);
		}
		set
		{
			_stepHeight = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
	}

	public event CharacterMotorEventHandler Jumped;

	public event CharacterMotorEventHandler Landed;

	public string GetStateName()
	{
		return (!state) ? string.Empty : state.GetStateName();
	}

	public void TriggerJumped()
	{
		if (this.Jumped != null)
		{
			this.Jumped(this, new CharacterMotorEventArgs(base.ground.gameObject));
		}
	}

	public void TriggerLanded()
	{
		if (this.Landed != null)
		{
			this.Landed(this, new CharacterMotorEventArgs(base.ground.gameObject));
		}
	}

	public virtual void EngageJump()
	{
		if ((bool)state)
		{
			state.EngageJump();
		}
	}

	public virtual void DisengageJump()
	{
		if ((bool)state)
		{
			state.DisengageJump();
		}
	}

	public virtual void EngageDrop()
	{
		if ((bool)state)
		{
			state.EngageDrop();
		}
	}

	public virtual void DisengageDrop()
	{
		if ((bool)state)
		{
			state.DisengageDrop();
		}
	}

	public void ActivateState(CharacterState oState)
	{
		if ((bool)oState && oState != state && oState.gameObject == base.gameObject)
		{
			if ((bool)state)
			{
				state.OnStateExit();
			}
			state = oState;
			state.OnStateEnter();
		}
		if (state == defaultState)
		{
			stateSelection = CharacterStateSelection.Default;
		}
		else
		{
			stateSelection = CharacterStateSelection.Custom;
		}
	}

	public void ActivateState<T>() where T : CharacterState
	{
		CharacterState component = GetComponent<T>();
		if ((bool)component && component != state)
		{
			if ((bool)state)
			{
				state.OnStateExit();
			}
			state = component;
			state.OnStateEnter();
		}
		if (state == defaultState)
		{
			stateSelection = CharacterStateSelection.Default;
		}
		else
		{
			stateSelection = CharacterStateSelection.Custom;
		}
	}

	public T AddState<T>() where T : CharacterState
	{
		return (!character) ? base.gameObject.AddComponent<T>() : character.AddComponent<T>();
	}

	public T AddOrActivateState<T>() where T : CharacterState
	{
		T val = GetComponent<T>();
		if (!(UnityEngine.Object)val)
		{
			val = AddState<T>();
		}
		ActivateState(val);
		return val;
	}

	public void SetStateSelection(CharacterStateSelection oSelection)
	{
		switch (oSelection)
		{
		case CharacterStateSelection.Animated:
			stateSelection = CharacterStateSelection.Animated;
			break;
		case CharacterStateSelection.Stationed:
			stateSelection = CharacterStateSelection.Stationed;
			break;
		case CharacterStateSelection.Default:
			stateSelection = CharacterStateSelection.Default;
			state = defaultState;
			break;
		case CharacterStateSelection.Custom:
			stateSelection = CharacterStateSelection.Custom;
			break;
		default:
			stateSelection = CharacterStateSelection.None;
			state = null;
			break;
		}
	}

	public void UpdateGround()
	{
		Collider collider = null;
		float num = Mathf.Clamp(Mathf.Tan((float)Math.PI / 3f) * base.size.x * 0.5f, stepHeight, base.size.y);
		float num2 = 0f - num;
		float num3 = 0f;
		Vector3 vector = base.worldBottomPoint;
		Vector3 vector2 = vector + base.transform.up * num;
		Vector3 vector3 = vector - base.transform.up * num;
		Vector3 zero = Vector3.zero;
		Vector3 to = base.transform.up;
		Vector3 vector4 = Vector3.zero;
		Vector3 vector5 = Vector3.zero;
		int num4 = 0;
		bool flag = false;
		bool flag2 = base.grounded;
		RaycastHit hitInfo;
		bool flag3 = Physics.Linecast(vector2, vector3, out hitInfo, base.groundLayers);
		base.ground.isFluid = false;
		if (flag3)
		{
			if ((Vector3.Angle(hitInfo.normal, to) <= 60f) & !hitInfo.collider.isTrigger)
			{
				collider = hitInfo.collider;
				vector4 = hitInfo.normal;
				num3 = Vector3.Dot(base.transform.up, hitInfo.point - vector);
				num2 = ((!(num3 < num2)) ? num3 : num2);
			}
			else
			{
				flag3 = false;
			}
		}
		if (!flag3)
		{
			base.grounded = false;
			return;
		}
		switch (base.physicsQuality)
		{
		case PhysicsQuality.Low:
			num4 = 0;
			break;
		case PhysicsQuality.Medium:
			num4 = 4;
			break;
		case PhysicsQuality.High:
			num4 = 8;
			break;
		}
		for (int i = 0; i < 8; i++)
		{
			Quaternion quaternion = Quaternion.Euler(base.transform.up * (0f + 45f * (float)i));
			Vector3 vector6 = quaternion * base.transform.forward * base.radius;
			Vector3 vector7 = quaternion * base.transform.right;
			if (Physics.Linecast(vector2 + vector6, vector3 + vector6, out hitInfo, base.groundLayers) && ((Vector3.Angle(hitInfo.normal, to) <= 60f) & !hitInfo.collider.isTrigger))
			{
				zero = Vector3.Cross(base.transform.up, Utility.RelativeVector(hitInfo.normal, vector7).normalized);
				float num5 = Vector3.Dot(base.transform.up, hitInfo.point - vector);
				num5 = Mathf.Clamp(Vector3.Dot(vector7, zero), -60f, 0f) * base.radius + num5;
				if (stepHeight >= num5 - num3)
				{
					vector5 += hitInfo.normal;
					num2 = ((!(num2 < num5)) ? num2 : num5);
				}
			}
		}
		if (vector5 == Vector3.zero)
		{
			vector5 = vector4;
		}
		if ((collider == null) | (vector5 == Vector3.zero))
		{
			base.grounded = false;
			return;
		}
		if (!base.grounded && ((num2 < -0.05f) | (Vector3.Dot(vector5, base.velocity) > 0.05f)))
		{
			base.grounded = false;
			return;
		}
		if (base.ground.collider != collider)
		{
			base.ground.SetData(collider, vector + base.transform.up * num2, vector5);
		}
		else
		{
			base.ground.point = vector + base.transform.up * num2;
			base.ground.normal = vector5;
		}
		if (!flag2)
		{
			flag = true;
		}
		if (flag && this.Landed != null)
		{
			this.Landed(this, new CharacterMotorEventArgs(base.ground.collider));
		}
	}

	public void ApplyJumpForce(Vector3 vDirection, float fPower, float fHeight)
	{
		base.velocity += Utility.VectorInDirection(vDirection.normalized * Mathf.Sqrt(fHeight * fPower * 2f) - base.velocity, vDirection.normalized);
		base.grounded = false;
	}

	public void ApplyJumpStopForce(float fPower)
	{
		base.velocity -= base.up * (Mathf.Clamp(Vector3.Dot(base.velocity, base.up), 0f, float.MaxValue) * (1f - Mathf.Clamp01(fPower)));
	}

	public void GroundAdhereMotion()
	{
		if (base.grounded)
		{
			drive = Utility.RelativeVector(drive.normalized, Quaternion.Inverse(base.transform.rotation) * base.ground.normal) * drive.magnitude;
			base.velocity = Utility.RelativeVector(base.velocity.normalized, base.ground.normal) * base.velocity.magnitude;
		}
	}

	public void GroundAdhereHeight()
	{
		if (base.grounded)
		{
			MovePosition(base.transform.position + base.transform.up * Vector3.Dot(base.ground.point - base.worldBottomPoint, base.ground.normal));
		}
	}

	private void Awake()
	{
		if ((bool)base.rigidbody)
		{
			base.rigidbody.angularDrag = 0f;
			base.rigidbody.drag = 0f;
			base.rigidbody.mass = base.mass;
			base.rigidbody.centerOfMass = base.centerOfMass;
		}
		if (!defaultState)
		{
			_defaultState = GetComponent<CharacterStateDefault>();
		}
		SetStateSelection(stateSelection);
	}

	private void FixedUpdate()
	{
		if (base.physicsQuality != 0)
		{
			UpdatePortals();
			UpdateGravity();
			UpdateVolumes();
			UpdatePhysics();
			UpdateGround();
			if (state != null)
			{
				state.OnStateUpdate();
			}
		}
	}
}

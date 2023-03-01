using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class CharacterAvatar : MonoBehaviour
{
	[SerializeField]
	private RuntimeAnimatorController _defaultAnimatorController;

	[SerializeField]
	private List<SkinnedMeshRenderer> _meshRenderers;

	private Animator _animator;

	private Character _character;

	private CharacterMotor _motor;

	private Vector3 _drive = Vector3.zero;

	private Vector3 _angularDrive = Vector3.zero;

	private int _state;

	private bool _grounded = true;

	private bool _brake = true;

	[SerializeField]
	private string _stateName = "State";

	[SerializeField]
	private string _driveMagnitudeName = "Drive";

	[SerializeField]
	private string _driveXName = "Drive X";

	[SerializeField]
	private string _driveYName = "Drive Y";

	[SerializeField]
	private string _driveZName = "Drive Z";

	[SerializeField]
	private string _angularDriveXName = "Drive Pitch";

	[SerializeField]
	private string _angularDriveYName = "Drive Yaw";

	[SerializeField]
	private string _angularDriveZName = "Drive Roll";

	[SerializeField]
	private string _groundedName = "Grounded";

	[SerializeField]
	private string _brakeName = "Brake";

	[SerializeField]
	private List<AvatarStateIndex> _states = new List<AvatarStateIndex>();

	[SerializeField]
	private List<SpecialEffectHandler> _effectHandlers = new List<SpecialEffectHandler>();

	public RuntimeAnimatorController defaultAnimatorController
	{
		get
		{
			return _defaultAnimatorController;
		}
		set
		{
			_defaultAnimatorController = value;
		}
	}

	public List<SkinnedMeshRenderer> meshRenderers
	{
		get
		{
			return _meshRenderers;
		}
	}

	public Animator animator
	{
		get
		{
			return (!_animator) ? (_animator = GetComponentInParent<Animator>()) : _animator;
		}
	}

	public Character character
	{
		get
		{
			return (!_character) ? (_character = GetComponentInParent<Character>()) : _character;
		}
	}

	public CharacterMotor motor
	{
		get
		{
			return (!_motor) ? (_motor = GetComponentInParent<CharacterMotor>()) : _motor;
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

	public int state
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

	public bool grounded
	{
		get
		{
			return _grounded;
		}
		set
		{
			_grounded = value;
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

	public string stateName
	{
		get
		{
			return _stateName;
		}
		set
		{
			_stateName = value;
		}
	}

	public string driveMagnitudeName
	{
		get
		{
			return _driveMagnitudeName;
		}
		set
		{
			_driveMagnitudeName = value;
		}
	}

	public string driveXName
	{
		get
		{
			return _driveXName;
		}
		set
		{
			_driveXName = value;
		}
	}

	public string driveYName
	{
		get
		{
			return _driveYName;
		}
		set
		{
			_driveYName = value;
		}
	}

	public string driveZName
	{
		get
		{
			return _driveZName;
		}
		set
		{
			_driveZName = value;
		}
	}

	public string angularDriveXName
	{
		get
		{
			return _angularDriveXName;
		}
		set
		{
			_angularDriveXName = value;
		}
	}

	public string angularDriveYName
	{
		get
		{
			return _angularDriveYName;
		}
		set
		{
			_angularDriveYName = value;
		}
	}

	public string angularDriveZName
	{
		get
		{
			return _angularDriveZName;
		}
		set
		{
			_angularDriveZName = value;
		}
	}

	public string groundedName
	{
		get
		{
			return _groundedName;
		}
		set
		{
			_groundedName = value;
		}
	}

	public string brakeName
	{
		get
		{
			return _brakeName;
		}
		set
		{
			_brakeName = value;
		}
	}

	public List<AvatarStateIndex> states
	{
		get
		{
			return _states;
		}
	}

	public List<SpecialEffectHandler> effectHandlers
	{
		get
		{
			return _effectHandlers;
		}
		set
		{
			_effectHandlers = value;
		}
	}

	public string GetGroundName()
	{
		if ((bool)motor && motor.grounded)
		{
			return motor.ground.materialName;
		}
		return string.Empty;
	}

	public void SetState(string sName)
	{
		state = states.FindIndex((AvatarStateIndex o) => o.name == sName);
		if (state > -1)
		{
			state = states[state].index;
		}
		else if ((bool)animator && (bool)animator.runtimeAnimatorController && stateName != string.Empty)
		{
			state = animator.GetInteger(stateName);
		}
	}

	public void SpawnEffect(string sParameter)
	{
		List<SpecialEffectHandler> list = effectHandlers.FindAll((SpecialEffectHandler o) => o.name == sParameter);
		foreach (SpecialEffectHandler item in list)
		{
			item.Spawn();
		}
	}

	public void RefreshEffect(string sParameter)
	{
		List<SpecialEffectHandler> list = effectHandlers.FindAll((SpecialEffectHandler o) => o.name == sParameter);
		foreach (SpecialEffectHandler item in list)
		{
			item.Refresh();
		}
	}

	public void DestroyEffect(string sParameter)
	{
		List<SpecialEffectHandler> list = effectHandlers.FindAll((SpecialEffectHandler o) => o.name == sParameter);
		foreach (SpecialEffectHandler item in list)
		{
			item.Destroy();
		}
	}

	public void ApplyAnimatorParameters()
	{
		if (!animator)
		{
			return;
		}
		if (animator.runtimeAnimatorController == null)
		{
			if (!(defaultAnimatorController != null))
			{
				return;
			}
			animator.runtimeAnimatorController = defaultAnimatorController;
		}
		if ((bool)animator && (bool)character)
		{
			if (stateName != string.Empty)
			{
				animator.SetInteger(stateName, state);
			}
			if (driveMagnitudeName != string.Empty)
			{
				animator.SetFloat(driveMagnitudeName, drive.magnitude);
			}
			if (driveXName != string.Empty)
			{
				animator.SetFloat(driveXName, drive.x);
			}
			if (driveYName != string.Empty)
			{
				animator.SetFloat(driveYName, drive.y);
			}
			if (driveZName != string.Empty)
			{
				animator.SetFloat(driveZName, drive.z);
			}
			if (angularDriveXName != string.Empty)
			{
				animator.SetFloat(angularDriveXName, angularDrive.x);
			}
			if (angularDriveYName != string.Empty)
			{
				animator.SetFloat(angularDriveYName, angularDrive.y);
			}
			if (angularDriveZName != string.Empty)
			{
				animator.SetFloat(angularDriveZName, angularDrive.z);
			}
			if (groundedName != string.Empty)
			{
				animator.SetBool(groundedName, grounded);
			}
			if (brakeName != string.Empty)
			{
				animator.SetBool(brakeName, brake);
			}
		}
	}

	private void Update()
	{
		ApplyAnimatorParameters();
	}
}

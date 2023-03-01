using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class Character : Pawn
{
	[SerializeField]
	private CharacterAvatar _avatar;

	private CharacterMotor _motor;

	private Vector3 _view = Vector3.zero;

	private Vector3 _desiredView = Vector3.zero;

	public CharacterAvatar avatar
	{
		get
		{
			return (!_avatar) ? null : _avatar;
		}
		protected set
		{
			_avatar = value;
		}
	}

	public CharacterMotor motor
	{
		get
		{
			return (!_motor) ? (_motor = GetComponent<CharacterMotor>()) : _motor;
		}
	}

	public Vector3 view
	{
		get
		{
			return _view.normalized;
		}
		protected set
		{
			if (value != Vector3.zero)
			{
				_view = value.normalized;
			}
		}
	}

	public Vector3 desiredView
	{
		get
		{
			return _desiredView.normalized;
		}
		set
		{
			_desiredView = value.normalized;
		}
	}

	public void EnableAvatar(CharacterAvatar oAvatar)
	{
		if (oAvatar.transform.parent == base.transform)
		{
			avatar = oAvatar;
		}
	}

	public override void OnRespawn()
	{
		base.transform.position = base.spawnPosition;
		base.transform.rotation = base.spawnRotation;
		if ((bool)base.health)
		{
			base.health.currentValue = base.health.maximum.totalValue;
		}
		base.alive = true;
		if ((bool)motor)
		{
			motor.SetStateSelection(CharacterStateSelection.Default);
		}
	}

	public override void OnDespawn()
	{
	}

	private void Start()
	{
		base.spawnPosition = base.transform.position;
		base.spawnRotation = base.transform.rotation;
	}
}

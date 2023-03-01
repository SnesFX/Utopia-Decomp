using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class Pawn : Actor
{
	protected Controller _controller;

	protected Actor _target;

	public Controller controller
	{
		get
		{
			return _controller;
		}
	}

	public Actor target
	{
		get
		{
			return _target;
		}
	}

	public User user
	{
		get
		{
			return (!(_controller is User)) ? null : (_controller as User);
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			return (!base.physics) ? Vector3.zero : base.physics.angularVelocity;
		}
		set
		{
			if ((bool)base.physics)
			{
				base.physics.angularVelocity = value;
			}
		}
	}

	public Vector3 gravity
	{
		get
		{
			return (!base.physics) ? Physics.gravity : base.physics.gravity;
		}
	}

	public Vector3 up
	{
		get
		{
			return -gravity.normalized;
		}
	}

	public Vector3 velocity
	{
		get
		{
			return (!base.physics) ? Vector3.zero : base.physics.velocity;
		}
		set
		{
			if ((bool)base.physics)
			{
				base.physics.velocity = value;
			}
		}
	}

	public event PawnPossessedEventHandler Possessed;

	public event PawnPossessedEventHandler Unpossessed;

	public void Possess(Controller oController)
	{
		if (!(oController == null))
		{
			if ((bool)_controller)
			{
				_controller.Unpossess(this);
			}
			_controller = oController;
			if (this.Possessed != null)
			{
				this.Possessed(this, new PawnPossessedEventArgs(oController));
			}
		}
	}

	public void Unpossess()
	{
		if ((bool)_controller)
		{
			if (this.Unpossessed != null)
			{
				this.Unpossessed(this, new PawnPossessedEventArgs(_controller));
			}
			_controller = null;
		}
	}

	public void Target(Actor oTarget)
	{
	}

	public void UnTarget()
	{
	}
}

using System;
using UnityEngine;

public class ActorPortalEventArgs : EventArgs
{
	private Portal _fromPortal;

	private Portal _toPortal;

	private Vector3 _deltaPosition = Vector3.zero;

	private Quaternion _deltaRotation = Quaternion.identity;

	public Portal fromPortal
	{
		get
		{
			return _fromPortal;
		}
	}

	public Portal toPortal
	{
		get
		{
			return _toPortal;
		}
	}

	public Vector3 deltaPosition
	{
		get
		{
			return _deltaPosition;
		}
	}

	public Quaternion deltaRotation
	{
		get
		{
			return _deltaRotation;
		}
	}

	public ActorPortalEventArgs(Vector3 vDeltaPosition, Quaternion qDeltaRotation)
	{
		_deltaPosition = vDeltaPosition;
		_deltaRotation = qDeltaRotation;
	}
}

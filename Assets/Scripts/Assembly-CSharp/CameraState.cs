using System;
using UnityEngine;

[Serializable]
public class CameraState : MonoBehaviour
{
	private CameraController _cameraController;

	public CameraController cameraController
	{
		get
		{
			return (!_cameraController) ? (_cameraController = GetComponent<CameraController>()) : _cameraController;
		}
	}

	public virtual void OnStateEnter()
	{
	}

	public virtual void OnStateExit()
	{
	}

	public virtual void OnStateUpdate()
	{
	}
}

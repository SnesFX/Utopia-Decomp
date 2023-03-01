using System;
using UnityEngine;

[Serializable]
public class BillboardGameObject : MonoBehaviour
{
	[SerializeField]
	private Vector3 _baseRotation = Vector3.zero;

	[SerializeField]
	private bool _useCameraUp;

	public Vector3 baseRotation
	{
		get
		{
			return _baseRotation;
		}
		set
		{
			_baseRotation = value;
		}
	}

	public bool useCameraUp
	{
		get
		{
			return _useCameraUp;
		}
		set
		{
			_useCameraUp = value;
		}
	}

	private void LateUpdate()
	{
		Vector3 vector = ((useCameraUp && (bool)Camera.main) ? Camera.main.transform.up : ((!base.transform.parent) ? Vector3.up : base.transform.parent.up));
		Vector3 forward = Utility.RelativeVector((!Camera.main) ? base.transform.forward : (-Camera.main.transform.forward), vector);
		if (forward.magnitude == 0f)
		{
			forward = Quaternion.FromToRotation(Vector3.up, vector) * Vector3.forward;
		}
		base.transform.rotation = Quaternion.LookRotation(forward, vector) * Quaternion.Inverse(Quaternion.Euler(baseRotation));
	}
}

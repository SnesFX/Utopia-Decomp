using System;
using UnityEngine;

[Serializable]
public class UtopiaSonicRunHandler : MonoBehaviour
{
	private CharacterMotor _motor;

	[SerializeField]
	private float _speedThreshold;

	[SerializeField]
	private GameObject _legNormal;

	[SerializeField]
	private GameObject _legWheel;

	public CharacterMotor motor
	{
		get
		{
			return (!_motor) ? (_motor = GetComponentInParent<CharacterMotor>()) : _motor;
		}
	}

	public float speedThreshold
	{
		get
		{
			return Mathf.Clamp(_speedThreshold, 0f, float.MaxValue);
		}
	}

	public GameObject legNormal
	{
		get
		{
			return _legNormal;
		}
	}

	public GameObject legWheel
	{
		get
		{
			return _legWheel;
		}
	}

	private void LateUpdate()
	{
		if ((bool)motor && motor.grounded && motor.stateSelection == CharacterStateSelection.Default && motor.drive.magnitude > speedThreshold)
		{
			if ((bool)legNormal)
			{
				legNormal.SetActive(false);
			}
			if ((bool)legWheel)
			{
				legWheel.SetActive(true);
			}
		}
		else
		{
			if ((bool)legNormal)
			{
				legNormal.SetActive(true);
			}
			if ((bool)legWheel)
			{
				legWheel.SetActive(false);
			}
		}
	}
}

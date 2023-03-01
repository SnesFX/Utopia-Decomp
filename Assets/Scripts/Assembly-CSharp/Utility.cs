using System;
using UnityEngine;

public static class Utility
{
	public static Vector3 RelativeVector(Vector3 vVector, Vector3 vRelativeVector)
	{
		return vVector - vRelativeVector.normalized * Vector3.Dot(vVector, vRelativeVector);
	}

	public static Vector3 VectorInDirection(Vector3 vVector, Vector3 vDirection)
	{
		return vVector - RelativeVector(vVector, vDirection.normalized);
	}

	public static Vector3 ClampVector(Vector3 vVector, float fMin, float fMax)
	{
		return vVector.normalized * Mathf.Clamp(vVector.magnitude, fMin, fMax);
	}

	public static Vector3 MaximizeVector(Vector3 vVector, float fValue)
	{
		return vVector.normalized * ((!(vVector.magnitude < fValue)) ? vVector.magnitude : fValue);
	}

	public static Vector3 Redirect(Vector3 vVector, Vector3 vToDirection, float fMaxAngle, float fMaxRadian)
	{
		if (vToDirection == Vector3.zero)
		{
			return vVector;
		}
		fMaxAngle = ClampRadian(fMaxAngle * ((float)Math.PI / 180f), vVector.magnitude, fMaxRadian);
		vVector = Vector3.RotateTowards(vVector, vToDirection.normalized * vVector.magnitude, fMaxAngle, float.PositiveInfinity);
		return vVector;
	}

	public static float ClampRadian(float fRadian, float fLength, float fMagnitude)
	{
		if (fRadian <= 0f || fLength <= 0f || fMagnitude <= 0f)
		{
			return 0f;
		}
		float num = fMagnitude / fLength * ((float)Math.PI * 2f);
		return (!(fRadian > num)) ? fRadian : num;
	}
}

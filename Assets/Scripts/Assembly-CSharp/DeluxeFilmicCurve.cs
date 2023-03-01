using System;
using UnityEngine;

[Serializable]
public class DeluxeFilmicCurve
{
	[SerializeField]
	public float m_BlackPoint;

	[SerializeField]
	public float m_WhitePoint = 1f;

	[SerializeField]
	public float m_CrossOverPoint = 0.3f;

	[SerializeField]
	public float m_ToeStrength = 0.98f;

	[SerializeField]
	public float m_ShoulderStrength;

	[SerializeField]
	public float m_Highlights = 0.5f;

	public float m_k;

	public Vector4 m_ToeCoef;

	public Vector4 m_ShoulderCoef;

	public float GetExposure()
	{
		float highlights = m_Highlights;
		float num = 2f + (1f - highlights) * 20f;
		return num * Mathf.Exp(-2f);
	}

	public float ComputeK(float t, float c, float b, float s, float w)
	{
		float num = (1f - t) * (c - b);
		float num2 = (1f - s) * (w - c) + (1f - t) * (c - b);
		return num / num2;
	}

	public float Toe(float x, float t, float c, float b, float s, float w, float k)
	{
		float num = m_ToeCoef.x * x;
		float num2 = m_ToeCoef.y * x;
		return (num + m_ToeCoef.z) / (num2 + m_ToeCoef.w);
	}

	public float Shoulder(float x, float t, float c, float b, float s, float w, float k)
	{
		float num = m_ShoulderCoef.x * x;
		float num2 = m_ShoulderCoef.y * x;
		return (num + m_ShoulderCoef.z) / (num2 + m_ShoulderCoef.w) + k;
	}

	public float Graph(float x, float t, float c, float b, float s, float w, float k)
	{
		if (x <= m_CrossOverPoint)
		{
			return Toe(x, t, c, b, s, w, k);
		}
		return Shoulder(x, t, c, b, s, w, k);
	}

	public void StoreK()
	{
		m_k = ComputeK(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint);
	}

	public void ComputeShaderCoefficients(float t, float c, float b, float s, float w, float k)
	{
		float x = k * (1f - t);
		float z = k * (1f - t) * (0f - b);
		float y = 0f - t;
		float w2 = c - (1f - t) * b;
		m_ToeCoef = new Vector4(x, y, z, w2);
		float x2 = 1f - k;
		float z2 = (1f - k) * (0f - c);
		float w3 = (1f - s) * w - c;
		m_ShoulderCoef = new Vector4(x2, s, z2, w3);
	}

	public void UpdateCoefficients()
	{
		StoreK();
		ComputeShaderCoefficients(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint, m_k);
	}
}

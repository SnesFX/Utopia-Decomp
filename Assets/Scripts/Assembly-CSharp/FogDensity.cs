using UnityEngine;

public class FogDensity : MonoBehaviour
{
	public LayerMask waterLayer;

	public float waterFogDensity = 0.005f;

	public float regularFogDensity = 0.00035f;

	private void Start()
	{
	}

	private void Update()
	{
		if (Physics.CheckSphere(base.transform.position, 0.4f, waterLayer))
		{
			RenderSettings.fogDensity = waterFogDensity;
		}
		else
		{
			RenderSettings.fogDensity = regularFogDensity;
		}
	}
}

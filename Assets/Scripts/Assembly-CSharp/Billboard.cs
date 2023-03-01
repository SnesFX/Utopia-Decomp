using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Vector3 upwards = ((!(base.transform.parent != null)) ? base.transform.up : base.transform.parent.up);
		base.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - base.transform.position, upwards) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
	}
}

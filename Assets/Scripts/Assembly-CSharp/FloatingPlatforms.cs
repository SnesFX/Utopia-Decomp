using UnityEngine;

public class FloatingPlatforms : MonoBehaviour
{
	private float y;

	private float time;

	public bool Activated;

	private readonly AnimationCurve easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, -0.35f);

	private void Start()
	{
		y = base.gameObject.transform.position.y;
	}

	private void FixedUpdate()
	{
		time = ((!Activated) ? Mathf.Max(time - Time.fixedDeltaTime * 2.5f, 0f) : Mathf.Min(time + Time.fixedDeltaTime * 2.5f, 1f));
		Vector3 position = base.gameObject.transform.position;
		base.gameObject.transform.position = new Vector3(position.x, y + easingCurve.Evaluate(time), position.z);
	}
}

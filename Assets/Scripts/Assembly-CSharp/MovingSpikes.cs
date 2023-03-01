using UnityEngine;

public class MovingSpikes : MonoBehaviour
{
	public bool isMoving;

	public float rate = 1f;

	public float sinkHeight = 1.6f;

	[SerializeField]
	private float timer;

	[SerializeField]
	private bool hidden;

	private BoxCollider collider;

	private Rigidbody rigidBody;

	private int moving;

	private void Start()
	{
		collider = GetComponent<BoxCollider>();
		rigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!isMoving)
		{
			return;
		}
		if (timer >= rate)
		{
			hidden = !hidden;
			moving = ((!hidden) ? 1 : (-1));
			collider.enabled = !hidden;
			timer = 0f;
		}
		if (moving != 0)
		{
			int num = (int)Mathf.Sign(moving);
			base.transform.localPosition += new Vector3(0f, 0f, sinkHeight / 100f * ((float)num / 4f));
			moving += num;
			if (Mathf.Abs(moving) >= 5)
			{
				moving = 0;
			}
		}
		timer += Time.deltaTime;
	}
}

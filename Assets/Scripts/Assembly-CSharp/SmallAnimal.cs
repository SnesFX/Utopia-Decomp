using UnityEngine;

public class SmallAnimal : Actor
{
	public enum Animals
	{
		Pocky = 0,
		Flicky = 1
	}

	public enum States
	{
		Freed = 0,
		Chase = 1,
		Wander = 2
	}

	public Animals animal;

	private Animation anim;

	private float freeTimer;

	private bool soundFreed;

	public SpecialEffectHandler sfx;

	private int layerMask;

	private int waterMask;

	private bool grounded;

	private bool edge;

	private bool wall;

	private bool ceiling;

	private Vector3 dir;

	private Vector3 targetDir;

	private Character player;

	private float startY;

	public float topSpeed = 15f;

	public float gravity = 32f;

	public float acceleration = 10f;

	public float launchHeight = 16f;

	private States state;

	private float dirTimer;

	private float chaseTimer = 1f;

	private float ySpeed;

	private float xSpeed;

	private Vector3 speed
	{
		get
		{
			return (animal != Animals.Flicky) ? new Vector3(xSpeed * dir.x, ySpeed, xSpeed * dir.z) : new Vector3(xSpeed * dir.x, ySpeed + xSpeed * dir.y, xSpeed * dir.z);
		}
	}

	public override void OnRespawn()
	{
		Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		layerMask = LayerMask.GetMask("Default");
		waterMask = LayerMask.GetMask("Water");
		player = GameObject.Find("Saturn Sonic").GetComponent<Character>();
		dir = player.velocity.normalized;
		dir.y = 0f;
		targetDir = dir;
		xSpeed = player.velocity.magnitude * 0.9f;
		ySpeed = launchHeight;
		startY = base.transform.position.y;
		anim = GetComponent<Animation>();
		float value = Random.value;
		switch (animal)
		{
		case Animals.Pocky:
			anim.Play((!(value >= 0.5f)) ? "PockyFreed2" : "PockyFreed1");
			anim.PlayQueued("PockyBounce");
			break;
		case Animals.Flicky:
			anim.Play((!(value >= 0.5f)) ? "FlickyFreed2" : "FlickyFreed1");
			anim.PlayQueued("FlickyFly");
			break;
		}
	}

	private void FixedUpdate()
	{
		RaycastHit hitInfo;
		grounded = Physics.Raycast(base.transform.position + new Vector3(0f, 1.5f, 0f), Vector3.down, out hitInfo, 1.5f, layerMask);
		RaycastHit hitInfo2;
		wall = Physics.Raycast(base.transform.position + new Vector3(0f, 0.7f, 0f), dir, out hitInfo2, 1.5f, layerMask);
		switch (animal)
		{
		case Animals.Pocky:
			edge = !Physics.Raycast(base.transform.position + new Vector3(0f, 0.5f, 0f) + dir * 0.75f, Vector3.down, 0.7f, layerMask);
			if (!grounded)
			{
				ySpeed -= gravity * Time.fixedDeltaTime;
				break;
			}
			base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y, base.transform.position.z);
			ySpeed = 0f;
			if (state != 0)
			{
				xSpeed = ((state != States.Chase) ? Mathf.MoveTowards(xSpeed, topSpeed, acceleration * Time.fixedDeltaTime) : Mathf.Max(Mathf.Min(player.velocity.magnitude, 70f), topSpeed));
			}
			break;
		case Animals.Flicky:
		{
			edge = Physics.Raycast(base.transform.position + new Vector3(0f, 0.5f, 0f) + dir * 0.75f, Vector3.down, 0.7f, waterMask);
			RaycastHit hitInfo3;
			ceiling = Physics.Raycast(base.transform.position + new Vector3(0f, 0.1f, 0f), Vector3.up, out hitInfo3, 0.5f, layerMask);
			if (state != 0)
			{
				xSpeed = ((state != States.Chase) ? Mathf.MoveTowards(xSpeed, topSpeed, acceleration * Time.fixedDeltaTime) : Mathf.Max(Mathf.Min(player.velocity.magnitude, 70f), topSpeed));
			}
			ySpeed = Mathf.MoveTowards(ySpeed, 0f, gravity * Time.fixedDeltaTime);
			if (ceiling)
			{
				dir = Vector3.Reflect(targetDir, Vector3.down);
				targetDir = dir;
				base.transform.position = new Vector3(base.transform.position.x, hitInfo3.point.y - 0.8f, base.transform.position.z);
			}
			else if (base.transform.position.y > startY + 30f && (targetDir.y > 0f || dir.y > 0f))
			{
				dir = Vector3.Reflect(targetDir, Vector3.down);
				targetDir = dir;
			}
			else if (grounded)
			{
				dir = Vector3.Reflect(targetDir, Vector3.up);
				targetDir = dir;
				base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y + 0.2f, base.transform.position.z);
			}
			break;
		}
		}
		if (wall)
		{
			dir = Vector3.Reflect(targetDir, hitInfo2.normal);
			if (animal == Animals.Pocky)
			{
				dir.y = 0f;
			}
			targetDir = dir;
		}
		if (grounded && edge)
		{
			if (animal == Animals.Pocky)
			{
				targetDir.y = 0f;
			}
			dir = targetDir * -1f;
			targetDir = dir;
		}
		switch (state)
		{
		case States.Freed:
			if (grounded || (ySpeed == 0f && animal == Animals.Flicky))
			{
				targetDir = (player.transform.position - base.transform.position).normalized;
				if (animal == Animals.Pocky)
				{
					targetDir.y = 0f;
				}
				chaseTimer = 3.5f;
				state = States.Chase;
			}
			break;
		case States.Chase:
			dir = Vector3.RotateTowards(dir, targetDir, 1.4f * Time.fixedDeltaTime, 1f);
			if (dirTimer >= 0.35f)
			{
				targetDir = (player.transform.position - base.transform.position).normalized;
				if (animal == Animals.Pocky)
				{
					targetDir.y = 0f;
				}
				dirTimer = 0f;
			}
			chaseTimer -= Time.fixedDeltaTime;
			if (chaseTimer <= 0f)
			{
				state = States.Wander;
			}
			break;
		case States.Wander:
			dir = Vector3.RotateTowards(dir, targetDir, 1f * Time.fixedDeltaTime, 1f);
			if (dirTimer >= 3f)
			{
				targetDir = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.1f, 0.7f), Random.Range(0.5f, 1f)).normalized;
				targetDir.x *= ((!(Random.value >= 0.5f)) ? 1 : (-1));
				targetDir.y *= ((!(Random.value >= 0.5f)) ? 1 : (-1));
				targetDir.z *= ((!(Random.value >= 0.5f)) ? 1 : (-1));
				if (animal == Animals.Pocky)
				{
					targetDir.y = 0f;
				}
				dirTimer = 0f;
			}
			break;
		}
		if (!soundFreed)
		{
			freeTimer = Mathf.Min(freeTimer + Time.fixedDeltaTime, 0.5f);
			if ((double)freeTimer >= 0.5)
			{
				float value = Random.value;
				sfx.Spawn();
				soundFreed = true;
			}
		}
		dirTimer += Time.fixedDeltaTime;
		base.rigidbody.velocity = speed;
		base.transform.LookAt(new Vector3(base.transform.position.x + dir.x, base.transform.position.y, base.transform.position.z + dir.z), Vector3.up);
	}
}

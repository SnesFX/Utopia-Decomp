using UnityEngine;

public class DrowningSFX : MonoBehaviour
{
	public Breath breath;

	public AudioSource drownSource;

	public AudioSource musicSource;

	public AudioClip drowningMusic;

	public AudioClip dingSound;

	[SerializeField]
	private bool submerged;

	[SerializeField]
	private float timer;

	[SerializeField]
	private int tick;

	private void Start()
	{
		breath.Restored += HandleRestored;
	}

	private void HandleRestored(object sender, ActorEventArgs e)
	{
		if (tick == 4)
		{
			drownSource.Stop();
			musicSource.Play();
		}
		timer = 0f;
		tick = 0;
	}

	private void FixedUpdate()
	{
		submerged = breath.submerged;
		if (submerged)
		{
			timer += Time.fixedDeltaTime;
			if (timer >= 5f && tick < 3)
			{
				timer = 0f;
				tick++;
				drownSource.PlayOneShot(dingSound);
			}
			if (timer >= 3f && tick == 3)
			{
				timer = 0f;
				tick++;
				musicSource.Pause();
				drownSource.PlayOneShot(drowningMusic);
			}
		}
		else
		{
			if (tick == 4)
			{
				drownSource.Stop();
				musicSource.Play();
			}
			timer = 0f;
			tick = 0;
		}
	}
}

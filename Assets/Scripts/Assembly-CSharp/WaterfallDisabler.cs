using UnityEngine;

public class WaterfallDisabler : MonoBehaviour
{
	private GameObject player;

	public float distance = 100f;

	public AudioSource source;

	public ParticleSystem emitter;

	private void Start()
	{
		player = GameObject.Find("Saturn Sonic");
	}

	private void Update()
	{
		if ((base.transform.position - player.transform.position).magnitude > distance)
		{
			if (emitter.isPlaying)
			{
				source.enabled = false;
				emitter.Pause();
			}
		}
		else if (!emitter.isPlaying)
		{
			source.enabled = true;
			emitter.Play();
		}
	}
}

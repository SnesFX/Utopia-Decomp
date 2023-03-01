using UnityEngine;

public class AudioSource3DEffects : MonoBehaviour
{
	private AudioSource source;

	private CharacterMotor motor;

	public float baseVelocity = 35f;

	public float maxPitch = 4f;

	public float minPitch = 1f;

	public bool decreaseVolume = true;

	public bool increasePitch = true;

	public bool decreasePitch;

	private void Start()
	{
		source = GetComponentInParent<AudioSource>();
		motor = GetComponentInParent<CharacterMotor>();
	}

	private void Update()
	{
		float magnitude = motor.drive.magnitude;
		if (decreaseVolume && magnitude < baseVelocity)
		{
			source.volume = magnitude / baseVelocity;
		}
		else
		{
			source.volume = 1f;
		}
		if ((increasePitch && magnitude > baseVelocity) || (decreasePitch && magnitude < baseVelocity))
		{
			source.pitch = Mathf.Max(Mathf.Min(magnitude / baseVelocity, maxPitch), minPitch);
		}
		else
		{
			source.pitch = 1f;
		}
	}
}

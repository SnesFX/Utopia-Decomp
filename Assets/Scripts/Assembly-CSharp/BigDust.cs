using UnityEngine;

public class BigDust : MonoBehaviour
{
	public CharacterMotor motor;

	public SpecialEffectHandler bigDust = new SpecialEffectHandler();

	public Breath breath;

	private float timer;

	private float timer2;

	private void FixedUpdate()
	{
		if (!motor || bigDust == null || !breath)
		{
			return;
		}
		if (motor.drive.magnitude >= 52f && motor.grounded && !breath.submerged)
		{
			if ((double)timer >= 0.7)
			{
				bigDust.Spawn(base.transform.position, base.transform.rotation);
				timer = 0f;
			}
			else if ((double)timer2 >= 0.2)
			{
				if ((double)Random.value >= 0.75)
				{
					bigDust.Spawn(base.transform.position, base.transform.rotation);
				}
				timer = 0f;
			}
			timer += Time.fixedDeltaTime;
			timer2 += Time.fixedDeltaTime;
		}
		else
		{
			timer = 0f;
			timer2 = 0f;
		}
	}
}

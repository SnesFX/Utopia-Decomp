using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class SpecialEffectPhysicalContext : SpecialEffect
	{
		private CharacterMotor _motor;

		[SerializeField]
		private GameObject _normalEffect;

		[SerializeField]
		private GameObject _waterSurfaceEffect;

		[SerializeField]
		private GameObject _underwaterEffect;

		public CharacterMotor motor
		{
			get
			{
				return _motor;
			}
			protected set
			{
				_motor = value;
			}
		}

		public GameObject normalEffect
		{
			get
			{
				return _normalEffect;
			}
		}

		public GameObject waterSurfaceEffect
		{
			get
			{
				return _waterSurfaceEffect;
			}
		}

		public GameObject underwaterEffect
		{
			get
			{
				return _underwaterEffect;
			}
		}

		public override void OnSpawn()
		{
			motor = GetComponentInParent<CharacterMotor>();
		}

		private void Update()
		{
			base.durationTimer -= Time.deltaTime;
			if (base.durationTimer == 0f)
			{
				Destroy();
			}
			if (!motor)
			{
				return;
			}
			if (motor.grounded)
			{
				if (motor.ground.isFluid)
				{
					if ((bool)normalEffect)
					{
						normalEffect.SetActive(false);
					}
					if ((bool)waterSurfaceEffect)
					{
						waterSurfaceEffect.SetActive(true);
					}
					if ((bool)underwaterEffect)
					{
						underwaterEffect.SetActive(false);
					}
				}
				else if (motor.physicsVolumeData.volumes.Count > 0)
				{
					if ((bool)normalEffect)
					{
						normalEffect.SetActive(false);
					}
					if ((bool)waterSurfaceEffect)
					{
						waterSurfaceEffect.SetActive(false);
					}
					if ((bool)underwaterEffect)
					{
						underwaterEffect.SetActive(true);
					}
				}
				else
				{
					if ((bool)normalEffect)
					{
						normalEffect.SetActive(true);
					}
					if ((bool)waterSurfaceEffect)
					{
						waterSurfaceEffect.SetActive(false);
					}
					if ((bool)underwaterEffect)
					{
						underwaterEffect.SetActive(false);
					}
				}
			}
			else if (motor.physicsVolumeData.volumes.Count > 0)
			{
				if ((bool)normalEffect)
				{
					normalEffect.SetActive(false);
				}
				if ((bool)waterSurfaceEffect)
				{
					waterSurfaceEffect.SetActive(false);
				}
				if ((bool)underwaterEffect)
				{
					underwaterEffect.SetActive(true);
				}
			}
		}
	}
}

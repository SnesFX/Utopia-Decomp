using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class RingDropperThresholdEffect
	{
		[SerializeField]
		private int _threshold;

		[SerializeField]
		private SpecialEffectHandler _effect = new SpecialEffectHandler();

		public int threshold
		{
			get
			{
				return Mathf.Clamp(_threshold, 0, int.MaxValue);
			}
			set
			{
				_threshold = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		public SpecialEffectHandler effect
		{
			get
			{
				return _effect;
			}
			set
			{
				_effect = value;
			}
		}
	}
}

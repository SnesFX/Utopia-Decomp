using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Spikes : MonoBehaviour
	{
		public enum SpikeType
		{
			Stationary = 0,
			Moving = 1
		}

		[SerializeField]
		private SpikeType _type;

		private Animation _animation;

		public SpikeType type
		{
			get
			{
				return _type;
			}
			protected set
			{
				_type = value;
			}
		}

		public Animation animation
		{
			get
			{
				return (!_animation) ? (_animation = GetComponent<Animation>()) : _animation;
			}
		}

		public void SetType(SpikeType oType)
		{
			type = oType;
			if ((bool)animation)
			{
				if (type == SpikeType.Moving)
				{
					animation.Play();
				}
				else
				{
					animation.Stop();
				}
			}
		}

		private void Awake()
		{
			SetType(type);
		}
	}
}

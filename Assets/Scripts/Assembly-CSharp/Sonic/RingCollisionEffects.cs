using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class RingCollisionEffects : MonoBehaviour
	{
		private AudioSource _audioSource;

		[SerializeField]
		private float _velocityVolumeMultiplier = 0.05f;

		[SerializeField]
		private float _velocityPitchMultiplier = 0.05f;

		[SerializeField]
		private float _pitchMin = 1f;

		[SerializeField]
		private float _pitchMax = 1.5f;

		public AudioSource audioSource
		{
			get
			{
				return (!_audioSource) ? (_audioSource = GetComponent<AudioSource>()) : _audioSource;
			}
		}

		public float velocityVolumeMultiplier
		{
			get
			{
				return Mathf.Clamp(_velocityVolumeMultiplier, 0f, float.MaxValue);
			}
		}

		public float velocityPitchMultiplier
		{
			get
			{
				return Mathf.Clamp(_velocityPitchMultiplier, 0f, float.MaxValue);
			}
		}

		public float pitchMin
		{
			get
			{
				return Mathf.Clamp(_pitchMin, 0f, float.MaxValue);
			}
		}

		public float pitchMax
		{
			get
			{
				return Mathf.Clamp(_pitchMax, 0f, float.MaxValue);
			}
		}

		private void OnCollisionEnter(Collision oCollision)
		{
			if ((bool)audioSource)
			{
				audioSource.pitch = Mathf.Lerp(pitchMin, pitchMax, oCollision.relativeVelocity.magnitude * velocityPitchMultiplier);
				audioSource.volume = Mathf.Lerp(0f, 1f, oCollision.relativeVelocity.magnitude * velocityVolumeMultiplier);
				audioSource.Play();
			}
		}
	}
}

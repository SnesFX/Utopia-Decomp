using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomAudioClip : MonoBehaviour
{
	[SerializeField]
	protected List<AudioClip> _audioClips = new List<AudioClip>();

	protected AudioSource _audioSource;

	public List<AudioClip> audioClips
	{
		get
		{
			return _audioClips;
		}
		set
		{
			_audioClips = value;
		}
	}

	public AudioSource audioSource
	{
		get
		{
			return (!_audioSource) ? (_audioSource = GetComponent<AudioSource>()) : _audioSource;
		}
	}

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		if ((bool)_audioSource && audioClips.Count > 0)
		{
			_audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
			if (_audioSource.playOnAwake)
			{
				_audioSource.Play();
			}
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Sonic
{
	[Serializable]
	public class MenuPause : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer _audioMixer;

		private bool _paused;

		[SerializeField]
		private string _musicVolumeName = "Music Volume";

		[SerializeField]
		private string _soundVolumeName = "Sound Effects Volume";

		[SerializeField]
		private string _pauseButton = string.Empty;

		[SerializeField]
		private string _cancelButton = string.Empty;

		[SerializeField]
		private GameObject _pauseMenu;

		public AudioMixer audioMixer
		{
			get
			{
				return _audioMixer;
			}
		}

		public string musicVolumeName
		{
			get
			{
				return _musicVolumeName;
			}
		}

		public string soundVolumeName
		{
			get
			{
				return _soundVolumeName;
			}
		}

		public bool paused
		{
			get
			{
				return _paused;
			}
			protected set
			{
				_paused = value;
			}
		}

		public string pauseButton
		{
			get
			{
				return _pauseButton;
			}
		}

		public GameObject pauseMenu
		{
			get
			{
				return _pauseMenu;
			}
		}

		public void Pause()
		{
			paused = true;
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			audioMixer.SetFloat(musicVolumeName, -80f);
			audioMixer.SetFloat(soundVolumeName, -80f);
			if ((bool)pauseMenu)
			{
				pauseMenu.SetActive(true);
			}
		}

		public void Unpause()
		{
			paused = false;
			Time.timeScale = 1f;
			audioMixer.SetFloat(musicVolumeName, 0f);
			audioMixer.SetFloat(soundVolumeName, 0f);
			if ((bool)pauseMenu)
			{
				pauseMenu.SetActive(false);
			}
		}

		private void Update()
		{
			if (pauseButton != string.Empty && Input.GetButtonDown(pauseButton) && (bool)pauseMenu)
			{
				if (paused)
				{
					Unpause();
				}
				else
				{
					Pause();
				}
			}
		}
	}
}

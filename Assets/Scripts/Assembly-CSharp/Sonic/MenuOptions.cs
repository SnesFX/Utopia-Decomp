using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sonic
{
	[Serializable]
	public class MenuOptions : MonoBehaviour
	{
		public float masterVolume;

		public float sfxVolume;

		public float musicVolume;

		public float uiVolume;

		public float mouseSensitivity;

		public float viewSensitivity = 1f;

		public bool invertMouseX;

		public bool invertMouseY;

		public bool invertViewX;

		public bool invertViewY;

		public bool motionBlur;

		public bool depthOfField;

		public bool bloom;

		public bool antialiasing;

		public Slider mouseSensitivitySlider;

		public Slider viewSensitivitySlider;

		public Toggle invertMouseXToggle;

		public Toggle invertMouseYToggle;

		public Toggle invertViewXToggle;

		public Toggle invertViewYToggle;

		public Toggle motionBlurToggle;

		public Toggle depthOfFieldToggle;

		public Toggle bloomToggle;

		public Toggle antialiasingToggle;

		public PlayerPref playerPref;

		public void SetMasterVolume(float fValue)
		{
			musicVolume = fValue;
			Apply();
		}

		public void SetMouseSensitivity(float fValue)
		{
			mouseSensitivity = fValue;
			Apply();
		}

		public void SetViewSensitivity(float fValue)
		{
			viewSensitivity = fValue;
			Apply();
		}

		public void SetInvertMouseX(bool bValue)
		{
			invertMouseX = bValue;
			Apply();
		}

		public void SetInvertMouseY(bool bValue)
		{
			invertMouseY = bValue;
			Apply();
		}

		public void SetInvertViewX(bool bValue)
		{
			invertViewX = bValue;
			Apply();
		}

		public void SetInvertViewY(bool bValue)
		{
			invertViewY = bValue;
			Apply();
		}

		public void SetMotionBlur(bool bValue)
		{
			motionBlur = bValue;
			Apply();
		}

		public void SetDepthOfField(bool bValue)
		{
			depthOfField = bValue;
			Apply();
		}

		public void SetBloom(bool bValue)
		{
			bloom = bValue;
			Apply();
		}

		public void SetAntialiasing(bool bValue)
		{
			antialiasing = bValue;
			Apply();
		}

		public void LoadPreferences()
		{
			masterVolume = PlayerPrefs.GetFloat("Master Volume", masterVolume);
			sfxVolume = PlayerPrefs.GetFloat("SFX Volume", sfxVolume);
			musicVolume = PlayerPrefs.GetFloat("Music Volume", musicVolume);
			uiVolume = PlayerPrefs.GetFloat("UI Volume", uiVolume);
			mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity", mouseSensitivity);
			viewSensitivity = PlayerPrefs.GetFloat("View Sensitivity", viewSensitivity);
			invertMouseX = PlayerPrefs.GetInt("Invert Mouse X", invertMouseX ? 1 : 0) != 0;
			invertMouseY = PlayerPrefs.GetInt("Invert Mouse Y", invertMouseY ? 1 : 0) != 0;
			invertViewX = PlayerPrefs.GetInt("Invert View X", invertViewX ? 1 : 0) != 0;
			invertViewY = PlayerPrefs.GetInt("Invert View Y", invertViewY ? 1 : 0) != 0;
			motionBlur = PlayerPrefs.GetInt("Motion Blur", motionBlur ? 1 : 0) != 0;
			depthOfField = PlayerPrefs.GetInt("Depth Of Field", depthOfField ? 1 : 0) != 0;
			bloom = PlayerPrefs.GetInt("Bloom", bloom ? 1 : 0) != 0;
			antialiasing = PlayerPrefs.GetInt("Antialiasing", antialiasing ? 1 : 0) != 0;
			if ((bool)mouseSensitivitySlider)
			{
				mouseSensitivitySlider.value = mouseSensitivity;
			}
			if ((bool)viewSensitivitySlider)
			{
				viewSensitivitySlider.value = viewSensitivity;
			}
			if ((bool)invertMouseXToggle)
			{
				invertMouseXToggle.isOn = invertMouseX;
			}
			if ((bool)invertMouseYToggle)
			{
				invertMouseYToggle.isOn = invertMouseY;
			}
			if ((bool)invertViewXToggle)
			{
				invertViewXToggle.isOn = invertViewX;
			}
			if ((bool)invertViewYToggle)
			{
				invertViewYToggle.isOn = invertViewY;
			}
			if ((bool)motionBlurToggle)
			{
				motionBlurToggle.isOn = motionBlur;
			}
			if ((bool)depthOfFieldToggle)
			{
				depthOfFieldToggle.isOn = depthOfField;
			}
			if ((bool)bloomToggle)
			{
				bloomToggle.isOn = bloom;
			}
			if ((bool)antialiasingToggle)
			{
				antialiasingToggle.isOn = antialiasing;
			}
		}

		public void SavePreferences()
		{
			PlayerPrefs.SetFloat("Master Volume", masterVolume);
			PlayerPrefs.SetFloat("SFX Volume", sfxVolume);
			PlayerPrefs.SetFloat("Music Volume", musicVolume);
			PlayerPrefs.SetFloat("UI Volume", uiVolume);
			PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity);
			PlayerPrefs.SetFloat("View Sensitivity", viewSensitivity);
			PlayerPrefs.SetInt("Invert Mouse X", invertMouseX ? 1 : 0);
			PlayerPrefs.SetInt("Invert Mouse Y", invertMouseY ? 1 : 0);
			PlayerPrefs.SetInt("Invert View X", invertViewX ? 1 : 0);
			PlayerPrefs.SetInt("Invert View Y", invertViewY ? 1 : 0);
			PlayerPrefs.SetInt("Motion Blur", motionBlur ? 1 : 0);
			PlayerPrefs.SetInt("Depth Of Field", depthOfField ? 1 : 0);
			PlayerPrefs.SetInt("Bloom", bloom ? 1 : 0);
			PlayerPrefs.SetInt("Antialiasing", antialiasing ? 1 : 0);
		}

		public void Apply()
		{
			if (playerPref != null)
			{
				SavePreferences();
				playerPref.Apply();
			}
		}

		private void Start()
		{
			LoadPreferences();
		}
	}
}

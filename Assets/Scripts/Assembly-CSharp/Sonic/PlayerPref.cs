using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Sonic
{
	public class PlayerPref : MonoBehaviour
	{
		public UserViewControls userViewControls;

		public AmplifyMotionEffect motionBlurScript;

		public DepthOfField depthOfFieldScript;

		public UltimateBloom bloomScript;

		public Antialiasing antiAliasingScript;

		private void Start()
		{
			Apply();
		}

		public void Apply()
		{
			AudioListener.volume = PlayerPrefs.GetFloat("Master Volume", AudioListener.volume);
			float @float = PlayerPrefs.GetFloat("View Sensitivity", 1f);
			userViewControls.mouseSensitivity = new Vector3(@float * 0.1f, @float * 0.1f, 1f);
			userViewControls.viewSensitivity = new Vector3(@float, @float, 1f);
			bool invertMouseX = userViewControls.invertMouseX;
			bool invertMouseY = userViewControls.invertMouseY;
			bool invertYaw = userViewControls.invertYaw;
			bool invertPitch = userViewControls.invertPitch;
			bool flag = (bool)motionBlurScript && motionBlurScript.enabled;
			bool flag2 = (bool)depthOfFieldScript && depthOfFieldScript.enabled;
			bool flag3 = (bool)bloomScript && bloomScript.enabled;
			bool flag4 = (bool)antiAliasingScript && antiAliasingScript.enabled;
			UserViewControls obj = userViewControls;
			bool flag5 = PlayerPrefs.GetInt("Invert View X", invertYaw ? 1 : 0) != 0;
			userViewControls.invertMouseX = flag5;
			obj.invertYaw = flag5;
			UserViewControls obj2 = userViewControls;
			flag5 = PlayerPrefs.GetInt("Invert View Y", invertPitch ? 1 : 0) != 0;
			userViewControls.invertMouseY = flag5;
			obj2.invertPitch = flag5;
			if ((bool)motionBlurScript)
			{
				motionBlurScript.enabled = PlayerPrefs.GetInt("Motion Blur", flag ? 1 : 0) != 0;
			}
			if ((bool)depthOfFieldScript)
			{
				depthOfFieldScript.enabled = PlayerPrefs.GetInt("Depth Of Field", flag2 ? 1 : 0) != 0;
			}
			if ((bool)bloomScript)
			{
				bloomScript.enabled = PlayerPrefs.GetInt("Bloom", flag3 ? 1 : 0) != 0;
			}
			if ((bool)antiAliasingScript)
			{
				antiAliasingScript.enabled = PlayerPrefs.GetInt("Antialiasing", flag4 ? 1 : 0) != 0;
			}
		}
	}
}

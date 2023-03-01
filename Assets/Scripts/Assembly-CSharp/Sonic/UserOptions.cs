using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Sonic
{
	[Serializable]
	public class UserOptions : MonoBehaviour
	{
		public UserViewControls userViewControls;

		public AmplifyMotionEffect motionBlurScript;

		public DepthOfField depthOfFieldScript;

		public UltimateBloom bloomScript;

		public ScreenSpaceAmbientOcclusion ambientOcclusionScript;

		public SunShafts sunShaftsScript;
	}
}

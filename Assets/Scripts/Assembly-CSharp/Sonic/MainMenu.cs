using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonic
{
	[Serializable]
	public class MainMenu : MonoBehaviour
	{
		[SerializeField]
		private List<CanvasRenderer> _splashScreens = new List<CanvasRenderer>();

		[SerializeField]
		private CanvasRenderer _mainPanel;

		[SerializeField]
		private CanvasRenderer _currentPanel;

		[SerializeField]
		private Graphic _blackScreen;

		[SerializeField]
		private float _splashScreenFadeDuration = 0.5f;

		[SerializeField]
		private float _splashScreenDuration = 3f;

		[SerializeField]
		private float _panelTransitionDuration = 0.5f;

		private bool _isBusy;

		public List<CanvasRenderer> splashScreens
		{
			get
			{
				return _splashScreens;
			}
		}

		public Graphic blackScreen
		{
			get
			{
				return _blackScreen;
			}
		}

		public CanvasRenderer mainPanel
		{
			get
			{
				return _mainPanel;
			}
		}

		public CanvasRenderer currentPanel
		{
			get
			{
				return _currentPanel;
			}
			protected set
			{
				_currentPanel = value;
			}
		}

		public float panelTransitionDuration
		{
			get
			{
				return Mathf.Clamp(_panelTransitionDuration, 0f, float.MaxValue);
			}
		}

		public float splashScreenFadeDuration
		{
			get
			{
				return Mathf.Clamp(_splashScreenFadeDuration, 0f, float.MaxValue);
			}
		}

		public float splashScreenDuration
		{
			get
			{
				return Mathf.Clamp(_splashScreenDuration, 0f, float.MaxValue);
			}
		}

		public bool isBusy
		{
			get
			{
				return _isBusy;
			}
			protected set
			{
				_isBusy = value;
			}
		}

		private IEnumerator TransitionToPanelCoroutine(CanvasRenderer oObject)
		{
			if (isBusy)
			{
				yield break;
			}
			isBusy = true;
			if ((bool)oObject)
			{
				GameplayHandler.instance.FadeUIGraphic(blackScreen, 0.00390625f, 1f, panelTransitionDuration);
				yield return new WaitForSecondsRealtime(panelTransitionDuration);
				if ((bool)currentPanel)
				{
					currentPanel.gameObject.SetActive(false);
				}
				oObject.gameObject.SetActive(true);
				currentPanel = oObject;
				GameplayHandler.instance.FadeUIGraphic(blackScreen, 1f, 0.00390625f, panelTransitionDuration);
				yield return new WaitForSecondsRealtime(panelTransitionDuration);
			}
			isBusy = false;
		}

		private IEnumerator CycleSplashScreens()
		{
			if (isBusy)
			{
				yield break;
			}
			isBusy = true;
			if ((bool)blackScreen)
			{
				blackScreen.color = new Color(0f, 0f, 0f, 1f);
				if ((bool)currentPanel)
				{
					currentPanel.gameObject.SetActive(false);
				}
				for (int i = 0; i < splashScreens.Count; i++)
				{
					splashScreens[i].gameObject.SetActive(true);
					GameplayHandler.instance.FadeUIGraphic(blackScreen, 1f, 0.00390625f, splashScreenFadeDuration);
					yield return new WaitForSecondsRealtime(splashScreenFadeDuration);
					yield return new WaitForSecondsRealtime(splashScreenDuration);
					GameplayHandler.instance.FadeUIGraphic(blackScreen, 0.00390625f, 1f, splashScreenFadeDuration);
					yield return new WaitForSecondsRealtime(splashScreenFadeDuration);
					splashScreens[i].gameObject.SetActive(false);
				}
			}
			if ((bool)mainPanel)
			{
				mainPanel.gameObject.SetActive(true);
			}
			if ((bool)currentPanel)
			{
				currentPanel.gameObject.SetActive(true);
			}
			GameplayHandler.instance.FadeUIGraphic(blackScreen, 1f, 0.00390625f, splashScreenFadeDuration);
			yield return new WaitForSecondsRealtime(splashScreenFadeDuration);
			isBusy = false;
		}

		public void LoadScene(int i)
		{
			GameplayHandler.instance.LoadScene(i);
		}

		public void TransitionToPanel(CanvasRenderer oObject)
		{
			StartCoroutine(TransitionToPanelCoroutine(oObject));
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		public void SplashScreensToMainMenu()
		{
			StartCoroutine(CycleSplashScreens());
		}

		private void Start()
		{
			SplashScreensToMainMenu();
		}
	}
}

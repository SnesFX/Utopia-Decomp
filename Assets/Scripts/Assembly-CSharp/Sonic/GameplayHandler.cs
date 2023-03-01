using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonic
{
	[Serializable]
	public class GameplayHandler : MonoBehaviour
	{
		[SerializeField]
		private GameplayHandler _prefab;

		private static GameplayHandler _instance;

		private static EventSystem _eventSystem;

		private User _user;

		[SerializeField]
		private Image _blackScreen;

		[SerializeField]
		private Image _whiteScreen;

		[SerializeField]
		private Image _loadingScreen;

		[SerializeField]
		private float _sceneCrossfadeDuration = 1f;

		[SerializeField]
		private float _screenCrossfadeDuration = 1f;

		[SerializeField]
		private int _mainMenuSceneIndex;

		private bool _isLevelLoaded;

		private static bool isInstantiated
		{
			get
			{
				return _instance;
			}
		}

		public GameplayHandler prefab
		{
			get
			{
				return _prefab;
			}
		}

		public static GameplayHandler instance
		{
			get
			{
				if ((bool)_instance)
				{
					return _instance;
				}
				_instance = UnityEngine.Object.FindObjectOfType<GameplayHandler>();
				if ((bool)_instance)
				{
					return _instance;
				}
				return _instance = new GameObject("Gameplay Handler").AddComponent<GameplayHandler>();
			}
		}

		public static EventSystem eventSystem
		{
			get
			{
				return _eventSystem ? _eventSystem : (_eventSystem = new GameObject("Event System").AddComponent<EventSystem>());
			}
		}

		public User user
		{
			get
			{
				return (!_user) ? (_user = GetComponentInChildren<User>()) : _user;
			}
			protected set
			{
				_user = value;
			}
		}

		public Image blackScreen
		{
			get
			{
				return _blackScreen;
			}
		}

		public Image whiteScreen
		{
			get
			{
				return _whiteScreen;
			}
		}

		public Image loadingScreen
		{
			get
			{
				return _loadingScreen;
			}
		}

		public float sceneCrossfadeDuration
		{
			get
			{
				return Mathf.Clamp(_sceneCrossfadeDuration, 0f, float.MaxValue);
			}
		}

		public float screenCrossfadeDuration
		{
			get
			{
				return Mathf.Clamp(_screenCrossfadeDuration, 0f, float.MaxValue);
			}
		}

		public int mainMenuSceneIndex
		{
			get
			{
				return _mainMenuSceneIndex;
			}
		}

		protected bool isLevelLoaded
		{
			get
			{
				return _isLevelLoaded;
			}
			set
			{
				_isLevelLoaded = value;
			}
		}

		public IEnumerator FadeUIGraphicCoroutine(Graphic oGraphic, float fFrom, float fTo, float fDuration)
		{
			if (!oGraphic)
			{
				yield break;
			}
			if (fDuration > 0f)
			{
				oGraphic.gameObject.SetActive(true);
				for (float f = 0f; f <= 1f; f += Time.deltaTime / fDuration)
				{
					if (f > 0.95f)
					{
						f = 1f;
					}
					if ((bool)oGraphic)
					{
						oGraphic.color = new Color(0f, 0f, 0f, Mathf.Lerp(fFrom, fTo, f));
					}
					yield return null;
				}
				if ((bool)oGraphic && fTo < 0.05f)
				{
					oGraphic.gameObject.SetActive(false);
				}
			}
			else
			{
				oGraphic.color = new Color(0f, 0f, 0f, fTo);
			}
		}

		public IEnumerator FadeAudioVolumeCoroutine(AudioSource oSource, float fFrom, float fTo, float fDuration)
		{
			if (!oSource)
			{
				yield break;
			}
			if (fDuration > 0f)
			{
				oSource.enabled = true;
				for (float f = 0f; f <= 1f; f += Time.deltaTime / fDuration)
				{
					if (f > 0.95f)
					{
						f = 1f;
					}
					oSource.volume = Mathf.Lerp(fFrom, fTo, f);
					yield return null;
				}
				if (fTo < 0.05f)
				{
					oSource.enabled = false;
				}
			}
			else
			{
				oSource.volume = fTo;
			}
		}

		public void FadeUIGraphic(Graphic oGraphic, float fFrom, float fTo, float fDuration)
		{
			StartCoroutine(FadeUIGraphicCoroutine(oGraphic, fFrom, fTo, fDuration));
		}

		public void FadeAudioVolume(AudioSource oSource, float fFrom, float fTo, float fDuration)
		{
			StartCoroutine(FadeAudioVolumeCoroutine(oSource, fFrom, fTo, fDuration));
		}

		private IEnumerator LoadSceneCoroutine(int i)
		{
			yield return StartCoroutine(FadeUIGraphicCoroutine(blackScreen, 0.05f, 1f, sceneCrossfadeDuration));
			if ((bool)loadingScreen)
			{
				loadingScreen.gameObject.SetActive(true);
			}
			StartCoroutine(FadeUIGraphicCoroutine(loadingScreen, 0.00390625f, 1f, sceneCrossfadeDuration));
			isLevelLoaded = false;
			SceneManager.LoadSceneAsync(i, LoadSceneMode.Single);
			while (!isLevelLoaded)
			{
				yield return null;
			}
			if ((bool)loadingScreen)
			{
				loadingScreen.color = new Color(1f, 1f, 1f, 0.00390625f);
				loadingScreen.gameObject.SetActive(false);
			}
			yield return StartCoroutine(FadeUIGraphicCoroutine(blackScreen, 1f, 0.05f, sceneCrossfadeDuration));
		}

		public void LoadScene(int i)
		{
			StartCoroutine(LoadSceneCoroutine(i));
		}

		private void Awake()
		{
			if (isInstantiated && _instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				_instance = this;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnLevelWasLoaded(int i)
		{
			isLevelLoaded = true;
			if (i == mainMenuSceneIndex && (bool)user)
			{
				if ((bool)user.possessedPawn)
				{
					UnityEngine.Object.Destroy(user.possessedPawn.gameObject);
				}
				if ((bool)user.possessedCamera)
				{
					UnityEngine.Object.Destroy(user.possessedCamera.gameObject);
				}
			}
		}
	}
}

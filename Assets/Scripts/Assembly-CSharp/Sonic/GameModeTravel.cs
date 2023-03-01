using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Sonic
{
	[Serializable]
	public class GameModeTravel : GameMode
	{
		[SerializeField]
		private AudioSource _backgroundMusic;

		[SerializeField]
		private Image _blackScreen;

		[SerializeField]
		private Image _whiteScreen;

		[SerializeField]
		private User _user;

		[SerializeField]
		private Item _lifeItem;

		[SerializeField]
		private Item _ringItem;

		[SerializeField]
		private Checkpoint _currentCheckpoint;

		private List<Checkpoint> _checkpoints = new List<Checkpoint>();

		[SerializeField]
		private AudioMixer _audioMixer;

		[SerializeField]
		private Animator _titleCard;

		[SerializeField]
		private string _titleCardTrigger;

		[SerializeField]
		private float _titleCardDuration = 1.5f;

		[SerializeField]
		private float _titleCardOutroDuration = 1f;

		[SerializeField]
		private float _respawnFadeDelay = 0.5f;

		[SerializeField]
		private float _respawnFadeOutDuration = 0.5f;

		[SerializeField]
		private float _respawnFadeInDuration = 0.5f;

		[SerializeField]
		private float _respawnWaitDuration = 1f;

		[SerializeField]
		private Graphic _gameOverScreen;

		[SerializeField]
		private float _gameOverFadeDelay = 0.5f;

		[SerializeField]
		private float _gameOverFadeDuration = 0.5f;

		[SerializeField]
		private float _gameOverDuration = 3f;

		[SerializeField]
		private GoalObject _goalObject;

		[SerializeField]
		private Graphic _goalScreen;

		[SerializeField]
		private float _goalFadeDelay = 0.5f;

		[SerializeField]
		private float _goalFadeDuration = 1f;

		[SerializeField]
		private float _goalDuration = 5f;

		private bool _isEnded;

		public User user
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		public AudioSource backgroundMusic
		{
			get
			{
				return _backgroundMusic;
			}
			set
			{
				_backgroundMusic = value;
			}
		}

		public Image blackScreen
		{
			get
			{
				return _blackScreen;
			}
			set
			{
				_blackScreen = value;
			}
		}

		public Image whiteScreen
		{
			get
			{
				return _whiteScreen;
			}
			set
			{
				_whiteScreen = value;
			}
		}

		public Checkpoint currentCheckpoint
		{
			get
			{
				return _currentCheckpoint;
			}
			set
			{
				_currentCheckpoint = value;
			}
		}

		public List<Checkpoint> checkpoints
		{
			get
			{
				return _checkpoints;
			}
			set
			{
				_checkpoints = value;
			}
		}

		public AudioMixer audioMixer
		{
			get
			{
				return _audioMixer;
			}
		}

		public Animator titleCard
		{
			get
			{
				return _titleCard;
			}
		}

		public string titleCardTrigger
		{
			get
			{
				return _titleCardTrigger;
			}
		}

		public float titleCardDuration
		{
			get
			{
				return Mathf.Clamp(_titleCardDuration, 0f, float.MaxValue);
			}
		}

		public float titleCardOutroDuration
		{
			get
			{
				return Mathf.Clamp(_titleCardOutroDuration, 0f, float.MaxValue);
			}
		}

		public Item lifeItem
		{
			get
			{
				return _lifeItem;
			}
		}

		public Item ringItem
		{
			get
			{
				return _ringItem;
			}
		}

		public float respawnFadeDelay
		{
			get
			{
				return Mathf.Clamp(_respawnFadeDelay, 0f, float.MaxValue);
			}
		}

		public float respawnFadeOutDuration
		{
			get
			{
				return Mathf.Clamp(_respawnFadeOutDuration, 0f, float.MaxValue);
			}
		}

		public float respawnFadeInDuration
		{
			get
			{
				return Mathf.Clamp(_respawnFadeInDuration, 0f, float.MaxValue);
			}
		}

		public float respawnWaitDuration
		{
			get
			{
				return Mathf.Clamp(_respawnWaitDuration, 0f, float.MaxValue);
			}
		}

		public Graphic gameOverScreen
		{
			get
			{
				return _gameOverScreen;
			}
		}

		public float gameOverFadeDelay
		{
			get
			{
				return Mathf.Clamp(_gameOverFadeDelay, 0f, float.MaxValue);
			}
		}

		public float gameOverFadeDuration
		{
			get
			{
				return Mathf.Clamp(_gameOverFadeDuration, 0f, float.MaxValue);
			}
		}

		public float gameOverDuration
		{
			get
			{
				return Mathf.Clamp(_gameOverDuration, 0f, float.MaxValue);
			}
		}

		public GoalObject goalObject
		{
			get
			{
				return _goalObject;
			}
		}

		public Graphic goalScreen
		{
			get
			{
				return _goalScreen;
			}
		}

		public float goalFadeDelay
		{
			get
			{
				return Mathf.Clamp(_goalFadeDelay, 0f, float.MaxValue);
			}
		}

		public float goalFadeDuration
		{
			get
			{
				return Mathf.Clamp(_goalFadeDuration, 0f, float.MaxValue);
			}
		}

		public float goalDuraton
		{
			get
			{
				return Mathf.Clamp(_goalDuration, 0f, float.MaxValue);
			}
		}

		public bool isEnded
		{
			get
			{
				return _isEnded;
			}
			set
			{
				_isEnded = value;
			}
		}

		private void HandleCheckpointActivated(object o, ActorEventArgs e)
		{
			if (e.context is ContextCheckpointActivated)
			{
				Checkpoint checkpoint = o as Checkpoint;
				if ((bool)currentCheckpoint)
				{
					currentCheckpoint.Passive();
				}
				currentCheckpoint = checkpoint;
			}
		}

		private void HandleUserPawnDeath(object o, ActorEventArgs e)
		{
			if (e.context is ContextDeath && (bool)user && (bool)user.possessedPawn)
			{
				ActorInventory inventory = user.possessedPawn.inventory;
				int num = 0;
				if ((bool)inventory && (num = inventory.GetItemTotalQuantity(lifeItem)) > 0)
				{
					inventory.RemoveItem(lifeItem, 1);
					inventory.RemoveAllOfItem(ringItem);
					RespawnAtCheckpoint();
				}
				else
				{
					GameOver();
				}
			}
		}

		private void HandleGoalActivated(object o, ActorEventArgs e)
		{
			if (e.context is ContextGoal)
			{
				Goal();
			}
		}

		public void PlayTitleCard()
		{
			if ((bool)titleCard && titleCardTrigger != string.Empty)
			{
				titleCard.SetTrigger(titleCardTrigger);
			}
		}

		private IEnumerator IntroCoroutine()
		{
			if ((bool)blackScreen)
			{
				blackScreen.gameObject.SetActive(true);
				blackScreen.color = new Color(0f, 0f, 0f, 1f);
			}
			for (float f2 = 0f; f2 < titleCardDuration; f2 += Time.deltaTime)
			{
				yield return null;
			}
			GameplayHandler.instance.FadeUIGraphic(blackScreen, 1f, 0.00390625f, respawnFadeInDuration);
			for (float f = 0f; f < titleCardOutroDuration; f += Time.deltaTime)
			{
				yield return null;
			}
			if ((bool)titleCard)
			{
				titleCard.gameObject.SetActive(false);
			}
		}

		private IEnumerator RespawnCoroutine()
		{
			float fVolume = ((!backgroundMusic) ? 0f : backgroundMusic.volume);
			yield return new WaitForSecondsRealtime(respawnFadeDelay);
			GameplayHandler.instance.FadeUIGraphic(blackScreen, 0.00390625f, 1f, respawnFadeOutDuration);
			GameplayHandler.instance.FadeAudioVolume(backgroundMusic, fVolume, 0f, respawnFadeOutDuration);
			yield return new WaitForSecondsRealtime(respawnFadeOutDuration);
			Time.timeScale = 0f;
			if ((bool)user && (bool)user.possessedPawn && (bool)currentCheckpoint)
			{
				if ((bool)currentCheckpoint.spawnPoint)
				{
					user.possessedPawn.spawnPosition = currentCheckpoint.spawnPoint.transform.position;
					user.possessedPawn.spawnRotation = currentCheckpoint.spawnPoint.transform.rotation;
				}
				else
				{
					user.possessedPawn.spawnPosition = currentCheckpoint.transform.position;
					user.possessedPawn.spawnRotation = currentCheckpoint.transform.rotation;
				}
			}
			for (int i = 0; i < GameMode.actors.Count; i++)
			{
				if ((bool)GameMode.actors[i] && (bool)GameMode.actors[i])
				{
					GameMode.actors[i].Respawn();
					if ((bool)GameMode.actors[i].physics)
					{
						GameMode.actors[i].physics.velocity = Vector3.zero;
						GameMode.actors[i].physics.angularVelocity = Vector3.zero;
					}
				}
			}
			if ((bool)user.possessedCamera && (bool)user.possessedPawn)
			{
				user.possessedCamera.viewPosition = Quaternion.AngleAxis(17.5f, user.possessedPawn.transform.right) * user.possessedPawn.transform.forward * -3.5f;
				user.possessedCamera.viewRotation = Quaternion.AngleAxis(17.5f, user.possessedPawn.transform.right) * user.possessedPawn.transform.rotation;
				user.possessedCamera.transform.position = user.possessedCamera.viewPosition;
				user.possessedCamera.transform.rotation = user.possessedCamera.viewRotation;
				CameraStateThirdPerson oState = user.possessedCamera.GetComponent<CameraStateThirdPerson>();
				if ((bool)oState)
				{
					oState.desiredPosition = user.possessedCamera.viewPosition;
					oState.desiredRotation = user.possessedCamera.viewRotation;
				}
			}
			yield return new WaitForSecondsRealtime(respawnWaitDuration);
			Time.timeScale = 1f;
			if ((bool)backgroundMusic)
			{
				backgroundMusic.enabled = true;
				backgroundMusic.Play();
				backgroundMusic.volume = fVolume;
			}
			GameplayHandler.instance.FadeUIGraphic(blackScreen, 1f, 0.00390625f, respawnFadeInDuration);
			yield return new WaitForSecondsRealtime(respawnFadeInDuration);
		}

		private IEnumerator GameOverCoroutine()
		{
			float fVolume = ((!backgroundMusic) ? 0f : backgroundMusic.volume);
			GameplayHandler.instance.FadeAudioVolume(backgroundMusic, fVolume, 0f, gameOverFadeDelay);
			yield return new WaitForSecondsRealtime(gameOverFadeDelay);
			GameplayHandler.instance.FadeUIGraphic(gameOverScreen, 0.00390625f, 1f, gameOverFadeDuration);
			yield return new WaitForSecondsRealtime(gameOverFadeDuration);
			Time.timeScale = 0f;
			yield return new WaitForSecondsRealtime(gameOverDuration);
			Time.timeScale = 1f;
			GameplayHandler.instance.LoadScene(GameplayHandler.instance.mainMenuSceneIndex);
		}

		private IEnumerator GoalCoroutine()
		{
			float fVolume = ((!backgroundMusic) ? 0f : backgroundMusic.volume);
			GameplayHandler.instance.FadeAudioVolume(backgroundMusic, fVolume, 0f, goalFadeDelay);
			yield return new WaitForSecondsRealtime(goalFadeDelay);
			if ((bool)audioMixer)
			{
				audioMixer.SetFloat("Sound Effects Volume", -80f);
			}
			GameplayHandler.instance.FadeUIGraphic(goalScreen, 0.00390625f, 1f, goalFadeDuration);
			yield return new WaitForSecondsRealtime(goalFadeDuration);
			yield return new WaitForSecondsRealtime(goalDuraton);
			if ((bool)audioMixer)
			{
				audioMixer.SetFloat("Sound Effects Volume", 0f);
			}
			GameplayHandler.instance.LoadScene(GameplayHandler.instance.mainMenuSceneIndex);
		}

		public void RespawnAtCheckpoint()
		{
			GameMode.actors.RemoveAll((Actor o) => o == null);
			StartCoroutine(RespawnCoroutine());
		}

		public void GameOver()
		{
			if (!isEnded)
			{
				StartCoroutine(GameOverCoroutine());
			}
		}

		public void Goal()
		{
			if (!isEnded)
			{
				StartCoroutine(GoalCoroutine());
			}
		}

		public void ExitStage()
		{
			GameMode.actors.RemoveAll((Actor o) => o == null);
			GameplayHandler.instance.LoadScene(GameplayHandler.instance.mainMenuSceneIndex);
		}

		private void Awake()
		{
			user = UnityEngine.Object.FindObjectOfType<User>();
			Checkpoint[] array = UnityEngine.Object.FindObjectsOfType<Checkpoint>();
			Actor[] array2 = UnityEngine.Object.FindObjectsOfType<Actor>();
			checkpoints.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				checkpoints.Add(array[i]);
				checkpoints[i].Activated += HandleCheckpointActivated;
			}
			if ((bool)user && (bool)user.possessedPawn)
			{
				user.possessedPawn.Death += HandleUserPawnDeath;
			}
			if ((bool)goalObject)
			{
				goalObject.Triggered += HandleGoalActivated;
			}
			StartCoroutine(IntroCoroutine());
		}
	}
}

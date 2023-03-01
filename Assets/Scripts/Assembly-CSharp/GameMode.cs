using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameMode : MonoBehaviour
{
	private static GameMode _instance;

	private static List<Actor> _actors = new List<Actor>();

	public static GameMode instance
	{
		get
		{
			return _instance ? _instance : (_instance = new GameObject("Game Mode").AddComponent<GameMode>());
		}
		private set
		{
			_instance = value;
		}
	}

	public static List<Actor> actors
	{
		get
		{
			return _actors;
		}
	}

	public static void SetGameMode(GameMode oMode)
	{
		if ((bool)oMode && !_instance)
		{
			_instance = oMode;
		}
	}

	public static void AddActorRegister(Actor oActor)
	{
		if ((bool)oActor && !actors.Contains(oActor))
		{
			actors.Add(oActor);
		}
	}

	public static void RemoveActorRegister(Actor oActor)
	{
		actors.Remove(oActor);
	}

	private void Awake()
	{
		SetGameMode(this);
	}
}

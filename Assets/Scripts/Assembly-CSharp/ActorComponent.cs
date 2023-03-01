using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Actor))]
public class ActorComponent : MonoBehaviour
{
	protected Actor _actor;

	public Actor actor
	{
		get
		{
			return (!_actor) ? GetComponent<Actor>() : _actor;
		}
	}
}

using System;
using UnityEngine;

[Serializable]
public class AbilityEffect : MonoBehaviour
{
	protected Ability _abilityBehaviour;

	protected Animator _animator;

	public Ability abilityBehaviour
	{
		get
		{
			return _abilityBehaviour;
		}
	}

	public Animator animator
	{
		get
		{
			return _animator;
		}
	}

	private void Awake()
	{
		_abilityBehaviour = GetComponentInParent<Ability>();
	}
}

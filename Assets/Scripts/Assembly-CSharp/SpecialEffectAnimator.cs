using System;
using UnityEngine;

[Serializable]
public class SpecialEffectAnimator : SpecialEffect
{
	private Animator _animator;

	[SerializeField]
	private string _activeLabel = string.Empty;

	public Animator animator
	{
		get
		{
			return (!_animator) ? (_animator = GetComponent<Animator>()) : _animator;
		}
	}

	public string activeLabel
	{
		get
		{
			return _activeLabel;
		}
	}

	public override void OnSpawn()
	{
		if ((bool)animator && activeLabel != string.Empty)
		{
			animator.SetBool(activeLabel, true);
		}
	}

	public override void OnRefresh()
	{
		if ((bool)animator && activeLabel != string.Empty)
		{
			animator.SetBool(activeLabel, true);
		}
	}

	public override void OnDestroyEffect()
	{
		if ((bool)animator && activeLabel != string.Empty)
		{
			animator.SetBool(activeLabel, false);
		}
	}
}

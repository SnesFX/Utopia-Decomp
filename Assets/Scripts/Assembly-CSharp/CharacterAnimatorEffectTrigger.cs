using System;
using UnityEngine;

[Serializable]
public class CharacterAnimatorEffectTrigger : StateMachineBehaviour
{
	public enum TriggerBehaviour
	{
		None = 0,
		Spawn = 1,
		Refresh = 2,
		Destroy = 3
	}

	[SerializeField]
	private string _effectName = string.Empty;

	[SerializeField]
	private TriggerBehaviour _entryBehaviour;

	[SerializeField]
	private TriggerBehaviour _updateBehaviour;

	[SerializeField]
	private TriggerBehaviour _exitBehaviour;

	public string effectName
	{
		get
		{
			return _effectName;
		}
	}

	public TriggerBehaviour entryBehaviour
	{
		get
		{
			return _entryBehaviour;
		}
	}

	public TriggerBehaviour updateBehaviour
	{
		get
		{
			return _updateBehaviour;
		}
	}

	public TriggerBehaviour exitBehaviour
	{
		get
		{
			return _exitBehaviour;
		}
	}

	public void Trigger(CharacterAvatar oAvatar, TriggerBehaviour oBehaviour)
	{
		if ((bool)oAvatar && (bool)oAvatar && effectName != string.Empty)
		{
			switch (oBehaviour)
			{
			case TriggerBehaviour.Spawn:
				oAvatar.SpawnEffect(effectName);
				break;
			case TriggerBehaviour.Refresh:
				oAvatar.RefreshEffect(effectName);
				break;
			case TriggerBehaviour.Destroy:
				oAvatar.DestroyEffect(effectName);
				break;
			}
		}
	}

	public override void OnStateMachineEnter(Animator oAnimator, int iStateMachinePathHash)
	{
		if (entryBehaviour != 0)
		{
			CharacterAvatar component = oAnimator.GetComponent<CharacterAvatar>();
			if ((bool)component)
			{
				Trigger(component, entryBehaviour);
			}
		}
	}

	public override void OnStateMachineExit(Animator oAnimator, int iStateMachinePathHash)
	{
		if (exitBehaviour != 0)
		{
			CharacterAvatar component = oAnimator.GetComponent<CharacterAvatar>();
			if ((bool)component)
			{
				Trigger(component, entryBehaviour);
			}
		}
	}

	public override void OnStateEnter(Animator oAnimator, AnimatorStateInfo oStateInfo, int oLayerIndex)
	{
		if (entryBehaviour != 0)
		{
			CharacterAvatar component = oAnimator.GetComponent<CharacterAvatar>();
			if ((bool)component)
			{
				Trigger(component, entryBehaviour);
			}
		}
	}

	public override void OnStateUpdate(Animator oAnimator, AnimatorStateInfo oStateInfo, int oLayerIndex)
	{
		if (updateBehaviour != 0)
		{
			CharacterAvatar component = oAnimator.GetComponent<CharacterAvatar>();
			if ((bool)component)
			{
				Trigger(component, updateBehaviour);
			}
		}
	}

	public override void OnStateExit(Animator oAnimator, AnimatorStateInfo oStateInfo, int oLayerIndex)
	{
		if (exitBehaviour != 0)
		{
			CharacterAvatar component = oAnimator.GetComponent<CharacterAvatar>();
			if ((bool)component)
			{
				Trigger(component, exitBehaviour);
			}
		}
	}
}

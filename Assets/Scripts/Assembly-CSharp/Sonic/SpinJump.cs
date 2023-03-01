using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class SpinJump : AbilityBehaviour
	{
		[SerializeField]
		private string _jumpEffectName = string.Empty;

		public string jumpEffectName
		{
			get
			{
				return _jumpEffectName;
			}
			set
			{
				_jumpEffectName = value;
			}
		}

		public override void Engage(Ability o)
		{
			if (!o.actor | !o.motor)
			{
				return;
			}
			CharacterStateRoll component = o.actor.GetComponent<CharacterStateRoll>();
			Ability ability = o.actorAbilityComponent.FindAbility("Spindash");
			Spindash spindash = ((!ability.behaviour || !(ability.behaviour is Spindash)) ? null : (ability.behaviour as Spindash));
			bool flag = (bool)spindash && ability.active;
			bool flag2 = (bool)component && ((component.substate == CharacterStateRoll.Substate.Crouch) | (component.substate == CharacterStateRoll.Substate.Charge));
			if (flag && flag2 && o.motor.state == component)
			{
				spindash.ChargeSpindash(ability, spindash.chargeRate);
			}
			else if (o.motor.grounded && o.CanActivate())
			{
				Activate(o);
				if (o.active)
				{
					component = o.motor.AddOrActivateState<CharacterStateRoll>();
					if ((bool)component)
					{
						component.substate = CharacterStateRoll.Substate.Free;
						if (o.motor.grounded && (bool)o.motor.defaultState)
						{
							if (jumpEffectName != string.Empty && (bool)o.character.avatar)
							{
								o.character.avatar.SpawnEffect(jumpEffectName);
							}
							o.motor.ApplyJumpForce(o.motor.ground.normal, o.character.GetAttributeValue("Jump Power"), o.character.GetAttributeValue("Jump Height"));
							o.motor.TriggerJumped();
						}
					}
				}
			}
			Deactivate(o);
		}

		public override void Disengage(Ability o)
		{
			if ((bool)o.motor && (bool)o.motor.state)
			{
				o.motor.state.DisengageJump();
			}
		}
	}
}

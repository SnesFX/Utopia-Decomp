using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Peelout : AbilityBehaviour
	{
		[SerializeField]
		private float _chargeRate = 24f;

		[SerializeField]
		private float _chargeMin;

		[SerializeField]
		private float _chargeMax = 60f;

		[SerializeField]
		private string _sustainChargeEffectName = string.Empty;

		[SerializeField]
		private string _peeloutEffectName = string.Empty;

		[SerializeField]
		private float _peeloutEffectThreshold = 10f;

		public float chargeRate
		{
			get
			{
				return Mathf.Clamp(_chargeRate, 0f, float.MaxValue);
			}
			set
			{
				_chargeRate = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float chargeMin
		{
			get
			{
				return Mathf.Clamp(_chargeMin, 0f, float.MaxValue);
			}
			set
			{
				_chargeMin = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float chargeMax
		{
			get
			{
				return Mathf.Clamp(_chargeMax, 0f, float.MaxValue);
			}
			set
			{
				_chargeMax = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public string sustainChargeEffectName
		{
			get
			{
				return _sustainChargeEffectName;
			}
		}

		public string peeloutEffectName
		{
			get
			{
				return _peeloutEffectName;
			}
		}

		public float peeloutEffectThreshold
		{
			get
			{
				return Mathf.Clamp(_peeloutEffectThreshold, 0f, float.MaxValue);
			}
		}

		public override void Engage(Ability o)
		{
			Activate(o);
			if (!o.active)
			{
				return;
			}
			float num = 0f;
			if (!o.variables.ContainsKey("charge"))
			{
				o.variables.Add("charge", 0f);
			}
			o.variables["charge"] = num;
			if ((bool)o.motor)
			{
				CharacterStatePeelout characterStatePeelout = o.motor.AddOrActivateState<CharacterStatePeelout>();
				if ((bool)characterStatePeelout)
				{
					characterStatePeelout.sustained = true;
				}
			}
		}

		public override void Disengage(Ability o)
		{
			Deactivate(o);
			CharacterStatePeelout characterStatePeelout = ((!o.motor) ? null : o.motor.GetComponent<CharacterStatePeelout>());
			if ((bool)characterStatePeelout && (bool)o.motor && o.motor.state == characterStatePeelout)
			{
				float num = 0f;
				if (!o.variables.ContainsKey("charge"))
				{
					o.variables.Add("charge", 0f);
				}
				num = Mathf.Clamp((float)o.variables["charge"], chargeMin, chargeMax);
				if (characterStatePeelout.substate == CharacterStatePeelout.Substate.Charge)
				{
					ApplyPeelout(o, num);
					return;
				}
				characterStatePeelout.sustained = false;
				o.motor.SetStateSelection(CharacterStateSelection.Default);
			}
		}

		public override void OnActivate(Ability o)
		{
		}

		public override void OnDeactivate(Ability o)
		{
		}

		public override void OnUpdate(Ability o)
		{
			if (!o.active)
			{
				return;
			}
			CharacterStatePeelout component = o.actor.GetComponent<CharacterStatePeelout>();
			if ((bool)component)
			{
				if (component.substate != 0)
				{
					return;
				}
				float num = 0f;
				if (!o.variables.ContainsKey("charge"))
				{
					o.variables.Add("charge", 0f);
				}
				num = Mathf.Clamp((float)o.variables["charge"] + chargeRate * Time.deltaTime, chargeMin, chargeMax);
				o.variables["charge"] = num;
				component.charge = num;
				if ((bool)o.motor.character.avatar && sustainChargeEffectName != string.Empty)
				{
					if (o.motor.grounded)
					{
						o.motor.character.avatar.SpawnEffect(sustainChargeEffectName);
					}
					else
					{
						o.motor.character.avatar.DestroyEffect(sustainChargeEffectName);
					}
				}
			}
			else
			{
				Deactivate(o);
			}
		}

		public void ApplyPeelout(Ability o, float fCharge)
		{
			if (!o.motor)
			{
				return;
			}
			CharacterStatePeelout component = o.motor.GetComponent<CharacterStatePeelout>();
			if (!component)
			{
				return;
			}
			if (o.motor.grounded)
			{
				Vector3 vector = ((!o.motor.grounded) ? o.motor.up : o.motor.ground.normal);
				Vector3 normalized = Utility.RelativeVector(o.motor.transform.forward, vector).normalized;
				normalized = normalized.normalized * Mathf.Clamp(fCharge, Vector3.Dot(o.motor.velocity, normalized.normalized), fCharge);
				o.motor.velocity = normalized + Utility.VectorInDirection(o.motor.velocity, vector);
				o.motor.drive = Quaternion.Inverse(o.motor.transform.rotation) * o.motor.velocity;
			}
			o.motor.SetStateSelection(CharacterStateSelection.Default);
			if ((bool)o.character.avatar)
			{
				if (sustainChargeEffectName != string.Empty)
				{
					o.character.avatar.DestroyEffect(sustainChargeEffectName);
				}
				if (o.motor.grounded && fCharge > peeloutEffectThreshold && peeloutEffectName != string.Empty)
				{
					o.character.avatar.SpawnEffect(peeloutEffectName);
				}
			}
		}
	}
}

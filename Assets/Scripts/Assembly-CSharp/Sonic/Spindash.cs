using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Spindash : AbilityBehaviour
	{
		[SerializeField]
		private float _chargeRate = 10f;

		[SerializeField]
		private float _chargeFalloffRate = 10f;

		[SerializeField]
		private float _chargeMin = 5f;

		[SerializeField]
		private float _chargeMax = 50f;

		[SerializeField]
		private float _rollEffectThreshold = 1f;

		[SerializeField]
		private float _chargePitchMin = 1f;

		[SerializeField]
		private float _chargePitchMax = 2f;

		[SerializeField]
		private string _rollEffectName = string.Empty;

		[SerializeField]
		private string _chargeEffectName = string.Empty;

		[SerializeField]
		private string _sustainChargeEffectName = string.Empty;

		[SerializeField]
		private string _spindashEffectName = string.Empty;

		[SerializeField]
		private float _spindashEffectThreshold = 10f;

		[SerializeField]
		private bool _releaseRollOnDisengage;

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

		public float chargeFalloffRate
		{
			get
			{
				return Mathf.Clamp(_chargeFalloffRate, 0f, float.MaxValue);
			}
			set
			{
				_chargeFalloffRate = Mathf.Clamp(value, 0f, float.MaxValue);
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

		public float rollEffectThreshold
		{
			get
			{
				return Mathf.Clamp(_rollEffectThreshold, 0f, float.MaxValue);
			}
			set
			{
				_rollEffectThreshold = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float chargePitchMin
		{
			get
			{
				return Mathf.Clamp(_chargePitchMin, 0f, float.MaxValue);
			}
		}

		public float chargePitchMax
		{
			get
			{
				return Mathf.Clamp(_chargePitchMax, 0f, float.MaxValue);
			}
		}

		public string rollEffectName
		{
			get
			{
				return _rollEffectName;
			}
		}

		public string chargeEffectName
		{
			get
			{
				return _chargeEffectName;
			}
		}

		public string sustainChargeEffectName
		{
			get
			{
				return _sustainChargeEffectName;
			}
		}

		public string spindashEffectName
		{
			get
			{
				return _spindashEffectName;
			}
		}

		public float spindashEffectThreshold
		{
			get
			{
				return Mathf.Clamp(_spindashEffectThreshold, 0f, float.MaxValue);
			}
		}

		public bool releaseRollOnDisengage
		{
			get
			{
				return _releaseRollOnDisengage;
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
				if (o.motor.grounded && o.motor.drive.magnitude > rollEffectThreshold && !(o.motor.state is CharacterStateRoll) && rollEffectName != string.Empty && (bool)o.character.avatar)
				{
					o.character.avatar.SpawnEffect(rollEffectName);
				}
				CharacterStateRoll characterStateRoll = o.motor.AddOrActivateState<CharacterStateRoll>();
				if ((bool)characterStateRoll)
				{
					characterStateRoll.sustained = true;
				}
			}
		}

		public override void Disengage(Ability o)
		{
			Deactivate(o);
			CharacterStateRoll characterStateRoll = ((!o.motor) ? null : o.motor.GetComponent<CharacterStateRoll>());
			if ((bool)characterStateRoll && (bool)o.motor && o.motor.state == characterStateRoll)
			{
				float num = 0f;
				if (!o.variables.ContainsKey("charge"))
				{
					o.variables.Add("charge", 0f);
				}
				num = Mathf.Clamp((float)o.variables["charge"], chargeMin, chargeMax);
				if (characterStateRoll.substate == CharacterStateRoll.Substate.Charge)
				{
					ApplySpindash(o, num);
				}
				else if (releaseRollOnDisengage)
				{
					characterStateRoll.sustained = false;
					o.motor.SetStateSelection(CharacterStateSelection.Default);
				}
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
			CharacterStateRoll component = o.actor.GetComponent<CharacterStateRoll>();
			if ((bool)component)
			{
				if (component.substate != CharacterStateRoll.Substate.Charge)
				{
					return;
				}
				float num = 0f;
				if (!o.variables.ContainsKey("charge"))
				{
					o.variables.Add("charge", 0f);
				}
				num = Mathf.Clamp((float)o.variables["charge"] - chargeFalloffRate * Time.deltaTime, chargeMin, chargeMax);
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

		public void ChargeSpindash(Ability o, float fCharge)
		{
			if (!o.motor)
			{
				return;
			}
			CharacterStateRoll component = o.motor.GetComponent<CharacterStateRoll>();
			if (!component)
			{
				return;
			}
			component.substate = CharacterStateRoll.Substate.Charge;
			float num = 0f;
			if (!o.variables.ContainsKey("charge"))
			{
				o.variables.Add("charge", 0f);
			}
			num = Mathf.Clamp((float)o.variables["charge"] + fCharge, chargeMin, chargeMax);
			o.variables["charge"] = num;
			component.charge = num;
			if (!(chargeEffectName != string.Empty) || !o.character.avatar)
			{
				return;
			}
			SpecialEffectHandler specialEffectHandler = o.character.avatar.effectHandlers.Find((SpecialEffectHandler oH) => oH.name == chargeEffectName);
			if (specialEffectHandler == null)
			{
				return;
			}
			specialEffectHandler.Spawn();
			if ((bool)specialEffectHandler.activeObject)
			{
				AudioSource component2 = specialEffectHandler.activeObject.GetComponent<AudioSource>();
				if ((bool)component2)
				{
					component2.pitch = Mathf.Lerp(chargePitchMin, chargePitchMax, (num - chargeMin) / ((!(chargeMax - chargeMin > 1f)) ? 1f : (chargeMax - chargeMin)));
				}
			}
		}

		public void ApplySpindash(Ability o, float fCharge)
		{
			if (!o.motor)
			{
				return;
			}
			CharacterStateRoll component = o.motor.GetComponent<CharacterStateRoll>();
			if (!component)
			{
				return;
			}
			Vector3 vector = ((!o.motor.grounded) ? o.motor.up : o.motor.ground.normal);
			Vector3 normalized = Utility.RelativeVector(o.motor.transform.forward, vector).normalized;
			normalized = normalized.normalized * Mathf.Clamp(fCharge, Vector3.Dot(o.motor.velocity, normalized.normalized), fCharge);
			o.motor.velocity = normalized + Utility.VectorInDirection(o.motor.velocity, vector);
			o.motor.drive = Quaternion.Inverse(o.motor.transform.rotation) * o.motor.velocity;
			component.substate = CharacterStateRoll.Substate.Roll;
			if (releaseRollOnDisengage)
			{
				component.sustained = false;
			}
			if ((bool)o.character.avatar)
			{
				if (sustainChargeEffectName != string.Empty)
				{
					o.character.avatar.DestroyEffect(sustainChargeEffectName);
				}
				if (o.motor.grounded && fCharge > spindashEffectThreshold && spindashEffectName != string.Empty)
				{
					o.character.avatar.SpawnEffect(spindashEffectName);
				}
			}
		}
	}
}

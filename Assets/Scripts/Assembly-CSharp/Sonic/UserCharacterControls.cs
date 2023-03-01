using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class UserCharacterControls : MonoBehaviour
	{
		private User _user;

		private UserInterface _userInterface;

		[Range(0f, 1f)]
		[SerializeField]
		private float _walkThreshold = 0.1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _runThreshold = 0.5f;

		[Range(0f, 1f)]
		[SerializeField]
		private float _sprintThreshold = 0.95f;

		[SerializeField]
		private float _minWalkSpeed = 0.2f;

		[SerializeField]
		private float _walkSpeed = 3.2f;

		[SerializeField]
		private string _driveX = "Horizontal";

		[SerializeField]
		private string _driveY = string.Empty;

		[SerializeField]
		private string _driveZ = "Vertical";

		[SerializeField]
		private string _jump = "Jump";

		[SerializeField]
		private string _drop = "Fire2";

		[SerializeField]
		private List<CharacterAbilityInput> _abilityInput = new List<CharacterAbilityInput>();

		[SerializeField]
		private string _jumpAbility = string.Empty;

		[SerializeField]
		private string _dropAbility = string.Empty;

		public User user
		{
			get
			{
				return (!_user) ? (_user = GetComponent<User>()) : _user;
			}
		}

		public UserInterface userInterface
		{
			get
			{
				return (!_userInterface) ? (_userInterface = GetComponent<UserInterface>()) : _userInterface;
			}
		}

		public float walkThreshold
		{
			get
			{
				return _walkThreshold;
			}
			set
			{
				_walkThreshold = value;
			}
		}

		public float runThreshold
		{
			get
			{
				return _runThreshold;
			}
			set
			{
				_runThreshold = value;
			}
		}

		public float sprintThreshold
		{
			get
			{
				return _sprintThreshold;
			}
			set
			{
				_sprintThreshold = value;
			}
		}

		public float minWalkSpeed
		{
			get
			{
				return _minWalkSpeed;
			}
			set
			{
				_minWalkSpeed = value;
			}
		}

		public float walkSpeed
		{
			get
			{
				return _walkSpeed;
			}
			set
			{
				_walkSpeed = value;
			}
		}

		public string driveX
		{
			get
			{
				return _driveX;
			}
		}

		public string driveY
		{
			get
			{
				return _driveY;
			}
		}

		public string driveZ
		{
			get
			{
				return _driveZ;
			}
		}

		public string jump
		{
			get
			{
				return _jump;
			}
		}

		public string drop
		{
			get
			{
				return _drop;
			}
		}

		public List<CharacterAbilityInput> abilityInput
		{
			get
			{
				return _abilityInput;
			}
		}

		public string jumpAbility
		{
			get
			{
				return _jumpAbility;
			}
		}

		public string dropAbility
		{
			get
			{
				return _dropAbility;
			}
		}

		private void FixedUpdate()
		{
			Quaternion qView = Quaternion.identity;
			User user = this.user;
			Character character = null;
			CharacterMotor characterMotor = null;
			if (!user)
			{
				user = userInterface.user;
			}
			if ((bool)user)
			{
				if ((bool)user.possessedCamera)
				{
					qView = user.possessedCamera.transform.rotation;
				}
				if ((bool)user.possessedPawn && user.possessedPawn is Character)
				{
					character = user.possessedPawn as Character;
				}
			}
			if ((bool)character)
			{
				characterMotor = character.motor;
			}
			if (!character || !characterMotor || !characterMotor.state)
			{
				return;
			}
			Quaternion identity = Quaternion.identity;
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			float speed = characterMotor.state.GetSpeed();
			float topSpeed = characterMotor.state.GetTopSpeed();
			vector.x = ((!(driveX == string.Empty)) ? Input.GetAxis(driveX) : 0f);
			vector.y = ((!(driveY == string.Empty)) ? Input.GetAxis(driveY) : 0f);
			vector.z = ((!(driveZ == string.Empty)) ? Input.GetAxis(driveZ) : 0f);
			if (characterMotor.state is CharacterStateFly)
			{
				zero2 = ((vector.magnitude > sprintThreshold) ? (vector.normalized * topSpeed) : ((vector.magnitude > runThreshold) ? (vector.normalized * Mathf.Lerp(walkSpeed, speed, (vector.magnitude - runThreshold) / (speed - walkSpeed))) : ((!(vector.magnitude > walkThreshold)) ? Vector3.zero : (vector.normalized * Mathf.Lerp(minWalkSpeed, walkSpeed, (vector.magnitude - walkThreshold) / (walkSpeed - minWalkSpeed))))));
				identity = characterMotor.state.GetControlRotation(qView);
				zero2 = Quaternion.Inverse(characterMotor.transform.rotation) * (identity * zero2);
				zero3 = Quaternion.Inverse(characterMotor.transform.rotation) * (identity * vector);
				characterMotor.desiredDrive = zero2;
				characterMotor.desiredAngle = zero3;
				return;
			}
			if (characterMotor.state is CharacterStateSwim)
			{
				if (!(vector.magnitude > sprintThreshold) && !(vector.magnitude > runThreshold) && !(vector.magnitude > walkThreshold))
				{
				}
				return;
			}
			if (characterMotor.grounded)
			{
				vector = Utility.RelativeVector(vector, Vector3.up);
			}
			zero2 = ((vector.magnitude > sprintThreshold) ? (vector.normalized * topSpeed) : ((vector.magnitude > runThreshold && sprintThreshold - runThreshold > 0f) ? (vector.normalized * Mathf.Lerp(walkSpeed, speed, (vector.magnitude - runThreshold) / (sprintThreshold - runThreshold))) : ((!(vector.magnitude > walkThreshold) || !(runThreshold - walkThreshold > 0f)) ? Vector3.zero : (vector.normalized * Mathf.Lerp(minWalkSpeed, walkSpeed, (vector.magnitude - walkThreshold) / (runThreshold - walkThreshold))))));
			identity = characterMotor.state.GetControlRotation(qView);
			zero2 = Quaternion.Inverse(characterMotor.transform.rotation) * (identity * zero2);
			zero3 = Quaternion.Inverse(characterMotor.transform.rotation) * (identity * (vector - Vector3.up * vector.y));
			characterMotor.desiredDrive = zero2;
			characterMotor.desiredAngle = zero3;
		}

		private void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			User user = this.user;
			Character character = null;
			CharacterMotor characterMotor = null;
			if (!user)
			{
				user = userInterface.user;
			}
			if ((bool)user && (bool)user.possessedPawn && user.possessedPawn is Character)
			{
				character = user.possessedPawn as Character;
			}
			if ((bool)character)
			{
				characterMotor = character.motor;
			}
			if (!character || !characterMotor || !characterMotor.state)
			{
				return;
			}
			bool flag = !(jump == string.Empty) && Input.GetButtonUp(jump);
			bool flag2 = !(jump == string.Empty) && Input.GetButtonDown(jump);
			bool flag3 = !(drop == string.Empty) && Input.GetButtonUp(drop);
			bool flag4 = !(drop == string.Empty) && Input.GetButtonDown(drop);
			if (flag2)
			{
				if ((bool)character.abilityComponent && jumpAbility != string.Empty)
				{
					character.abilityComponent.EngageAbility(jumpAbility);
				}
				else if ((bool)characterMotor)
				{
					characterMotor.EngageJump();
				}
			}
			else if (flag)
			{
				if ((bool)character.abilityComponent && jumpAbility != string.Empty)
				{
					character.abilityComponent.DisengageAbility(jumpAbility);
				}
				else if ((bool)characterMotor)
				{
					characterMotor.DisengageJump();
				}
			}
			if (flag4)
			{
				if ((bool)character.abilityComponent && dropAbility != string.Empty)
				{
					character.abilityComponent.DisengageAbility(dropAbility);
				}
				else
				{
					characterMotor.EngageDrop();
				}
			}
			else if (flag3)
			{
				if ((bool)character.abilityComponent && dropAbility != string.Empty)
				{
					character.abilityComponent.DisengageAbility(dropAbility);
				}
				else if ((bool)characterMotor)
				{
					characterMotor.DisengageDrop();
				}
			}
			if (!character.abilityComponent)
			{
				return;
			}
			ActorAbilityComponent abilityComponent = character.abilityComponent;
			for (int i = 0; i < abilityInput.Count; i++)
			{
				if ((abilityInput[i].name != string.Empty) & (abilityInput[i].abilityName != string.Empty))
				{
					if (Input.GetButtonDown(abilityInput[i].name))
					{
						abilityComponent.EngageAbility(abilityInput[i].abilityName);
					}
					if (Input.GetButtonUp(abilityInput[i].name))
					{
						abilityComponent.DisengageAbility(abilityInput[i].abilityName);
					}
				}
			}
		}
	}
}

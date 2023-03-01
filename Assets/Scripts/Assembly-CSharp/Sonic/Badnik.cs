using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Badnik : Character
	{
		[SerializeField]
		private SpecialEffectHandler _bustEffect = new SpecialEffectHandler();

		[SerializeField]
		private List<GameObject> _smallAnimals = new List<GameObject>();

		private Vector3 _desiredDrive = Vector3.zero;

		private Vector3 _desiredAngle = Vector3.zero;

		[SerializeField]
		private float _speed = 15f;

		[SerializeField]
		private float _rotationSpeed = 360f;

		public SpecialEffectHandler bustEffect
		{
			get
			{
				return _bustEffect;
			}
		}

		public List<GameObject> smallAnimals
		{
			get
			{
				return _smallAnimals;
			}
		}

		public Vector3 desiredDrive
		{
			get
			{
				return _desiredDrive;
			}
			set
			{
				_desiredDrive = value;
			}
		}

		public Vector3 desiredAngle
		{
			get
			{
				return _desiredAngle;
			}
			set
			{
				_desiredAngle = value;
			}
		}

		public float speed
		{
			get
			{
				return Mathf.Clamp(_speed, 0f, float.MaxValue);
			}
			set
			{
				_speed = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public float rotateSpeed
		{
			get
			{
				return Mathf.Clamp(_rotationSpeed, 0f, float.MaxValue);
			}
			set
			{
				_rotationSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public void SpawnSmallAnimal()
		{
			Vector3 position = base.transform.position;
			if ((bool)base.motor)
			{
				position = base.motor.worldCenter;
			}
			if (smallAnimals.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, smallAnimals.Count);
				GameObject gameObject = UnityEngine.Object.Instantiate(smallAnimals[index]);
				gameObject.transform.position = position;
				gameObject.transform.rotation = base.transform.rotation;
			}
		}

		public override void OnDeath()
		{
			bustEffect.Spawn();
			SpawnSmallAnimal();
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
			base.gameObject.SetActive(false);
		}

		public override void OnRespawn()
		{
			base.alive = true;
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			base.transform.position = base.spawnPosition;
			base.transform.rotation = base.spawnRotation;
			base.gameObject.SetActive(true);
		}
	}
}

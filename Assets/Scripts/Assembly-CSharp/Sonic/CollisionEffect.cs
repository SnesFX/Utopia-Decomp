using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CollisionEffect : MonoBehaviour
	{
		[SerializeField]
		private SpecialEffect _effectPrefab;

		[SerializeField]
		private LayerMask _waterLayers;

		[SerializeField]
		private float _threshold = 10f;

		[SerializeField]
		private bool _applyOnExit = true;

		public SpecialEffect effectPrefab
		{
			get
			{
				return _effectPrefab;
			}
		}

		public LayerMask waterLayers
		{
			get
			{
				return _waterLayers;
			}
		}

		public float threshold
		{
			get
			{
				return _threshold;
			}
		}

		public bool applyOnExit
		{
			get
			{
				return _applyOnExit;
			}
		}

		private void OnTriggerEnter(Collider oCollider)
		{
			if (!oCollider.isTrigger && (bool)oCollider.attachedRigidbody && Vector3.Dot(oCollider.attachedRigidbody.velocity, Physics.gravity.normalized) > threshold)
			{
				ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
				if ((bool)component && component.physicsVolumeData.volumes.Count < 2)
				{
					SpecialEffect specialEffect = UnityEngine.Object.Instantiate(effectPrefab);
					specialEffect.transform.position = oCollider.transform.position;
					specialEffect.transform.rotation = oCollider.transform.rotation;
				}
			}
		}

		private void OnTriggerExit(Collider oCollider)
		{
			if (applyOnExit && !oCollider.isTrigger && (bool)oCollider.attachedRigidbody && Vector3.Dot(oCollider.attachedRigidbody.velocity, -Physics.gravity.normalized) > threshold)
			{
				ActorPhysics component = oCollider.GetComponent<ActorPhysics>();
				if ((bool)component && component.physicsVolumeData.volumes.Count == 0)
				{
					SpecialEffect specialEffect = UnityEngine.Object.Instantiate(effectPrefab);
					specialEffect.transform.position = oCollider.transform.position;
					specialEffect.transform.rotation = oCollider.transform.rotation;
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class HomingAttackTarget : MonoBehaviour
	{
		private SphereCollider _sphereCollider;

		[SerializeField]
		private Vector3 _offset = Vector3.zero;

		public SphereCollider sphereCollider
		{
			get
			{
				return (!_sphereCollider) ? (_sphereCollider = GetComponent<SphereCollider>()) : _sphereCollider;
			}
		}

		public Vector3 offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		public Vector3 worldOffset
		{
			get
			{
				return base.transform.position + base.transform.rotation * offset;
			}
		}

		public float radius
		{
			get
			{
				return (!sphereCollider) ? 0f : sphereCollider.radius;
			}
		}
	}
}

using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class SceneRingHandler : MonoBehaviour
	{
		[SerializeField]
		private float _rotationRate = 360f;

		public float rotationRate
		{
			get
			{
				return _rotationRate;
			}
			set
			{
				_rotationRate = value;
			}
		}

		private void Update()
		{
			SceneRing.globalAngle += rotationRate * Time.deltaTime;
		}
	}
}

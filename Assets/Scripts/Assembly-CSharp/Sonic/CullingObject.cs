using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CullingObject : MonoBehaviour
	{
		public float cullingDistance = 400f;

		private void Awake()
		{
			if ((bool)CullingHandler.instance)
			{
				CullingHandler.instance.AddObject(this);
			}
		}

		private void OnDestroy()
		{
			if ((bool)CullingHandler.instance)
			{
				CullingHandler.instance.RemoveObject(this);
			}
		}
	}
}

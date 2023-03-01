using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class SelectMonitorPrefab : MonoBehaviour
	{
		public enum MonitorType
		{
			Rings = 0,
			Lives = 1,
			Shoes = 2,
			Barrier = 3,
			Invincibility = 4
		}

		public MonitorType type;

		public Monitor activeMonitor;

		public Monitor rings;

		public Monitor lives;

		public Monitor shoes;

		public Monitor barrier;

		public Monitor invincibility;

		private void Awake()
		{
			if ((bool)activeMonitor)
			{
				UnityEngine.Object.Destroy(activeMonitor.gameObject);
			}
			switch (type)
			{
			case MonitorType.Rings:
				activeMonitor = UnityEngine.Object.Instantiate(rings);
				break;
			case MonitorType.Lives:
				activeMonitor = UnityEngine.Object.Instantiate(lives);
				break;
			case MonitorType.Shoes:
				activeMonitor = UnityEngine.Object.Instantiate(shoes);
				break;
			case MonitorType.Barrier:
				activeMonitor = UnityEngine.Object.Instantiate(barrier);
				break;
			case MonitorType.Invincibility:
				activeMonitor = UnityEngine.Object.Instantiate(invincibility);
				break;
			}
			if ((bool)activeMonitor)
			{
				activeMonitor.transform.parent = base.transform;
				activeMonitor.transform.localPosition = Vector3.zero;
				activeMonitor.transform.localRotation = Quaternion.identity;
			}
		}
	}
}

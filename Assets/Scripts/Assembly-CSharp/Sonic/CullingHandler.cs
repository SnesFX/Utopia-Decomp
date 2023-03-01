using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class CullingHandler : MonoBehaviour
	{
		private List<CullingObject> _objects = new List<CullingObject>();

		private static CullingHandler _instance;

		private static bool _isApplicationQuitting;

		public List<CullingObject> objects
		{
			get
			{
				return _objects;
			}
		}

		public static bool isApplicationQuitting
		{
			get
			{
				return _isApplicationQuitting;
			}
			protected set
			{
				_isApplicationQuitting = value;
			}
		}

		public static CullingHandler instance
		{
			get
			{
				return ((bool)_instance | isApplicationQuitting) ? _instance : (_instance = new GameObject("Culling Handler").AddComponent<CullingHandler>());
			}
			protected set
			{
				_instance = value;
			}
		}

		public void AddObject(CullingObject oObject)
		{
			objects.Add(oObject);
		}

		public void RemoveObject(CullingObject oObject)
		{
			objects.Remove(oObject);
		}

		private void Awake()
		{
			instance = this;
		}

		private void OnApplicationQuit()
		{
			isApplicationQuitting = true;
		}

		private void Update()
		{
			if ((bool)Camera.main)
			{
				Vector3 position = Camera.main.transform.position;
				for (int i = 0; i < objects.Count; i++)
				{
					objects[i].gameObject.SetActive((position - objects[i].transform.position).magnitude < objects[i].cullingDistance);
				}
			}
		}
	}
}

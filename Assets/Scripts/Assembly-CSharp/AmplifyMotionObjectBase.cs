using System;
using System.Collections.Generic;
using AmplifyMotion;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("")]
public class AmplifyMotionObjectBase : MonoBehaviour
{
	public enum MinMaxCurveState
	{
		Scalar = 0,
		Curve = 1,
		TwoCurves = 2,
		TwoScalars = 3
	}

	internal static bool ApplyToChildren = true;

	[SerializeField]
	private bool m_applyToChildren = ApplyToChildren;

	private ObjectType m_type;

	private Dictionary<Camera, MotionState> m_states = new Dictionary<Camera, MotionState>();

	private bool m_fixedStep;

	private int m_objectId;

	internal bool FixedStep
	{
		get
		{
			return m_fixedStep;
		}
	}

	internal int ObjectId
	{
		get
		{
			return m_objectId;
		}
	}

	public ObjectType Type
	{
		get
		{
			return m_type;
		}
	}

	internal void RegisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		if ((component.cullingMask & (1 << base.gameObject.layer)) != 0 && !m_states.ContainsKey(component))
		{
			MotionState motionState = null;
			switch (m_type)
			{
			case ObjectType.Solid:
				motionState = new SolidState(camera, this);
				break;
			case ObjectType.Skinned:
				motionState = new SkinnedState(camera, this);
				break;
			case ObjectType.Cloth:
				motionState = new ClothState(camera, this);
				break;
			case ObjectType.Particle:
				motionState = new ParticleState(camera, this);
				break;
			default:
				throw new Exception("[AmplifyMotion] Invalid object type.");
			}
			camera.RegisterObject(this);
			m_states.Add(component, motionState);
		}
	}

	internal void UnregisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		MotionState value;
		if (m_states.TryGetValue(component, out value))
		{
			camera.UnregisterObject(this);
			if (m_states.TryGetValue(component, out value))
			{
				value.Shutdown();
			}
			m_states.Remove(component);
		}
	}

	private bool InitializeType()
	{
		Renderer component = GetComponent<Renderer>();
		if (AmplifyMotionEffectBase.CanRegister(base.gameObject, false))
		{
			ParticleSystem component2 = GetComponent<ParticleSystem>();
			if (component2 != null)
			{
				m_type = ObjectType.Particle;
				AmplifyMotionEffectBase.RegisterObject(this);
			}
			else if (component != null)
			{
				if (component.GetType() == typeof(MeshRenderer))
				{
					m_type = ObjectType.Solid;
				}
				else if (component.GetType() == typeof(SkinnedMeshRenderer))
				{
					if (GetComponent<Cloth>() != null)
					{
						m_type = ObjectType.Cloth;
					}
					else
					{
						m_type = ObjectType.Skinned;
					}
				}
				AmplifyMotionEffectBase.RegisterObject(this);
			}
		}
		return component != null;
	}

	private void OnEnable()
	{
		bool flag = InitializeType();
		if (flag)
		{
			if (m_type == ObjectType.Cloth)
			{
				m_fixedStep = false;
			}
			else if (m_type == ObjectType.Solid)
			{
				Rigidbody component = GetComponent<Rigidbody>();
				if (component != null && component.interpolation == RigidbodyInterpolation.None && !component.isKinematic)
				{
					m_fixedStep = true;
				}
			}
		}
		if (m_applyToChildren)
		{
			foreach (Transform item in base.gameObject.transform)
			{
				AmplifyMotionEffectBase.RegisterRecursivelyS(item.gameObject);
			}
		}
		if (!flag)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		AmplifyMotionEffectBase.UnregisterObject(this);
	}

	private void TryInitializeStates()
	{
		Dictionary<Camera, MotionState>.Enumerator enumerator = m_states.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MotionState value = enumerator.Current.Value;
			if (value.Owner.Initialized && !value.Error && !value.Initialized)
			{
				value.Initialize();
			}
		}
	}

	private void Start()
	{
		if (AmplifyMotionEffectBase.Instance != null)
		{
			TryInitializeStates();
		}
	}

	private void Update()
	{
		if (AmplifyMotionEffectBase.Instance != null)
		{
			TryInitializeStates();
		}
	}

	internal void OnUpdateTransform(Camera camera, CommandBuffer updateCB, bool starting)
	{
		MotionState value;
		if (m_states.TryGetValue(camera, out value) && !value.Error)
		{
			value.UpdateTransform(updateCB, starting);
		}
	}

	internal void OnRenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
	{
		MotionState value;
		if (m_states.TryGetValue(camera, out value) && !value.Error)
		{
			value.RenderVectors(camera, renderCB, scale, quality);
		}
	}

	internal void OnRenderDebugHUD(Camera camera)
	{
		MotionState value;
		if (m_states.TryGetValue(camera, out value) && !value.Error)
		{
			value.RenderDebugHUD();
		}
	}
}

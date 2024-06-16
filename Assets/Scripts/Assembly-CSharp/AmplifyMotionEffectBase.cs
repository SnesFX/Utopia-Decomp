using System;
using System.Collections.Generic;
using System.Linq;
using AmplifyMotion;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class AmplifyMotionEffectBase : MonoBehaviour
{
	private const CameraEvent m_updateCBEvent = CameraEvent.BeforeImageEffectsOpaque;

	private const CameraEvent m_fixedUpdateCBEvent = CameraEvent.BeforeImageEffectsOpaque;

	private const CameraEvent m_renderCBEvent = CameraEvent.BeforeImageEffects;

	public Quality QualityLevel = Quality.Standard;

	public bool AutoRegisterObjs = true;

	public Camera[] OverlayCameras = new Camera[0];

	public LayerMask CullingMask = -1;

	public int QualitySteps = 1;

	public float MotionScale = 3f;

	public float CameraMotionMult = 1f;

	public float MinVelocity = 1f;

	public float MaxVelocity = 10f;

	public float DepthThreshold = 0.001f;

	[FormerlySerializedAs("workerThreads")]
	public int WorkerThreads;

	public bool SystemThreadPool;

	public bool ForceCPUOnly;

	public bool DebugMode;

	private Camera m_camera;

	private bool m_starting = true;

	private int m_width;

	private int m_height;

	private RenderTexture m_motionRT;

	private Material m_blurMaterial;

	private Material m_solidVectorsMaterial;

	private Material m_skinnedVectorsMaterial;

	private Material m_clothVectorsMaterial;

	private Material m_reprojectionMaterial;

	private Material m_combineMaterial;

	private Material m_dilationMaterial;

	private Material m_depthMaterial;

	private Material m_debugMaterial;

	private Dictionary<Camera, AmplifyMotionCamera> m_linkedCameras = new Dictionary<Camera, AmplifyMotionCamera>();

	internal Camera[] m_linkedCameraKeys;

	internal AmplifyMotionCamera[] m_linkedCameraValues;

	internal bool m_linkedCamerasChanged = true;

	private AmplifyMotionPostProcess m_currentPostProcess;

	private int m_globalObjectId = 1;

	private float m_deltaTime;

	private float m_fixedDeltaTime;

	private float m_motionScaleNorm;

	private float m_fixedMotionScaleNorm;

	private Quality m_qualityLevel;

	private AmplifyMotionCamera m_baseCamera;

	private WorkerThreadPool m_workerThreadPool;

	public static Dictionary<GameObject, AmplifyMotionObjectBase> m_activeObjects = new Dictionary<GameObject, AmplifyMotionObjectBase>();

	public static Dictionary<Camera, AmplifyMotionCamera> m_activeCameras = new Dictionary<Camera, AmplifyMotionCamera>();

	private static bool m_isD3D = false;

	private bool m_canUseGPU;

	private CommandBuffer m_updateCB;

	private CommandBuffer m_fixedUpdateCB;

	private CommandBuffer m_renderCB;

	private static bool m_ignoreMotionScaleWarning = false;

	private static AmplifyMotionEffectBase m_firstInstance = null;

	[Obsolete("workerThreads is deprecated, please use WorkerThreads instead.")]
	public int workerThreads
	{
		get
		{
			return WorkerThreads;
		}
		set
		{
			WorkerThreads = value;
		}
	}

	internal Material SolidVectorsMaterial
	{
		get
		{
			return m_solidVectorsMaterial;
		}
	}

	internal Material SkinnedVectorsMaterial
	{
		get
		{
			return m_skinnedVectorsMaterial;
		}
	}

	internal Material ClothVectorsMaterial
	{
		get
		{
			return m_clothVectorsMaterial;
		}
	}

	internal RenderTexture MotionRenderTexture
	{
		get
		{
			return m_motionRT;
		}
	}

	public Dictionary<Camera, AmplifyMotionCamera> LinkedCameras
	{
		get
		{
			return m_linkedCameras;
		}
	}

	internal float MotionScaleNorm
	{
		get
		{
			return m_motionScaleNorm;
		}
	}

	internal float FixedMotionScaleNorm
	{
		get
		{
			return m_fixedMotionScaleNorm;
		}
	}

	public AmplifyMotionCamera BaseCamera
	{
		get
		{
			return m_baseCamera;
		}
	}

	internal WorkerThreadPool WorkerPool
	{
		get
		{
			return m_workerThreadPool;
		}
	}

	public static bool IsD3D
	{
		get
		{
			return m_isD3D;
		}
	}

	public bool CanUseGPU
	{
		get
		{
			return m_canUseGPU;
		}
	}

	public static bool IgnoreMotionScaleWarning
	{
		get
		{
			return m_ignoreMotionScaleWarning;
		}
	}

	public static AmplifyMotionEffectBase FirstInstance
	{
		get
		{
			return m_firstInstance;
		}
	}

	public static AmplifyMotionEffectBase Instance
	{
		get
		{
			return m_firstInstance;
		}
	}

	private void Awake()
	{
		if (m_firstInstance == null)
		{
			m_firstInstance = this;
		}
		m_isD3D = SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D");
		m_globalObjectId = 1;
		m_width = (m_height = 0);
		if (ForceCPUOnly)
		{
			m_canUseGPU = false;
			return;
		}
		bool supportsRenderTextures = SystemInfo.supportsRenderTextures;
		bool flag = SystemInfo.graphicsShaderLevel >= 30;
		bool flag2 = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf);
		bool flag3 = SystemInfo.SupportsTextureFormat(TextureFormat.RGHalf);
		bool flag4 = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf);
		bool flag5 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat);
		m_canUseGPU = supportsRenderTextures && flag && flag2 && flag3 && flag4 && flag5;
	}

	internal void ResetObjectId()
	{
		m_globalObjectId = 1;
	}

	internal int GenerateObjectId(GameObject obj)
	{
		if (obj.isStatic)
		{
			return 0;
		}
		m_globalObjectId++;
		if (m_globalObjectId > 254)
		{
			m_globalObjectId = 1;
		}
		return m_globalObjectId;
	}

	private void SafeDestroyMaterial(ref Material mat)
	{
		if (mat != null)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	private bool CheckMaterialAndShader(Material material, string name)
	{
		bool result = true;
		if (material == null || material.shader == null)
		{
			Debug.LogWarning("[AmplifyMotion] Error creating " + name + " material");
			result = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogWarning("[AmplifyMotion] " + name + " shader not supported on this platform");
			result = false;
		}
		return result;
	}

	private void DestroyMaterials()
	{
		SafeDestroyMaterial(ref m_blurMaterial);
		SafeDestroyMaterial(ref m_solidVectorsMaterial);
		SafeDestroyMaterial(ref m_skinnedVectorsMaterial);
		SafeDestroyMaterial(ref m_clothVectorsMaterial);
		SafeDestroyMaterial(ref m_reprojectionMaterial);
		SafeDestroyMaterial(ref m_combineMaterial);
		SafeDestroyMaterial(ref m_dilationMaterial);
		SafeDestroyMaterial(ref m_depthMaterial);
		SafeDestroyMaterial(ref m_debugMaterial);
	}

	private bool CreateMaterials()
	{
		DestroyMaterials();
		int num = ((SystemInfo.graphicsShaderLevel < 30) ? 2 : 3);
		string text = "Hidden/Amplify Motion/MotionBlurSM" + num;
		string text2 = "Hidden/Amplify Motion/SolidVectors";
		string text3 = "Hidden/Amplify Motion/SkinnedVectors";
		string text4 = "Hidden/Amplify Motion/ClothVectors";
		string text5 = "Hidden/Amplify Motion/ReprojectionVectors";
		string text6 = "Hidden/Amplify Motion/Combine";
		string text7 = "Hidden/Amplify Motion/Dilation";
		string text8 = "Hidden/Amplify Motion/Depth";
		string text9 = "Hidden/Amplify Motion/Debug";
		try
		{
			m_blurMaterial = new Material(Shader.Find(text))
			{
				hideFlags = HideFlags.DontSave
			};
			m_solidVectorsMaterial = new Material(Shader.Find(text2))
			{
				hideFlags = HideFlags.DontSave
			};
			m_skinnedVectorsMaterial = new Material(Shader.Find(text3))
			{
				hideFlags = HideFlags.DontSave
			};
			m_clothVectorsMaterial = new Material(Shader.Find(text4))
			{
				hideFlags = HideFlags.DontSave
			};
			m_reprojectionMaterial = new Material(Shader.Find(text5))
			{
				hideFlags = HideFlags.DontSave
			};
			m_combineMaterial = new Material(Shader.Find(text6))
			{
				hideFlags = HideFlags.DontSave
			};
			m_dilationMaterial = new Material(Shader.Find(text7))
			{
				hideFlags = HideFlags.DontSave
			};
			m_depthMaterial = new Material(Shader.Find(text8))
			{
				hideFlags = HideFlags.DontSave
			};
			m_debugMaterial = new Material(Shader.Find(text9))
			{
				hideFlags = HideFlags.DontSave
			};
		}
		catch (Exception)
		{
		}
		return CheckMaterialAndShader(m_blurMaterial, text) && CheckMaterialAndShader(m_solidVectorsMaterial, text2) && CheckMaterialAndShader(m_skinnedVectorsMaterial, text3) && CheckMaterialAndShader(m_clothVectorsMaterial, text4) && CheckMaterialAndShader(m_reprojectionMaterial, text5) && CheckMaterialAndShader(m_combineMaterial, text6) && CheckMaterialAndShader(m_dilationMaterial, text7) && CheckMaterialAndShader(m_depthMaterial, text8) && CheckMaterialAndShader(m_debugMaterial, text9);
	}

	private RenderTexture CreateRenderTexture(string name, int depth, RenderTextureFormat fmt, RenderTextureReadWrite rw, FilterMode fm)
	{
		RenderTexture renderTexture = new RenderTexture(m_width, m_height, depth, fmt, rw);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = fm;
		renderTexture.Create();
		return renderTexture;
	}

	private void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			RenderTexture.active = null;
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	private void SafeDestroyTexture(ref Texture tex)
	{
		if (tex != null)
		{
			UnityEngine.Object.DestroyImmediate(tex);
			tex = null;
		}
	}

	private void DestroyRenderTextures()
	{
		RenderTexture.active = null;
		SafeDestroyRenderTexture(ref m_motionRT);
	}

	private void UpdateRenderTextures(bool qualityChanged)
	{
		int num = Mathf.FloorToInt((float)m_camera.pixelWidth + 0.5f);
		int num2 = Mathf.FloorToInt((float)m_camera.pixelHeight + 0.5f);
		if (QualityLevel == Quality.Mobile)
		{
			num /= 2;
			num2 /= 2;
		}
		if (m_width != num || m_height != num2 || qualityChanged)
		{
			m_width = num;
			m_height = num2;
			DestroyRenderTextures();
		}
		if (m_motionRT == null)
		{
			m_motionRT = CreateRenderTexture("AM-MotionVectors", 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Point);
		}
	}

	public bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			Debug.LogError("[AmplifyMotion] Initialization failed. This plugin requires support for Image Effects and Render Textures.");
			return false;
		}
		return true;
	}

	private void InitializeThreadPool()
	{
		if (WorkerThreads <= 0)
		{
			WorkerThreads = Mathf.Max(Environment.ProcessorCount / 2, 1);
		}
		m_workerThreadPool = new WorkerThreadPool();
		m_workerThreadPool.InitializeAsyncUpdateThreads(WorkerThreads, SystemThreadPool);
	}

	private void ShutdownThreadPool()
	{
		if (m_workerThreadPool != null)
		{
			m_workerThreadPool.FinalizeAsyncUpdateThreads();
			m_workerThreadPool = null;
		}
	}

	private void InitializeCommandBuffers()
	{
		ShutdownCommandBuffers();
		m_updateCB = new CommandBuffer();
		m_updateCB.name = "AmplifyMotion.Update";
		m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_updateCB);
		m_fixedUpdateCB = new CommandBuffer();
		m_fixedUpdateCB.name = "AmplifyMotion.FixedUpdate";
		m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_fixedUpdateCB);
		m_renderCB = new CommandBuffer();
		m_renderCB.name = "AmplifyMotion.Render";
		m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, m_renderCB);
	}

	private void ShutdownCommandBuffers()
	{
		if (m_updateCB != null)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_updateCB);
			m_updateCB.Release();
			m_updateCB = null;
		}
		if (m_fixedUpdateCB != null)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_fixedUpdateCB);
			m_fixedUpdateCB.Release();
			m_fixedUpdateCB = null;
		}
		if (m_renderCB != null)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_renderCB);
			m_renderCB.Release();
			m_renderCB = null;
		}
	}

	private void OnEnable()
	{
		m_camera = GetComponent<Camera>();
		if (!CheckSupport())
		{
			base.enabled = false;
			return;
		}
		InitializeThreadPool();
		m_starting = true;
		if (!CreateMaterials())
		{
			Debug.LogError("[AmplifyMotion] Failed loading or compiling necessary shaders. Please try reinstalling Amplify Motion or contact support@amplify.pt");
			base.enabled = false;
			return;
		}
		if (AutoRegisterObjs)
		{
			UpdateActiveObjects();
		}
		InitializeCameras();
		InitializeCommandBuffers();
		UpdateRenderTextures(true);
		m_linkedCameras.TryGetValue(m_camera, out m_baseCamera);
		if (m_baseCamera == null)
		{
			Debug.LogError("[AmplifyMotion] Failed setting up Base Camera. Please contact support@amplify.pt");
			base.enabled = false;
			return;
		}
		if (m_currentPostProcess != null)
		{
			m_currentPostProcess.enabled = true;
		}
		m_qualityLevel = QualityLevel;
	}

	private void OnDisable()
	{
		if (m_currentPostProcess != null)
		{
			m_currentPostProcess.enabled = false;
		}
		ShutdownCommandBuffers();
		ShutdownThreadPool();
	}

	private void Start()
	{
		UpdatePostProcess();
	}

	internal void RemoveCamera(Camera reference)
	{
		m_linkedCameras.Remove(reference);
	}

	private void OnDestroy()
	{
		AmplifyMotionCamera[] array = m_linkedCameras.Values.ToArray();
		AmplifyMotionCamera[] array2 = array;
		foreach (AmplifyMotionCamera amplifyMotionCamera in array2)
		{
			if (amplifyMotionCamera != null && amplifyMotionCamera.gameObject != base.gameObject)
			{
				Camera component = amplifyMotionCamera.GetComponent<Camera>();
				if (component != null)
				{
					component.targetTexture = null;
				}
				UnityEngine.Object.DestroyImmediate(amplifyMotionCamera);
			}
		}
		DestroyRenderTextures();
		DestroyMaterials();
	}

	private GameObject RecursiveFindCamera(GameObject obj, string auxCameraName)
	{
		GameObject gameObject = null;
		if (obj.name == auxCameraName)
		{
			gameObject = obj;
		}
		else
		{
			foreach (Transform item in obj.transform)
			{
				gameObject = RecursiveFindCamera(item.gameObject, auxCameraName);
				if (gameObject != null)
				{
					break;
				}
			}
		}
		return gameObject;
	}

	private void InitializeCameras()
	{
		List<Camera> list = new List<Camera>(OverlayCameras.Length);
		for (int i = 0; i < OverlayCameras.Length; i++)
		{
			if (OverlayCameras[i] != null)
			{
				list.Add(OverlayCameras[i]);
			}
		}
		Camera[] array = new Camera[list.Count + 1];
		array[0] = m_camera;
		for (int j = 0; j < list.Count; j++)
		{
			array[j + 1] = list[j];
		}
		m_linkedCameras.Clear();
		for (int k = 0; k < array.Length; k++)
		{
			Camera camera = array[k];
			if (!m_linkedCameras.ContainsKey(camera))
			{
				AmplifyMotionCamera amplifyMotionCamera = camera.gameObject.GetComponent<AmplifyMotionCamera>();
				if (amplifyMotionCamera != null)
				{
					amplifyMotionCamera.enabled = false;
					amplifyMotionCamera.enabled = true;
				}
				else
				{
					amplifyMotionCamera = camera.gameObject.AddComponent<AmplifyMotionCamera>();
				}
				amplifyMotionCamera.LinkTo(this, k > 0);
				m_linkedCameras.Add(camera, amplifyMotionCamera);
				m_linkedCamerasChanged = true;
			}
		}
	}

	public void UpdateActiveCameras()
	{
		InitializeCameras();
	}

	internal static void RegisterCamera(AmplifyMotionCamera cam)
	{
		m_activeCameras.Add(cam.GetComponent<Camera>(), cam);
		foreach (AmplifyMotionObjectBase value in m_activeObjects.Values)
		{
			value.RegisterCamera(cam);
		}
	}

	internal static void UnregisterCamera(AmplifyMotionCamera cam)
	{
		foreach (AmplifyMotionObjectBase value in m_activeObjects.Values)
		{
			value.UnregisterCamera(cam);
		}
		m_activeCameras.Remove(cam.GetComponent<Camera>());
	}

	public void UpdateActiveObjects()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0; i < array.Length; i++)
		{
			if (!m_activeObjects.ContainsKey(array[i]))
			{
				TryRegister(array[i], true);
			}
		}
	}

	internal static void RegisterObject(AmplifyMotionObjectBase obj)
	{
		m_activeObjects.Add(obj.gameObject, obj);
		foreach (AmplifyMotionCamera value in m_activeCameras.Values)
		{
			obj.RegisterCamera(value);
		}
	}

	internal static void UnregisterObject(AmplifyMotionObjectBase obj)
	{
		foreach (AmplifyMotionCamera value in m_activeCameras.Values)
		{
			obj.UnregisterCamera(value);
		}
		m_activeObjects.Remove(obj.gameObject);
	}

	internal static bool FindValidTag(Material[] materials)
	{
		foreach (Material material in materials)
		{
			if (material != null)
			{
				string text = material.GetTag("RenderType", false);
				if (text == "Opaque" || text == "TransparentCutout")
				{
					return !material.IsKeywordEnabled("_ALPHABLEND_ON") && !material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON");
				}
			}
		}
		return false;
	}

	internal static bool CanRegister(GameObject gameObj, bool autoReg)
	{
		if (gameObj.isStatic)
		{
			return false;
		}
		Renderer component = gameObj.GetComponent<Renderer>();
		if (component == null || component.sharedMaterials == null || component.isPartOfStaticBatch)
		{
			return false;
		}
		if (!component.enabled)
		{
			return false;
		}
		if (component.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
		{
			return false;
		}
		if (component.GetType() == typeof(SpriteRenderer))
		{
			return false;
		}
		if (!FindValidTag(component.sharedMaterials))
		{
			return false;
		}
		Type type = component.GetType();
		if (type == typeof(MeshRenderer) || type == typeof(SkinnedMeshRenderer))
		{
			return true;
		}
		if (type == typeof(ParticleSystemRenderer) && !autoReg)
		{
			ParticleSystemRenderMode renderMode = (component as ParticleSystemRenderer).renderMode;
			return renderMode == ParticleSystemRenderMode.Mesh || renderMode == ParticleSystemRenderMode.Billboard;
		}
		return false;
	}

	internal static void TryRegister(GameObject gameObj, bool autoReg)
	{
		if (CanRegister(gameObj, autoReg) && gameObj.GetComponent<AmplifyMotionObjectBase>() == null)
		{
			AmplifyMotionObjectBase.ApplyToChildren = false;
			gameObj.AddComponent<AmplifyMotionObjectBase>();
			AmplifyMotionObjectBase.ApplyToChildren = true;
		}
	}

	internal static void TryUnregister(GameObject gameObj)
	{
		AmplifyMotionObjectBase component = gameObj.GetComponent<AmplifyMotionObjectBase>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	public void Register(GameObject gameObj)
	{
		if (!m_activeObjects.ContainsKey(gameObj))
		{
			TryRegister(gameObj, false);
		}
	}

	public static void RegisterS(GameObject gameObj)
	{
		if (!m_activeObjects.ContainsKey(gameObj))
		{
			TryRegister(gameObj, false);
		}
	}

	public void RegisterRecursively(GameObject gameObj)
	{
		if (!m_activeObjects.ContainsKey(gameObj))
		{
			TryRegister(gameObj, false);
		}
		foreach (Transform item in gameObj.transform)
		{
			RegisterRecursively(item.gameObject);
		}
	}

	public static void RegisterRecursivelyS(GameObject gameObj)
	{
		if (!m_activeObjects.ContainsKey(gameObj))
		{
			TryRegister(gameObj, false);
		}
		foreach (Transform item in gameObj.transform)
		{
			RegisterRecursivelyS(item.gameObject);
		}
	}

	public void Unregister(GameObject gameObj)
	{
		if (m_activeObjects.ContainsKey(gameObj))
		{
			TryUnregister(gameObj);
		}
	}

	public static void UnregisterS(GameObject gameObj)
	{
		if (m_activeObjects.ContainsKey(gameObj))
		{
			TryUnregister(gameObj);
		}
	}

	public void UnregisterRecursively(GameObject gameObj)
	{
		if (m_activeObjects.ContainsKey(gameObj))
		{
			TryUnregister(gameObj);
		}
		foreach (Transform item in gameObj.transform)
		{
			UnregisterRecursively(item.gameObject);
		}
	}

	public static void UnregisterRecursivelyS(GameObject gameObj)
	{
		if (m_activeObjects.ContainsKey(gameObj))
		{
			TryUnregister(gameObj);
		}
		foreach (Transform item in gameObj.transform)
		{
			UnregisterRecursivelyS(item.gameObject);
		}
	}

	private void UpdatePostProcess()
	{
		Camera camera = null;
		float num = float.MinValue;
		if (m_linkedCamerasChanged)
		{
			UpdateLinkedCameras();
		}
		for (int i = 0; i < m_linkedCameraKeys.Length; i++)
		{
			if (m_linkedCameraKeys[i] != null && m_linkedCameraKeys[i].isActiveAndEnabled && m_linkedCameraKeys[i].depth > num)
			{
				camera = m_linkedCameraKeys[i];
				num = m_linkedCameraKeys[i].depth;
			}
		}
		if (m_currentPostProcess != null && m_currentPostProcess.gameObject != camera.gameObject)
		{
			UnityEngine.Object.DestroyImmediate(m_currentPostProcess);
			m_currentPostProcess = null;
		}
		if (!(m_currentPostProcess == null) || !(camera != null) || !(camera != m_camera))
		{
			return;
		}
		AmplifyMotionPostProcess[] components = base.gameObject.GetComponents<AmplifyMotionPostProcess>();
		if (components != null && components.Length > 0)
		{
			for (int j = 0; j < components.Length; j++)
			{
				UnityEngine.Object.DestroyImmediate(components[j]);
			}
		}
		m_currentPostProcess = camera.gameObject.AddComponent<AmplifyMotionPostProcess>();
		m_currentPostProcess.Instance = this;
	}

	private void LateUpdate()
	{
		if (m_baseCamera.AutoStep)
		{
			float num = ((!Application.isPlaying) ? Time.fixedDeltaTime : Time.deltaTime);
			float fixedDeltaTime = Time.fixedDeltaTime;
			m_deltaTime = ((!(num > float.Epsilon)) ? m_deltaTime : num);
			m_fixedDeltaTime = ((!(num > float.Epsilon)) ? m_fixedDeltaTime : fixedDeltaTime);
		}
		QualitySteps = Mathf.Clamp(QualitySteps, 0, 16);
		MotionScale = Mathf.Max(MotionScale, 0f);
		MinVelocity = Mathf.Min(MinVelocity, MaxVelocity);
		DepthThreshold = Mathf.Max(DepthThreshold, 0f);
		UpdatePostProcess();
	}

	public void StopAutoStep()
	{
		foreach (AmplifyMotionCamera value in m_linkedCameras.Values)
		{
			value.StopAutoStep();
		}
	}

	public void StartAutoStep()
	{
		foreach (AmplifyMotionCamera value in m_linkedCameras.Values)
		{
			value.StartAutoStep();
		}
	}

	public void Step(float delta)
	{
		m_deltaTime = delta;
		m_fixedDeltaTime = delta;
		foreach (AmplifyMotionCamera value in m_linkedCameras.Values)
		{
			value.Step();
		}
	}

	private void UpdateLinkedCameras()
	{
		Dictionary<Camera, AmplifyMotionCamera>.KeyCollection keys = m_linkedCameras.Keys;
		Dictionary<Camera, AmplifyMotionCamera>.ValueCollection values = m_linkedCameras.Values;
		if (m_linkedCameraKeys == null || keys.Count != m_linkedCameraKeys.Length)
		{
			m_linkedCameraKeys = new Camera[keys.Count];
		}
		if (m_linkedCameraValues == null || values.Count != m_linkedCameraValues.Length)
		{
			m_linkedCameraValues = new AmplifyMotionCamera[values.Count];
		}
		keys.CopyTo(m_linkedCameraKeys, 0);
		values.CopyTo(m_linkedCameraValues, 0);
		m_linkedCamerasChanged = false;
	}

	private void FixedUpdate()
	{
		if (!m_camera.enabled)
		{
			return;
		}
		if (m_linkedCamerasChanged)
		{
			UpdateLinkedCameras();
		}
		m_fixedUpdateCB.Clear();
		for (int i = 0; i < m_linkedCameraValues.Length; i++)
		{
			if (m_linkedCameraValues[i] != null && m_linkedCameraValues[i].isActiveAndEnabled)
			{
				m_linkedCameraValues[i].FixedUpdateTransform(m_fixedUpdateCB);
			}
		}
	}

	private void OnPreRender()
	{
		if (!m_camera.enabled || (Time.frameCount != 1 && !(Mathf.Abs(Time.deltaTime) > float.Epsilon)))
		{
			return;
		}
		if (m_linkedCamerasChanged)
		{
			UpdateLinkedCameras();
		}
		m_updateCB.Clear();
		for (int i = 0; i < m_linkedCameraValues.Length; i++)
		{
			if (m_linkedCameraValues[i] != null && m_linkedCameraValues[i].isActiveAndEnabled)
			{
				m_linkedCameraValues[i].UpdateTransform(m_updateCB);
			}
		}
	}

	private void RenderReprojectionVectors(CommandBuffer commandBuffer, RenderTexture destination, float scale)
	{
		commandBuffer.SetGlobalMatrix("_AM_MATRIX_CURR_REPROJ", m_baseCamera.PrevViewProjMatrix * m_baseCamera.InvViewProjMatrix);
		commandBuffer.SetGlobalFloat("_AM_MOTION_SCALE", scale);
		RenderTexture rt = null;
		commandBuffer.Blit(new RenderTargetIdentifier(rt), destination, m_reprojectionMaterial);
	}

	public static void DiscardContents(RenderTexture rtex)
	{
		rtex.DiscardContents();
	}

	private void OnPostRender()
	{
		bool flag = QualityLevel != m_qualityLevel;
		if (flag)
		{
			CreateMaterials();
			m_qualityLevel = QualityLevel;
		}
		UpdateRenderTextures(flag);
		ResetObjectId();
		bool flag2 = CameraMotionMult > float.Epsilon;
		bool clearColor = !flag2 || m_starting;
		float y = ((!(DepthThreshold > float.Epsilon)) ? float.MaxValue : (1f / DepthThreshold));
		m_motionScaleNorm = ((!(m_deltaTime >= float.Epsilon)) ? 0f : (MotionScale * (1f / m_deltaTime)));
		m_fixedMotionScaleNorm = ((!(m_fixedDeltaTime >= float.Epsilon)) ? 0f : (MotionScale * (1f / m_fixedDeltaTime)));
		float scale = (m_starting ? 0f : m_motionScaleNorm);
		float fixedScale = (m_starting ? 0f : m_fixedMotionScaleNorm);
		DiscardContents(m_motionRT);
		m_updateCB.Clear();
		m_renderCB.Clear();
		m_renderCB.SetGlobalFloat("_AM_MIN_VELOCITY", MinVelocity);
		m_renderCB.SetGlobalFloat("_AM_MAX_VELOCITY", MaxVelocity);
		m_renderCB.SetGlobalFloat("_AM_RCP_TOTAL_VELOCITY", 1f / (MaxVelocity - MinVelocity));
		m_renderCB.SetGlobalVector("_AM_DEPTH_THRESHOLD", new Vector2(DepthThreshold, y));
		m_renderCB.SetRenderTarget(m_motionRT);
		m_renderCB.ClearRenderTarget(true, clearColor, Color.black);
		if (flag2)
		{
			float num = ((!(m_deltaTime >= float.Epsilon)) ? 0f : (MotionScale * CameraMotionMult * (1f / m_deltaTime)));
			float scale2 = (m_starting ? 0f : num);
			RenderReprojectionVectors(m_renderCB, m_motionRT, scale2);
		}
		m_baseCamera.RenderVectors(m_renderCB, scale, fixedScale, QualityLevel);
		for (int i = 0; i < m_linkedCameraValues.Length; i++)
		{
			AmplifyMotionCamera amplifyMotionCamera = m_linkedCameraValues[i];
			if (amplifyMotionCamera != null && amplifyMotionCamera.Overlay && amplifyMotionCamera.isActiveAndEnabled)
			{
				m_linkedCameraValues[i].RenderVectors(m_renderCB, scale, fixedScale, QualityLevel);
			}
		}
		m_starting = false;
	}

	private void ApplyMotionBlur(RenderTexture source, RenderTexture destination, Vector4 blurStep)
	{
		bool flag = QualityLevel == Quality.Mobile;
		int qualityLevel = (int)QualityLevel;
		RenderTexture renderTexture = null;
		if (flag)
		{
			renderTexture = RenderTexture.GetTemporary(m_width, m_height, 0, RenderTextureFormat.ARGB32);
			renderTexture.name = "AM-DepthTemp";
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.filterMode = FilterMode.Point;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(m_width, m_height, 0, source.format);
		temporary.name = "AM-CombinedTemp";
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Point;
		DiscardContents(temporary);
		m_combineMaterial.SetTexture("_MotionTex", m_motionRT);
		source.filterMode = FilterMode.Point;
		Graphics.Blit(source, temporary, m_combineMaterial, 0);
		m_blurMaterial.SetTexture("_MotionTex", m_motionRT);
		if (flag)
		{
			Graphics.Blit(null, renderTexture, m_depthMaterial, 0);
			m_blurMaterial.SetTexture("_DepthTex", renderTexture);
		}
		if (QualitySteps > 1)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(m_width, m_height, 0, source.format);
			temporary2.name = "AM-CombinedTemp2";
			temporary2.filterMode = FilterMode.Point;
			float num = 1f / (float)QualitySteps;
			float num2 = 1f;
			RenderTexture renderTexture2 = temporary;
			RenderTexture renderTexture3 = temporary2;
			for (int i = 0; i < QualitySteps; i++)
			{
				if (renderTexture3 != destination)
				{
					DiscardContents(renderTexture3);
				}
				m_blurMaterial.SetVector("_AM_BLUR_STEP", blurStep * num2);
				Graphics.Blit(renderTexture2, renderTexture3, m_blurMaterial, qualityLevel);
				if (i < QualitySteps - 2)
				{
					RenderTexture renderTexture4 = renderTexture3;
					renderTexture3 = renderTexture2;
					renderTexture2 = renderTexture4;
				}
				else
				{
					renderTexture2 = renderTexture3;
					renderTexture3 = destination;
				}
				num2 -= num;
			}
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			m_blurMaterial.SetVector("_AM_BLUR_STEP", blurStep);
			Graphics.Blit(temporary, destination, m_blurMaterial, qualityLevel);
		}
		if (flag)
		{
			m_combineMaterial.SetTexture("_MotionTex", m_motionRT);
			Graphics.Blit(source, destination, m_combineMaterial, 1);
		}
		RenderTexture.ReleaseTemporary(temporary);
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (m_currentPostProcess == null)
		{
			PostProcess(source, destination);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}

	public void PostProcess(RenderTexture source, RenderTexture destination)
	{
		Vector4 zero = Vector4.zero;
		zero.x = MaxVelocity / 1000f;
		zero.y = MaxVelocity / 1000f;
		RenderTexture renderTexture = null;
		if (QualitySettings.antiAliasing > 1)
		{
			renderTexture = RenderTexture.GetTemporary(m_width, m_height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			renderTexture.name = "AM-DilatedTemp";
			renderTexture.filterMode = FilterMode.Point;
			m_dilationMaterial.SetTexture("_MotionTex", m_motionRT);
			Graphics.Blit(m_motionRT, renderTexture, m_dilationMaterial, 0);
			m_dilationMaterial.SetTexture("_MotionTex", renderTexture);
			Graphics.Blit(renderTexture, m_motionRT, m_dilationMaterial, 1);
		}
		if (DebugMode)
		{
			m_debugMaterial.SetTexture("_MotionTex", m_motionRT);
			Graphics.Blit(source, destination, m_debugMaterial);
		}
		else
		{
			ApplyMotionBlur(source, destination, zero);
		}
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}

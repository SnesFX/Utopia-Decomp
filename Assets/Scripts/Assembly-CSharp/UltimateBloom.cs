using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UltimateBloom : MonoBehaviour
{
	public enum BloomQualityPreset
	{
		Optimized = 0,
		Standard = 1,
		HighVisuals = 2,
		Custom = 3
	}

	public enum BloomSamplingQuality
	{
		VerySmallKernel = 0,
		SmallKernel = 1,
		MediumKernel = 2,
		LargeKernel = 3,
		LargerKernel = 4,
		VeryLargeKernel = 5
	}

	public enum BloomScreenBlendMode
	{
		Screen = 0,
		Add = 1
	}

	public enum HDRBloomMode
	{
		Auto = 0,
		On = 1,
		Off = 2
	}

	public enum BlurSampleCount
	{
		Nine = 0,
		Seventeen = 1,
		Thirteen = 2,
		TwentyThree = 3,
		TwentySeven = 4,
		ThrirtyOne = 5,
		NineCurve = 6,
		FourSimple = 7
	}

	public enum FlareRendering
	{
		Sharp = 0,
		Blurred = 1,
		MoreBlurred = 2
	}

	public enum SimpleSampleCount
	{
		Four = 0,
		Nine = 1,
		FourCurve = 2,
		ThirteenTemporal = 3,
		ThirteenTemporalCurve = 4
	}

	public enum FlareType
	{
		Single = 0,
		Double = 1
	}

	public enum BloomIntensityManagement
	{
		FilmicCurve = 0,
		Threshold = 1
	}

	private enum FlareStripeType
	{
		Anamorphic = 0,
		Star = 1,
		DiagonalUpright = 2,
		DiagonalUpleft = 3
	}

	public enum AnamorphicDirection
	{
		Horizontal = 0,
		Vertical = 1
	}

	public enum BokehFlareQuality
	{
		Low = 0,
		Medium = 1,
		High = 2,
		VeryHigh = 3
	}

	public enum BlendMode
	{
		ADD = 0,
		SCREEN = 1
	}

	public enum SamplingMode
	{
		Fixed = 0,
		HeightRelative = 1
	}

	public enum FlareBlurQuality
	{
		Fast = 0,
		Normal = 1,
		High = 2
	}

	public enum FlarePresets
	{
		ChoosePreset = 0,
		GhostFast = 1,
		Ghost1 = 2,
		Ghost2 = 3,
		Ghost3 = 4,
		Bokeh1 = 5,
		Bokeh2 = 6,
		Bokeh3 = 7
	}

	private delegate void BlurFunction(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity);

	public float m_SamplingMinHeight = 400f;

	public float[] m_ResSamplingPixelCount = new float[6];

	public SamplingMode m_SamplingMode;

	public BlendMode m_BlendMode;

	public float m_ScreenMaxIntensity;

	public BloomQualityPreset m_QualityPreset;

	public HDRBloomMode m_HDR;

	public BloomScreenBlendMode m_ScreenBlendMode = BloomScreenBlendMode.Add;

	public float m_BloomIntensity = 1f;

	public float m_BloomThreshhold = 0.5f;

	public Color m_BloomThreshholdColor = Color.white;

	public int m_DownscaleCount = 5;

	public BloomIntensityManagement m_IntensityManagement;

	public float[] m_BloomIntensities;

	public Color[] m_BloomColors;

	public bool[] m_BloomUsages;

	[SerializeField]
	public DeluxeFilmicCurve m_BloomCurve = new DeluxeFilmicCurve();

	private int m_LastDownscaleCount = 5;

	public bool m_UseLensFlare;

	public float m_FlareTreshold = 0.8f;

	public float m_FlareIntensity = 0.25f;

	public Color m_FlareTint0 = new Color(0.5372549f, 0.32156864f, 0f);

	public Color m_FlareTint1 = new Color(0f, 21f / 85f, 42f / 85f);

	public Color m_FlareTint2 = new Color(24f / 85f, 0.5921569f, 0f);

	public Color m_FlareTint3 = new Color(38f / 85f, 7f / 51f, 0f);

	public Color m_FlareTint4 = new Color(0.47843137f, 0.34509805f, 0f);

	public Color m_FlareTint5 = new Color(0.5372549f, 0.2784314f, 0f);

	public Color m_FlareTint6 = new Color(0.38039216f, 0.54509807f, 0f);

	public Color m_FlareTint7 = new Color(8f / 51f, 0.5568628f, 0f);

	public float m_FlareGlobalScale = 1f;

	public Vector4 m_FlareScales = new Vector4(1f, 0.6f, 0.5f, 0.4f);

	public Vector4 m_FlareScalesNear = new Vector4(1f, 0.8f, 0.6f, 0.5f);

	public Texture2D m_FlareMask;

	public FlareRendering m_FlareRendering = FlareRendering.Blurred;

	public FlareType m_FlareType = FlareType.Double;

	public Texture2D m_FlareShape;

	public FlareBlurQuality m_FlareBlurQuality = FlareBlurQuality.High;

	private BokehRenderer m_FlareSpriteRenderer;

	private Mesh[] m_BokehMeshes;

	public bool m_UseBokehFlare;

	public float m_BokehScale = 0.4f;

	public BokehFlareQuality m_BokehFlareQuality = BokehFlareQuality.Medium;

	public bool m_UseAnamorphicFlare;

	public float m_AnamorphicFlareTreshold = 0.8f;

	public float m_AnamorphicFlareIntensity = 1f;

	public int m_AnamorphicDownscaleCount = 3;

	public int m_AnamorphicBlurPass = 2;

	private int m_LastAnamorphicDownscaleCount;

	private RenderTexture[] m_AnamorphicUpscales;

	public float[] m_AnamorphicBloomIntensities;

	public Color[] m_AnamorphicBloomColors;

	public bool[] m_AnamorphicBloomUsages;

	public bool m_AnamorphicSmallVerticalBlur = true;

	public AnamorphicDirection m_AnamorphicDirection;

	public float m_AnamorphicScale = 3f;

	public bool m_UseStarFlare;

	public float m_StarFlareTreshol = 0.8f;

	public float m_StarFlareIntensity = 1f;

	public float m_StarScale = 2f;

	public int m_StarDownscaleCount = 3;

	public int m_StarBlurPass = 2;

	private int m_LastStarDownscaleCount;

	private RenderTexture[] m_StarUpscales;

	public float[] m_StarBloomIntensities;

	public Color[] m_StarBloomColors;

	public bool[] m_StarBloomUsages;

	public bool m_UseLensDust;

	public float m_DustIntensity = 1f;

	public Texture2D m_DustTexture;

	public float m_DirtLightIntensity = 5f;

	public BloomSamplingQuality m_DownsamplingQuality;

	public BloomSamplingQuality m_UpsamplingQuality;

	public bool m_TemporalStableDownsampling = true;

	public bool m_InvertImage;

	private Material m_FlareMaterial;

	private Shader m_FlareShader;

	private Material m_SamplingMaterial;

	private Shader m_SamplingShader;

	private Material m_CombineMaterial;

	private Shader m_CombineShader;

	private Material m_BrightpassMaterial;

	private Shader m_BrightpassShader;

	private Material m_FlareMaskMaterial;

	private Shader m_FlareMaskShader;

	private Material m_MixerMaterial;

	private Shader m_MixerShader;

	private Material m_FlareBokehMaterial;

	private Shader m_FlareBokehShader;

	public bool m_DirectDownSample;

	public bool m_DirectUpsample;

	public bool m_UiShowBloomScales;

	public bool m_UiShowAnamorphicBloomScales;

	public bool m_UiShowStarBloomScales;

	public bool m_UiShowHeightSampling;

	public bool m_UiShowBloomSettings;

	public bool m_UiShowSampling;

	public bool m_UiShowIntensity;

	public bool m_UiShowOptimizations;

	public bool m_UiShowLensDirt;

	public bool m_UiShowLensFlare;

	public bool m_UiShowAnamorphic;

	public bool m_UiShowStar;

	private RenderTexture[] m_DownSamples;

	private RenderTexture[] m_UpSamples;

	private RenderTextureFormat m_Format;

	private bool[] m_BufferUsage;

	private RenderTexture m_LastBloomUpsample;

	private void DestroyMaterial(Material mat)
	{
		if ((bool)mat)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	private void LoadShader(ref Material material, ref Shader shader, string shaderPath)
	{
		if (!(shader != null))
		{
			shader = Shader.Find(shaderPath);
			if (shader == null)
			{
				Debug.LogError("Shader not found: " + shaderPath);
			}
			else if (!shader.isSupported)
			{
				Debug.LogError("Shader contains error: " + shaderPath + "\n Maybe include path? Try rebuilding the shader.");
			}
			else
			{
				material = CreateMaterial(shader);
			}
		}
	}

	public void CreateMaterials()
	{
		int num = 8;
		if (m_BloomIntensities == null || m_BloomIntensities.Length < num)
		{
			m_BloomIntensities = new float[num];
			for (int i = 0; i < 8; i++)
			{
				m_BloomIntensities[i] = 1f;
			}
		}
		if (m_BloomColors == null || m_BloomColors.Length < num)
		{
			m_BloomColors = new Color[num];
			for (int j = 0; j < 8; j++)
			{
				m_BloomColors[j] = Color.white;
			}
		}
		if (m_BloomUsages == null || m_BloomUsages.Length < num)
		{
			m_BloomUsages = new bool[num];
			for (int k = 0; k < 8; k++)
			{
				m_BloomUsages[k] = true;
			}
		}
		if (m_AnamorphicBloomIntensities == null || m_AnamorphicBloomIntensities.Length < num)
		{
			m_AnamorphicBloomIntensities = new float[num];
			for (int l = 0; l < 8; l++)
			{
				m_AnamorphicBloomIntensities[l] = 1f;
			}
		}
		if (m_AnamorphicBloomColors == null || m_AnamorphicBloomColors.Length < num)
		{
			m_AnamorphicBloomColors = new Color[num];
			for (int m = 0; m < 8; m++)
			{
				m_AnamorphicBloomColors[m] = Color.white;
			}
		}
		if (m_AnamorphicBloomUsages == null || m_AnamorphicBloomUsages.Length < num)
		{
			m_AnamorphicBloomUsages = new bool[num];
			for (int n = 0; n < 8; n++)
			{
				m_AnamorphicBloomUsages[n] = true;
			}
		}
		if (m_StarBloomIntensities == null || m_StarBloomIntensities.Length < num)
		{
			m_StarBloomIntensities = new float[num];
			for (int num2 = 0; num2 < 8; num2++)
			{
				m_StarBloomIntensities[num2] = 1f;
			}
		}
		if (m_StarBloomColors == null || m_StarBloomColors.Length < num)
		{
			m_StarBloomColors = new Color[num];
			for (int num3 = 0; num3 < 8; num3++)
			{
				m_StarBloomColors[num3] = Color.white;
			}
		}
		if (m_StarBloomUsages == null || m_StarBloomUsages.Length < num)
		{
			m_StarBloomUsages = new bool[num];
			for (int num4 = 0; num4 < 8; num4++)
			{
				m_StarBloomUsages[num4] = true;
			}
		}
		if (m_FlareSpriteRenderer == null && m_FlareShape != null && m_UseBokehFlare)
		{
			if (m_FlareSpriteRenderer != null)
			{
				m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);
			}
			m_FlareSpriteRenderer = new BokehRenderer();
		}
		if (m_SamplingMaterial == null)
		{
			m_DownSamples = new RenderTexture[GetNeededDownsamples()];
			m_UpSamples = new RenderTexture[m_DownscaleCount];
			m_AnamorphicUpscales = new RenderTexture[m_AnamorphicDownscaleCount];
			m_StarUpscales = new RenderTexture[m_StarDownscaleCount];
		}
		string shaderPath = ((m_FlareType != 0) ? "Hidden/Ultimate/FlareDouble" : "Hidden/Ultimate/FlareSingle");
		LoadShader(ref m_FlareMaterial, ref m_FlareShader, shaderPath);
		LoadShader(ref m_SamplingMaterial, ref m_SamplingShader, "Hidden/Ultimate/Sampling");
		LoadShader(ref m_BrightpassMaterial, ref m_BrightpassShader, "Hidden/Ultimate/BrightpassMask");
		LoadShader(ref m_FlareMaskMaterial, ref m_FlareMaskShader, "Hidden/Ultimate/FlareMask");
		LoadShader(ref m_MixerMaterial, ref m_MixerShader, "Hidden/Ultimate/BloomMixer");
		LoadShader(ref m_FlareBokehMaterial, ref m_FlareBokehShader, "Hidden/Ultimate/FlareMesh");
		bool flag = m_UseLensDust || m_UseLensFlare || m_UseAnamorphicFlare || m_UseStarFlare;
		string shaderPath2 = "Hidden/Ultimate/BloomCombine";
		if (flag)
		{
			shaderPath2 = "Hidden/Ultimate/BloomCombineFlareDirt";
		}
		LoadShader(ref m_CombineMaterial, ref m_CombineShader, shaderPath2);
	}

	private Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			return null;
		}
		Material material = new Material(shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		return material;
	}

	private void OnDisable()
	{
		ForceShadersReload();
		if (m_FlareSpriteRenderer != null)
		{
			m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);
			m_FlareSpriteRenderer = null;
		}
	}

	public void ForceShadersReload()
	{
		DestroyMaterial(m_FlareMaterial);
		m_FlareMaterial = null;
		m_FlareShader = null;
		DestroyMaterial(m_SamplingMaterial);
		m_SamplingMaterial = null;
		m_SamplingShader = null;
		DestroyMaterial(m_CombineMaterial);
		m_CombineMaterial = null;
		m_CombineShader = null;
		DestroyMaterial(m_BrightpassMaterial);
		m_BrightpassMaterial = null;
		m_BrightpassShader = null;
		DestroyMaterial(m_FlareBokehMaterial);
		m_FlareBokehMaterial = null;
		m_FlareBokehShader = null;
		DestroyMaterial(m_FlareMaskMaterial);
		m_FlareMaskMaterial = null;
		m_FlareMaskShader = null;
		DestroyMaterial(m_MixerMaterial);
		m_MixerMaterial = null;
		m_MixerShader = null;
	}

	private int GetNeededDownsamples()
	{
		int a = Mathf.Max(m_DownscaleCount, m_UseAnamorphicFlare ? m_AnamorphicDownscaleCount : 0);
		a = Mathf.Max(a, m_UseLensFlare ? (GetGhostBokehLayer() + 1) : 0);
		return Mathf.Max(a, m_UseStarFlare ? m_StarDownscaleCount : 0);
	}

	private void ComputeBufferOptimization()
	{
		if (m_BufferUsage == null)
		{
			m_BufferUsage = new bool[m_DownSamples.Length];
		}
		if (m_BufferUsage.Length != m_DownSamples.Length)
		{
			m_BufferUsage = new bool[m_DownSamples.Length];
		}
		for (int i = 0; i < m_BufferUsage.Length; i++)
		{
			m_BufferUsage[i] = false;
		}
		for (int j = 0; j < m_BufferUsage.Length; j++)
		{
			m_BufferUsage[j] = m_BloomUsages[j] || m_BufferUsage[j];
		}
		if (m_UseAnamorphicFlare)
		{
			for (int k = 0; k < m_BufferUsage.Length; k++)
			{
				m_BufferUsage[k] = m_AnamorphicBloomUsages[k] || m_BufferUsage[k];
			}
		}
		if (m_UseStarFlare)
		{
			for (int l = 0; l < m_BufferUsage.Length; l++)
			{
				m_BufferUsage[l] = m_StarBloomUsages[l] || m_BufferUsage[l];
			}
		}
	}

	private int GetGhostBokehLayer()
	{
		if (m_UseBokehFlare && m_FlareShape != null)
		{
			if (m_BokehFlareQuality == BokehFlareQuality.VeryHigh)
			{
				return 1;
			}
			if (m_BokehFlareQuality == BokehFlareQuality.High)
			{
				return 2;
			}
			if (m_BokehFlareQuality == BokehFlareQuality.Medium)
			{
				return 3;
			}
			if (m_BokehFlareQuality == BokehFlareQuality.Low)
			{
				return 4;
			}
		}
		return 0;
	}

	private BlurSampleCount GetUpsamplingSize()
	{
		if (m_SamplingMode == SamplingMode.Fixed)
		{
			BlurSampleCount result = BlurSampleCount.ThrirtyOne;
			if (m_UpsamplingQuality == BloomSamplingQuality.VerySmallKernel)
			{
				result = BlurSampleCount.Nine;
			}
			else if (m_UpsamplingQuality == BloomSamplingQuality.SmallKernel)
			{
				result = BlurSampleCount.Thirteen;
			}
			else if (m_UpsamplingQuality == BloomSamplingQuality.MediumKernel)
			{
				result = BlurSampleCount.Seventeen;
			}
			else if (m_UpsamplingQuality == BloomSamplingQuality.LargeKernel)
			{
				result = BlurSampleCount.TwentyThree;
			}
			else if (m_UpsamplingQuality == BloomSamplingQuality.LargerKernel)
			{
				result = BlurSampleCount.TwentySeven;
			}
			return result;
		}
		float num = Screen.height;
		int num2 = 0;
		float num3 = float.MaxValue;
		for (int i = 0; i < m_ResSamplingPixelCount.Length; i++)
		{
			float num4 = Math.Abs(num - m_ResSamplingPixelCount[i]);
			if (num4 < num3)
			{
				num3 = num4;
				num2 = i;
			}
		}
		switch (num2)
		{
		case 0:
			return BlurSampleCount.Nine;
		case 1:
			return BlurSampleCount.Thirteen;
		case 2:
			return BlurSampleCount.Seventeen;
		case 3:
			return BlurSampleCount.TwentyThree;
		case 4:
			return BlurSampleCount.TwentySeven;
		default:
			return BlurSampleCount.ThrirtyOne;
		}
	}

	public void ComputeResolutionRelativeData()
	{
		float num = m_SamplingMinHeight;
		float num2 = 9f;
		for (int i = 0; i < m_ResSamplingPixelCount.Length; i++)
		{
			m_ResSamplingPixelCount[i] = num;
			float num3 = num2 + 4f;
			float num4 = num3 / num2;
			num *= num4;
			num2 = num3;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool flag = false;
		flag = ((m_HDR != 0) ? (m_HDR == HDRBloomMode.On) : (source.format == RenderTextureFormat.ARGBHalf && GetComponent<Camera>().allowHDR));
		m_Format = ((!flag) ? RenderTextureFormat.Default : RenderTextureFormat.ARGBHalf);
		if (m_DownSamples != null && m_DownSamples.Length != GetNeededDownsamples())
		{
			OnDisable();
		}
		if (m_LastDownscaleCount != m_DownscaleCount || m_LastAnamorphicDownscaleCount != m_AnamorphicDownscaleCount || m_LastStarDownscaleCount != m_StarDownscaleCount)
		{
			OnDisable();
		}
		m_LastDownscaleCount = m_DownscaleCount;
		m_LastAnamorphicDownscaleCount = m_AnamorphicDownscaleCount;
		m_LastStarDownscaleCount = m_StarDownscaleCount;
		CreateMaterials();
		if (m_DirectDownSample || m_DirectUpsample)
		{
			ComputeBufferOptimization();
		}
		bool flag2 = false;
		if (m_SamplingMode == SamplingMode.HeightRelative)
		{
			ComputeResolutionRelativeData();
		}
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
		temporary.filterMode = FilterMode.Bilinear;
		if (m_IntensityManagement == BloomIntensityManagement.Threshold)
		{
			BrightPass(source, temporary, m_BloomThreshhold * m_BloomThreshholdColor);
		}
		else
		{
			m_BloomCurve.UpdateCoefficients();
			Graphics.Blit(source, temporary);
		}
		if (m_IntensityManagement == BloomIntensityManagement.Threshold)
		{
			CachedDownsample(temporary, m_DownSamples, null, flag);
		}
		else
		{
			CachedDownsample(temporary, m_DownSamples, m_BloomCurve, flag);
		}
		BlurSampleCount upsamplingSize = GetUpsamplingSize();
		CachedUpsample(m_DownSamples, m_UpSamples, source.width, source.height, upsamplingSize);
		Texture flareRT = Texture2D.blackTexture;
		RenderTexture renderTexture = null;
		if (m_UseLensFlare)
		{
			int ghostBokehLayer = GetGhostBokehLayer();
			int num = source.width / (int)Mathf.Pow(2f, ghostBokehLayer);
			int num2 = source.height / (int)Mathf.Pow(2f, ghostBokehLayer);
			if (m_FlareShape != null && m_UseBokehFlare)
			{
				float num3 = 15f;
				if (m_BokehFlareQuality == BokehFlareQuality.Medium)
				{
					num3 *= 2f;
				}
				if (m_BokehFlareQuality == BokehFlareQuality.High)
				{
					num3 *= 4f;
				}
				if (m_BokehFlareQuality == BokehFlareQuality.VeryHigh)
				{
					num3 *= 8f;
				}
				num3 *= m_BokehScale;
				m_FlareSpriteRenderer.SetMaterial(m_FlareBokehMaterial);
				m_FlareSpriteRenderer.RebuildMeshIfNeeded(num, num2, 1f / (float)num * num3, 1f / (float)num2 * num3, ref m_BokehMeshes);
				m_FlareSpriteRenderer.SetTexture(m_FlareShape);
				renderTexture = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, m_Format);
				int num4 = ghostBokehLayer;
				RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / (int)Mathf.Pow(2f, num4 + 1), source.height / (int)Mathf.Pow(2f, num4 + 1), 0, m_Format);
				BrightPass(m_DownSamples[ghostBokehLayer], temporary2, m_FlareTreshold * Vector4.one);
				m_FlareSpriteRenderer.RenderFlare(temporary2, renderTexture, (!m_UseBokehFlare) ? m_FlareIntensity : 1f, ref m_BokehMeshes);
				RenderTexture.ReleaseTemporary(temporary2);
				RenderTexture temporary3 = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, m_Format);
				m_FlareMaskMaterial.SetTexture("_MaskTex", m_FlareMask);
				Graphics.Blit(renderTexture, temporary3, m_FlareMaskMaterial, 0);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = null;
				RenderFlares(temporary3, source, ref flareRT);
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				int ghostBokehLayer2 = GetGhostBokehLayer();
				RenderTexture renderTexture2 = m_DownSamples[ghostBokehLayer2];
				RenderTexture temporary4 = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, m_Format);
				BrightPassWithMask(m_DownSamples[ghostBokehLayer2], temporary4, m_FlareTreshold * Vector4.one, m_FlareMask);
				RenderFlares(temporary4, source, ref flareRT);
				RenderTexture.ReleaseTemporary(temporary4);
			}
		}
		if (!m_UseLensFlare && m_FlareSpriteRenderer != null)
		{
			m_FlareSpriteRenderer.Clear(ref m_BokehMeshes);
		}
		if (m_UseAnamorphicFlare)
		{
			RenderTexture renderTexture3 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.Anamorphic);
			if (renderTexture3 != null)
			{
				if (m_UseLensFlare)
				{
					RenderTextureAdditive(renderTexture3, (RenderTexture)flareRT, 1f);
					RenderTexture.ReleaseTemporary(renderTexture3);
				}
				else
				{
					flareRT = renderTexture3;
				}
			}
		}
		if (m_UseStarFlare)
		{
			RenderTexture renderTexture4 = null;
			if (m_StarBlurPass == 1)
			{
				renderTexture4 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.Star);
				if (renderTexture4 != null)
				{
					if (m_UseLensFlare || m_UseAnamorphicFlare)
					{
						RenderTextureAdditive(renderTexture4, (RenderTexture)flareRT, m_StarFlareIntensity);
					}
					else
					{
						flareRT = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
						BlitIntensity(renderTexture4, (RenderTexture)flareRT, m_StarFlareIntensity);
					}
					RenderTexture.ReleaseTemporary(renderTexture4);
				}
			}
			else if (m_UseLensFlare || m_UseAnamorphicFlare)
			{
				renderTexture4 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.DiagonalUpright);
				if (renderTexture4 != null)
				{
					RenderTextureAdditive(renderTexture4, (RenderTexture)flareRT, m_StarFlareIntensity);
					RenderTexture.ReleaseTemporary(renderTexture4);
					renderTexture4 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.DiagonalUpleft);
					RenderTextureAdditive(renderTexture4, (RenderTexture)flareRT, m_StarFlareIntensity);
					RenderTexture.ReleaseTemporary(renderTexture4);
				}
			}
			else
			{
				renderTexture4 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.DiagonalUpleft);
				if (renderTexture4 != null)
				{
					RenderTexture renderTexture5 = RenderStripe(m_DownSamples, upsamplingSize, source.width, source.height, FlareStripeType.DiagonalUpright);
					CombineAdditive(renderTexture5, renderTexture4, m_StarFlareIntensity, m_StarFlareIntensity);
					RenderTexture.ReleaseTemporary(renderTexture5);
					flareRT = renderTexture4;
				}
			}
		}
		if (m_DirectDownSample)
		{
			for (int i = 0; i < m_DownSamples.Length; i++)
			{
				if (m_BufferUsage[i])
				{
					RenderTexture.ReleaseTemporary(m_DownSamples[i]);
				}
			}
		}
		else
		{
			for (int j = 0; j < m_DownSamples.Length; j++)
			{
				RenderTexture.ReleaseTemporary(m_DownSamples[j]);
			}
		}
		m_CombineMaterial.SetFloat("_Intensity", m_BloomIntensity);
		m_CombineMaterial.SetFloat("_FlareIntensity", m_FlareIntensity);
		m_CombineMaterial.SetTexture("_ColorBuffer", source);
		m_CombineMaterial.SetTexture("_FlareTexture", flareRT);
		m_CombineMaterial.SetTexture("_AdditiveTexture", (!m_UseLensDust) ? Texture2D.whiteTexture : m_DustTexture);
		m_CombineMaterial.SetTexture("_brightTexture", temporary);
		if (m_UseLensDust)
		{
			m_CombineMaterial.SetFloat("_DirtIntensity", m_DustIntensity);
			m_CombineMaterial.SetFloat("_DirtLightIntensity", m_DirtLightIntensity);
		}
		else
		{
			m_CombineMaterial.SetFloat("_DirtIntensity", 1f);
			m_CombineMaterial.SetFloat("_DirtLightIntensity", 0f);
		}
		if (m_BlendMode == BlendMode.SCREEN)
		{
			m_CombineMaterial.SetFloat("_ScreenMaxIntensity", m_ScreenMaxIntensity);
		}
		if (m_InvertImage)
		{
			Graphics.Blit(m_LastBloomUpsample, destination, m_CombineMaterial, 1);
		}
		else
		{
			Graphics.Blit(m_LastBloomUpsample, destination, m_CombineMaterial, 0);
		}
		for (int k = 0; k < m_UpSamples.Length; k++)
		{
			if (m_UpSamples[k] != null)
			{
				RenderTexture.ReleaseTemporary(m_UpSamples[k]);
			}
		}
		if (flag2)
		{
			Graphics.Blit(renderTexture, destination);
		}
		if ((m_UseLensFlare || m_UseAnamorphicFlare || m_UseStarFlare) && flareRT != null && flareRT is RenderTexture)
		{
			RenderTexture.ReleaseTemporary((RenderTexture)flareRT);
		}
		RenderTexture.ReleaseTemporary(temporary);
		if (m_FlareShape != null && m_UseBokehFlare && renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	private RenderTexture RenderStar(RenderTexture[] sources, BlurSampleCount upsamplingCount, int sourceWidth, int sourceHeight)
	{
		for (int num = m_StarUpscales.Length - 1; num >= 0; num--)
		{
			m_StarUpscales[num] = RenderTexture.GetTemporary(sourceWidth / (int)Mathf.Pow(2f, num), sourceHeight / (int)Mathf.Pow(2f, num), 0, m_Format);
			m_StarUpscales[num].filterMode = FilterMode.Bilinear;
			float num2 = 1f / (float)sources[num].width;
			float num3 = 1f / (float)sources[num].height;
			if (num < m_StarDownscaleCount - 1)
			{
				GaussianBlur2(sources[num], m_StarUpscales[num], num2 * m_StarScale, num3 * m_StarScale, m_StarUpscales[num + 1], upsamplingCount, Color.white, 1f);
			}
			else
			{
				GaussianBlur2(sources[num], m_StarUpscales[num], num2 * m_StarScale, num3 * m_StarScale, null, upsamplingCount, Color.white, 1f);
			}
		}
		for (int i = 1; i < m_StarUpscales.Length; i++)
		{
			if (m_StarUpscales[i] != null)
			{
				RenderTexture.ReleaseTemporary(m_StarUpscales[i]);
			}
		}
		return m_StarUpscales[0];
	}

	private RenderTexture RenderStripe(RenderTexture[] sources, BlurSampleCount upsamplingCount, int sourceWidth, int sourceHeight, FlareStripeType type)
	{
		RenderTexture[] array = m_AnamorphicUpscales;
		bool[] array2 = m_AnamorphicBloomUsages;
		float[] array3 = m_AnamorphicBloomIntensities;
		Color[] array4 = m_AnamorphicBloomColors;
		bool flag = m_AnamorphicSmallVerticalBlur;
		float num = m_AnamorphicBlurPass;
		float num2 = m_AnamorphicScale;
		float num3 = m_AnamorphicFlareIntensity;
		float num4 = 1f;
		float num5 = 0f;
		if (m_AnamorphicDirection == AnamorphicDirection.Vertical)
		{
			num4 = 0f;
			num5 = 1f;
		}
		if (type != 0)
		{
			array = m_StarUpscales;
			array2 = m_StarBloomUsages;
			array3 = m_StarBloomIntensities;
			array4 = m_StarBloomColors;
			flag = false;
			num = m_StarBlurPass;
			num2 = m_StarScale;
			num3 = m_StarFlareIntensity;
			num5 = ((type != FlareStripeType.DiagonalUpleft) ? 1f : (-1f));
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = null;
		}
		RenderTexture renderTexture = null;
		for (int num6 = array.Length - 1; num6 >= 0; num6--)
		{
			if ((!(sources[num6] == null) || !m_DirectUpsample) && (array2[num6] || !m_DirectUpsample))
			{
				array[num6] = RenderTexture.GetTemporary(sourceWidth / (int)Mathf.Pow(2f, num6), sourceHeight / (int)Mathf.Pow(2f, num6), 0, m_Format);
				array[num6].filterMode = FilterMode.Bilinear;
				float num7 = 1f / (float)array[num6].width;
				float num8 = 1f / (float)array[num6].height;
				RenderTexture source = sources[num6];
				RenderTexture renderTexture2 = array[num6];
				if (!array2[num6])
				{
					if (renderTexture != null)
					{
						if (flag)
						{
							GaussianBlur1(renderTexture, renderTexture2, (m_AnamorphicDirection != AnamorphicDirection.Vertical) ? 0f : num7, (m_AnamorphicDirection != 0) ? 0f : num8, null, BlurSampleCount.FourSimple, Color.white, 1f);
						}
						else
						{
							Graphics.Blit(renderTexture, renderTexture2);
						}
					}
					else
					{
						Graphics.Blit(Texture2D.blackTexture, renderTexture2);
					}
					renderTexture = array[num6];
				}
				else
				{
					RenderTexture renderTexture3 = null;
					if (flag && renderTexture != null)
					{
						renderTexture3 = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, m_Format);
						GaussianBlur1(renderTexture, renderTexture3, (m_AnamorphicDirection != AnamorphicDirection.Vertical) ? 0f : num7, (m_AnamorphicDirection != 0) ? 0f : num8, null, BlurSampleCount.FourSimple, Color.white, 1f);
						renderTexture = renderTexture3;
					}
					if (num == 1f)
					{
						if (type != 0)
						{
							GaussianBlur2(source, renderTexture2, num7 * num2 * num4, num8 * num2 * num5, renderTexture, upsamplingCount, array4[num6], array3[num6] * num3);
						}
						else
						{
							GaussianBlur1(source, renderTexture2, num7 * num2 * num4, num8 * num2 * num5, renderTexture, upsamplingCount, array4[num6], array3[num6] * num3);
						}
					}
					else
					{
						RenderTexture temporary = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, m_Format);
						bool flag2 = false;
						for (int j = 0; (float)j < num; j++)
						{
							RenderTexture additiveTexture = (((float)j != num - 1f) ? null : renderTexture);
							if (j == 0)
							{
								if (type != 0)
								{
									GaussianBlur2(source, temporary, num7 * num2 * num4, num8 * num2 * num5, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								else
								{
									GaussianBlur1(source, temporary, num7 * num2 * num4, num8 * num2 * num5, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								continue;
							}
							num7 = 1f / (float)renderTexture2.width;
							num8 = 1f / (float)renderTexture2.height;
							if (j % 2 == 1)
							{
								if (type != 0)
								{
									GaussianBlur2(temporary, renderTexture2, num7 * num2 * num4 * 1.5f, num8 * num2 * num5 * 1.5f, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								else
								{
									GaussianBlur1(temporary, renderTexture2, num7 * num2 * num4 * 1.5f, num8 * num2 * num5 * 1.5f, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								flag2 = false;
							}
							else
							{
								if (type != 0)
								{
									GaussianBlur2(renderTexture2, temporary, num7 * num2 * num4 * 1.5f, num8 * num2 * num5 * 1.5f, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								else
								{
									GaussianBlur1(renderTexture2, temporary, num7 * num2 * num4 * 1.5f, num8 * num2 * num5 * 1.5f, additiveTexture, upsamplingCount, array4[num6], array3[num6] * num3);
								}
								flag2 = true;
							}
						}
						if (flag2)
						{
							Graphics.Blit(temporary, renderTexture2);
						}
						if (renderTexture3 != null)
						{
							RenderTexture.ReleaseTemporary(renderTexture3);
						}
						RenderTexture.ReleaseTemporary(temporary);
					}
					renderTexture = array[num6];
				}
			}
		}
		RenderTexture renderTexture4 = null;
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k] != null)
			{
				if (renderTexture4 == null)
				{
					renderTexture4 = array[k];
				}
				else
				{
					RenderTexture.ReleaseTemporary(array[k]);
				}
			}
		}
		return renderTexture4;
	}

	private void RenderFlares(RenderTexture brightTexture, RenderTexture source, ref Texture flareRT)
	{
		flareRT = RenderTexture.GetTemporary(source.width, source.height, 0, m_Format);
		flareRT.filterMode = FilterMode.Bilinear;
		m_FlareMaterial.SetVector("_FlareScales", m_FlareScales * m_FlareGlobalScale);
		m_FlareMaterial.SetVector("_FlareScalesNear", m_FlareScalesNear * m_FlareGlobalScale);
		m_FlareMaterial.SetVector("_FlareTint0", m_FlareTint0);
		m_FlareMaterial.SetVector("_FlareTint1", m_FlareTint1);
		m_FlareMaterial.SetVector("_FlareTint2", m_FlareTint2);
		m_FlareMaterial.SetVector("_FlareTint3", m_FlareTint3);
		m_FlareMaterial.SetVector("_FlareTint4", m_FlareTint4);
		m_FlareMaterial.SetVector("_FlareTint5", m_FlareTint5);
		m_FlareMaterial.SetVector("_FlareTint6", m_FlareTint6);
		m_FlareMaterial.SetVector("_FlareTint7", m_FlareTint7);
		m_FlareMaterial.SetFloat("_Intensity", m_FlareIntensity);
		if (m_FlareRendering == FlareRendering.Sharp)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, m_Format);
			temporary.filterMode = FilterMode.Bilinear;
			RenderSimple(brightTexture, temporary, 1f / (float)brightTexture.width, 1f / (float)brightTexture.height, SimpleSampleCount.Four);
			Graphics.Blit(temporary, (RenderTexture)flareRT, m_FlareMaterial, 0);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (m_FlareBlurQuality == FlareBlurQuality.Fast)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
			temporary2.filterMode = FilterMode.Bilinear;
			RenderTexture temporary3 = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
			temporary3.filterMode = FilterMode.Bilinear;
			Graphics.Blit(brightTexture, temporary2, m_FlareMaterial, 0);
			if (m_FlareRendering == FlareRendering.Blurred)
			{
				GaussianBlurSeparate(temporary2, temporary3, 1f / (float)temporary2.width, 1f / (float)temporary2.height, null, BlurSampleCount.Thirteen, Color.white, 1f);
				RenderSimple(temporary3, (RenderTexture)flareRT, 1f / (float)temporary3.width, 1f / (float)temporary3.height, SimpleSampleCount.Four);
			}
			else if (m_FlareRendering == FlareRendering.MoreBlurred)
			{
				GaussianBlurSeparate(temporary2, temporary3, 1f / (float)temporary2.width, 1f / (float)temporary2.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1f);
				RenderSimple(temporary3, (RenderTexture)flareRT, 1f / (float)temporary3.width, 1f / (float)temporary3.height, SimpleSampleCount.Four);
			}
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		else if (m_FlareBlurQuality == FlareBlurQuality.Normal)
		{
			RenderTexture temporary4 = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
			temporary4.filterMode = FilterMode.Bilinear;
			RenderTexture temporary5 = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
			temporary5.filterMode = FilterMode.Bilinear;
			RenderTexture temporary6 = RenderTexture.GetTemporary(brightTexture.width / 4, brightTexture.height / 4, 0, m_Format);
			temporary6.filterMode = FilterMode.Bilinear;
			RenderSimple(brightTexture, temporary4, 1f / (float)brightTexture.width, 1f / (float)brightTexture.height, SimpleSampleCount.Four);
			RenderSimple(temporary4, temporary5, 1f / (float)temporary4.width, 1f / (float)temporary4.height, SimpleSampleCount.Four);
			Graphics.Blit(temporary5, temporary6, m_FlareMaterial, 0);
			if (m_FlareRendering == FlareRendering.Blurred)
			{
				GaussianBlurSeparate(temporary6, temporary5, 1f / (float)temporary5.width, 1f / (float)temporary5.height, null, BlurSampleCount.Thirteen, Color.white, 1f);
				RenderSimple(temporary5, (RenderTexture)flareRT, 1f / (float)temporary5.width, 1f / (float)temporary5.height, SimpleSampleCount.Four);
			}
			else if (m_FlareRendering == FlareRendering.MoreBlurred)
			{
				GaussianBlurSeparate(temporary6, temporary5, 1f / (float)temporary5.width, 1f / (float)temporary5.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1f);
				RenderSimple(temporary5, (RenderTexture)flareRT, 1f / (float)temporary5.width, 1f / (float)temporary5.height, SimpleSampleCount.Four);
			}
			RenderTexture.ReleaseTemporary(temporary4);
			RenderTexture.ReleaseTemporary(temporary5);
			RenderTexture.ReleaseTemporary(temporary6);
		}
		else if (m_FlareBlurQuality == FlareBlurQuality.High)
		{
			RenderTexture temporary7 = RenderTexture.GetTemporary(brightTexture.width / 2, brightTexture.height / 2, 0, m_Format);
			temporary7.filterMode = FilterMode.Bilinear;
			RenderTexture temporary8 = RenderTexture.GetTemporary(temporary7.width / 2, temporary7.height / 2, 0, m_Format);
			temporary8.filterMode = FilterMode.Bilinear;
			RenderTexture temporary9 = RenderTexture.GetTemporary(temporary8.width / 2, temporary8.height / 2, 0, m_Format);
			temporary9.filterMode = FilterMode.Bilinear;
			RenderTexture temporary10 = RenderTexture.GetTemporary(temporary8.width / 2, temporary8.height / 2, 0, m_Format);
			temporary10.filterMode = FilterMode.Bilinear;
			RenderSimple(brightTexture, temporary7, 1f / (float)brightTexture.width, 1f / (float)brightTexture.height, SimpleSampleCount.Four);
			RenderSimple(temporary7, temporary8, 1f / (float)temporary7.width, 1f / (float)temporary7.height, SimpleSampleCount.Four);
			RenderSimple(temporary8, temporary9, 1f / (float)temporary8.width, 1f / (float)temporary8.height, SimpleSampleCount.Four);
			Graphics.Blit(temporary9, temporary10, m_FlareMaterial, 0);
			if (m_FlareRendering == FlareRendering.Blurred)
			{
				GaussianBlurSeparate(temporary10, temporary9, 1f / (float)temporary9.width, 1f / (float)temporary9.height, null, BlurSampleCount.Thirteen, Color.white, 1f);
				RenderSimple(temporary9, (RenderTexture)flareRT, 1f / (float)temporary9.width, 1f / (float)temporary9.height, SimpleSampleCount.Four);
			}
			else if (m_FlareRendering == FlareRendering.MoreBlurred)
			{
				GaussianBlurSeparate(temporary10, temporary9, 1f / (float)temporary9.width, 1f / (float)temporary9.height, null, BlurSampleCount.ThrirtyOne, Color.white, 1f);
				RenderSimple(temporary9, (RenderTexture)flareRT, 1f / (float)temporary9.width, 1f / (float)temporary9.height, SimpleSampleCount.Four);
			}
			RenderTexture.ReleaseTemporary(temporary7);
			RenderTexture.ReleaseTemporary(temporary8);
			RenderTexture.ReleaseTemporary(temporary9);
			RenderTexture.ReleaseTemporary(temporary10);
		}
	}

	private void CachedUpsample(RenderTexture[] sources, RenderTexture[] destinations, int originalWidth, int originalHeight, BlurSampleCount upsamplingCount)
	{
		RenderTexture renderTexture = null;
		for (int i = 0; i < m_UpSamples.Length; i++)
		{
			m_UpSamples[i] = null;
		}
		for (int num = destinations.Length - 1; num >= 0; num--)
		{
			if (m_BloomUsages[num] || !m_DirectUpsample)
			{
				m_UpSamples[num] = RenderTexture.GetTemporary(originalWidth / (int)Mathf.Pow(2f, num), originalHeight / (int)Mathf.Pow(2f, num), 0, m_Format);
				m_UpSamples[num].filterMode = FilterMode.Bilinear;
			}
			float num2 = 1f;
			if (m_BloomUsages[num])
			{
				float num3 = 1f / (float)sources[num].width;
				float verticalBlur = 1f / (float)sources[num].height;
				GaussianBlurSeparate(m_DownSamples[num], m_UpSamples[num], num3 * num2, verticalBlur, renderTexture, upsamplingCount, m_BloomColors[num], m_BloomIntensities[num]);
			}
			else if (num < m_DownscaleCount - 1)
			{
				if (!m_DirectUpsample)
				{
					RenderSimple(renderTexture, m_UpSamples[num], 1f / (float)m_UpSamples[num].width, 1f / (float)m_UpSamples[num].height, SimpleSampleCount.Four);
				}
			}
			else
			{
				Graphics.Blit(Texture2D.blackTexture, m_UpSamples[num]);
			}
			if (m_BloomUsages[num] || !m_DirectUpsample)
			{
				renderTexture = m_UpSamples[num];
			}
		}
		m_LastBloomUpsample = renderTexture;
	}

	private void CachedDownsample(RenderTexture source, RenderTexture[] destinations, DeluxeFilmicCurve intensityCurve, bool hdr)
	{
		int num = destinations.Length;
		RenderTexture renderTexture = source;
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			if (m_DirectDownSample && !m_BufferUsage[i])
			{
				continue;
			}
			destinations[i] = RenderTexture.GetTemporary(source.width / (int)Mathf.Pow(2f, i + 1), source.height / (int)Mathf.Pow(2f, i + 1), 0, m_Format);
			destinations[i].filterMode = FilterMode.Bilinear;
			RenderTexture destination = destinations[i];
			float num2 = 1f;
			float num3 = 1f / (float)renderTexture.width;
			float num4 = 1f / (float)renderTexture.height;
			if (intensityCurve != null && !flag)
			{
				intensityCurve.StoreK();
				m_SamplingMaterial.SetFloat("_CurveExposure", intensityCurve.GetExposure());
				m_SamplingMaterial.SetFloat("_K", intensityCurve.m_k);
				m_SamplingMaterial.SetFloat("_Crossover", intensityCurve.m_CrossOverPoint);
				m_SamplingMaterial.SetVector("_Toe", intensityCurve.m_ToeCoef);
				m_SamplingMaterial.SetVector("_Shoulder", intensityCurve.m_ShoulderCoef);
				float value = ((!hdr) ? 1f : 2f);
				m_SamplingMaterial.SetFloat("_MaxValue", value);
				num3 = 1f / (float)renderTexture.width;
				num4 = 1f / (float)renderTexture.height;
				if (m_TemporalStableDownsampling)
				{
					RenderSimple(renderTexture, destination, num3 * num2, num4 * num2, SimpleSampleCount.ThirteenTemporalCurve);
				}
				else
				{
					RenderSimple(renderTexture, destination, num3 * num2, num4 * num2, SimpleSampleCount.FourCurve);
				}
				flag = true;
			}
			else if (m_TemporalStableDownsampling)
			{
				RenderSimple(renderTexture, destination, num3 * num2, num4 * num2, SimpleSampleCount.ThirteenTemporal);
			}
			else
			{
				RenderSimple(renderTexture, destination, num3 * num2, num4 * num2, SimpleSampleCount.Four);
			}
			renderTexture = destinations[i];
		}
	}

	private void BrightPass(RenderTexture source, RenderTexture destination, Vector4 treshold)
	{
		m_BrightpassMaterial.SetTexture("_MaskTex", Texture2D.whiteTexture);
		m_BrightpassMaterial.SetVector("_Threshhold", treshold);
		Graphics.Blit(source, destination, m_BrightpassMaterial, 0);
	}

	private void BrightPassWithMask(RenderTexture source, RenderTexture destination, Vector4 treshold, Texture mask)
	{
		m_BrightpassMaterial.SetTexture("_MaskTex", mask);
		m_BrightpassMaterial.SetVector("_Threshhold", treshold);
		Graphics.Blit(source, destination, m_BrightpassMaterial, 0);
	}

	private void RenderSimple(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, SimpleSampleCount sampleCount)
	{
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0f, 0f));
		switch (sampleCount)
		{
		case SimpleSampleCount.Four:
			Graphics.Blit(source, destination, m_SamplingMaterial, 0);
			break;
		case SimpleSampleCount.Nine:
			Graphics.Blit(source, destination, m_SamplingMaterial, 1);
			break;
		case SimpleSampleCount.FourCurve:
			Graphics.Blit(source, destination, m_SamplingMaterial, 5);
			break;
		case SimpleSampleCount.ThirteenTemporal:
			Graphics.Blit(source, destination, m_SamplingMaterial, 11);
			break;
		case SimpleSampleCount.ThirteenTemporalCurve:
			Graphics.Blit(source, destination, m_SamplingMaterial, 12);
			break;
		}
	}

	private void GaussianBlur1(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
	{
		int pass = 2;
		if (sampleCount == BlurSampleCount.Seventeen)
		{
			pass = 3;
		}
		if (sampleCount == BlurSampleCount.Nine)
		{
			pass = 4;
		}
		if (sampleCount == BlurSampleCount.NineCurve)
		{
			pass = 6;
		}
		if (sampleCount == BlurSampleCount.FourSimple)
		{
			pass = 7;
		}
		if (sampleCount == BlurSampleCount.Thirteen)
		{
			pass = 8;
		}
		if (sampleCount == BlurSampleCount.TwentyThree)
		{
			pass = 9;
		}
		if (sampleCount == BlurSampleCount.TwentySeven)
		{
			pass = 10;
		}
		Texture texture = null;
		texture = ((!(additiveTexture == null)) ? ((Texture)additiveTexture) : ((Texture)Texture2D.blackTexture));
		m_SamplingMaterial.SetTexture("_AdditiveTexture", texture);
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0f, 0f));
		m_SamplingMaterial.SetVector("_Tint", tint);
		m_SamplingMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, destination, m_SamplingMaterial, pass);
	}

	private void GaussianBlur2(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(destination.width, destination.height, destination.depth, destination.format);
		temporary.filterMode = FilterMode.Bilinear;
		int pass = 2;
		if (sampleCount == BlurSampleCount.Seventeen)
		{
			pass = 3;
		}
		if (sampleCount == BlurSampleCount.Nine)
		{
			pass = 4;
		}
		if (sampleCount == BlurSampleCount.NineCurve)
		{
			pass = 6;
		}
		if (sampleCount == BlurSampleCount.FourSimple)
		{
			pass = 7;
		}
		if (sampleCount == BlurSampleCount.Thirteen)
		{
			pass = 8;
		}
		if (sampleCount == BlurSampleCount.TwentyThree)
		{
			pass = 9;
		}
		if (sampleCount == BlurSampleCount.TwentySeven)
		{
			pass = 10;
		}
		Texture texture = null;
		texture = ((!(additiveTexture == null)) ? ((Texture)additiveTexture) : ((Texture)Texture2D.blackTexture));
		m_SamplingMaterial.SetTexture("_AdditiveTexture", texture);
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, verticalBlur, 0f, 0f));
		m_SamplingMaterial.SetVector("_Tint", tint);
		m_SamplingMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, temporary, m_SamplingMaterial, pass);
		texture = temporary;
		m_SamplingMaterial.SetTexture("_AdditiveTexture", texture);
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(0f - horizontalBlur, verticalBlur, 0f, 0f));
		m_SamplingMaterial.SetVector("_Tint", tint);
		m_SamplingMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, destination, m_SamplingMaterial, pass);
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void GaussianBlurSeparate(RenderTexture source, RenderTexture destination, float horizontalBlur, float verticalBlur, RenderTexture additiveTexture, BlurSampleCount sampleCount, Color tint, float intensity)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(destination.width, destination.height, destination.depth, destination.format);
		temporary.filterMode = FilterMode.Bilinear;
		int pass = 2;
		if (sampleCount == BlurSampleCount.Seventeen)
		{
			pass = 3;
		}
		if (sampleCount == BlurSampleCount.Nine)
		{
			pass = 4;
		}
		if (sampleCount == BlurSampleCount.NineCurve)
		{
			pass = 6;
		}
		if (sampleCount == BlurSampleCount.FourSimple)
		{
			pass = 7;
		}
		if (sampleCount == BlurSampleCount.Thirteen)
		{
			pass = 8;
		}
		if (sampleCount == BlurSampleCount.TwentyThree)
		{
			pass = 9;
		}
		if (sampleCount == BlurSampleCount.TwentySeven)
		{
			pass = 10;
		}
		m_SamplingMaterial.SetTexture("_AdditiveTexture", Texture2D.blackTexture);
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(0f, verticalBlur, 0f, 0f));
		m_SamplingMaterial.SetVector("_Tint", tint);
		m_SamplingMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, temporary, m_SamplingMaterial, pass);
		Texture texture = null;
		texture = ((!(additiveTexture == null)) ? ((Texture)additiveTexture) : ((Texture)Texture2D.blackTexture));
		m_SamplingMaterial.SetTexture("_AdditiveTexture", texture);
		m_SamplingMaterial.SetVector("_OffsetInfos", new Vector4(horizontalBlur, 0f, 1f / (float)destination.width, 1f / (float)destination.height));
		m_SamplingMaterial.SetVector("_Tint", Color.white);
		m_SamplingMaterial.SetFloat("_Intensity", 1f);
		Graphics.Blit(temporary, destination, m_SamplingMaterial, pass);
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void RenderTextureAdditive(RenderTexture source, RenderTexture destination, float intensity)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
		Graphics.Blit(destination, temporary);
		m_MixerMaterial.SetTexture("_ColorBuffer", temporary);
		m_MixerMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, destination, m_MixerMaterial, 0);
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void BlitIntensity(RenderTexture source, RenderTexture destination, float intensity)
	{
		m_MixerMaterial.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, destination, m_MixerMaterial, 2);
	}

	private void CombineAdditive(RenderTexture source, RenderTexture destination, float intensitySource, float intensityDestination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
		Graphics.Blit(destination, temporary);
		m_MixerMaterial.SetTexture("_ColorBuffer", temporary);
		m_MixerMaterial.SetFloat("_Intensity0", intensitySource);
		m_MixerMaterial.SetFloat("_Intensity1", intensityDestination);
		Graphics.Blit(source, destination, m_MixerMaterial, 1);
		RenderTexture.ReleaseTemporary(temporary);
	}

	public void SetFilmicCurveParameters(float middle, float dark, float bright, float highlights)
	{
		m_BloomCurve.m_ToeStrength = -1f * dark;
		m_BloomCurve.m_ShoulderStrength = bright;
		m_BloomCurve.m_Highlights = highlights;
		m_BloomCurve.m_CrossOverPoint = middle;
		m_BloomCurve.UpdateCoefficients();
	}
}

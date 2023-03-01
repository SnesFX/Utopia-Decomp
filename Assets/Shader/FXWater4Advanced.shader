Shader "FX/Water4" {
Properties {
 _ReflectionTex ("Internal reflection", 2D) = "white" { }
 _MainTex ("Fallback texture", 2D) = "black" { }
 _ShoreTex ("Shore & Foam texture ", 2D) = "black" { }
 _BumpMap ("Normals ", 2D) = "bump" { }
 _DistortParams ("Distortions (Bump waves, Reflection, Fresnel power, Fresnel bias)", Vector) = (1.000000,1.000000,2.000000,1.150000)
 _InvFadeParemeter ("Auto blend parameter (Edge, Shore, Distance scale)", Vector) = (0.150000,0.150000,0.500000,1.000000)
 _AnimationTiling ("Animation Tiling (Displacement)", Vector) = (2.200000,2.200000,-1.100000,-1.100000)
 _AnimationDirection ("Animation Direction (displacement)", Vector) = (1.000000,1.000000,1.000000,1.000000)
 _BumpTiling ("Bump Tiling", Vector) = (1.000000,1.000000,-2.000000,3.000000)
 _BumpDirection ("Bump Direction & Speed", Vector) = (1.000000,1.000000,-1.000000,1.000000)
 _FresnelScale ("FresnelScale", Range(0.150000,4.000000)) = 0.750000
 _BaseColor ("Base color", Color) = (0.540000,0.950000,0.990000,0.500000)
 _ReflectionColor ("Reflection color", Color) = (0.540000,0.950000,0.990000,0.500000)
 _SpecularColor ("Specular color", Color) = (0.720000,0.720000,0.720000,1.000000)
 _WorldLightDir ("Specular light direction", Vector) = (0.000000,0.100000,-0.500000,0.000000)
 _Shininess ("Shininess", Range(2.000000,500.000000)) = 200.000000
 _Foam ("Foam (intensity, cutoff)", Vector) = (0.100000,0.375000,0.000000,0.000000)
 _GerstnerIntensity ("Per vertex displacement", Float) = 1.000000
 _GAmplitude ("Wave Amplitude", Vector) = (0.300000,0.350000,0.250000,0.250000)
 _GFrequency ("Wave Frequency", Vector) = (1.300000,1.350000,1.250000,1.250000)
 _GSteepness ("Wave Steepness", Vector) = (1.000000,1.000000,1.000000,1.000000)
 _GSpeed ("Wave Speed", Vector) = (1.200000,1.375000,1.100000,1.500000)
 _GDirectionAB ("Wave Direction", Vector) = (0.300000,0.850000,0.850000,0.250000)
 _GDirectionCD ("Wave Direction", Vector) = (0.100000,0.900000,0.500000,0.500000)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}
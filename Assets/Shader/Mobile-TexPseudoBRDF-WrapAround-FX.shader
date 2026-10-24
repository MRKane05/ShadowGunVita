Shader "MADFINGER/Characters/BRDFLit FX (Supports Backlight)" {
Properties {
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
	_LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
	_NoiseTex ("Noise tex", 2D) = "white" {}
	_FXColor ("FXColor", Color) = (0,0.97,0.89,1)
	_TimeOffs ("Time offs", Float) = 0
	_Duration ("Duration", Float) = 2
	_Invert ("Invert", Float) = 0
}	
SubShader { 
	Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
	LOD 400
	
CGPROGRAM
#pragma surface surf PseudoBRDF exclude_path:prepass vertex:separateSH nolightmap noforwardadd noambient approxview

struct MySurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 OcclusionAndAmbientLight;
};

sampler2D _BRDFTex;
float _LightProbesLightingAmount;

inline fixed4 LightingPseudoBRDF (MySurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
	// Half vector
	fixed3 halfDir = normalize (lightDir + viewDir);
	
	// N.L
	fixed NdotL = dot (s.Normal, lightDir);
	// N.H
	fixed NdotH = dot (s.Normal, halfDir);
	
	// remap N.L from [-1..1] to [0..1]
	// this way we can shade pixels facing away from the light - helps to simulate bounce lights
	fixed biasNdotL = NdotL * 0.5 + 0.5;
	
	// lookup light texture
	//  rgb = diffuse term
	//    a = specular term
	fixed4 l = tex2D (_BRDFTex, fixed2(biasNdotL, NdotH));

	fixed4 c;
	// mask specular term by Gloss factor
	// modulate specular with Albedo to allow metalic-ish look
	//
	// Shadowgun style: instead of adding LightProbes, treat them as both occlusion for MainLight and Ambient bounce
	// that is not physically correct, but it
	// 1) provides way to occlude MainLight without using runtime shadows
	// 2) allows bounce light to be affected by per-pixel normals
	// note that bounce lights becomes much weaker!
	c.rgb = s.OcclusionAndAmbientLight * s.Albedo * (l.rgb + s.Gloss * l.a) * 2;
	c.a = 0;
	
	return c;
}


sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _NoiseTex;
fixed4 _FXColor;
float _TimeOffs;
float _Duration;
float _Invert;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	fixed3 shOcclusionAndAmbient;
};

void separateSH (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	float3 worldN = mul ((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
	o.shOcclusionAndAmbient = lerp(fixed3(1,1,1), ShadeSH9 (float4(worldN,1.0)), _LightProbesLightingAmount);
}

void surf (Input IN, inout MySurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	o.OcclusionAndAmbientLight = IN.shOcclusionAndAmbient;
	// Spawn/teleport FX: a band of _FXColor sweeps through the noise texture over
	// _Duration seconds starting at _TimeOffs (reversed when _Invert is set).
	float t = saturate((_Time.y - _TimeOffs) / max(_Duration, 0.001));
	t = lerp(t, 1 - t, saturate(_Invert));
	fixed n = tex2D(_NoiseTex, IN.uv_MainTex).r;
	fixed band = saturate(1 - abs(n - t) * 8) * (t < 1 ? 1 : 0);
	o.Emission = _FXColor.rgb * band;
}
ENDCG

	}

FallBack "Mobile/Diffuse"
}

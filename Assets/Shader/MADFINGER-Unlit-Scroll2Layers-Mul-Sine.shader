// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative No Lightmap Sine)" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _SineAmplX ("Base layer sine amplitude X", Float) = 0.5
 _SineAmplY ("Base layer sine amplitude Y", Float) = 0.5
 _SineFreqX ("Base layer sine freq X", Float) = 10
 _SineFreqY ("Base layer sine freq Y", Float) = 10
 _SineAmplX2 ("2nd layer sine amplitude X", Float) = 0.5
 _SineAmplY2 ("2nd layer sine amplitude Y", Float) = 0.5
 _SineFreqX2 ("2nd layer sine freq X", Float) = 10
 _SineFreqY2 ("2nd layer sine freq Y", Float) = 10
 _MMultiplier ("Layer Multiplier", Float) = 2
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  sampler2D _DetailTex;
  float4 _DetailTex_ST;
  float _MMultiplier;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  float _Scroll2X;
  float _Scroll2Y;
  float _ScrollX;
  float _ScrollY;
  float _SineAmplX;
  float _SineAmplX2;
  float _SineAmplY;
  float _SineAmplY2;
  float _SineFreqX;
  float _SineFreqX2;
  float _SineFreqY;
  float _SineFreqY2;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    half4 tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3.x = _ScrollX;
    tmpvar_3.y = _ScrollY;
    tmpvar_1.xy = (((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw) + frac((tmpvar_3 * _Time.xy)));
    float2 tmpvar_4;
    tmpvar_4.x = _Scroll2X;
    tmpvar_4.y = _Scroll2Y;
    tmpvar_1.zw = (((v.texcoord.xy * _DetailTex_ST.xy) + _DetailTex_ST.zw) + frac((tmpvar_4 * _Time.xy)));
    tmpvar_1.x = (tmpvar_1.x + (sin((_Time * _SineFreqX)) * _SineAmplX).x);
    tmpvar_1.y = (tmpvar_1.y + (sin((_Time * _SineFreqY)) * _SineAmplY).x);
    tmpvar_1.z = (tmpvar_1.z + (sin((_Time * _SineFreqX2)) * _SineAmplX2).x);
    tmpvar_1.w = (tmpvar_1.w + (sin((_Time * _SineFreqY2)) * _SineAmplY2).x);
    float4 tmpvar_5;
    tmpvar_5 = ((float4)_MMultiplier);
    tmpvar_2 = tmpvar_5;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_1;
    o.xlv_TEXCOORD1 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = ((tex2D(_MainTex, i.xlv_TEXCOORD0.xy) * tex2D(_DetailTex, i.xlv_TEXCOORD0.zw)) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    half4 tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3.x = _ScrollX;
    tmpvar_3.y = _ScrollY;
    tmpvar_1.xy = (((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw) + frac((tmpvar_3 * _Time.xy)));
    float2 tmpvar_4;
    tmpvar_4.x = _Scroll2X;
    tmpvar_4.y = _Scroll2Y;
    tmpvar_1.zw = (((v.texcoord.xy * _DetailTex_ST.xy) + _DetailTex_ST.zw) + frac((tmpvar_4 * _Time.xy)));
    tmpvar_1.x = (tmpvar_1.x + (sin((_Time * _SineFreqX)) * _SineAmplX).x);
    tmpvar_1.y = (tmpvar_1.y + (sin((_Time * _SineFreqY)) * _SineAmplY).x);
    tmpvar_1.z = (tmpvar_1.z + (sin((_Time * _SineFreqX2)) * _SineAmplX2).x);
    tmpvar_1.w = (tmpvar_1.w + (sin((_Time * _SineFreqY2)) * _SineAmplY2).x);
    float4 tmpvar_5;
    tmpvar_5 = ((float4)_MMultiplier);
    tmpvar_2 = tmpvar_5;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_1;
    o.xlv_TEXCOORD1 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = ((tex2D(_MainTex, i.xlv_TEXCOORD0.xy) * tex2D(_DetailTex, i.xlv_TEXCOORD0.zw)) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
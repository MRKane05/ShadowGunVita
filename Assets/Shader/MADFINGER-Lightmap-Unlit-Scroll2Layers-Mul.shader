// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative (Supports Lightmap)" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _MMultiplier ("Layer Multiplier", Float) = 2
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  sampler2D _DetailTex;
  float _MMultiplier;
  sampler2D _MainTex;
  float _Scroll2X;
  float _Scroll2Y;
  float _ScrollX;
  float _ScrollY;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD3 : TEXCOORD3;
    half4 xlv_TEXCOORD2 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float2 tmpvar_2;
    tmpvar_2.x = _ScrollX;
    tmpvar_2.y = _ScrollY;
    float2 tmpvar_3;
    tmpvar_3.x = _Scroll2X;
    tmpvar_3.y = _Scroll2Y;
    float4 tmpvar_4;
    tmpvar_4.x = _MMultiplier;
    tmpvar_4.y = _MMultiplier;
    tmpvar_4.z = _MMultiplier;
    tmpvar_4.w = _MMultiplier;
    tmpvar_1 = tmpvar_4;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (v.texcoord.xy + frac((tmpvar_2 * _Time.xy)));
    o.xlv_TEXCOORD1 = (v.texcoord.xy + frac((tmpvar_3 * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    o.xlv_TEXCOORD3 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 o_g;
    half4 tmpvar_1;
    tmpvar_1 = ((tex2D(_MainTex, i.xlv_TEXCOORD0) * tex2D(_DetailTex, i.xlv_TEXCOORD1)) * i.xlv_TEXCOORD2);
    o_g = tmpvar_1;
    o_g.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD3).xyz));
    sg_FragColor = o_g;
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD2 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float2 tmpvar_2;
    tmpvar_2.x = _ScrollX;
    tmpvar_2.y = _ScrollY;
    float2 tmpvar_3;
    tmpvar_3.x = _Scroll2X;
    tmpvar_3.y = _Scroll2Y;
    float4 tmpvar_4;
    tmpvar_4.x = _MMultiplier;
    tmpvar_4.y = _MMultiplier;
    tmpvar_4.z = _MMultiplier;
    tmpvar_4.w = _MMultiplier;
    tmpvar_1 = tmpvar_4;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (v.texcoord.xy + frac((tmpvar_2 * _Time.xy)));
    o.xlv_TEXCOORD1 = (v.texcoord.xy + frac((tmpvar_3 * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = ((tex2D(_MainTex, i.xlv_TEXCOORD0) * tex2D(_DetailTex, i.xlv_TEXCOORD1)) * i.xlv_TEXCOORD2);
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
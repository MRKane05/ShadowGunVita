// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Bullet trail" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _AMultiplier ("Layer Multiplier", Float) = 0.5
 _TintColor ("Tint color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  float _AMultiplier;
  sampler2D _DetailTex;
  float4 _DetailTex_ST;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  float _Scroll2X;
  float _Scroll2Y;
  float _ScrollX;
  float _ScrollY;
  float4 _TintColor;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
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
    tmpvar_4 = (((v.color * _TintColor) * _TintColor.w) * 2.0);
    tmpvar_1 = tmpvar_4;
    float3 tmpvar_5;
    tmpvar_5 = (tmpvar_1.xyz * _AMultiplier);
    tmpvar_1.xyz = tmpvar_5;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw) + frac((tmpvar_2 * _Time.xy)));
    o.xlv_TEXCOORD1 = (((v.texcoord.xy * _DetailTex_ST.xy) + _DetailTex_ST.zw) + frac((tmpvar_3 * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    sg_FragColor = (((tmpvar_1 + tex2D(_DetailTex, i.xlv_TEXCOORD1)) * tmpvar_1.x) * i.xlv_TEXCOORD2);
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
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
    tmpvar_4 = (((v.color * _TintColor) * _TintColor.w) * 2.0);
    tmpvar_1 = tmpvar_4;
    float3 tmpvar_5;
    tmpvar_5 = (tmpvar_1.xyz * _AMultiplier);
    tmpvar_1.xyz = tmpvar_5;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw) + frac((tmpvar_2 * _Time.xy)));
    o.xlv_TEXCOORD1 = (((v.texcoord.xy * _DetailTex_ST.xy) + _DetailTex_ST.zw) + frac((tmpvar_3 * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    sg_FragColor = (((tmpvar_1 + tex2D(_DetailTex, i.xlv_TEXCOORD1)) * tmpvar_1.x) * i.xlv_TEXCOORD2);
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Menu - 3 layers" {
Properties {
 _BaseTex ("Base layer", 2D) = "white" {}
 _MulTex ("Mul layer", 2D) = "white" {}
 _AddTex ("Add layer", 2D) = "white" {}
 _ScrollBase ("Base layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollMul ("Mul layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollAdd ("Add layer scroll speed ", Vector) = (0,0,0,0)
 _Multiplier ("Multiplier", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Transparent" }
 Pass {
  Tags { "RenderType"="Transparent" }
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  sampler2D _AddTex;
  sampler2D _BaseTex;
  sampler2D _MulTex;
  float _Multiplier;
  float4 _ScrollAdd;
  float4 _ScrollBase;
  float4 _ScrollMul;
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
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2.xy = (v.texcoord.xy + frac((_ScrollBase.xy * _Time.xy)));
    tmpvar_2.zw = (v.texcoord.xy + frac((_ScrollMul.xy * _Time.xy)));
    tmpvar_1 = v.color;
    float3 tmpvar_3;
    tmpvar_3 = (v.color.xyz * _Multiplier);
    tmpvar_1.xyz = tmpvar_3;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_2;
    o.xlv_TEXCOORD1 = (v.texcoord.xy + frac((_ScrollAdd.xy * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (((tex2D(_BaseTex, i.xlv_TEXCOORD0.xy) + tex2D(_AddTex, i.xlv_TEXCOORD1)) * tex2D(_MulTex, i.xlv_TEXCOORD0.zw)) * i.xlv_TEXCOORD2);
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
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2.xy = (v.texcoord.xy + frac((_ScrollBase.xy * _Time.xy)));
    tmpvar_2.zw = (v.texcoord.xy + frac((_ScrollMul.xy * _Time.xy)));
    tmpvar_1 = v.color;
    float3 tmpvar_3;
    tmpvar_3 = (v.color.xyz * _Multiplier);
    tmpvar_1.xyz = tmpvar_3;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_2;
    o.xlv_TEXCOORD1 = (v.texcoord.xy + frac((_ScrollAdd.xy * _Time.xy)));
    o.xlv_TEXCOORD2 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (((tex2D(_BaseTex, i.xlv_TEXCOORD0.xy) + tex2D(_AddTex, i.xlv_TEXCOORD1)) * tex2D(_MulTex, i.xlv_TEXCOORD0.zw)) * i.xlv_TEXCOORD2);
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
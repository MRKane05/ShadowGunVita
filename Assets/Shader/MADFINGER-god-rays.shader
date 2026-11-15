// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Transparent/GodRays" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Multiplier", Float) = 1
 _ContractionAmount ("Near contraction amount", Float) = 5
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
  #include "UnityCG.cginc"
  float _ContractionAmount;
  float _FadeOutDistFar;
  float _FadeOutDistNear;
  sampler2D _MainTex;
  float _Multiplier;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float4 vpos;
    float tmpvar_2;
    tmpvar_2 = length((mul(UNITY_MATRIX_MV, v.vertex)).xyz);
    float tmpvar_3;
    tmpvar_3 = clamp((tmpvar_2 / _FadeOutDistNear), 0.0, 1.0);
    float tmpvar_4;
    tmpvar_4 = (1.0 - clamp((max((tmpvar_2 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
    float tmpvar_5;
    tmpvar_5 = (tmpvar_3 * tmpvar_3);
    float tmpvar_6;
    tmpvar_6 = ((tmpvar_5 * tmpvar_5) * (tmpvar_4 * tmpvar_4));
    vpos = v.vertex;
    vpos.xyz = (v.vertex.xyz - (((normalize(v.normal) * clamp((1.0 - tmpvar_6), 0.0, 1.0)) * v.color.w) * _ContractionAmount));
    float4 tmpvar_7;
    tmpvar_7 = ((tmpvar_6 * v.color) * _Multiplier);
    tmpvar_1 = tmpvar_7;
    o.pos = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, vpos)));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
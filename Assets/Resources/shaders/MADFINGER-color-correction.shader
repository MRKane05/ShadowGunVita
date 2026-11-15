// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/PostFX/ColorCorrection" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float4x4 _ColorMatrix;
  sampler2D _MainTex;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float2 tmpvar_1;
    half2 tmpvar_2;
    tmpvar_2 = v.texcoord.xy;
    tmpvar_1 = tmpvar_2;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 dst;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    float4 tmpvar_2;
    tmpvar_2 = (mul(tmpvar_1, _ColorMatrix));
    dst = tmpvar_2;
    half4 tmpvar_3;
    tmpvar_3.xyz = dst.xyz;
    tmpvar_3.w = tmpvar_1.w;
    sg_FragColor = tmpvar_3;
    return sg_FragColor;
  }
  ENDCG
 }
}
Fallback Off
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Lightmap + emissivity" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _EmisColor ("Emisive color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float3 _EmisColor;
  sampler2D _MainTex;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half3 lm;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    half3 tmpvar_2;
    tmpvar_2 = (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1).xyz);
    float3 tmpvar_3;
    tmpvar_3 = (tmpvar_2 + (tmpvar_1.www * _EmisColor));
    lm = tmpvar_3;
    c.xyz = (tmpvar_1.xyz * lm);
    sg_FragColor = c;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
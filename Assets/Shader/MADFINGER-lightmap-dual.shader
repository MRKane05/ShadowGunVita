// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Dual diffuse with lightmap" {
Properties {
 _MainTex0 ("Base layer0 (RGB)", 2D) = "white" {}
 _MainTex1 ("Base layer1 (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  sampler2D _MainTex0;
  sampler2D _MainTex1;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    tmpvar_1.xy = v.texcoord.xy;
    tmpvar_1.zw = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_1;
    o.xlv_COLOR = v.color;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 res;
    half4 tmpvar_1;
    tmpvar_1 = lerp(tex2D(_MainTex0, i.xlv_TEXCOORD0.xy), tex2D(_MainTex1, i.xlv_TEXCOORD0.xy), i.xlv_COLOR.wwww);
    res = tmpvar_1;
    res.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD0.zw).xyz));
    sg_FragColor = res;
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    tmpvar_1.xy = v.texcoord.xy;
    tmpvar_1.zw = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_1;
    o.xlv_COLOR = v.color;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 res;
    half4 tmpvar_1;
    tmpvar_1 = lerp(tex2D(_MainTex0, i.xlv_TEXCOORD0.xy), tex2D(_MainTex1, i.xlv_TEXCOORD0.xy), i.xlv_COLOR.wwww);
    res = tmpvar_1;
    res.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD0.zw).xyz));
    sg_FragColor = res;
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
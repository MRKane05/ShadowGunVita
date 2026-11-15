// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Scroll 1 Layer" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _ScrollX ("Scroll speed X", Float) = 1
 _ScrollY ("Scroll speed Y", Float) = 0
 _TimeOffs ("Time offs", Float) = 0
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
  sampler2D _MainTex;
  float _ScrollX;
  float _ScrollY;
  float _TimeOffs;
  #if defined(LIGHTMAP_ON)
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
    tmpvar_1.x = _ScrollX;
    tmpvar_1.y = _ScrollY;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (v.texcoord.xy + frac((tmpvar_1 * (_Time + _TimeOffs).xy)));
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
    return sg_FragColor;
  }
  #else
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
    tmpvar_1.x = _ScrollX;
    tmpvar_1.y = _ScrollY;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (v.texcoord.xy + frac((tmpvar_1 * (_Time + _TimeOffs).xy)));
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
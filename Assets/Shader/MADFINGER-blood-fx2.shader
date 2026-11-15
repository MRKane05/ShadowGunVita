// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Blood FX - alpha blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _FadeInSpeed ("Fade in speed", Float) = 5
 _DrippingSpeed ("Dripping speed", Float) = 0.1
}
SubShader { 
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  // The game writes GL clip space itself here (no matrix): z in [-w, w], never flipped. Follow this platform's
  // depth range (reversed Z on D3D11; PSP2 is GL-like) and Unity's projection flip when rendering into a texture.
  inline float4 sg_FromGLClip (float4 p)
  {
  #if defined(UNITY_REVERSED_Z)
    p.z = (p.w - p.z) * 0.5;
  #endif
    p.y *= _ProjectionParams.x;
    return p;
  }
  float _DrippingSpeed;
  sampler2D _MainTex;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    half4 tmpvar_2;
    float tmpvar_3;
    tmpvar_3 = (_Time.y + v.vertex.z);
    float4 tmpvar_4;
    tmpvar_4.zw = float2(0.0, 1.0);
    tmpvar_4.xy = v.vertex.xy;
    tmpvar_1 = tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5.xyz = float3(1.0, 1.0, 1.0);
    tmpvar_5.w = (1.0 - max((tmpvar_3 - (0.25 * v.texcoord1.x)), 0.0));
    tmpvar_2 = tmpvar_5;
    tmpvar_1.y = (v.vertex.y - ((tmpvar_3 * _DrippingSpeed) * v.texcoord1.y));
    o.pos = tmpvar_1;
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_COLOR = tmpvar_2;
    o.pos = sg_FromGLClip(o.pos);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_COLOR);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
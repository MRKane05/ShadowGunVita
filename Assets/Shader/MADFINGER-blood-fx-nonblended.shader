// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Blood FX nonblended" {
Properties {
 _Params ("Params", Vector) = (0,1,0,0)
 _Color ("Color", Color) = (0,0,0,0)
}
SubShader { 
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
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
  float4 _Color;
  float4 _Params;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    float2 tmpvar_2;
    tmpvar_2 = normalize(-(v.vertex.xy));
    float tmpvar_3;
    tmpvar_3 = _Params.x;
    float tmpvar_4;
    tmpvar_4 = ((_Time.y * _Params.y) / 1.25);
    float tmpvar_5;
    tmpvar_5 = (frac(abs(tmpvar_4)) * 1.25);
    float tmpvar_6;
    if (tmpvar_4 >= 0.0) {
      tmpvar_6 = tmpvar_5;
    } else {
      tmpvar_6 = -(tmpvar_5);
    }
    float tmpvar_7;
    if (tmpvar_6 < 0.5) {
      float tmpvar_8;
      tmpvar_8 = (tmpvar_6 / 0.5);
      float tmpvar_9;
      tmpvar_9 = (frac(abs(tmpvar_8)) * 0.5);
      float tmpvar_10;
      if (tmpvar_8 >= 0.0) {
        tmpvar_10 = tmpvar_9;
      } else {
        tmpvar_10 = -(tmpvar_9);
      }
      float tmpvar_11;
      tmpvar_11 = (20.0 * tmpvar_10);
      tmpvar_7 = (tmpvar_11 * exp((1.0 - tmpvar_11)));
    } else {
      tmpvar_7 = 0.0;
    }
    float tmpvar_12;
    if (_Params.x > 0.0) {
      tmpvar_12 = lerp(0.1, 0.0, tmpvar_3);
    } else {
      tmpvar_12 = 0.0;
    }
    if (_Params.x > 0.0) {
    }
    float4 tmpvar_13;
    tmpvar_13.zw = float2(0.0, 1.0);
    tmpvar_13.xy = (v.vertex.xy * 2.0);
    tmpvar_1 = tmpvar_13;
    tmpvar_1.xy = (tmpvar_13.xy - ((tmpvar_2 * (_Params.x + (tmpvar_7 * tmpvar_12))) * v.color.w));
    o.pos = tmpvar_1;
    o.pos = sg_FromGLClip(o.pos);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 tmpvar_1;
    tmpvar_1 = _Color;
    sg_FragColor = tmpvar_1;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
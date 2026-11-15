// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Blood FX blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Params ("Params", Vector) = (0,1,0,0)
 _ColorBooster ("Color booster", Color) = (0.5,0,0,0)
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
  Blend DstColor SrcColor
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
  float4 _ColorBooster;
  sampler2D _MainTex;
  float4 _Params;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD0;
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
    float2 tmpvar_3;
    tmpvar_3 = normalize(-(v.vertex.xy));
    float tmpvar_4;
    tmpvar_4 = _Params.x;
    float tmpvar_5;
    tmpvar_5 = ((_Time.y * _Params.y) / 1.25);
    float tmpvar_6;
    tmpvar_6 = (frac(abs(tmpvar_5)) * 1.25);
    float tmpvar_7;
    if (tmpvar_5 >= 0.0) {
      tmpvar_7 = tmpvar_6;
    } else {
      tmpvar_7 = -(tmpvar_6);
    }
    float tmpvar_8;
    if (tmpvar_7 < 0.5) {
      float tmpvar_9;
      tmpvar_9 = (tmpvar_7 / 0.5);
      float tmpvar_10;
      tmpvar_10 = (frac(abs(tmpvar_9)) * 0.5);
      float tmpvar_11;
      if (tmpvar_9 >= 0.0) {
        tmpvar_11 = tmpvar_10;
      } else {
        tmpvar_11 = -(tmpvar_10);
      }
      float tmpvar_12;
      tmpvar_12 = (20.0 * tmpvar_11);
      tmpvar_8 = (tmpvar_12 * exp((1.0 - tmpvar_12)));
    } else {
      tmpvar_8 = 0.0;
    }
    float tmpvar_13;
    if (_Params.x > 0.0) {
      tmpvar_13 = lerp(0.1, 0.0, tmpvar_4);
    } else {
      tmpvar_13 = 0.0;
    }
    float tmpvar_14;
    if (_Params.x > 0.0) {
      tmpvar_14 = lerp(1.0, 0.0, tmpvar_4);
    } else {
      tmpvar_14 = 0.0;
    }
    float4 tmpvar_15;
    tmpvar_15.zw = float2(0.0, 1.0);
    tmpvar_15.xy = (v.vertex.xy * 2.0);
    tmpvar_1 = tmpvar_15;
    float4 tmpvar_16;
    tmpvar_16.xyz = (((_ColorBooster.xyz * tmpvar_14) * v.color.w) * tmpvar_8);
    tmpvar_16.w = v.color.w;
    tmpvar_2 = tmpvar_16;
    tmpvar_1.xy = (tmpvar_15.xy - ((tmpvar_3 * (_Params.x + (tmpvar_8 * tmpvar_13))) * v.color.w));
    o.pos = tmpvar_1;
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_COLOR = tmpvar_2;
    o.pos = sg_FromGLClip(o.pos);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
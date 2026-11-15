// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/PostFX/ExplosionFX" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _UVOffsAndAspectScale ("UVOffsAndAspectScale", Vector) = (0,0,0,0)
 _Wave0ParamSet0 ("Wave0ParamSet0", Vector) = (0,0,0,0)
 _Wave0ParamSet1 ("Wave0ParamSet1", Vector) = (0,0,0,0)
 _Wave1ParamSet0 ("Wave1ParamSet0", Vector) = (0,0,0,0)
 _Wave1ParamSet1 ("Wave1ParamSet1", Vector) = (0,0,0,0)
 _Wave2ParamSet0 ("Wave2ParamSet0", Vector) = (0,0,0,0)
 _Wave2ParamSet1 ("Wave2ParamSet1", Vector) = (0,0,0,0)
 _Wave3ParamSet0 ("Wave3ParamSet0", Vector) = (0,0,0,0)
 _Wave3ParamSet1 ("Wave3ParamSet1", Vector) = (0,0,0,0)
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
  sampler2D _MainTex;
  float4 _UVOffsAndAspectScale;
  float4 _Wave0ParamSet0;
  float4 _Wave0ParamSet1;
  float4 _Wave1ParamSet0;
  float4 _Wave1ParamSet1;
  float4 _Wave2ParamSet0;
  float4 _Wave2ParamSet1;
  float4 _Wave3ParamSet0;
  float4 _Wave3ParamSet1;
  struct sg_appdata {
    float4 vertex : POSITION;
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
    half4 tmpvar_1;
    float2 tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3 = ((v.vertex.xy - _Wave0ParamSet0.xy) * _UVOffsAndAspectScale.zw);
    float tmpvar_4;
    tmpvar_4 = length(tmpvar_3);
    float time_i0;
    time_i0 = (max(((_Time.y - _Wave0ParamSet1.z) - (tmpvar_4 / _Wave0ParamSet1.y)), 0.0) * _Wave0ParamSet0.w);
    float tmpvar_5;
    tmpvar_5 = ((sin(time_i0) * (1.0 / (1.0 + (time_i0 * time_i0)))) * _Wave0ParamSet0.z);
    float tmpvar_6;
    tmpvar_6 = clamp((tmpvar_4 * _Wave0ParamSet1.x), 0.0, 1.0);
    float tmpvar_7;
    tmpvar_7 = (max((_Time.y - _Wave0ParamSet1.z), 0.0) * 16.25);
    float tmpvar_8;
    tmpvar_8 = ((tmpvar_7 * exp((1.0 - tmpvar_7))) + 0.0001);
    float tmpvar_9;
    tmpvar_9 = (1.0 / (_Wave0ParamSet1.x));
    float tmpvar_10;
    if (tmpvar_9 > 0.65) {
      tmpvar_10 = (tmpvar_9 * 2.0);
    } else {
      tmpvar_10 = tmpvar_9;
    }
    float tmpvar_11;
    tmpvar_11 = (1.0 - clamp((tmpvar_4 / (tmpvar_10 * tmpvar_8)), 0.0, 1.0));
    float4 tmpvar_12;
    tmpvar_12 = (((((tmpvar_11 * tmpvar_11) * _Wave0ParamSet1.x) * lerp(float4(0.5, 0.3, 0.0, 1.0), float4(1.0, 1.0, 1.0, 1.0), ((float4)tmpvar_8))) * 1.5) * tmpvar_8);
    tmpvar_2 = ((tmpvar_5 * (tmpvar_3 / tmpvar_4)) * (1.0 - (tmpvar_6 * tmpvar_6)));
    float2 tmpvar_13;
    float2 tmpvar_14;
    tmpvar_14 = ((v.vertex.xy - _Wave1ParamSet0.xy) * _UVOffsAndAspectScale.zw);
    float tmpvar_15;
    tmpvar_15 = length(tmpvar_14);
    float time_i0_i0;
    time_i0_i0 = (max(((_Time.y - _Wave1ParamSet1.z) - (tmpvar_15 / _Wave1ParamSet1.y)), 0.0) * _Wave1ParamSet0.w);
    float tmpvar_16;
    tmpvar_16 = ((sin(time_i0_i0) * (1.0 / (1.0 + (time_i0_i0 * time_i0_i0)))) * _Wave1ParamSet0.z);
    float tmpvar_17;
    tmpvar_17 = clamp((tmpvar_15 * _Wave1ParamSet1.x), 0.0, 1.0);
    float tmpvar_18;
    tmpvar_18 = (max((_Time.y - _Wave1ParamSet1.z), 0.0) * 16.25);
    float tmpvar_19;
    tmpvar_19 = ((tmpvar_18 * exp((1.0 - tmpvar_18))) + 0.0001);
    float tmpvar_20;
    tmpvar_20 = (1.0 / (_Wave1ParamSet1.x));
    float tmpvar_21;
    if (tmpvar_20 > 0.65) {
      tmpvar_21 = (tmpvar_20 * 2.0);
    } else {
      tmpvar_21 = tmpvar_20;
    }
    float tmpvar_22;
    tmpvar_22 = (1.0 - clamp((tmpvar_15 / (tmpvar_21 * tmpvar_19)), 0.0, 1.0));
    float4 tmpvar_23;
    tmpvar_23 = (((((tmpvar_22 * tmpvar_22) * _Wave1ParamSet1.x) * lerp(float4(0.5, 0.3, 0.0, 1.0), float4(1.0, 1.0, 1.0, 1.0), ((float4)tmpvar_19))) * 1.5) * tmpvar_19);
    tmpvar_13 = ((tmpvar_16 * (tmpvar_14 / tmpvar_15)) * (1.0 - (tmpvar_17 * tmpvar_17)));
    float2 tmpvar_24;
    float2 tmpvar_25;
    tmpvar_25 = ((v.vertex.xy - _Wave2ParamSet0.xy) * _UVOffsAndAspectScale.zw);
    float tmpvar_26;
    tmpvar_26 = length(tmpvar_25);
    float time_i0_i0_i1;
    time_i0_i0_i1 = (max(((_Time.y - _Wave2ParamSet1.z) - (tmpvar_26 / _Wave2ParamSet1.y)), 0.0) * _Wave2ParamSet0.w);
    float tmpvar_27;
    tmpvar_27 = ((sin(time_i0_i0_i1) * (1.0 / (1.0 + (time_i0_i0_i1 * time_i0_i0_i1)))) * _Wave2ParamSet0.z);
    float tmpvar_28;
    tmpvar_28 = clamp((tmpvar_26 * _Wave2ParamSet1.x), 0.0, 1.0);
    float tmpvar_29;
    tmpvar_29 = (max((_Time.y - _Wave2ParamSet1.z), 0.0) * 16.25);
    float tmpvar_30;
    tmpvar_30 = ((tmpvar_29 * exp((1.0 - tmpvar_29))) + 0.0001);
    float tmpvar_31;
    tmpvar_31 = (1.0 / (_Wave2ParamSet1.x));
    float tmpvar_32;
    if (tmpvar_31 > 0.65) {
      tmpvar_32 = (tmpvar_31 * 2.0);
    } else {
      tmpvar_32 = tmpvar_31;
    }
    float tmpvar_33;
    tmpvar_33 = (1.0 - clamp((tmpvar_26 / (tmpvar_32 * tmpvar_30)), 0.0, 1.0));
    float4 tmpvar_34;
    tmpvar_34 = (((((tmpvar_33 * tmpvar_33) * _Wave2ParamSet1.x) * lerp(float4(0.5, 0.3, 0.0, 1.0), float4(1.0, 1.0, 1.0, 1.0), ((float4)tmpvar_30))) * 1.5) * tmpvar_30);
    tmpvar_24 = ((tmpvar_27 * (tmpvar_25 / tmpvar_26)) * (1.0 - (tmpvar_28 * tmpvar_28)));
    float2 tmpvar_35;
    tmpvar_35 = ((v.vertex.xy - _Wave3ParamSet0.xy) * _UVOffsAndAspectScale.zw);
    float tmpvar_36;
    tmpvar_36 = length(tmpvar_35);
    float time_i0_i0_i1_i2;
    time_i0_i0_i1_i2 = (max(((_Time.y - _Wave3ParamSet1.z) - (tmpvar_36 / _Wave3ParamSet1.y)), 0.0) * _Wave3ParamSet0.w);
    float tmpvar_37;
    tmpvar_37 = ((sin(time_i0_i0_i1_i2) * (1.0 / (1.0 + (time_i0_i0_i1_i2 * time_i0_i0_i1_i2)))) * _Wave3ParamSet0.z);
    float tmpvar_38;
    tmpvar_38 = clamp((tmpvar_36 * _Wave3ParamSet1.x), 0.0, 1.0);
    float tmpvar_39;
    tmpvar_39 = (max((_Time.y - _Wave3ParamSet1.z), 0.0) * 16.25);
    float tmpvar_40;
    tmpvar_40 = ((tmpvar_39 * exp((1.0 - tmpvar_39))) + 0.0001);
    float tmpvar_41;
    tmpvar_41 = (1.0 / (_Wave3ParamSet1.x));
    float tmpvar_42;
    if (tmpvar_41 > 0.65) {
      tmpvar_42 = (tmpvar_41 * 2.0);
    } else {
      tmpvar_42 = tmpvar_41;
    }
    float tmpvar_43;
    tmpvar_43 = (1.0 - clamp((tmpvar_36 / (tmpvar_42 * tmpvar_40)), 0.0, 1.0));
    float4 tmpvar_44;
    tmpvar_44.zw = float2(0.0, 1.0);
    tmpvar_44.xy = ((v.vertex.xy * 2.0) - float2(1.0, 1.0));
    float4 tmpvar_45;
    tmpvar_45 = (((tmpvar_12 + tmpvar_23) + tmpvar_34) + (((((tmpvar_43 * tmpvar_43) * _Wave3ParamSet1.x) * lerp(float4(0.5, 0.3, 0.0, 1.0), float4(1.0, 1.0, 1.0, 1.0), ((float4)tmpvar_40))) * 1.5) * tmpvar_40));
    tmpvar_1 = tmpvar_45;
    o.pos = tmpvar_44;
    o.xlv_TEXCOORD0 = ((v.vertex.xy + _UVOffsAndAspectScale.xy) + (((tmpvar_2 + tmpvar_13) + tmpvar_24) + ((tmpvar_37 * (tmpvar_35 / tmpvar_36)) * (1.0 - (tmpvar_38 * tmpvar_38)))));
    o.xlv_COLOR = tmpvar_1;
    o.pos = sg_FromGLClip(o.pos);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) + i.xlv_COLOR);
    return sg_FragColor;
  }
  ENDCG
 }
}
Fallback Off
}
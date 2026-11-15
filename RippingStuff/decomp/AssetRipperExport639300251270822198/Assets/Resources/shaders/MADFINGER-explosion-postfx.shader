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
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Wave3ParamSet1;
uniform highp vec4 _Wave3ParamSet0;
uniform highp vec4 _Wave2ParamSet1;
uniform highp vec4 _Wave2ParamSet0;
uniform highp vec4 _Wave1ParamSet1;
uniform highp vec4 _Wave1ParamSet0;
uniform highp vec4 _Wave0ParamSet1;
uniform highp vec4 _Wave0ParamSet0;
uniform highp vec4 _UVOffsAndAspectScale;
uniform highp vec4 _Time;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = ((_glesVertex.xy - _Wave0ParamSet0.xy) * _UVOffsAndAspectScale.zw);
  highp float tmpvar_4;
  tmpvar_4 = length (tmpvar_3);
  highp float time_i0;
  time_i0 = (max (((_Time.y - _Wave0ParamSet1.z) - (tmpvar_4 / _Wave0ParamSet1.y)), 0.0) * _Wave0ParamSet0.w);
  highp float tmpvar_5;
  tmpvar_5 = ((sin (time_i0) * (1.0/((1.0 + (time_i0 * time_i0))))) * _Wave0ParamSet0.z);
  highp float tmpvar_6;
  tmpvar_6 = clamp ((tmpvar_4 * _Wave0ParamSet1.x), 0.0, 1.0);
  highp float tmpvar_7;
  tmpvar_7 = (max ((_Time.y - _Wave0ParamSet1.z), 0.0) * 16.25);
  highp float tmpvar_8;
  tmpvar_8 = ((tmpvar_7 * exp ((1.0 - tmpvar_7))) + 0.0001);
  highp float tmpvar_9;
  tmpvar_9 = (1.0/(_Wave0ParamSet1.x));
  highp float tmpvar_10;
  if ((tmpvar_9 > 0.65)) {
    tmpvar_10 = (tmpvar_9 * 2.0);
  } else {
    tmpvar_10 = tmpvar_9;
  };
  float tmpvar_11;
  tmpvar_11 = (1.0 - clamp ((tmpvar_4 / (tmpvar_10 * tmpvar_8)), 0.0, 1.0));
  highp vec4 tmpvar_12;
  tmpvar_12 = (((((tmpvar_11 * tmpvar_11) * _Wave0ParamSet1.x) * mix (vec4(0.5, 0.3, 0.0, 1.0), vec4(1.0, 1.0, 1.0, 1.0), vec4(tmpvar_8))) * 1.5) * tmpvar_8);
  tmpvar_2 = ((tmpvar_5 * (tmpvar_3 / tmpvar_4)) * (1.0 - (tmpvar_6 * tmpvar_6)));
  highp vec2 tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = ((_glesVertex.xy - _Wave1ParamSet0.xy) * _UVOffsAndAspectScale.zw);
  highp float tmpvar_15;
  tmpvar_15 = length (tmpvar_14);
  highp float time_i0_i0;
  time_i0_i0 = (max (((_Time.y - _Wave1ParamSet1.z) - (tmpvar_15 / _Wave1ParamSet1.y)), 0.0) * _Wave1ParamSet0.w);
  highp float tmpvar_16;
  tmpvar_16 = ((sin (time_i0_i0) * (1.0/((1.0 + (time_i0_i0 * time_i0_i0))))) * _Wave1ParamSet0.z);
  highp float tmpvar_17;
  tmpvar_17 = clamp ((tmpvar_15 * _Wave1ParamSet1.x), 0.0, 1.0);
  highp float tmpvar_18;
  tmpvar_18 = (max ((_Time.y - _Wave1ParamSet1.z), 0.0) * 16.25);
  highp float tmpvar_19;
  tmpvar_19 = ((tmpvar_18 * exp ((1.0 - tmpvar_18))) + 0.0001);
  highp float tmpvar_20;
  tmpvar_20 = (1.0/(_Wave1ParamSet1.x));
  highp float tmpvar_21;
  if ((tmpvar_20 > 0.65)) {
    tmpvar_21 = (tmpvar_20 * 2.0);
  } else {
    tmpvar_21 = tmpvar_20;
  };
  float tmpvar_22;
  tmpvar_22 = (1.0 - clamp ((tmpvar_15 / (tmpvar_21 * tmpvar_19)), 0.0, 1.0));
  highp vec4 tmpvar_23;
  tmpvar_23 = (((((tmpvar_22 * tmpvar_22) * _Wave1ParamSet1.x) * mix (vec4(0.5, 0.3, 0.0, 1.0), vec4(1.0, 1.0, 1.0, 1.0), vec4(tmpvar_19))) * 1.5) * tmpvar_19);
  tmpvar_13 = ((tmpvar_16 * (tmpvar_14 / tmpvar_15)) * (1.0 - (tmpvar_17 * tmpvar_17)));
  highp vec2 tmpvar_24;
  highp vec2 tmpvar_25;
  tmpvar_25 = ((_glesVertex.xy - _Wave2ParamSet0.xy) * _UVOffsAndAspectScale.zw);
  highp float tmpvar_26;
  tmpvar_26 = length (tmpvar_25);
  highp float time_i0_i0_i1;
  time_i0_i0_i1 = (max (((_Time.y - _Wave2ParamSet1.z) - (tmpvar_26 / _Wave2ParamSet1.y)), 0.0) * _Wave2ParamSet0.w);
  highp float tmpvar_27;
  tmpvar_27 = ((sin (time_i0_i0_i1) * (1.0/((1.0 + (time_i0_i0_i1 * time_i0_i0_i1))))) * _Wave2ParamSet0.z);
  highp float tmpvar_28;
  tmpvar_28 = clamp ((tmpvar_26 * _Wave2ParamSet1.x), 0.0, 1.0);
  highp float tmpvar_29;
  tmpvar_29 = (max ((_Time.y - _Wave2ParamSet1.z), 0.0) * 16.25);
  highp float tmpvar_30;
  tmpvar_30 = ((tmpvar_29 * exp ((1.0 - tmpvar_29))) + 0.0001);
  highp float tmpvar_31;
  tmpvar_31 = (1.0/(_Wave2ParamSet1.x));
  highp float tmpvar_32;
  if ((tmpvar_31 > 0.65)) {
    tmpvar_32 = (tmpvar_31 * 2.0);
  } else {
    tmpvar_32 = tmpvar_31;
  };
  float tmpvar_33;
  tmpvar_33 = (1.0 - clamp ((tmpvar_26 / (tmpvar_32 * tmpvar_30)), 0.0, 1.0));
  highp vec4 tmpvar_34;
  tmpvar_34 = (((((tmpvar_33 * tmpvar_33) * _Wave2ParamSet1.x) * mix (vec4(0.5, 0.3, 0.0, 1.0), vec4(1.0, 1.0, 1.0, 1.0), vec4(tmpvar_30))) * 1.5) * tmpvar_30);
  tmpvar_24 = ((tmpvar_27 * (tmpvar_25 / tmpvar_26)) * (1.0 - (tmpvar_28 * tmpvar_28)));
  highp vec2 tmpvar_35;
  tmpvar_35 = ((_glesVertex.xy - _Wave3ParamSet0.xy) * _UVOffsAndAspectScale.zw);
  highp float tmpvar_36;
  tmpvar_36 = length (tmpvar_35);
  highp float time_i0_i0_i1_i2;
  time_i0_i0_i1_i2 = (max (((_Time.y - _Wave3ParamSet1.z) - (tmpvar_36 / _Wave3ParamSet1.y)), 0.0) * _Wave3ParamSet0.w);
  highp float tmpvar_37;
  tmpvar_37 = ((sin (time_i0_i0_i1_i2) * (1.0/((1.0 + (time_i0_i0_i1_i2 * time_i0_i0_i1_i2))))) * _Wave3ParamSet0.z);
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_36 * _Wave3ParamSet1.x), 0.0, 1.0);
  highp float tmpvar_39;
  tmpvar_39 = (max ((_Time.y - _Wave3ParamSet1.z), 0.0) * 16.25);
  highp float tmpvar_40;
  tmpvar_40 = ((tmpvar_39 * exp ((1.0 - tmpvar_39))) + 0.0001);
  highp float tmpvar_41;
  tmpvar_41 = (1.0/(_Wave3ParamSet1.x));
  highp float tmpvar_42;
  if ((tmpvar_41 > 0.65)) {
    tmpvar_42 = (tmpvar_41 * 2.0);
  } else {
    tmpvar_42 = tmpvar_41;
  };
  float tmpvar_43;
  tmpvar_43 = (1.0 - clamp ((tmpvar_36 / (tmpvar_42 * tmpvar_40)), 0.0, 1.0));
  highp vec4 tmpvar_44;
  tmpvar_44.zw = vec2(0.0, 1.0);
  tmpvar_44.xy = ((_glesVertex.xy * 2.0) - vec2(1.0, 1.0));
  highp vec4 tmpvar_45;
  tmpvar_45 = (((tmpvar_12 + tmpvar_23) + tmpvar_34) + (((((tmpvar_43 * tmpvar_43) * _Wave3ParamSet1.x) * mix (vec4(0.5, 0.3, 0.0, 1.0), vec4(1.0, 1.0, 1.0, 1.0), vec4(tmpvar_40))) * 1.5) * tmpvar_40));
  tmpvar_1 = tmpvar_45;
  gl_Position = tmpvar_44;
  xlv_TEXCOORD0 = ((_glesVertex.xy + _UVOffsAndAspectScale.xy) + (((tmpvar_2 + tmpvar_13) + tmpvar_24) + ((tmpvar_37 * (tmpvar_35 / tmpvar_36)) * (1.0 - (tmpvar_38 * tmpvar_38)))));
  xlv_COLOR = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) + xlv_COLOR);
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
}
 }
}
Fallback Off
}
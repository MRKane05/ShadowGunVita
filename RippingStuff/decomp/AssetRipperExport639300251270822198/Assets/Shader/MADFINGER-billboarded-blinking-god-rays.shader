Shader "MADFINGER/Transparent/Blinking GodRays Billboarded" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _TimeOnDuration ("ON duration", Float) = 0.5
 _TimeOffDuration ("OFF duration", Float) = 0.5
 _BlinkingTimeOffsScale ("Blinking time offset scale (seconds)", Float) = 5
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _NoiseAmount ("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
 _VerticalBillboarding ("Vertical billboarding amount", Range(0,1)) = 1
 _ViewerOffset ("Viewer offset", Float) = 0
 _Color ("Color", Color) = (1,1,1,1)
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
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp float _ViewerOffset;
uniform highp float _VerticalBillboarding;
uniform highp float _TimeOnDuration;
uniform highp float _TimeOffDuration;
uniform highp vec4 _Time;
uniform highp float _SizeGrowStartDist;
uniform highp float _SizeGrowEndDist;
uniform highp float _NoiseAmount;
uniform highp float _Multiplier;
uniform highp float _MaxGrowSize;
uniform highp float _FadeOutDistNear;
uniform highp float _FadeOutDistFar;
uniform highp vec4 _Color;
uniform highp float _BlinkingTimeOffsScale;
uniform highp float _Bias;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  lowp vec4 tmpvar_2;
  highp vec3 localDir;
  lowp vec3 tmpvar_3;
  tmpvar_3.z = 0.0;
  tmpvar_3.xy = (vec2(0.5, 0.5) - _glesColor.xy);
  highp vec3 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * _glesMultiTexCoord1.xyy);
  highp vec3 tmpvar_5;
  tmpvar_5 = (_glesVertex.xyz + tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_7;
  tmpvar_7 = ((_World2Object * tmpvar_6).xyz - tmpvar_5);
  localDir = tmpvar_7;
  localDir.y = mix (0.0, tmpvar_7.y, _VerticalBillboarding);
  highp float tmpvar_8;
  tmpvar_8 = length (localDir);
  highp vec3 dir;
  dir = (localDir / tmpvar_8);
  highp float tmpvar_9;
  tmpvar_9 = abs (dir.y);
  vec3 tmpvar_10;
  if ((tmpvar_9 > 0.999)) {
    tmpvar_10 = vec3(0.0, 0.0, 1.0);
  } else {
    tmpvar_10 = vec3(0.0, 1.0, 0.0);
  };
  highp vec3 tmpvar_11;
  tmpvar_11 = normalize (cross (tmpvar_10, dir));
  highp vec3 tmpvar_12;
  tmpvar_12 = cross (dir, tmpvar_11);
  highp float tmpvar_13;
  tmpvar_13 = min ((max ((tmpvar_8 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
  highp vec3 tmpvar_14;
  tmpvar_14 = (((tmpvar_5 - ((tmpvar_11 * tmpvar_4.x) + (tmpvar_12 * tmpvar_4.y))) + (((tmpvar_11 * tmpvar_1.x) + (tmpvar_12 * tmpvar_1.y)) * (((tmpvar_13 * tmpvar_13) * _MaxGrowSize) * _glesColor.w))) + (_ViewerOffset * localDir));
  highp float tmpvar_15;
  tmpvar_15 = (_Time.y + (_BlinkingTimeOffsScale * _glesColor.z));
  highp float y;
  y = (_TimeOnDuration + _TimeOffDuration);
  float tmpvar_16;
  tmpvar_16 = (tmpvar_15 / y);
  float tmpvar_17;
  tmpvar_17 = (fract (abs (tmpvar_16)) * y);
  float tmpvar_18;
  if ((tmpvar_16 >= 0.0)) {
    tmpvar_18 = tmpvar_17;
  } else {
    tmpvar_18 = -(tmpvar_17);
  };
  highp float tmpvar_19;
  tmpvar_19 = (smoothstep (0.0, (_TimeOnDuration * 0.25), tmpvar_18) * (1.0 - smoothstep ((_TimeOnDuration * 0.75), _TimeOnDuration, tmpvar_18)));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_15 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_21;
  tmpvar_21 = ((_NoiseAmount * (sin (tmpvar_20) * ((0.5 * cos (((tmpvar_20 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
  highp float tmpvar_22;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_22 = tmpvar_19;
  } else {
    tmpvar_22 = tmpvar_21;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.w = 1.0;
  tmpvar_23.xyz = tmpvar_14;
  highp float tmpvar_24;
  tmpvar_24 = clamp ((tmpvar_8 / _FadeOutDistNear), 0.0, 1.0);
  float tmpvar_25;
  tmpvar_25 = (1.0 - clamp ((max ((tmpvar_8 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_24 * tmpvar_24);
  highp vec4 tmpvar_27;
  tmpvar_27 = (((((tmpvar_26 * tmpvar_26) * (tmpvar_25 * tmpvar_25)) * _Color) * _Multiplier) * (tmpvar_22 + _Bias));
  tmpvar_2 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_23);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_TEXCOORD1);
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
}
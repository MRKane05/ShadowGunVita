Shader "MADFINGER/Transparent/Blinking GodRays" {
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
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;


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
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  lowp vec4 tmpvar_2;
  highp vec4 mdlPos;
  highp float tmpvar_3;
  tmpvar_3 = (_Time.y + (_BlinkingTimeOffsScale * _glesColor.z));
  highp float tmpvar_4;
  tmpvar_4 = length ((gl_ModelViewMatrix * _glesVertex).xyz);
  highp float tmpvar_5;
  tmpvar_5 = clamp ((tmpvar_4 / _FadeOutDistNear), 0.0, 1.0);
  float tmpvar_6;
  tmpvar_6 = (1.0 - clamp ((max ((tmpvar_4 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float y;
  y = (_TimeOnDuration + _TimeOffDuration);
  float tmpvar_7;
  tmpvar_7 = (tmpvar_3 / y);
  float tmpvar_8;
  tmpvar_8 = (fract (abs (tmpvar_7)) * y);
  float tmpvar_9;
  if ((tmpvar_7 >= 0.0)) {
    tmpvar_9 = tmpvar_8;
  } else {
    tmpvar_9 = -(tmpvar_8);
  };
  highp float tmpvar_10;
  tmpvar_10 = (smoothstep (0.0, (_TimeOnDuration * 0.25), tmpvar_9) * (1.0 - smoothstep ((_TimeOnDuration * 0.75), _TimeOnDuration, tmpvar_9)));
  highp float tmpvar_11;
  tmpvar_11 = (tmpvar_3 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_12;
  tmpvar_12 = ((_NoiseAmount * (sin (tmpvar_11) * ((0.5 * cos (((tmpvar_11 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
  highp float tmpvar_13;
  tmpvar_13 = min ((max ((tmpvar_4 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
  highp float tmpvar_14;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_14 = tmpvar_10;
  } else {
    tmpvar_14 = tmpvar_12;
  };
  highp float tmpvar_15;
  tmpvar_15 = (tmpvar_5 * tmpvar_5);
  mdlPos = _glesVertex;
  mdlPos.xyz = (_glesVertex.xyz + ((((tmpvar_13 * tmpvar_13) * _MaxGrowSize) * _glesColor.w) * tmpvar_1));
  highp vec4 tmpvar_16;
  tmpvar_16 = (((((tmpvar_15 * tmpvar_15) * (tmpvar_6 * tmpvar_6)) * _Color) * _Multiplier) * (tmpvar_14 + _Bias));
  tmpvar_2 = tmpvar_16;
  gl_Position = (gl_ModelViewProjectionMatrix * mdlPos);
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
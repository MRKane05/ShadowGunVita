Shader "MADFINGER/Transparent/GodRays" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Multiplier", Float) = 1
 _ContractionAmount ("Near contraction amount", Float) = 5
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


uniform highp float _Multiplier;
uniform highp float _FadeOutDistNear;
uniform highp float _FadeOutDistFar;
uniform highp float _ContractionAmount;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 vpos;
  highp float tmpvar_2;
  tmpvar_2 = length ((gl_ModelViewMatrix * _glesVertex).xyz);
  highp float tmpvar_3;
  tmpvar_3 = clamp ((tmpvar_2 / _FadeOutDistNear), 0.0, 1.0);
  float tmpvar_4;
  tmpvar_4 = (1.0 - clamp ((max ((tmpvar_2 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float tmpvar_5;
  tmpvar_5 = (tmpvar_3 * tmpvar_3);
  highp float tmpvar_6;
  tmpvar_6 = ((tmpvar_5 * tmpvar_5) * (tmpvar_4 * tmpvar_4));
  vpos = _glesVertex;
  vpos.xyz = (_glesVertex.xyz - (((normalize (_glesNormal) * clamp ((1.0 - tmpvar_6), 0.0, 1.0)) * _glesColor.w) * _ContractionAmount));
  highp vec4 tmpvar_7;
  tmpvar_7 = ((tmpvar_6 * _glesColor) * _Multiplier);
  tmpvar_1 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * vpos);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
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
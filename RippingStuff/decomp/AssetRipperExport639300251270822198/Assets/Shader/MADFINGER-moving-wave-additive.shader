Shader "MADFINGER/Transparent/Moving waves FX" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _WaveSpeed ("Waves speed", Vector) = (0.25,0.25,0.25,0.25)
 _WaveSize ("Waves size", Vector) = (0.25,0.25,0.25,0.25)
 _WaveAmplitude ("Waves amp", Vector) = (1,0,0,0)
 _WaveBias ("Waves bias", Vector) = (0,0,0,0)
 _WaveTimeOffs ("Wave time offs", Vector) = (0,0,0,0)
 _Color0 ("Color0", Color) = (1,1,1,1)
 _Color1 ("Color1", Color) = (1,0,0,1)
 _Color2 ("Color2", Color) = (0,1,0,1)
 _Color3 ("Color3", Color) = (0,0,1,1)
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


uniform highp vec4 _WaveTimeOffs;
uniform highp vec4 _WaveSpeed;
uniform highp vec4 _WaveSize;
uniform highp vec4 _WaveBias;
uniform highp vec4 _WaveAmplitude;
uniform highp vec4 _Time;
uniform highp float _SizeGrowStartDist;
uniform highp float _SizeGrowEndDist;
uniform highp float _Multiplier;
uniform highp float _MaxGrowSize;
uniform highp float _FadeOutDistNear;
uniform highp float _FadeOutDistFar;
uniform highp vec4 _Color3;
uniform highp vec4 _Color2;
uniform highp vec4 _Color1;
uniform highp vec4 _Color0;
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
  tmpvar_3 = length ((gl_ModelViewMatrix * _glesVertex).xyz);
  highp float tmpvar_4;
  tmpvar_4 = clamp ((tmpvar_3 / _FadeOutDistNear), 0.0, 1.0);
  float tmpvar_5;
  tmpvar_5 = (1.0 - clamp ((max ((tmpvar_3 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float tmpvar_6;
  tmpvar_6 = min ((max ((tmpvar_3 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
  highp vec4 y;
  y = (vec4(1.0, 1.0, 1.0, 1.0) + (2.0 * _WaveSize));
  vec4 tmpvar_7;
  tmpvar_7 = (((_Time.y + _WaveTimeOffs) * _WaveSpeed) / y);
  vec4 tmpvar_8;
  tmpvar_8 = (fract (abs (tmpvar_7)) * y);
  float tmpvar_9;
  if ((tmpvar_7.x >= 0.0)) {
    tmpvar_9 = tmpvar_8.x;
  } else {
    tmpvar_9 = -(tmpvar_8.x);
  };
  float tmpvar_10;
  if ((tmpvar_7.y >= 0.0)) {
    tmpvar_10 = tmpvar_8.y;
  } else {
    tmpvar_10 = -(tmpvar_8.y);
  };
  float tmpvar_11;
  if ((tmpvar_7.z >= 0.0)) {
    tmpvar_11 = tmpvar_8.z;
  } else {
    tmpvar_11 = -(tmpvar_8.z);
  };
  float tmpvar_12;
  if ((tmpvar_7.w >= 0.0)) {
    tmpvar_12 = tmpvar_8.w;
  } else {
    tmpvar_12 = -(tmpvar_8.w);
  };
  vec4 tmpvar_13;
  tmpvar_13.x = tmpvar_9;
  tmpvar_13.y = tmpvar_10;
  tmpvar_13.z = tmpvar_11;
  tmpvar_13.w = tmpvar_12;
  highp vec4 tmpvar_14;
  tmpvar_14 = abs ((_glesColor.xxxx - (-(_WaveSize) + tmpvar_13)));
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 / _WaveSize);
  bvec4 tmpvar_16;
  tmpvar_16 = greaterThan (tmpvar_14, _WaveSize);
  highp vec4 c_i0;
  c_i0 = (vec4(1.0, 1.0, 1.0, 1.0) - ((tmpvar_15 * tmpvar_15) * (vec4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_15))));
  highp float tmpvar_17;
  if (tmpvar_16.x) {
    tmpvar_17 = 0.0;
  } else {
    tmpvar_17 = c_i0.x;
  };
  highp float tmpvar_18;
  if (tmpvar_16.y) {
    tmpvar_18 = 0.0;
  } else {
    tmpvar_18 = c_i0.y;
  };
  highp float tmpvar_19;
  if (tmpvar_16.z) {
    tmpvar_19 = 0.0;
  } else {
    tmpvar_19 = c_i0.z;
  };
  highp float tmpvar_20;
  if (tmpvar_16.w) {
    tmpvar_20 = 0.0;
  } else {
    tmpvar_20 = c_i0.w;
  };
  highp vec4 tmpvar_21;
  tmpvar_21.x = tmpvar_17;
  tmpvar_21.y = tmpvar_18;
  tmpvar_21.z = tmpvar_19;
  tmpvar_21.w = tmpvar_20;
  highp vec4 tmpvar_22;
  tmpvar_22 = ((tmpvar_21 * _WaveAmplitude) + _WaveBias);
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_4 * tmpvar_4);
  mdlPos = _glesVertex;
  mdlPos.xyz = (_glesVertex.xyz + ((((tmpvar_6 * tmpvar_6) * _MaxGrowSize) * _glesColor.w) * tmpvar_1));
  highp vec4 tmpvar_24;
  tmpvar_24 = ((((tmpvar_23 * tmpvar_23) * (tmpvar_5 * tmpvar_5)) * ((((_Color0 * tmpvar_22.x) + (_Color1 * tmpvar_22.y)) + (_Color2 * tmpvar_22.z)) + (_Color3 * tmpvar_22.w))) * _Multiplier);
  tmpvar_2 = tmpvar_24;
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
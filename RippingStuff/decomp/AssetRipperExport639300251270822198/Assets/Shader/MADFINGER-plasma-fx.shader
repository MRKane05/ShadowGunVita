Shader "MADFINGER/FX/Plasma FX" {
Properties {
 _MainTex ("Noise tex", 2D) = "white" {}
 _ColorStart ("Color start", Color) = (1,0.8,0,1)
 _ColorEnd ("Color end", Color) = (0.5,0.3,0,1)
 _Speed ("Speed", Float) = 2
 _BorderWidth ("Border width", Float) = 4
 _TimeOffs ("TimeOffs", Float) = 9999
 _ExpansionSpeed ("ExpansionSpeed", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR;
varying highp vec3 xlv_TEXCOORD0;

uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp float _Speed;
uniform highp vec4 _MainTex_ST;
uniform highp float _ExpansionSpeed;
uniform highp vec4 _ColorStart;
uniform highp vec4 _ColorEnd;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 tmpvar_1;
  lowp vec4 tmpvar_2;
  highp float tmpvar_3;
  tmpvar_3 = ((_Time.y + _TimeOffs) * _Speed);
  highp float tmpvar_4;
  tmpvar_4 = clamp (tmpvar_3, 0.0, 1.0);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (_glesVertex.xyz + ((normalize (_glesNormal) * tmpvar_4) * _ExpansionSpeed));
  tmpvar_1.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.z = tmpvar_3;
  highp vec4 tmpvar_6;
  tmpvar_6.xyz = mix (_ColorStart.xyz, _ColorEnd.xyz, vec3(tmpvar_4));
  tmpvar_6.w = (_glesColor.w * pow ((1.0 - tmpvar_4), 3.0));
  tmpvar_2 = tmpvar_6;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_5);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_COLOR = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec3 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp float _BorderWidth;
void main ()
{
  lowp float threshold;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0.xy);
  highp float tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.z;
  threshold = tmpvar_2;
  float tmpvar_3;
  tmpvar_3 = (1.0 - clamp (((tmpvar_1 - threshold).x * _BorderWidth), 0.0, 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = (tmpvar_3 * tmpvar_3);
  gl_FragData[0] = ((xlv_COLOR + vec4((tmpvar_4 * tmpvar_4))) * (xlv_COLOR.w * step (threshold, tmpvar_1.x)));
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
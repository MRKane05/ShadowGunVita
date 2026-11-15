Shader "MADFINGER/FX/Mine-FX" {
Properties {
 _MainTex ("MainTex", 2D) = "white" {}
 _Params ("X = period, Y = max scale, Z = time offs, W = delay", Vector) = (1,5,0,0)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp vec4 _Params;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp float tmpvar_2;
  tmpvar_2 = (_Params.x + _Params.w);
  float tmpvar_3;
  tmpvar_3 = ((_Time.y + _Params.z) / tmpvar_2);
  float tmpvar_4;
  tmpvar_4 = (fract (abs (tmpvar_3)) * tmpvar_2);
  float tmpvar_5;
  if ((tmpvar_3 >= 0.0)) {
    tmpvar_5 = tmpvar_4;
  } else {
    tmpvar_5 = -(tmpvar_4);
  };
  highp float tmpvar_6;
  tmpvar_6 = clamp ((tmpvar_5 / _Params.x), 0.0, 1.0);
  highp vec4 tmpvar_7;
  tmpvar_7.xyz = vec3(1.0, 1.0, 1.0);
  tmpvar_7.w = (1.0 - pow (tmpvar_6, 3.0));
  tmpvar_1 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = (((_glesMultiTexCoord0.xy - vec2(0.5, 0.5)) * (1.0/(((tmpvar_6 * _Params.y) + 0.05)))) + vec2(0.5, 0.5));
  xlv_COLOR = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
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
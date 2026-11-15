Shader "MADFINGER/Environment/Scroll 1 Layer" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _ScrollX ("Scroll speed X", Float) = 1
 _ScrollY ("Scroll speed Y", Float) = 0
 _TimeOffs ("Time offs", Float) = 0
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp float _ScrollY;
uniform highp float _ScrollX;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1.x = _ScrollX;
  tmpvar_1.y = _ScrollY;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = (_glesMultiTexCoord0.xy + fract ((tmpvar_1 * (_Time + _TimeOffs).xy)));
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD0);
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp float _ScrollY;
uniform highp float _ScrollX;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1.x = _ScrollX;
  tmpvar_1.y = _ScrollY;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = (_glesMultiTexCoord0.xy + fract ((tmpvar_1 * (_Time + _TimeOffs).xy)));
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD0);
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" }
"!!GLES"
}
}
 }
}
}
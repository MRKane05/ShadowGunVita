Shader "RenderFX/Skybox" {
Properties {
 _Tint ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _FrontTex ("Front (+Z)", 2D) = "white" {}
 _BackTex ("Back (-Z)", 2D) = "white" {}
 _LeftTex ("Left (+X)", 2D) = "white" {}
 _RightTex ("Right (-X)", 2D) = "white" {}
 _UpTex ("Up (+Y)", 2D) = "white" {}
 _DownTex ("down (-Y)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Background" "RenderType"="Background" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform sampler2D _FrontTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_FrontTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform sampler2D _BackTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_BackTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform sampler2D _LeftTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_LeftTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform sampler2D _RightTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_RightTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform sampler2D _UpTex;
uniform lowp vec4 _Tint;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_UpTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform sampler2D _DownTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_DownTex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
  gl_FragData[0] = col;
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
SubShader { 
 Tags { "QUEUE"="Background" "RenderType"="Background" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_FrontTex] { combine texture +- primary, texture alpha * primary alpha }
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_BackTex] { combine texture +- primary, texture alpha * primary alpha }
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_LeftTex] { combine texture +- primary, texture alpha * primary alpha }
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_RightTex] { combine texture +- primary, texture alpha * primary alpha }
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_UpTex] { combine texture +- primary, texture alpha * primary alpha }
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_DownTex] { combine texture +- primary, texture alpha * primary alpha }
 }
}
}
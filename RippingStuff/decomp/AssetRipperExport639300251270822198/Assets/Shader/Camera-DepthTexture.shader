Shader "Hidden/Camera-DepthTexture" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _Cutoff ("", Float) = 0.5
 _Color ("", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;


attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TransparentCutout" }
 Pass {
  Tags { "RenderType"="TransparentCutout" }
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

uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
void main ()
{
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TreeBark" }
 Pass {
  Tags { "RenderType"="TreeBark" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;


uniform highp vec4 _Wind;
uniform highp vec4 _Time;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  tmpvar_1.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 pos;
  pos = tmpvar_1;
  highp vec3 bend;
  vec4 v_i0_i0;
  v_i0_i0.x = _Object2World[0].w;
  v_i0_i0.y = _Object2World[1].w;
  v_i0_i0.z = _Object2World[2].w;
  v_i0_i0.w = _Object2World[3].w;
  highp float tmpvar_2;
  tmpvar_2 = (dot (v_i0_i0.xyz, vec3(1.0, 1.0, 1.0)) + _glesColor.x);
  highp vec2 tmpvar_3;
  tmpvar_3.x = dot (tmpvar_1.xyz, vec3((_glesColor.y + tmpvar_2)));
  tmpvar_3.y = tmpvar_2;
  highp vec4 tmpvar_4;
  tmpvar_4 = abs (((fract ((((fract (((_Time.yy + tmpvar_3).xxyy * vec4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  highp vec4 tmpvar_5;
  tmpvar_5 = ((tmpvar_4 * tmpvar_4) * (3.0 - (2.0 * tmpvar_4)));
  highp vec2 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xz + tmpvar_5.yw);
  bend = ((_glesColor.y * 0.1) * _glesNormal);
  bend.y = (_glesMultiTexCoord1.y * 0.3);
  pos.xyz = (tmpvar_1.xyz + (((tmpvar_6.xyx * bend) + ((_Wind.xyz * tmpvar_6.y) * _glesMultiTexCoord1.y)) * _Wind.w));
  pos.xyz = (pos.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  tmpvar_1 = tmpvar_7;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_7);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TreeLeaf" }
 Pass {
  Tags { "RenderType"="TreeLeaf" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec2 xlv_TEXCOORD0;


uniform highp vec4 _Wind;
uniform highp vec4 _Time;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp mat4 _Object2World;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp float tmpvar_2;
  tmpvar_2 = (1.0 - abs (_glesTANGENT.w));
  highp vec4 tmpvar_3;
  tmpvar_3.w = 0.0;
  tmpvar_3.xyz = _glesNormal;
  highp vec4 tmpvar_4;
  tmpvar_4.zw = vec2(0.0, 0.0);
  tmpvar_4.xy = _glesNormal.xy;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_glesVertex + ((tmpvar_4 * gl_ModelViewMatrixInverseTranspose) * tmpvar_2));
  tmpvar_1 = tmpvar_5;
  tmpvar_1.xyz = (tmpvar_5.xyz * _Scale.xyz);
  highp vec4 pos_i0;
  pos_i0 = tmpvar_1;
  highp vec3 bend;
  vec4 v_i0_i0;
  v_i0_i0.x = _Object2World[0].w;
  v_i0_i0.y = _Object2World[1].w;
  v_i0_i0.z = _Object2World[2].w;
  v_i0_i0.w = _Object2World[3].w;
  highp float tmpvar_6;
  tmpvar_6 = (dot (v_i0_i0.xyz, vec3(1.0, 1.0, 1.0)) + _glesColor.x);
  highp vec2 tmpvar_7;
  tmpvar_7.x = dot (tmpvar_1.xyz, vec3((_glesColor.y + tmpvar_6)));
  tmpvar_7.y = tmpvar_6;
  highp vec4 tmpvar_8;
  tmpvar_8 = abs (((fract ((((fract (((_Time.yy + tmpvar_7).xxyy * vec4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  highp vec4 tmpvar_9;
  tmpvar_9 = ((tmpvar_8 * tmpvar_8) * (3.0 - (2.0 * tmpvar_8)));
  highp vec2 tmpvar_10;
  tmpvar_10 = (tmpvar_9.xz + tmpvar_9.yw);
  bend = ((_glesColor.y * 0.1) * mix (_glesNormal, normalize ((tmpvar_3 * gl_ModelViewMatrixInverseTranspose)).xyz, vec3(tmpvar_2)));
  bend.y = (_glesMultiTexCoord1.y * 0.3);
  pos_i0.xyz = (tmpvar_1.xyz + (((tmpvar_10.xyx * bend) + ((_Wind.xyz * tmpvar_10.y) * _glesMultiTexCoord1.y)) * _Wind.w));
  pos_i0.xyz = (pos_i0.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = mix ((pos_i0.xyz - ((dot (_SquashPlaneNormal.xyz, pos_i0.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos_i0.xyz, vec3(_SquashAmount));
  tmpvar_1 = tmpvar_11;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_11);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  mediump float alpha;
  lowp float tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha = tmpvar_1;
  mediump float x;
  x = (alpha - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TreeOpaque" }
 Pass {
  Tags { "RenderType"="TreeOpaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;


uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 pos;
  pos = _glesVertex;
  pos.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = pos.xyz;
  pos.xyz = mix (pos.xyz, (_TerrainEngineBendTree * tmpvar_1).xyz, _glesColor.www);
  highp vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  pos = tmpvar_2;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_2);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TreeTransparentCutout" }
 Pass {
  Tags { "RenderType"="TreeTransparentCutout" }
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

uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 pos;
  pos = _glesVertex;
  pos.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = pos.xyz;
  pos.xyz = mix (pos.xyz, (_TerrainEngineBendTree * tmpvar_1).xyz, _glesColor.www);
  highp vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  pos = tmpvar_2;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_2);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  mediump float alpha;
  lowp float tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha = tmpvar_1;
  mediump float x;
  x = (alpha - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="TreeBillboard" }
 Pass {
  Tags { "RenderType"="TreeBillboard" }
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

uniform highp vec4 _TreeBillboardDistances;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardCameraFront;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesMultiTexCoord1.xy;
  highp float offsetz;
  offsetz = _glesMultiTexCoord0.y;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_3;
  tmpvar_3 = dot (tmpvar_2, tmpvar_2);
  if ((tmpvar_3 > _TreeBillboardDistances.x)) {
    offsetz = 0.0;
    offset = vec2(0.0, 0.0);
  };
  pos.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset.x));
  pos.xyz = (pos.xyz + (_TreeBillboardCameraUp.xyz * mix (offset.y, offsetz, _TreeBillboardCameraPos.w)));
  pos.xyz = (pos.xyz + ((_TreeBillboardCameraFront.xyz * abs (offset.x)) * _TreeBillboardCameraUp.w));
  tmpvar_1.x = _glesMultiTexCoord0.x;
  tmpvar_1.y = float((_glesMultiTexCoord0.y > 0.0));
  gl_Position = (gl_ModelViewProjectionMatrix * pos);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  lowp float x;
  x = (texture2D (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="GrassBillboard" }
 Pass {
  Tags { "RenderType"="GrassBillboard" }
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
varying lowp vec4 xlv_COLOR;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_1;
  tmpvar_1 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_2;
  tmpvar_2 = dot (tmpvar_1, tmpvar_1);
  if ((tmpvar_2 > _WaveAndDistance.w)) {
    offset = vec2(0.0, 0.0);
  };
  pos.xyz = (_glesVertex.xyz + (offset.x * _CameraRight));
  pos.xyz = (pos.xyz + (offset.y * _CameraUp));
  highp vec4 vertex;
  vertex = pos;
  lowp vec4 color;
  color = _glesColor;
  lowp vec3 waveColor;
  highp vec3 waveMove;
  waveMove = vec3(0.0, 0.0, 0.0);
  highp vec4 tmpvar_3;
  tmpvar_3 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * tmpvar_3);
  highp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * tmpvar_3);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_4);
  highp vec4 tmpvar_7;
  tmpvar_7 = (((tmpvar_3 + (tmpvar_5 * -0.161616)) + (tmpvar_6 * 0.0083333)) + ((tmpvar_6 * tmpvar_4) * -0.00019841));
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_10, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_10, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_11;
  tmpvar_11 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_9, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_13;
  tmpvar_13 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_12, tmpvar_12))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_13;
  lowp vec4 tmpvar_14;
  tmpvar_14.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_14.w = color.w;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_COLOR = tmpvar_14;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
 Tags { "RenderType"="Grass" }
 Pass {
  Tags { "RenderType"="Grass" }
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
varying lowp vec4 xlv_COLOR;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 vertex;
  vertex = _glesVertex;
  lowp vec4 color;
  color = _glesColor;
  lowp vec3 waveColor;
  highp vec3 waveMove;
  waveMove = vec3(0.0, 0.0, 0.0);
  highp vec4 tmpvar_1;
  tmpvar_1 = ((fract ((((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * tmpvar_1);
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * tmpvar_1);
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * tmpvar_2);
  highp vec4 tmpvar_5;
  tmpvar_5 = (((tmpvar_1 + (tmpvar_3 * -0.161616)) + (tmpvar_4 * 0.0083333)) + ((tmpvar_4 * tmpvar_2) * -0.00019841));
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_6);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (_glesColor.w * _WaveAndDistance.z));
  waveMove.x = dot (tmpvar_8, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_8, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (_glesVertex.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_9;
  tmpvar_9 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_7, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_11;
  tmpvar_11 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_10, tmpvar_10))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_11;
  lowp vec4 tmpvar_12;
  tmpvar_12.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_12.w = color.w;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_COLOR = tmpvar_12;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
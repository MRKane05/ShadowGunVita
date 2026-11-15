Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
Properties {
 _WavingTint ("Fade Color", Color) = (0.7,0.6,0.5,0)
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
 _WaveAndDistance ("Wave and distance", Vector) = (12,3.6,1,1)
 _Cutoff ("Cutoff", Float) = 0.5
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
  Cull Off
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_18;
  mediump vec3 tmpvar_20;
  mediump vec4 normal;
  normal = tmpvar_19;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_21;
  tmpvar_21 = dot (unity_SHAr, normal);
  x1.x = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAg, normal);
  x1.y = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAb, normal);
  x1.z = tmpvar_23;
  mediump vec4 tmpvar_24;
  tmpvar_24 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHBr, tmpvar_24);
  x2.x = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBg, tmpvar_24);
  x2.y = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBb, tmpvar_24);
  x2.z = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_28;
  highp vec3 tmpvar_29;
  tmpvar_29 = (unity_SHC.xyz * vC);
  x3 = tmpvar_29;
  tmpvar_20 = ((x1 + x2) + x3);
  shlight = tmpvar_20;
  tmpvar_2 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_14;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_1.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz));
  c.w = (tmpvar_1.w * xlv_COLOR0.w);
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_14;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  c = vec4(0.0, 0.0, 0.0, 0.0);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_2;
  tmpvar_2 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz);
  lm_i0 = tmpvar_2;
  mediump vec3 tmpvar_3;
  tmpvar_3 = (tmpvar_1.xyz * lm_i0);
  c.xyz = tmpvar_3;
  c.w = (tmpvar_1.w * xlv_COLOR0.w);
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  highp vec4 tmpvar_17;
  tmpvar_17 = (gl_ModelViewProjectionMatrix * vertex);
  mat3 tmpvar_18;
  tmpvar_18[0] = _Object2World[0].xyz;
  tmpvar_18[1] = _Object2World[1].xyz;
  tmpvar_18[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_19;
  highp vec4 tmpvar_20;
  tmpvar_20.w = 1.0;
  tmpvar_20.xyz = tmpvar_19;
  mediump vec3 tmpvar_21;
  mediump vec4 normal;
  normal = tmpvar_20;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAr, normal);
  x1.x = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAg, normal);
  x1.y = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAb, normal);
  x1.z = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBr, tmpvar_25);
  x2.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBg, tmpvar_25);
  x2.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBb, tmpvar_25);
  x2.z = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (unity_SHC.xyz * vC);
  x3 = tmpvar_30;
  tmpvar_21 = ((x1 + x2) + x3);
  shlight = tmpvar_21;
  tmpvar_2 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_31;
  tmpvar_31 = (tmpvar_17 * 0.5);
  o_i0 = tmpvar_31;
  highp vec2 tmpvar_32;
  tmpvar_32.x = tmpvar_31.x;
  tmpvar_32.y = (tmpvar_31.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_32 + tmpvar_31.w);
  o_i0.zw = tmpvar_17.zw;
  gl_Position = tmpvar_17;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD3).x) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  highp vec4 tmpvar_15;
  tmpvar_15 = (gl_ModelViewProjectionMatrix * vertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * 0.5);
  o_i0 = tmpvar_16;
  highp vec2 tmpvar_17;
  tmpvar_17.x = tmpvar_16.x;
  tmpvar_17.y = (tmpvar_16.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_17 + tmpvar_16.w);
  o_i0.zw = tmpvar_15.zw;
  gl_Position = tmpvar_15;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_14;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD2 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_1.xyz * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD2).x * 2.0))));
  c.w = (tmpvar_1.w * xlv_COLOR0.w);
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  highp vec4 tmpvar_15;
  tmpvar_15 = (gl_ModelViewProjectionMatrix * vertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * 0.5);
  o_i0 = tmpvar_16;
  highp vec2 tmpvar_17;
  tmpvar_17.x = tmpvar_16.x;
  tmpvar_17.y = (tmpvar_16.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_17 + tmpvar_16.w);
  o_i0.zw = tmpvar_15.zw;
  gl_Position = tmpvar_15;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_14;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD2 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  c = vec4(0.0, 0.0, 0.0, 0.0);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_2;
  tmpvar_2 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz);
  lm_i0 = tmpvar_2;
  mediump vec4 tmpvar_3;
  tmpvar_3.w = 0.0;
  tmpvar_3.xyz = lm_i0;
  lowp vec3 tmpvar_4;
  tmpvar_4 = vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD2).x * 2.0));
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_1.xyz * min (tmpvar_3.xyz, tmpvar_4));
  c.xyz = tmpvar_5;
  c.w = (tmpvar_1.w * xlv_COLOR0.w);
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_18;
  mediump vec3 tmpvar_20;
  mediump vec4 normal;
  normal = tmpvar_19;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_21;
  tmpvar_21 = dot (unity_SHAr, normal);
  x1.x = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAg, normal);
  x1.y = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAb, normal);
  x1.z = tmpvar_23;
  mediump vec4 tmpvar_24;
  tmpvar_24 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHBr, tmpvar_24);
  x2.x = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBg, tmpvar_24);
  x2.y = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBb, tmpvar_24);
  x2.z = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_28;
  highp vec3 tmpvar_29;
  tmpvar_29 = (unity_SHC.xyz * vC);
  x3 = tmpvar_29;
  tmpvar_20 = ((x1 + x2) + x3);
  shlight = tmpvar_20;
  tmpvar_2 = shlight;
  highp vec3 tmpvar_30;
  tmpvar_30 = (_Object2World * vertex).xyz;
  highp vec4 tmpvar_31;
  tmpvar_31 = (unity_4LightPosX0 - tmpvar_30.x);
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosY0 - tmpvar_30.y);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosZ0 - tmpvar_30.z);
  highp vec4 tmpvar_34;
  tmpvar_34 = (((tmpvar_31 * tmpvar_31) + (tmpvar_32 * tmpvar_32)) + (tmpvar_33 * tmpvar_33));
  highp vec4 tmpvar_35;
  tmpvar_35 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_31 * tmpvar_18.x) + (tmpvar_32 * tmpvar_18.y)) + (tmpvar_33 * tmpvar_18.z)) * inversesqrt (tmpvar_34))) * (1.0/((1.0 + (tmpvar_34 * unity_4LightAtten0)))));
  highp vec3 tmpvar_36;
  tmpvar_36 = (tmpvar_2 + ((((unity_LightColor[0].xyz * tmpvar_35.x) + (unity_LightColor[1].xyz * tmpvar_35.y)) + (unity_LightColor[2].xyz * tmpvar_35.z)) + (unity_LightColor[3].xyz * tmpvar_35.w)));
  tmpvar_2 = tmpvar_36;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec3 shlight;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  highp vec4 tmpvar_17;
  tmpvar_17 = (gl_ModelViewProjectionMatrix * vertex);
  mat3 tmpvar_18;
  tmpvar_18[0] = _Object2World[0].xyz;
  tmpvar_18[1] = _Object2World[1].xyz;
  tmpvar_18[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_19;
  highp vec4 tmpvar_20;
  tmpvar_20.w = 1.0;
  tmpvar_20.xyz = tmpvar_19;
  mediump vec3 tmpvar_21;
  mediump vec4 normal;
  normal = tmpvar_20;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAr, normal);
  x1.x = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAg, normal);
  x1.y = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAb, normal);
  x1.z = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBr, tmpvar_25);
  x2.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBg, tmpvar_25);
  x2.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBb, tmpvar_25);
  x2.z = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (unity_SHC.xyz * vC);
  x3 = tmpvar_30;
  tmpvar_21 = ((x1 + x2) + x3);
  shlight = tmpvar_21;
  tmpvar_2 = shlight;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_Object2World * vertex).xyz;
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosX0 - tmpvar_31.x);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosY0 - tmpvar_31.y);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosZ0 - tmpvar_31.z);
  highp vec4 tmpvar_35;
  tmpvar_35 = (((tmpvar_32 * tmpvar_32) + (tmpvar_33 * tmpvar_33)) + (tmpvar_34 * tmpvar_34));
  highp vec4 tmpvar_36;
  tmpvar_36 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_32 * tmpvar_19.x) + (tmpvar_33 * tmpvar_19.y)) + (tmpvar_34 * tmpvar_19.z)) * inversesqrt (tmpvar_35))) * (1.0/((1.0 + (tmpvar_35 * unity_4LightAtten0)))));
  highp vec3 tmpvar_37;
  tmpvar_37 = (tmpvar_2 + ((((unity_LightColor[0].xyz * tmpvar_36.x) + (unity_LightColor[1].xyz * tmpvar_36.y)) + (unity_LightColor[2].xyz * tmpvar_36.z)) + (unity_LightColor[3].xyz * tmpvar_36.w)));
  tmpvar_2 = tmpvar_37;
  highp vec4 o_i0;
  highp vec4 tmpvar_38;
  tmpvar_38 = (tmpvar_17 * 0.5);
  o_i0 = tmpvar_38;
  highp vec2 tmpvar_39;
  tmpvar_39.x = tmpvar_38.x;
  tmpvar_39.y = (tmpvar_38.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_39 + tmpvar_38.w);
  o_i0.zw = tmpvar_17.zw;
  gl_Position = tmpvar_17;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * texture2DProj (_ShadowMapTexture, xlv_TEXCOORD3).x) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * xlv_TEXCOORD2));
  gl_FragData[0] = c;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_WorldSpaceLightPos0.xyz - (_Object2World * vertex).xyz);
  tmpvar_2 = tmpvar_19;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * vertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  mediump vec3 tmpvar_2;
  tmpvar_2 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * texture2D (_LightTexture0, tmpvar_3).w) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_19;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lightDir = xlv_TEXCOORD2;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD1, lightDir)) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_WorldSpaceLightPos0.xyz - (_Object2World * vertex).xyz);
  tmpvar_2 = tmpvar_19;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * vertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  mediump vec3 tmpvar_2;
  tmpvar_2 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_2;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD3.xyz;
  highp vec2 tmpvar_3;
  tmpvar_3 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD3.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD3.xy / xlv_TEXCOORD3.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_3).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * atten) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_WorldSpaceLightPos0.xyz - (_Object2World * vertex).xyz);
  tmpvar_2 = tmpvar_19;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * vertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  mediump vec3 tmpvar_2;
  tmpvar_2 = normalize (xlv_TEXCOORD2);
  lightDir = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * (texture2D (_LightTextureB0, tmpvar_3).w * textureCube (_LightTexture0, xlv_TEXCOORD3).w)) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + (tmpvar_7 * -0.161616)) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_16.w = color.w;
  mat3 tmpvar_17;
  tmpvar_17[0] = _Object2World[0].xyz;
  tmpvar_17[1] = _Object2World[1].xyz;
  tmpvar_17[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_glesNormal * unity_Scale.w));
  tmpvar_1 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_19;
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_16;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * vertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  lowp float x;
  x = (tmpvar_1.w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  lightDir = xlv_TEXCOORD2;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD1, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD3).w) * 2.0));
  c_i0_i1.w = (tmpvar_1.w * xlv_COLOR0.w);
  c = c_i0_i1;
  c.w = 0.0;
  gl_FragData[0] = c;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}
}
 }
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
  Cull Off
  Fog { Mode Off }
  ColorMask RGB
  Offset 1, 1
Program "vp" {
SubProgram "gles " {
Keywords { "SHADOWS_DEPTH" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD1;
uniform highp vec4 unity_LightShadowBias;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_3;
  tmpvar_3 = dot (tmpvar_2, tmpvar_2);
  if ((tmpvar_3 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_4;
  tmpvar_4 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_4);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (((tmpvar_4 + (tmpvar_6 * -0.161616)) + (tmpvar_7 * 0.0083333)) + ((tmpvar_7 * tmpvar_5) * -0.00019841));
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_11, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_11, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_12;
  tmpvar_12 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_10, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_14;
  tmpvar_14 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_13, tmpvar_13))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_14;
  lowp vec4 tmpvar_15;
  tmpvar_15.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_15.w = color.w;
  highp vec4 tmpvar_16;
  tmpvar_16 = (gl_ModelViewProjectionMatrix * vertex);
  tmpvar_1 = tmpvar_16;
  tmpvar_1.z = (tmpvar_16.z + unity_LightShadowBias.x);
  tmpvar_1.z = mix (tmpvar_1.z, max (tmpvar_1.z, -(tmpvar_16.w)), unity_LightShadowBias.y);
  gl_Position = tmpvar_1;
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_15;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD1;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD1) * xlv_COLOR0).w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;

uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _LightPositionRange;
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
  xlv_TEXCOORD0 = ((_Object2World * vertex).xyz - _LightPositionRange.xyz);
  xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_14;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform highp vec4 _LightPositionRange;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD1) * xlv_COLOR0).w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 tmpvar_2;
  tmpvar_2 = fract ((vec4(1.0, 255.0, 65025.0, 160580992.0) * (length (xlv_TEXCOORD0) * _LightPositionRange.w)));
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 - (tmpvar_2.yzww * 0.00392157));
  tmpvar_1 = tmpvar_3;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "SHADOWS_DEPTH" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SHADOWS_CUBE" }
"!!GLES"
}
}
 }
 Pass {
  Name "SHADOWCOLLECTOR"
  Tags { "LIGHTMODE"="SHADOWCOLLECTOR" "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
  Cull Off
  Fog { Mode Off }
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow[4];


uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_3;
  tmpvar_3 = dot (tmpvar_2, tmpvar_2);
  if ((tmpvar_3 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_4;
  tmpvar_4 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_4);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (((tmpvar_4 + (tmpvar_6 * -0.161616)) + (tmpvar_7 * 0.0083333)) + ((tmpvar_7 * tmpvar_5) * -0.00019841));
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_11, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_11, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_12;
  tmpvar_12 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_10, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_14;
  tmpvar_14 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_13, tmpvar_13))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_14;
  lowp vec4 tmpvar_15;
  tmpvar_15.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_15.w = color.w;
  highp vec4 tmpvar_16;
  tmpvar_16 = (_Object2World * vertex);
  tmpvar_1.xyz = tmpvar_16.xyz;
  tmpvar_1.w = -((gl_ModelViewMatrix * vertex).z);
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = (unity_World2Shadow[0] * tmpvar_16).xyz;
  xlv_TEXCOORD1 = (unity_World2Shadow[1] * tmpvar_16).xyz;
  xlv_TEXCOORD2 = (unity_World2Shadow[2] * tmpvar_16).xyz;
  xlv_TEXCOORD3 = (unity_World2Shadow[3] * tmpvar_16).xyz;
  xlv_TEXCOORD4 = tmpvar_1;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_15;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _MainTex;
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp vec4 _LightShadowData;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res;
  highp vec4 zFar;
  highp vec4 zNear;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD5) * xlv_COLOR0).w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD4.wwww;
  lowp vec4 tmpvar_3;
  tmpvar_3 = vec4(greaterThanEqual (tmpvar_2, _LightSplitsNear));
  zNear = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = vec4(lessThan (tmpvar_2, _LightSplitsFar));
  zFar = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (zNear * zFar);
  highp float tmpvar_6;
  tmpvar_6 = clamp (((xlv_TEXCOORD4.w * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((((xlv_TEXCOORD0 * tmpvar_5.x) + (xlv_TEXCOORD1 * tmpvar_5.y)) + (xlv_TEXCOORD2 * tmpvar_5.z)) + (xlv_TEXCOORD3 * tmpvar_5.w));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_ShadowMapTexture, tmpvar_7.xy);
  highp float tmpvar_9;
  if ((tmpvar_8.x < tmpvar_7.z)) {
    tmpvar_9 = _LightShadowData.x;
  } else {
    tmpvar_9 = 1.0;
  };
  res.x = clamp ((tmpvar_9 + tmpvar_6), 0.0, 1.0);
  res.y = 1.0;
  highp vec2 enc;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract ((vec2(1.0, 255.0) * (1.0 - (xlv_TEXCOORD4.w * _ProjectionParams.w))));
  enc = tmpvar_10;
  enc.x = (tmpvar_10.x - (tmpvar_10.y * 0.00392157));
  res.zw = enc;
  tmpvar_1 = res;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow[4];


uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec3 _CameraUp;
uniform highp vec3 _CameraRight;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesTANGENT.xy;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_3;
  tmpvar_3 = dot (tmpvar_2, tmpvar_2);
  if ((tmpvar_3 > _WaveAndDistance.w)) {
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
  highp vec4 tmpvar_4;
  tmpvar_4 = ((fract ((((pos.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_4);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (((tmpvar_4 + (tmpvar_6 * -0.161616)) + (tmpvar_7 * 0.0083333)) + ((tmpvar_7 * tmpvar_5) * -0.00019841));
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * _glesTANGENT.y);
  waveMove.x = dot (tmpvar_11, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_11, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (pos.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_12;
  tmpvar_12 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_10, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_14;
  tmpvar_14 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_13, tmpvar_13))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_14;
  lowp vec4 tmpvar_15;
  tmpvar_15.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_15.w = color.w;
  highp vec4 tmpvar_16;
  tmpvar_16 = (_Object2World * vertex);
  tmpvar_1.xyz = tmpvar_16.xyz;
  tmpvar_1.w = -((gl_ModelViewMatrix * vertex).z);
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_TEXCOORD0 = (unity_World2Shadow[0] * tmpvar_16).xyz;
  xlv_TEXCOORD1 = (unity_World2Shadow[1] * tmpvar_16).xyz;
  xlv_TEXCOORD2 = (unity_World2Shadow[2] * tmpvar_16).xyz;
  xlv_TEXCOORD3 = (unity_World2Shadow[3] * tmpvar_16).xyz;
  xlv_TEXCOORD4 = tmpvar_1;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR0 = tmpvar_15;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _MainTex;
uniform highp vec4 _LightShadowData;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res;
  highp vec4 cascadeWeights;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD5) * xlv_COLOR0).w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec3 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD4.xyz - unity_ShadowSplitSpheres[0].xyz);
  highp vec3 tmpvar_3;
  tmpvar_3 = (xlv_TEXCOORD4.xyz - unity_ShadowSplitSpheres[1].xyz);
  highp vec3 tmpvar_4;
  tmpvar_4 = (xlv_TEXCOORD4.xyz - unity_ShadowSplitSpheres[2].xyz);
  highp vec3 tmpvar_5;
  tmpvar_5 = (xlv_TEXCOORD4.xyz - unity_ShadowSplitSpheres[3].xyz);
  highp vec4 tmpvar_6;
  tmpvar_6.x = dot (tmpvar_2, tmpvar_2);
  tmpvar_6.y = dot (tmpvar_3, tmpvar_3);
  tmpvar_6.z = dot (tmpvar_4, tmpvar_4);
  tmpvar_6.w = dot (tmpvar_5, tmpvar_5);
  lowp vec4 tmpvar_7;
  tmpvar_7 = vec4(lessThan (tmpvar_6, unity_ShadowSplitSqRadii));
  cascadeWeights = tmpvar_7;
  cascadeWeights.yzw = clamp ((cascadeWeights.yzw - cascadeWeights.xyz), 0.0, 1.0);
  highp float tmpvar_8;
  tmpvar_8 = clamp (((distance (xlv_TEXCOORD4.xyz, unity_ShadowFadeCenterAndType.xyz) * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = ((((xlv_TEXCOORD0 * cascadeWeights.x) + (xlv_TEXCOORD1 * cascadeWeights.y)) + (xlv_TEXCOORD2 * cascadeWeights.z)) + (xlv_TEXCOORD3 * cascadeWeights.w));
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_ShadowMapTexture, tmpvar_9.xy);
  highp float tmpvar_11;
  if ((tmpvar_10.x < tmpvar_9.z)) {
    tmpvar_11 = _LightShadowData.x;
  } else {
    tmpvar_11 = 1.0;
  };
  res.x = clamp ((tmpvar_11 + tmpvar_8), 0.0, 1.0);
  res.y = 1.0;
  highp vec2 enc;
  highp vec2 tmpvar_12;
  tmpvar_12 = fract ((vec2(1.0, 255.0) * (1.0 - (xlv_TEXCOORD4.w * _ProjectionParams.w))));
  enc = tmpvar_12;
  enc.x = (tmpvar_12.x - (tmpvar_12.y * 0.00392157));
  res.zw = enc;
  tmpvar_1 = res;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES"
}
}
 }
}
SubShader { 
 Tags { "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
 Pass {
  Tags { "QUEUE"="Geometry+200" "IGNOREPROJECTOR"="True" "RenderType"="GrassBillboard" }
  Lighting On
  Cull Off
  AlphaTest Greater [_Cutoff]
  ColorMask RGB
Program "vp" {
}
  SetTexture [_MainTex] { combine texture * primary double, texture alpha * primary alpha }
 }
}
Fallback Off
}
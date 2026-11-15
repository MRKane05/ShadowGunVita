Shader "Hidden/Camera-DepthNormalTexture" {
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD0;



uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  mat3 tmpvar_2;
  tmpvar_2[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_2[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_2[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;



uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  mat3 tmpvar_2;
  tmpvar_2[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_2[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_2[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;



uniform highp vec4 _Wind;
uniform highp vec4 _Time;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = _glesVertex;
  tmpvar_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 pos;
  pos = tmpvar_2;
  highp vec3 bend;
  vec4 v_i0_i0;
  v_i0_i0.x = _Object2World[0].w;
  v_i0_i0.y = _Object2World[1].w;
  v_i0_i0.z = _Object2World[2].w;
  v_i0_i0.w = _Object2World[3].w;
  highp float tmpvar_3;
  tmpvar_3 = (dot (v_i0_i0.xyz, vec3(1.0, 1.0, 1.0)) + _glesColor.x);
  highp vec2 tmpvar_4;
  tmpvar_4.x = dot (tmpvar_2.xyz, vec3((_glesColor.y + tmpvar_3)));
  tmpvar_4.y = tmpvar_3;
  highp vec4 tmpvar_5;
  tmpvar_5 = abs (((fract ((((fract (((_Time.yy + tmpvar_4).xxyy * vec4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  highp vec4 tmpvar_6;
  tmpvar_6 = ((tmpvar_5 * tmpvar_5) * (3.0 - (2.0 * tmpvar_5)));
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6.xz + tmpvar_6.yw);
  bend = ((_glesColor.y * 0.1) * _glesNormal);
  bend.y = (_glesMultiTexCoord1.y * 0.3);
  pos.xyz = (tmpvar_2.xyz + (((tmpvar_7.xyx * bend) + ((_Wind.xyz * tmpvar_7.y) * _glesMultiTexCoord1.y)) * _Wind.w));
  pos.xyz = (pos.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  tmpvar_2 = tmpvar_8;
  mat3 tmpvar_9;
  tmpvar_9[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_9[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_9[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_9 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * tmpvar_8).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_8);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;



uniform highp vec4 _Wind;
uniform highp vec4 _Time;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp vec4 _ProjectionParams;
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
  highp vec4 tmpvar_2;
  highp float tmpvar_3;
  tmpvar_3 = (1.0 - abs (_glesTANGENT.w));
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = _glesNormal;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = _glesNormal.xy;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_glesVertex + ((tmpvar_5 * gl_ModelViewMatrixInverseTranspose) * tmpvar_3));
  highp vec3 tmpvar_7;
  tmpvar_7 = mix (_glesNormal, normalize ((tmpvar_4 * gl_ModelViewMatrixInverseTranspose)).xyz, vec3(tmpvar_3));
  tmpvar_2 = tmpvar_6;
  tmpvar_2.xyz = (tmpvar_6.xyz * _Scale.xyz);
  highp vec4 pos_i0;
  pos_i0 = tmpvar_2;
  highp vec3 bend;
  vec4 v_i0_i0;
  v_i0_i0.x = _Object2World[0].w;
  v_i0_i0.y = _Object2World[1].w;
  v_i0_i0.z = _Object2World[2].w;
  v_i0_i0.w = _Object2World[3].w;
  highp float tmpvar_8;
  tmpvar_8 = (dot (v_i0_i0.xyz, vec3(1.0, 1.0, 1.0)) + _glesColor.x);
  highp vec2 tmpvar_9;
  tmpvar_9.x = dot (tmpvar_2.xyz, vec3((_glesColor.y + tmpvar_8)));
  tmpvar_9.y = tmpvar_8;
  highp vec4 tmpvar_10;
  tmpvar_10 = abs (((fract ((((fract (((_Time.yy + tmpvar_9).xxyy * vec4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = ((tmpvar_10 * tmpvar_10) * (3.0 - (2.0 * tmpvar_10)));
  highp vec2 tmpvar_12;
  tmpvar_12 = (tmpvar_11.xz + tmpvar_11.yw);
  bend = ((_glesColor.y * 0.1) * tmpvar_7);
  bend.y = (_glesMultiTexCoord1.y * 0.3);
  pos_i0.xyz = (tmpvar_2.xyz + (((tmpvar_12.xyx * bend) + ((_Wind.xyz * tmpvar_12.y) * _glesMultiTexCoord1.y)) * _Wind.w));
  pos_i0.xyz = (pos_i0.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = mix ((pos_i0.xyz - ((dot (_SquashPlaneNormal.xyz, pos_i0.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos_i0.xyz, vec3(_SquashAmount));
  tmpvar_2 = tmpvar_13;
  mat3 tmpvar_14;
  tmpvar_14[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_14[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_14[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_14 * normalize (tmpvar_7));
  tmpvar_1.w = -(((gl_ModelViewMatrix * tmpvar_13).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_13);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha = tmpvar_2;
  mediump float x;
  x = (alpha - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_3;
  tmpvar_3 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_3;
  enc_i0_i1.x = (tmpvar_3.x - (tmpvar_3.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD0;



uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  pos.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_2;
  tmpvar_2.w = 0.0;
  tmpvar_2.xyz = pos.xyz;
  pos.xyz = mix (pos.xyz, (_TerrainEngineBendTree * tmpvar_2).xyz, _glesColor.www);
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  pos = tmpvar_3;
  mat3 tmpvar_4;
  tmpvar_4[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_4[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_4[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_4 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * tmpvar_3).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;



uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp vec4 _ProjectionParams;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  pos.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_2;
  tmpvar_2.w = 0.0;
  tmpvar_2.xyz = pos.xyz;
  pos.xyz = mix (pos.xyz, (_TerrainEngineBendTree * tmpvar_2).xyz, _glesColor.www);
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  pos = tmpvar_3;
  mat3 tmpvar_4;
  tmpvar_4[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_4[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_4[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_4 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * tmpvar_3).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_3);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha = tmpvar_2;
  mediump float x;
  x = (alpha - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_3;
  tmpvar_3 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_3;
  enc_i0_i1.x = (tmpvar_3.x - (tmpvar_3.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
  Tags { "RenderType"="TreeTransparentCutout" }
  Cull Front
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

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;



uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Scale;
uniform highp vec4 _ProjectionParams;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  pos.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_2;
  tmpvar_2.w = 0.0;
  tmpvar_2.xyz = pos.xyz;
  pos.xyz = mix (pos.xyz, (_TerrainEngineBendTree * tmpvar_2).xyz, _glesColor.www);
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = mix ((pos.xyz - ((dot (_SquashPlaneNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * _SquashPlaneNormal.xyz)), pos.xyz, vec3(_SquashAmount));
  pos = tmpvar_3;
  mat3 tmpvar_4;
  tmpvar_4[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_4[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_4[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = -((tmpvar_4 * normalize (_glesNormal)));
  tmpvar_1.w = -(((gl_ModelViewMatrix * tmpvar_3).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_3);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = (texture2D (_MainTex, xlv_TEXCOORD0).w - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;


uniform highp vec4 _TreeBillboardDistances;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardCameraFront;
uniform highp vec4 _ProjectionParams;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  highp vec4 tmpvar_2;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesMultiTexCoord1.xy;
  highp float offsetz;
  offsetz = _glesMultiTexCoord0.y;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_4;
  tmpvar_4 = dot (tmpvar_3, tmpvar_3);
  if ((tmpvar_4 > _TreeBillboardDistances.x)) {
    offsetz = 0.0;
    offset = vec2(0.0, 0.0);
  };
  pos.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset.x));
  pos.xyz = (pos.xyz + (_TreeBillboardCameraUp.xyz * mix (offset.y, offsetz, _TreeBillboardCameraPos.w)));
  pos.xyz = (pos.xyz + ((_TreeBillboardCameraFront.xyz * abs (offset.x)) * _TreeBillboardCameraUp.w));
  tmpvar_1.x = _glesMultiTexCoord0.x;
  tmpvar_1.y = float((_glesMultiTexCoord0.y > 0.0));
  tmpvar_2.xyz = vec3(0.0, 0.0, 1.0);
  tmpvar_2.w = -(((gl_ModelViewMatrix * pos).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * pos);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = (texture2D (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;



uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
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
  mat3 tmpvar_16;
  tmpvar_16[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_16[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_16[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_16 * _glesNormal);
  tmpvar_1.w = -(((gl_ModelViewMatrix * vertex).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_COLOR = tmpvar_15;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;



uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _CameraPosition;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 vertex;
  vertex = _glesVertex;
  lowp vec4 color;
  color = _glesColor;
  lowp vec3 waveColor;
  highp vec3 waveMove;
  waveMove = vec3(0.0, 0.0, 0.0);
  highp vec4 tmpvar_2;
  tmpvar_2 = ((fract ((((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))) * 6.40885) - 3.14159);
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * tmpvar_2);
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * tmpvar_2);
  highp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * tmpvar_3);
  highp vec4 tmpvar_6;
  tmpvar_6 = (((tmpvar_2 + (tmpvar_4 * -0.161616)) + (tmpvar_5 * 0.0083333)) + ((tmpvar_5 * tmpvar_3) * -0.00019841));
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_6);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_glesColor.w * _WaveAndDistance.z));
  waveMove.x = dot (tmpvar_9, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove.z = dot (tmpvar_9, vec4(0.006, 0.02, -0.02, 0.1));
  vertex.xz = (_glesVertex.xz - (waveMove.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_10;
  tmpvar_10 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_8, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = (vertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_12;
  tmpvar_12 = clamp (((2.0 * (_WaveAndDistance.w - dot (tmpvar_11, tmpvar_11))) * _CameraPosition.w), 0.0, 1.0);
  color.w = tmpvar_12;
  lowp vec4 tmpvar_13;
  tmpvar_13.xyz = ((2.0 * waveColor) * _glesColor.xyz);
  tmpvar_13.w = color.w;
  mat3 tmpvar_14;
  tmpvar_14[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_14[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_14[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  tmpvar_1.xyz = (tmpvar_14 * normalize (_glesNormal));
  tmpvar_1.w = -(((gl_ModelViewMatrix * vertex).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * vertex);
  xlv_COLOR = tmpvar_13;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x;
  x = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x < 0.0)) {
    discard;
  };
  highp vec4 enc;
  enc.xy = ((((xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0)) / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_i0_i1;
  highp vec2 tmpvar_2;
  tmpvar_2 = fract ((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_i0_i1 = tmpvar_2;
  enc_i0_i1.x = (tmpvar_2.x - (tmpvar_2.y * 0.00392157));
  enc.zw = enc_i0_i1;
  tmpvar_1 = enc;
  gl_FragData[0] = tmpvar_1;
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
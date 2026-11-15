Shader "MADFINGER/Characters/BRDFLit (Supports Backlight)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
 _LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
}
SubShader { 
 LOD 400
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  highp vec4 tmpvar_3;
  mediump vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  highp vec3 SHLighting;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = (tmpvar_8 * tmpvar_2);
  mediump vec3 tmpvar_10;
  mediump vec4 normal;
  normal = tmpvar_9;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAr, normal);
  x1.x = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAg, normal);
  x1.y = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAb, normal);
  x1.z = tmpvar_13;
  mediump vec4 tmpvar_14;
  tmpvar_14 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBr, tmpvar_14);
  x2.x = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBg, tmpvar_14);
  x2.y = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBb, tmpvar_14);
  x2.z = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (unity_SHC.xyz * vC);
  x3 = tmpvar_19;
  tmpvar_10 = ((x1 + x2) + x3);
  SHLighting = tmpvar_10;
  highp vec3 tmpvar_20;
  tmpvar_20 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_7 = tmpvar_20;
  tmpvar_4 = tmpvar_7;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_21;
  tmpvar_21[0] = tmpvar_1.xyz;
  tmpvar_21[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_21[2] = tmpvar_2;
  mat3 tmpvar_22;
  tmpvar_22[0].x = tmpvar_21[0].x;
  tmpvar_22[0].y = tmpvar_21[1].x;
  tmpvar_22[0].z = tmpvar_21[2].x;
  tmpvar_22[1].x = tmpvar_21[0].y;
  tmpvar_22[1].y = tmpvar_21[1].y;
  tmpvar_22[1].z = tmpvar_21[2].y;
  tmpvar_22[2].x = tmpvar_21[0].z;
  tmpvar_22[2].y = tmpvar_21[1].z;
  tmpvar_22[2].z = tmpvar_21[2].z;
  highp vec3 tmpvar_23;
  tmpvar_23 = (tmpvar_22 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_25;
  tmpvar_25 = normalize ((tmpvar_22 * (((_World2Object * tmpvar_24).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_6 = tmpvar_25;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = vec3(0.0, 0.0, 0.0);
  xlv_TEXCOORD4 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_7;
  tmpvar_7.x = ((dot (tmpvar_6, xlv_TEXCOORD2) * 0.5) + 0.5);
  tmpvar_7.y = dot (tmpvar_6, normalize ((xlv_TEXCOORD2 + xlv_TEXCOORD4)));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_BRDFTex, tmpvar_7);
  c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_5 * xlv_TEXCOORD3));
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

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  highp vec4 tmpvar_3;
  mediump vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  highp vec3 SHLighting;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = (tmpvar_8 * tmpvar_2);
  mediump vec3 tmpvar_10;
  mediump vec4 normal;
  normal = tmpvar_9;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_11;
  tmpvar_11 = dot (unity_SHAr, normal);
  x1.x = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAg, normal);
  x1.y = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAb, normal);
  x1.z = tmpvar_13;
  mediump vec4 tmpvar_14;
  tmpvar_14 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHBr, tmpvar_14);
  x2.x = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBg, tmpvar_14);
  x2.y = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBb, tmpvar_14);
  x2.z = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (unity_SHC.xyz * vC);
  x3 = tmpvar_19;
  tmpvar_10 = ((x1 + x2) + x3);
  SHLighting = tmpvar_10;
  highp vec3 tmpvar_20;
  tmpvar_20 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_7 = tmpvar_20;
  tmpvar_4 = tmpvar_7;
  highp vec4 tmpvar_21;
  tmpvar_21 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_22;
  tmpvar_22[0] = tmpvar_1.xyz;
  tmpvar_22[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_22[2] = tmpvar_2;
  mat3 tmpvar_23;
  tmpvar_23[0].x = tmpvar_22[0].x;
  tmpvar_23[0].y = tmpvar_22[1].x;
  tmpvar_23[0].z = tmpvar_22[2].x;
  tmpvar_23[1].x = tmpvar_22[0].y;
  tmpvar_23[1].y = tmpvar_22[1].y;
  tmpvar_23[1].z = tmpvar_22[2].y;
  tmpvar_23[2].x = tmpvar_22[0].z;
  tmpvar_23[2].y = tmpvar_22[1].z;
  tmpvar_23[2].z = tmpvar_22[2].z;
  highp vec3 tmpvar_24;
  tmpvar_24 = (tmpvar_23 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_24;
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_26;
  tmpvar_26 = normalize ((tmpvar_23 * (((_World2Object * tmpvar_25).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_6 = tmpvar_26;
  highp vec4 o_i0_i1;
  highp vec4 tmpvar_27;
  tmpvar_27 = (tmpvar_21 * 0.5);
  o_i0_i1 = tmpvar_27;
  highp vec2 tmpvar_28;
  tmpvar_28.x = tmpvar_27.x;
  tmpvar_28.y = (tmpvar_27.y * _ProjectionParams.x);
  o_i0_i1.xy = (tmpvar_28 + tmpvar_27.w);
  o_i0_i1.zw = tmpvar_21.zw;
  gl_Position = tmpvar_21;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = vec3(0.0, 0.0, 0.0);
  xlv_TEXCOORD4 = tmpvar_6;
  xlv_TEXCOORD5 = o_i0_i1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_7;
  tmpvar_7.x = ((dot (tmpvar_6, xlv_TEXCOORD2) * 0.5) + 0.5);
  tmpvar_7.y = dot (tmpvar_6, normalize ((xlv_TEXCOORD2 + xlv_TEXCOORD4)));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_BRDFTex, tmpvar_7);
  c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_5 * xlv_TEXCOORD3));
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

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
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

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  highp vec4 tmpvar_3;
  mediump vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  highp vec3 SHLighting;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = (tmpvar_9 * tmpvar_2);
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  SHLighting = tmpvar_11;
  highp vec3 tmpvar_21;
  tmpvar_21 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_8 = tmpvar_21;
  tmpvar_4 = tmpvar_8;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  mat3 tmpvar_22;
  tmpvar_22[0] = _Object2World[0].xyz;
  tmpvar_22[1] = _Object2World[1].xyz;
  tmpvar_22[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_23;
  tmpvar_23 = (tmpvar_22 * (tmpvar_2 * unity_Scale.w));
  highp mat3 tmpvar_24;
  tmpvar_24[0] = tmpvar_1.xyz;
  tmpvar_24[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_24[2] = tmpvar_2;
  mat3 tmpvar_25;
  tmpvar_25[0].x = tmpvar_24[0].x;
  tmpvar_25[0].y = tmpvar_24[1].x;
  tmpvar_25[0].z = tmpvar_24[2].x;
  tmpvar_25[1].x = tmpvar_24[0].y;
  tmpvar_25[1].y = tmpvar_24[1].y;
  tmpvar_25[1].z = tmpvar_24[2].y;
  tmpvar_25[2].x = tmpvar_24[0].z;
  tmpvar_25[2].y = tmpvar_24[1].z;
  tmpvar_25[2].z = tmpvar_24[2].z;
  highp vec3 tmpvar_26;
  tmpvar_26 = (tmpvar_25 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_26;
  highp vec4 tmpvar_27;
  tmpvar_27.w = 1.0;
  tmpvar_27.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize ((tmpvar_25 * (((_World2Object * tmpvar_27).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_7 = tmpvar_28;
  highp vec3 tmpvar_29;
  tmpvar_29 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_30;
  tmpvar_30 = (unity_4LightPosX0 - tmpvar_29.x);
  highp vec4 tmpvar_31;
  tmpvar_31 = (unity_4LightPosY0 - tmpvar_29.y);
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosZ0 - tmpvar_29.z);
  highp vec4 tmpvar_33;
  tmpvar_33 = (((tmpvar_30 * tmpvar_30) + (tmpvar_31 * tmpvar_31)) + (tmpvar_32 * tmpvar_32));
  highp vec4 tmpvar_34;
  tmpvar_34 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_30 * tmpvar_23.x) + (tmpvar_31 * tmpvar_23.y)) + (tmpvar_32 * tmpvar_23.z)) * inversesqrt (tmpvar_33))) * (1.0/((1.0 + (tmpvar_33 * unity_4LightAtten0)))));
  highp vec3 tmpvar_35;
  tmpvar_35 = ((((unity_LightColor[0].xyz * tmpvar_34.x) + (unity_LightColor[1].xyz * tmpvar_34.y)) + (unity_LightColor[2].xyz * tmpvar_34.z)) + (unity_LightColor[3].xyz * tmpvar_34.w));
  tmpvar_6 = tmpvar_35;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_7;
  tmpvar_7.x = ((dot (tmpvar_6, xlv_TEXCOORD2) * 0.5) + 0.5);
  tmpvar_7.y = dot (tmpvar_6, normalize ((xlv_TEXCOORD2 + xlv_TEXCOORD4)));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_BRDFTex, tmpvar_7);
  c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_5 * xlv_TEXCOORD3));
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

varying highp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
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

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  highp vec4 tmpvar_3;
  mediump vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  highp vec3 SHLighting;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = (tmpvar_9 * tmpvar_2);
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  SHLighting = tmpvar_11;
  highp vec3 tmpvar_21;
  tmpvar_21 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_8 = tmpvar_21;
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_22;
  tmpvar_22 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  mat3 tmpvar_23;
  tmpvar_23[0] = _Object2World[0].xyz;
  tmpvar_23[1] = _Object2World[1].xyz;
  tmpvar_23[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_24;
  tmpvar_24 = (tmpvar_23 * (tmpvar_2 * unity_Scale.w));
  highp mat3 tmpvar_25;
  tmpvar_25[0] = tmpvar_1.xyz;
  tmpvar_25[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_25[2] = tmpvar_2;
  mat3 tmpvar_26;
  tmpvar_26[0].x = tmpvar_25[0].x;
  tmpvar_26[0].y = tmpvar_25[1].x;
  tmpvar_26[0].z = tmpvar_25[2].x;
  tmpvar_26[1].x = tmpvar_25[0].y;
  tmpvar_26[1].y = tmpvar_25[1].y;
  tmpvar_26[1].z = tmpvar_25[2].y;
  tmpvar_26[2].x = tmpvar_25[0].z;
  tmpvar_26[2].y = tmpvar_25[1].z;
  tmpvar_26[2].z = tmpvar_25[2].z;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_26 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_27;
  highp vec4 tmpvar_28;
  tmpvar_28.w = 1.0;
  tmpvar_28.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_29;
  tmpvar_29 = normalize ((tmpvar_26 * (((_World2Object * tmpvar_28).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_7 = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_31;
  tmpvar_31 = (unity_4LightPosX0 - tmpvar_30.x);
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosY0 - tmpvar_30.y);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosZ0 - tmpvar_30.z);
  highp vec4 tmpvar_34;
  tmpvar_34 = (((tmpvar_31 * tmpvar_31) + (tmpvar_32 * tmpvar_32)) + (tmpvar_33 * tmpvar_33));
  highp vec4 tmpvar_35;
  tmpvar_35 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_31 * tmpvar_24.x) + (tmpvar_32 * tmpvar_24.y)) + (tmpvar_33 * tmpvar_24.z)) * inversesqrt (tmpvar_34))) * (1.0/((1.0 + (tmpvar_34 * unity_4LightAtten0)))));
  highp vec3 tmpvar_36;
  tmpvar_36 = ((((unity_LightColor[0].xyz * tmpvar_35.x) + (unity_LightColor[1].xyz * tmpvar_35.y)) + (unity_LightColor[2].xyz * tmpvar_35.z)) + (unity_LightColor[3].xyz * tmpvar_35.w));
  tmpvar_6 = tmpvar_36;
  highp vec4 o_i0_i1;
  highp vec4 tmpvar_37;
  tmpvar_37 = (tmpvar_22 * 0.5);
  o_i0_i1 = tmpvar_37;
  highp vec2 tmpvar_38;
  tmpvar_38.x = tmpvar_37.x;
  tmpvar_38.y = (tmpvar_37.y * _ProjectionParams.x);
  o_i0_i1.xy = (tmpvar_38 + tmpvar_37.w);
  o_i0_i1.zw = tmpvar_22.zw;
  gl_Position = tmpvar_22;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = o_i0_i1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_7;
  tmpvar_7.x = ((dot (tmpvar_6, xlv_TEXCOORD2) * 0.5) + 0.5);
  tmpvar_7.y = dot (tmpvar_6, normalize ((xlv_TEXCOORD2 + xlv_TEXCOORD4)));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_BRDFTex, tmpvar_7);
  c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_5 * xlv_TEXCOORD3));
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
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
}
 }
}
Fallback "Diffuse"
}
Shader "Reflective/Bumped Specular" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
 _Cube ("Reflection Cubemap", CUBE) = "" { TexGen CubeReflect }
 _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader { 
 LOD 400
 Tags { "RenderType"="Opaque" }
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

varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
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
  highp vec3 shlight;
  highp vec4 tmpvar_3;
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (_glesVertex.xyz - ((_World2Object * tmpvar_9).xyz * unity_Scale.w)));
  highp mat3 tmpvar_12;
  tmpvar_12[0] = tmpvar_1.xyz;
  tmpvar_12[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_12[2] = tmpvar_2;
  mat3 tmpvar_13;
  tmpvar_13[0].x = tmpvar_12[0].x;
  tmpvar_13[0].y = tmpvar_12[1].x;
  tmpvar_13[0].z = tmpvar_12[2].x;
  tmpvar_13[1].x = tmpvar_12[0].y;
  tmpvar_13[1].y = tmpvar_12[1].y;
  tmpvar_13[1].z = tmpvar_12[2].y;
  tmpvar_13[2].x = tmpvar_12[0].z;
  tmpvar_13[2].y = tmpvar_12[1].z;
  tmpvar_13[2].z = tmpvar_12[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_14;
  tmpvar_14.xyz = (tmpvar_13 * v_i0_i1.xyz);
  tmpvar_14.w = tmpvar_11.x;
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * unity_Scale.w);
  tmpvar_4 = tmpvar_15;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (tmpvar_13 * v_i0_i1_i2.xyz);
  tmpvar_16.w = tmpvar_11.y;
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * unity_Scale.w);
  tmpvar_5 = tmpvar_17;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_18;
  tmpvar_18.xyz = (tmpvar_13 * v_i0_i1_i2_i3.xyz);
  tmpvar_18.w = tmpvar_11.z;
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * unity_Scale.w);
  tmpvar_6 = tmpvar_19;
  mat3 tmpvar_20;
  tmpvar_20[0] = _Object2World[0].xyz;
  tmpvar_20[1] = _Object2World[1].xyz;
  tmpvar_20[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_21;
  tmpvar_21 = (tmpvar_13 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_7 = tmpvar_21;
  highp vec4 tmpvar_22;
  tmpvar_22.w = 1.0;
  tmpvar_22.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_23;
  tmpvar_23.w = 1.0;
  tmpvar_23.xyz = (tmpvar_20 * (tmpvar_2 * unity_Scale.w));
  mediump vec3 tmpvar_24;
  mediump vec4 normal;
  normal = tmpvar_23;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAr, normal);
  x1.x = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAg, normal);
  x1.y = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHAb, normal);
  x1.z = tmpvar_27;
  mediump vec4 tmpvar_28;
  tmpvar_28 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBr, tmpvar_28);
  x2.x = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBg, tmpvar_28);
  x2.y = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = dot (unity_SHBb, tmpvar_28);
  x2.z = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_32;
  highp vec3 tmpvar_33;
  tmpvar_33 = (unity_SHC.xyz * vC);
  x3 = tmpvar_33;
  tmpvar_24 = ((x1 + x2) + x3);
  shlight = tmpvar_24;
  tmpvar_8 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
  xlv_TEXCOORD6 = (tmpvar_13 * (((_World2Object * tmpvar_22).xyz * unity_Scale.w) - _glesVertex.xyz));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  lowp float tmpvar_17;
  tmpvar_17 = (tmpvar_16.w * _ReflectColor.w);
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize (xlv_TEXCOORD6);
  mediump vec3 viewDir;
  viewDir = tmpvar_18;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_19;
  tmpvar_19 = max (0.0, dot (tmpvar_13, xlv_TEXCOORD4));
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (tmpvar_13, normalize ((xlv_TEXCOORD4 + viewDir))));
  nh = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (pow (nh, (_Shininess * 128.0)) * tmpvar_11.w);
  highp vec3 tmpvar_22;
  tmpvar_22 = ((((tmpvar_12.xyz * _LightColor0.xyz) * tmpvar_19) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_21)) * 2.0);
  c_i0_i1.xyz = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_17 + ((_LightColor0.w * _SpecColor.w) * tmpvar_21));
  c_i0_i1.w = tmpvar_23;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_12.xyz * xlv_TEXCOORD5));
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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

varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_glesVertex.xyz - ((_World2Object * tmpvar_7).xyz * unity_Scale.w)));
  highp mat3 tmpvar_10;
  tmpvar_10[0] = tmpvar_1.xyz;
  tmpvar_10[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_10[2] = tmpvar_2;
  mat3 tmpvar_11;
  tmpvar_11[0].x = tmpvar_10[0].x;
  tmpvar_11[0].y = tmpvar_10[1].x;
  tmpvar_11[0].z = tmpvar_10[2].x;
  tmpvar_11[1].x = tmpvar_10[0].y;
  tmpvar_11[1].y = tmpvar_10[1].y;
  tmpvar_11[1].z = tmpvar_10[2].y;
  tmpvar_11[2].x = tmpvar_10[0].z;
  tmpvar_11[2].y = tmpvar_10[1].z;
  tmpvar_11[2].z = tmpvar_10[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_12;
  tmpvar_12.xyz = (tmpvar_11 * v_i0_i1.xyz);
  tmpvar_12.w = tmpvar_9.x;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * unity_Scale.w);
  tmpvar_4 = tmpvar_13;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_14;
  tmpvar_14.xyz = (tmpvar_11 * v_i0_i1_i2.xyz);
  tmpvar_14.w = tmpvar_9.y;
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * unity_Scale.w);
  tmpvar_5 = tmpvar_15;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (tmpvar_11 * v_i0_i1_i2_i3.xyz);
  tmpvar_16.w = tmpvar_9.z;
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * unity_Scale.w);
  tmpvar_6 = tmpvar_17;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec3 tmpvar_12;
  tmpvar_12 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_2, tmpvar_12);
  tmpvar_13.y = dot (tmpvar_3, tmpvar_12);
  tmpvar_13.z = dot (tmpvar_4, tmpvar_12);
  highp vec3 tmpvar_14;
  tmpvar_14 = reflect (tmpvar_1, tmpvar_13);
  lowp vec4 tmpvar_15;
  tmpvar_15 = (textureCube (_Cube, tmpvar_14) * tmpvar_11.w);
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = ((tmpvar_11 * _Color).xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz));
  c.w = (tmpvar_15.w * _ReflectColor.w);
  c.xyz = (c.xyz + (tmpvar_15.xyz * _ReflectColor.xyz));
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

varying highp vec3 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_glesVertex.xyz - ((_World2Object * tmpvar_7).xyz * unity_Scale.w)));
  highp mat3 tmpvar_10;
  tmpvar_10[0] = tmpvar_1.xyz;
  tmpvar_10[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_10[2] = tmpvar_2;
  mat3 tmpvar_11;
  tmpvar_11[0].x = tmpvar_10[0].x;
  tmpvar_11[0].y = tmpvar_10[1].x;
  tmpvar_11[0].z = tmpvar_10[2].x;
  tmpvar_11[1].x = tmpvar_10[0].y;
  tmpvar_11[1].y = tmpvar_10[1].y;
  tmpvar_11[1].z = tmpvar_10[2].y;
  tmpvar_11[2].x = tmpvar_10[0].z;
  tmpvar_11[2].y = tmpvar_10[1].z;
  tmpvar_11[2].z = tmpvar_10[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_12;
  tmpvar_12.xyz = (tmpvar_11 * v_i0_i1.xyz);
  tmpvar_12.w = tmpvar_9.x;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * unity_Scale.w);
  tmpvar_4 = tmpvar_13;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_14;
  tmpvar_14.xyz = (tmpvar_11 * v_i0_i1_i2.xyz);
  tmpvar_14.w = tmpvar_9.y;
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * unity_Scale.w);
  tmpvar_5 = tmpvar_15;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (tmpvar_11 * v_i0_i1_i2_i3.xyz);
  tmpvar_16.w = tmpvar_9.z;
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * unity_Scale.w);
  tmpvar_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = _WorldSpaceCameraPos;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (tmpvar_11 * (((_World2Object * tmpvar_18).xyz * unity_Scale.w) - _glesVertex.xyz));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_LightmapInd;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  c = vec4(0.0, 0.0, 0.0, 0.0);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize (xlv_TEXCOORD5);
  mediump vec4 tmpvar_18;
  mediump vec3 viewDir;
  viewDir = tmpvar_17;
  mediump vec3 specColor_i0;
  highp float nh;
  mediump vec3 normal;
  normal = tmpvar_13;
  mediump vec3 scalePerBasisVector_i0;
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_19;
  tmpvar_19 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz);
  lm_i0 = tmpvar_19;
  lowp vec3 tmpvar_20;
  tmpvar_20 = (2.0 * texture2D (unity_LightmapInd, xlv_TEXCOORD4).xyz);
  scalePerBasisVector_i0 = tmpvar_20;
  lm_i0 = (lm_i0 * dot (clamp ((mat3(0.816497, -0.408248, -0.408248, 0.0, 0.707107, -0.707107, 0.57735, 0.57735, 0.57735) * normal), 0.0, 1.0), scalePerBasisVector_i0));
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (tmpvar_13, normalize ((normalize ((((scalePerBasisVector_i0.x * vec3(0.816497, 0.0, 0.57735)) + (scalePerBasisVector_i0.y * vec3(-0.408248, 0.707107, 0.57735))) + (scalePerBasisVector_i0.z * vec3(-0.408248, -0.707107, 0.57735)))) + viewDir))));
  nh = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = pow (nh, (_Shininess * 128.0));
  highp vec3 tmpvar_23;
  tmpvar_23 = (((lm_i0 * _SpecColor.xyz) * tmpvar_11.w) * tmpvar_22);
  specColor_i0 = tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.xyz = lm_i0;
  tmpvar_24.w = tmpvar_22;
  tmpvar_18 = tmpvar_24;
  c.xyz = specColor_i0;
  mediump vec3 tmpvar_25;
  tmpvar_25 = (c.xyz + (tmpvar_12.xyz * tmpvar_18.xyz));
  c.xyz = tmpvar_25;
  c.w = (tmpvar_16.w * _ReflectColor.w);
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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

varying highp vec4 xlv_TEXCOORD7;
varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
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
  highp vec3 shlight;
  highp vec4 tmpvar_3;
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_11;
  tmpvar_11[0] = _Object2World[0].xyz;
  tmpvar_11[1] = _Object2World[1].xyz;
  tmpvar_11[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_glesVertex.xyz - ((_World2Object * tmpvar_10).xyz * unity_Scale.w)));
  highp mat3 tmpvar_13;
  tmpvar_13[0] = tmpvar_1.xyz;
  tmpvar_13[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_13[2] = tmpvar_2;
  mat3 tmpvar_14;
  tmpvar_14[0].x = tmpvar_13[0].x;
  tmpvar_14[0].y = tmpvar_13[1].x;
  tmpvar_14[0].z = tmpvar_13[2].x;
  tmpvar_14[1].x = tmpvar_13[0].y;
  tmpvar_14[1].y = tmpvar_13[1].y;
  tmpvar_14[1].z = tmpvar_13[2].y;
  tmpvar_14[2].x = tmpvar_13[0].z;
  tmpvar_14[2].y = tmpvar_13[1].z;
  tmpvar_14[2].z = tmpvar_13[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_15;
  tmpvar_15.xyz = (tmpvar_14 * v_i0_i1.xyz);
  tmpvar_15.w = tmpvar_12.x;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * unity_Scale.w);
  tmpvar_4 = tmpvar_16;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (tmpvar_14 * v_i0_i1_i2.xyz);
  tmpvar_17.w = tmpvar_12.y;
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * unity_Scale.w);
  tmpvar_5 = tmpvar_18;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_19;
  tmpvar_19.xyz = (tmpvar_14 * v_i0_i1_i2_i3.xyz);
  tmpvar_19.w = tmpvar_12.z;
  highp vec4 tmpvar_20;
  tmpvar_20 = (tmpvar_19 * unity_Scale.w);
  tmpvar_6 = tmpvar_20;
  mat3 tmpvar_21;
  tmpvar_21[0] = _Object2World[0].xyz;
  tmpvar_21[1] = _Object2World[1].xyz;
  tmpvar_21[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_22;
  tmpvar_22 = (tmpvar_14 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_7 = tmpvar_22;
  highp vec4 tmpvar_23;
  tmpvar_23.w = 1.0;
  tmpvar_23.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = (tmpvar_21 * (tmpvar_2 * unity_Scale.w));
  mediump vec3 tmpvar_25;
  mediump vec4 normal;
  normal = tmpvar_24;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAr, normal);
  x1.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHAg, normal);
  x1.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHAb, normal);
  x1.z = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBr, tmpvar_29);
  x2.x = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = dot (unity_SHBg, tmpvar_29);
  x2.y = tmpvar_31;
  highp float tmpvar_32;
  tmpvar_32 = dot (unity_SHBb, tmpvar_29);
  x2.z = tmpvar_32;
  mediump float tmpvar_33;
  tmpvar_33 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (unity_SHC.xyz * vC);
  x3 = tmpvar_34;
  tmpvar_25 = ((x1 + x2) + x3);
  shlight = tmpvar_25;
  tmpvar_8 = shlight;
  highp vec4 o_i0;
  highp vec4 tmpvar_35;
  tmpvar_35 = (tmpvar_9 * 0.5);
  o_i0 = tmpvar_35;
  highp vec2 tmpvar_36;
  tmpvar_36.x = tmpvar_35.x;
  tmpvar_36.y = (tmpvar_35.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_36 + tmpvar_35.w);
  o_i0.zw = tmpvar_9.zw;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
  xlv_TEXCOORD6 = (tmpvar_14 * (((_World2Object * tmpvar_23).xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD7 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD7;
varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform sampler2D _ShadowMapTexture;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  lowp float tmpvar_17;
  tmpvar_17 = (tmpvar_16.w * _ReflectColor.w);
  lowp vec4 tmpvar_18;
  tmpvar_18 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD7);
  highp vec3 tmpvar_19;
  tmpvar_19 = normalize (xlv_TEXCOORD6);
  mediump vec3 viewDir;
  viewDir = tmpvar_19;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_20;
  tmpvar_20 = max (0.0, dot (tmpvar_13, xlv_TEXCOORD4));
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (tmpvar_13, normalize ((xlv_TEXCOORD4 + viewDir))));
  nh = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (pow (nh, (_Shininess * 128.0)) * tmpvar_11.w);
  highp vec3 tmpvar_23;
  tmpvar_23 = ((((tmpvar_12.xyz * _LightColor0.xyz) * tmpvar_20) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_22)) * (tmpvar_18.x * 2.0));
  c_i0_i1.xyz = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = (tmpvar_17 + (((_LightColor0.w * _SpecColor.w) * tmpvar_22) * tmpvar_18.x));
  c_i0_i1.w = tmpvar_24;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_12.xyz * xlv_TEXCOORD5));
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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

varying highp vec4 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (_glesVertex.xyz - ((_World2Object * tmpvar_8).xyz * unity_Scale.w)));
  highp mat3 tmpvar_11;
  tmpvar_11[0] = tmpvar_1.xyz;
  tmpvar_11[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_11[2] = tmpvar_2;
  mat3 tmpvar_12;
  tmpvar_12[0].x = tmpvar_11[0].x;
  tmpvar_12[0].y = tmpvar_11[1].x;
  tmpvar_12[0].z = tmpvar_11[2].x;
  tmpvar_12[1].x = tmpvar_11[0].y;
  tmpvar_12[1].y = tmpvar_11[1].y;
  tmpvar_12[1].z = tmpvar_11[2].y;
  tmpvar_12[2].x = tmpvar_11[0].z;
  tmpvar_12[2].y = tmpvar_11[1].z;
  tmpvar_12[2].z = tmpvar_11[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_13;
  tmpvar_13.xyz = (tmpvar_12 * v_i0_i1.xyz);
  tmpvar_13.w = tmpvar_10.x;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * unity_Scale.w);
  tmpvar_4 = tmpvar_14;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_15;
  tmpvar_15.xyz = (tmpvar_12 * v_i0_i1_i2.xyz);
  tmpvar_15.w = tmpvar_10.y;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * unity_Scale.w);
  tmpvar_5 = tmpvar_16;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (tmpvar_12 * v_i0_i1_i2_i3.xyz);
  tmpvar_17.w = tmpvar_10.z;
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * unity_Scale.w);
  tmpvar_6 = tmpvar_18;
  highp vec4 o_i0;
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_7 * 0.5);
  o_i0 = tmpvar_19;
  highp vec2 tmpvar_20;
  tmpvar_20.x = tmpvar_19.x;
  tmpvar_20.y = (tmpvar_19.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_20 + tmpvar_19.w);
  o_i0.zw = tmpvar_7.zw;
  gl_Position = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _ShadowMapTexture;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec3 tmpvar_12;
  tmpvar_12 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_2, tmpvar_12);
  tmpvar_13.y = dot (tmpvar_3, tmpvar_12);
  tmpvar_13.z = dot (tmpvar_4, tmpvar_12);
  highp vec3 tmpvar_14;
  tmpvar_14 = reflect (tmpvar_1, tmpvar_13);
  lowp vec4 tmpvar_15;
  tmpvar_15 = (textureCube (_Cube, tmpvar_14) * tmpvar_11.w);
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = ((tmpvar_11 * _Color).xyz * min ((2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz), vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD5).x * 2.0))));
  c.w = (tmpvar_15.w * _ReflectColor.w);
  c.xyz = (c.xyz + (tmpvar_15.xyz * _ReflectColor.xyz));
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

varying highp vec4 xlv_TEXCOORD6;
varying highp vec3 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _BumpMap_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord1;
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
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_9;
  tmpvar_9[0] = _Object2World[0].xyz;
  tmpvar_9[1] = _Object2World[1].xyz;
  tmpvar_9[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * (_glesVertex.xyz - ((_World2Object * tmpvar_8).xyz * unity_Scale.w)));
  highp mat3 tmpvar_11;
  tmpvar_11[0] = tmpvar_1.xyz;
  tmpvar_11[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_11[2] = tmpvar_2;
  mat3 tmpvar_12;
  tmpvar_12[0].x = tmpvar_11[0].x;
  tmpvar_12[0].y = tmpvar_11[1].x;
  tmpvar_12[0].z = tmpvar_11[2].x;
  tmpvar_12[1].x = tmpvar_11[0].y;
  tmpvar_12[1].y = tmpvar_11[1].y;
  tmpvar_12[1].z = tmpvar_11[2].y;
  tmpvar_12[2].x = tmpvar_11[0].z;
  tmpvar_12[2].y = tmpvar_11[1].z;
  tmpvar_12[2].z = tmpvar_11[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_13;
  tmpvar_13.xyz = (tmpvar_12 * v_i0_i1.xyz);
  tmpvar_13.w = tmpvar_10.x;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * unity_Scale.w);
  tmpvar_4 = tmpvar_14;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_15;
  tmpvar_15.xyz = (tmpvar_12 * v_i0_i1_i2.xyz);
  tmpvar_15.w = tmpvar_10.y;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * unity_Scale.w);
  tmpvar_5 = tmpvar_16;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (tmpvar_12 * v_i0_i1_i2_i3.xyz);
  tmpvar_17.w = tmpvar_10.z;
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * unity_Scale.w);
  tmpvar_6 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = _WorldSpaceCameraPos;
  highp vec4 o_i0;
  highp vec4 tmpvar_20;
  tmpvar_20 = (tmpvar_7 * 0.5);
  o_i0 = tmpvar_20;
  highp vec2 tmpvar_21;
  tmpvar_21.x = tmpvar_20.x;
  tmpvar_21.y = (tmpvar_20.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_21 + tmpvar_20.w);
  o_i0.zw = tmpvar_7.zw;
  gl_Position = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (tmpvar_12 * (((_World2Object * tmpvar_19).xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD6 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD6;
varying highp vec3 xlv_TEXCOORD5;
varying highp vec2 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_LightmapInd;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform sampler2D _ShadowMapTexture;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  c = vec4(0.0, 0.0, 0.0, 0.0);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize (xlv_TEXCOORD5);
  mediump vec4 tmpvar_18;
  mediump vec3 viewDir;
  viewDir = tmpvar_17;
  mediump vec3 specColor_i0;
  highp float nh;
  mediump vec3 normal;
  normal = tmpvar_13;
  mediump vec3 scalePerBasisVector_i0;
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_19;
  tmpvar_19 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD4).xyz);
  lm_i0 = tmpvar_19;
  lowp vec3 tmpvar_20;
  tmpvar_20 = (2.0 * texture2D (unity_LightmapInd, xlv_TEXCOORD4).xyz);
  scalePerBasisVector_i0 = tmpvar_20;
  lm_i0 = (lm_i0 * dot (clamp ((mat3(0.816497, -0.408248, -0.408248, 0.0, 0.707107, -0.707107, 0.57735, 0.57735, 0.57735) * normal), 0.0, 1.0), scalePerBasisVector_i0));
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (tmpvar_13, normalize ((normalize ((((scalePerBasisVector_i0.x * vec3(0.816497, 0.0, 0.57735)) + (scalePerBasisVector_i0.y * vec3(-0.408248, 0.707107, 0.57735))) + (scalePerBasisVector_i0.z * vec3(-0.408248, -0.707107, 0.57735)))) + viewDir))));
  nh = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = pow (nh, (_Shininess * 128.0));
  highp vec3 tmpvar_23;
  tmpvar_23 = (((lm_i0 * _SpecColor.xyz) * tmpvar_11.w) * tmpvar_22);
  specColor_i0 = tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.xyz = lm_i0;
  tmpvar_24.w = tmpvar_22;
  tmpvar_18 = tmpvar_24;
  c.xyz = specColor_i0;
  lowp vec3 tmpvar_25;
  tmpvar_25 = vec3((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD6).x * 2.0));
  mediump vec3 tmpvar_26;
  tmpvar_26 = (c.xyz + (tmpvar_12.xyz * min (tmpvar_18.xyz, tmpvar_25)));
  c.xyz = tmpvar_26;
  c.w = (tmpvar_16.w * _ReflectColor.w);
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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

varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
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
  highp vec3 shlight;
  highp vec4 tmpvar_3;
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * (_glesVertex.xyz - ((_World2Object * tmpvar_9).xyz * unity_Scale.w)));
  highp mat3 tmpvar_12;
  tmpvar_12[0] = tmpvar_1.xyz;
  tmpvar_12[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_12[2] = tmpvar_2;
  mat3 tmpvar_13;
  tmpvar_13[0].x = tmpvar_12[0].x;
  tmpvar_13[0].y = tmpvar_12[1].x;
  tmpvar_13[0].z = tmpvar_12[2].x;
  tmpvar_13[1].x = tmpvar_12[0].y;
  tmpvar_13[1].y = tmpvar_12[1].y;
  tmpvar_13[1].z = tmpvar_12[2].y;
  tmpvar_13[2].x = tmpvar_12[0].z;
  tmpvar_13[2].y = tmpvar_12[1].z;
  tmpvar_13[2].z = tmpvar_12[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_14;
  tmpvar_14.xyz = (tmpvar_13 * v_i0_i1.xyz);
  tmpvar_14.w = tmpvar_11.x;
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * unity_Scale.w);
  tmpvar_4 = tmpvar_15;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (tmpvar_13 * v_i0_i1_i2.xyz);
  tmpvar_16.w = tmpvar_11.y;
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * unity_Scale.w);
  tmpvar_5 = tmpvar_17;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_18;
  tmpvar_18.xyz = (tmpvar_13 * v_i0_i1_i2_i3.xyz);
  tmpvar_18.w = tmpvar_11.z;
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * unity_Scale.w);
  tmpvar_6 = tmpvar_19;
  mat3 tmpvar_20;
  tmpvar_20[0] = _Object2World[0].xyz;
  tmpvar_20[1] = _Object2World[1].xyz;
  tmpvar_20[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_21;
  tmpvar_21 = (tmpvar_20 * (tmpvar_2 * unity_Scale.w));
  highp vec3 tmpvar_22;
  tmpvar_22 = (tmpvar_13 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_7 = tmpvar_22;
  highp vec4 tmpvar_23;
  tmpvar_23.w = 1.0;
  tmpvar_23.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_21;
  mediump vec3 tmpvar_25;
  mediump vec4 normal;
  normal = tmpvar_24;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAr, normal);
  x1.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHAg, normal);
  x1.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHAb, normal);
  x1.z = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBr, tmpvar_29);
  x2.x = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = dot (unity_SHBg, tmpvar_29);
  x2.y = tmpvar_31;
  highp float tmpvar_32;
  tmpvar_32 = dot (unity_SHBb, tmpvar_29);
  x2.z = tmpvar_32;
  mediump float tmpvar_33;
  tmpvar_33 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (unity_SHC.xyz * vC);
  x3 = tmpvar_34;
  tmpvar_25 = ((x1 + x2) + x3);
  shlight = tmpvar_25;
  tmpvar_8 = shlight;
  highp vec3 tmpvar_35;
  tmpvar_35 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_36;
  tmpvar_36 = (unity_4LightPosX0 - tmpvar_35.x);
  highp vec4 tmpvar_37;
  tmpvar_37 = (unity_4LightPosY0 - tmpvar_35.y);
  highp vec4 tmpvar_38;
  tmpvar_38 = (unity_4LightPosZ0 - tmpvar_35.z);
  highp vec4 tmpvar_39;
  tmpvar_39 = (((tmpvar_36 * tmpvar_36) + (tmpvar_37 * tmpvar_37)) + (tmpvar_38 * tmpvar_38));
  highp vec4 tmpvar_40;
  tmpvar_40 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_36 * tmpvar_21.x) + (tmpvar_37 * tmpvar_21.y)) + (tmpvar_38 * tmpvar_21.z)) * inversesqrt (tmpvar_39))) * (1.0/((1.0 + (tmpvar_39 * unity_4LightAtten0)))));
  highp vec3 tmpvar_41;
  tmpvar_41 = (tmpvar_8 + ((((unity_LightColor[0].xyz * tmpvar_40.x) + (unity_LightColor[1].xyz * tmpvar_40.y)) + (unity_LightColor[2].xyz * tmpvar_40.z)) + (unity_LightColor[3].xyz * tmpvar_40.w)));
  tmpvar_8 = tmpvar_41;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
  xlv_TEXCOORD6 = (tmpvar_13 * (((_World2Object * tmpvar_23).xyz * unity_Scale.w) - _glesVertex.xyz));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  lowp float tmpvar_17;
  tmpvar_17 = (tmpvar_16.w * _ReflectColor.w);
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize (xlv_TEXCOORD6);
  mediump vec3 viewDir;
  viewDir = tmpvar_18;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_19;
  tmpvar_19 = max (0.0, dot (tmpvar_13, xlv_TEXCOORD4));
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (tmpvar_13, normalize ((xlv_TEXCOORD4 + viewDir))));
  nh = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (pow (nh, (_Shininess * 128.0)) * tmpvar_11.w);
  highp vec3 tmpvar_22;
  tmpvar_22 = ((((tmpvar_12.xyz * _LightColor0.xyz) * tmpvar_19) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_21)) * 2.0);
  c_i0_i1.xyz = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_17 + ((_LightColor0.w * _SpecColor.w) * tmpvar_21));
  c_i0_i1.w = tmpvar_23;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_12.xyz * xlv_TEXCOORD5));
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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

varying highp vec4 xlv_TEXCOORD7;
varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
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
  highp vec3 shlight;
  highp vec4 tmpvar_3;
  lowp vec4 tmpvar_4;
  lowp vec4 tmpvar_5;
  lowp vec4 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_11;
  tmpvar_11[0] = _Object2World[0].xyz;
  tmpvar_11[1] = _Object2World[1].xyz;
  tmpvar_11[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_glesVertex.xyz - ((_World2Object * tmpvar_10).xyz * unity_Scale.w)));
  highp mat3 tmpvar_13;
  tmpvar_13[0] = tmpvar_1.xyz;
  tmpvar_13[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_13[2] = tmpvar_2;
  mat3 tmpvar_14;
  tmpvar_14[0].x = tmpvar_13[0].x;
  tmpvar_14[0].y = tmpvar_13[1].x;
  tmpvar_14[0].z = tmpvar_13[2].x;
  tmpvar_14[1].x = tmpvar_13[0].y;
  tmpvar_14[1].y = tmpvar_13[1].y;
  tmpvar_14[1].z = tmpvar_13[2].y;
  tmpvar_14[2].x = tmpvar_13[0].z;
  tmpvar_14[2].y = tmpvar_13[1].z;
  tmpvar_14[2].z = tmpvar_13[2].z;
  vec4 v_i0_i1;
  v_i0_i1.x = _Object2World[0].x;
  v_i0_i1.y = _Object2World[1].x;
  v_i0_i1.z = _Object2World[2].x;
  v_i0_i1.w = _Object2World[3].x;
  highp vec4 tmpvar_15;
  tmpvar_15.xyz = (tmpvar_14 * v_i0_i1.xyz);
  tmpvar_15.w = tmpvar_12.x;
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * unity_Scale.w);
  tmpvar_4 = tmpvar_16;
  vec4 v_i0_i1_i2;
  v_i0_i1_i2.x = _Object2World[0].y;
  v_i0_i1_i2.y = _Object2World[1].y;
  v_i0_i1_i2.z = _Object2World[2].y;
  v_i0_i1_i2.w = _Object2World[3].y;
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (tmpvar_14 * v_i0_i1_i2.xyz);
  tmpvar_17.w = tmpvar_12.y;
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * unity_Scale.w);
  tmpvar_5 = tmpvar_18;
  vec4 v_i0_i1_i2_i3;
  v_i0_i1_i2_i3.x = _Object2World[0].z;
  v_i0_i1_i2_i3.y = _Object2World[1].z;
  v_i0_i1_i2_i3.z = _Object2World[2].z;
  v_i0_i1_i2_i3.w = _Object2World[3].z;
  highp vec4 tmpvar_19;
  tmpvar_19.xyz = (tmpvar_14 * v_i0_i1_i2_i3.xyz);
  tmpvar_19.w = tmpvar_12.z;
  highp vec4 tmpvar_20;
  tmpvar_20 = (tmpvar_19 * unity_Scale.w);
  tmpvar_6 = tmpvar_20;
  mat3 tmpvar_21;
  tmpvar_21[0] = _Object2World[0].xyz;
  tmpvar_21[1] = _Object2World[1].xyz;
  tmpvar_21[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_22;
  tmpvar_22 = (tmpvar_21 * (tmpvar_2 * unity_Scale.w));
  highp vec3 tmpvar_23;
  tmpvar_23 = (tmpvar_14 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_7 = tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_22;
  mediump vec3 tmpvar_26;
  mediump vec4 normal;
  normal = tmpvar_25;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHAr, normal);
  x1.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHAg, normal);
  x1.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHAb, normal);
  x1.z = tmpvar_29;
  mediump vec4 tmpvar_30;
  tmpvar_30 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_31;
  tmpvar_31 = dot (unity_SHBr, tmpvar_30);
  x2.x = tmpvar_31;
  highp float tmpvar_32;
  tmpvar_32 = dot (unity_SHBg, tmpvar_30);
  x2.y = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = dot (unity_SHBb, tmpvar_30);
  x2.z = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_34;
  highp vec3 tmpvar_35;
  tmpvar_35 = (unity_SHC.xyz * vC);
  x3 = tmpvar_35;
  tmpvar_26 = ((x1 + x2) + x3);
  shlight = tmpvar_26;
  tmpvar_8 = shlight;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_37;
  tmpvar_37 = (unity_4LightPosX0 - tmpvar_36.x);
  highp vec4 tmpvar_38;
  tmpvar_38 = (unity_4LightPosY0 - tmpvar_36.y);
  highp vec4 tmpvar_39;
  tmpvar_39 = (unity_4LightPosZ0 - tmpvar_36.z);
  highp vec4 tmpvar_40;
  tmpvar_40 = (((tmpvar_37 * tmpvar_37) + (tmpvar_38 * tmpvar_38)) + (tmpvar_39 * tmpvar_39));
  highp vec4 tmpvar_41;
  tmpvar_41 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_37 * tmpvar_22.x) + (tmpvar_38 * tmpvar_22.y)) + (tmpvar_39 * tmpvar_22.z)) * inversesqrt (tmpvar_40))) * (1.0/((1.0 + (tmpvar_40 * unity_4LightAtten0)))));
  highp vec3 tmpvar_42;
  tmpvar_42 = (tmpvar_8 + ((((unity_LightColor[0].xyz * tmpvar_41.x) + (unity_LightColor[1].xyz * tmpvar_41.y)) + (unity_LightColor[2].xyz * tmpvar_41.z)) + (unity_LightColor[3].xyz * tmpvar_41.w)));
  tmpvar_8 = tmpvar_42;
  highp vec4 o_i0;
  highp vec4 tmpvar_43;
  tmpvar_43 = (tmpvar_9 * 0.5);
  o_i0 = tmpvar_43;
  highp vec2 tmpvar_44;
  tmpvar_44.x = tmpvar_43.x;
  tmpvar_44.y = (tmpvar_43.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_44 + tmpvar_43.w);
  o_i0.zw = tmpvar_9.zw;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
  xlv_TEXCOORD6 = (tmpvar_14 * (((_World2Object * tmpvar_24).xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD7 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD7;
varying highp vec3 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec4 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform sampler2D _ShadowMapTexture;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  highp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_TEXCOORD0.zw;
  lowp vec3 tmpvar_7;
  tmpvar_7.x = xlv_TEXCOORD1.w;
  tmpvar_7.y = xlv_TEXCOORD2.w;
  tmpvar_7.z = xlv_TEXCOORD3.w;
  tmpvar_1 = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = xlv_TEXCOORD1.xyz;
  tmpvar_2 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = xlv_TEXCOORD2.xyz;
  tmpvar_3 = tmpvar_9;
  lowp vec3 tmpvar_10;
  tmpvar_10 = xlv_TEXCOORD3.xyz;
  tmpvar_4 = tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, tmpvar_5);
  lowp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * _Color);
  lowp vec3 tmpvar_13;
  tmpvar_13 = ((texture2D (_BumpMap, tmpvar_6).xyz * 2.0) - 1.0);
  mediump vec3 tmpvar_14;
  tmpvar_14.x = dot (tmpvar_2, tmpvar_13);
  tmpvar_14.y = dot (tmpvar_3, tmpvar_13);
  tmpvar_14.z = dot (tmpvar_4, tmpvar_13);
  highp vec3 tmpvar_15;
  tmpvar_15 = reflect (tmpvar_1, tmpvar_14);
  lowp vec4 tmpvar_16;
  tmpvar_16 = (textureCube (_Cube, tmpvar_15) * tmpvar_11.w);
  lowp float tmpvar_17;
  tmpvar_17 = (tmpvar_16.w * _ReflectColor.w);
  lowp vec4 tmpvar_18;
  tmpvar_18 = texture2DProj (_ShadowMapTexture, xlv_TEXCOORD7);
  highp vec3 tmpvar_19;
  tmpvar_19 = normalize (xlv_TEXCOORD6);
  mediump vec3 viewDir;
  viewDir = tmpvar_19;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_20;
  tmpvar_20 = max (0.0, dot (tmpvar_13, xlv_TEXCOORD4));
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (tmpvar_13, normalize ((xlv_TEXCOORD4 + viewDir))));
  nh = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (pow (nh, (_Shininess * 128.0)) * tmpvar_11.w);
  highp vec3 tmpvar_23;
  tmpvar_23 = ((((tmpvar_12.xyz * _LightColor0.xyz) * tmpvar_20) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_22)) * (tmpvar_18.x * 2.0));
  c_i0_i1.xyz = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = (tmpvar_17 + (((_LightColor0.w * _SpecColor.w) * tmpvar_22) * tmpvar_18.x));
  c_i0_i1.w = tmpvar_24;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_12.xyz * xlv_TEXCOORD5));
  c.xyz = (c.xyz + (tmpvar_16.xyz * _ReflectColor.xyz));
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
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
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
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
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
  mediump vec3 tmpvar_5;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (((_World2Object * _WorldSpaceLightPos0).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_5 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * _Color);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp float tmpvar_7;
  tmpvar_7 = ((textureCube (_Cube, tmpvar_1) * tmpvar_4.w).w * _ReflectColor.w);
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD1);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp float atten;
  atten = texture2D (_LightTexture0, tmpvar_9).w;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_10;
  tmpvar_10 = max (0.0, dot (tmpvar_6, lightDir));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (tmpvar_6, normalize ((lightDir + normalize (xlv_TEXCOORD2)))));
  nh = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (pow (nh, (_Shininess * 128.0)) * tmpvar_4.w);
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_5.xyz * _LightColor0.xyz) * tmpvar_10) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_12)) * (atten * 2.0));
  c_i0_i1.xyz = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = (tmpvar_7 + (((_LightColor0.w * _SpecColor.w) * tmpvar_12) * atten));
  c_i0_i1.w = tmpvar_14;
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
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _MainTex_ST;
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
  mediump vec3 tmpvar_5;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_5 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * _Color);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp float tmpvar_7;
  tmpvar_7 = ((textureCube (_Cube, tmpvar_1) * tmpvar_4.w).w * _ReflectColor.w);
  lightDir = xlv_TEXCOORD1;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_8;
  tmpvar_8 = max (0.0, dot (tmpvar_6, lightDir));
  mediump float tmpvar_9;
  tmpvar_9 = max (0.0, dot (tmpvar_6, normalize ((lightDir + normalize (xlv_TEXCOORD2)))));
  nh = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (pow (nh, (_Shininess * 128.0)) * tmpvar_4.w);
  highp vec3 tmpvar_11;
  tmpvar_11 = ((((tmpvar_5.xyz * _LightColor0.xyz) * tmpvar_8) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_10)) * 2.0);
  c_i0_i1.xyz = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (tmpvar_7 + ((_LightColor0.w * _SpecColor.w) * tmpvar_10));
  c_i0_i1.w = tmpvar_12;
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
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
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
  mediump vec3 tmpvar_5;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (((_World2Object * _WorldSpaceLightPos0).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_5 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * _Color);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp float tmpvar_7;
  tmpvar_7 = ((textureCube (_Cube, tmpvar_1) * tmpvar_4.w).w * _ReflectColor.w);
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD1);
  lightDir = tmpvar_8;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD3.xyz;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD3.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD3.xy / xlv_TEXCOORD3.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_9).w);
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_10;
  tmpvar_10 = max (0.0, dot (tmpvar_6, lightDir));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (tmpvar_6, normalize ((lightDir + normalize (xlv_TEXCOORD2)))));
  nh = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (pow (nh, (_Shininess * 128.0)) * tmpvar_4.w);
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_5.xyz * _LightColor0.xyz) * tmpvar_10) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_12)) * (atten * 2.0));
  c_i0_i1.xyz = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = (tmpvar_7 + (((_LightColor0.w * _SpecColor.w) * tmpvar_12) * atten));
  c_i0_i1.w = tmpvar_14;
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
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
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
  mediump vec3 tmpvar_5;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (((_World2Object * _WorldSpaceLightPos0).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_5 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * _Color);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp float tmpvar_7;
  tmpvar_7 = ((textureCube (_Cube, tmpvar_1) * tmpvar_4.w).w * _ReflectColor.w);
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD1);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD3, xlv_TEXCOORD3));
  lowp float atten;
  atten = (texture2D (_LightTextureB0, tmpvar_9).w * textureCube (_LightTexture0, xlv_TEXCOORD3).w);
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_10;
  tmpvar_10 = max (0.0, dot (tmpvar_6, lightDir));
  mediump float tmpvar_11;
  tmpvar_11 = max (0.0, dot (tmpvar_6, normalize ((lightDir + normalize (xlv_TEXCOORD2)))));
  nh = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (pow (nh, (_Shininess * 128.0)) * tmpvar_4.w);
  highp vec3 tmpvar_13;
  tmpvar_13 = ((((tmpvar_5.xyz * _LightColor0.xyz) * tmpvar_10) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_12)) * (atten * 2.0));
  c_i0_i1.xyz = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = (tmpvar_7 + (((_LightColor0.w * _SpecColor.w) * tmpvar_12) * atten));
  c_i0_i1.w = tmpvar_14;
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
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp mat4 _LightMatrix0;
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
  mediump vec3 tmpvar_5;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_5 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _MainTex;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  highp vec3 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.zw;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, tmpvar_2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (tmpvar_4 * _Color);
  lowp vec3 tmpvar_6;
  tmpvar_6 = ((texture2D (_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
  lowp float tmpvar_7;
  tmpvar_7 = ((textureCube (_Cube, tmpvar_1) * tmpvar_4.w).w * _ReflectColor.w);
  lightDir = xlv_TEXCOORD1;
  lowp float atten;
  atten = texture2D (_LightTexture0, xlv_TEXCOORD3).w;
  lowp vec4 c_i0_i1;
  highp float nh;
  lowp float tmpvar_8;
  tmpvar_8 = max (0.0, dot (tmpvar_6, lightDir));
  mediump float tmpvar_9;
  tmpvar_9 = max (0.0, dot (tmpvar_6, normalize ((lightDir + normalize (xlv_TEXCOORD2)))));
  nh = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = (pow (nh, (_Shininess * 128.0)) * tmpvar_4.w);
  highp vec3 tmpvar_11;
  tmpvar_11 = ((((tmpvar_5.xyz * _LightColor0.xyz) * tmpvar_8) + ((_LightColor0.xyz * _SpecColor.xyz) * tmpvar_10)) * (atten * 2.0));
  c_i0_i1.xyz = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (tmpvar_7 + (((_LightColor0.w * _SpecColor.w) * tmpvar_10) * atten));
  c_i0_i1.w = tmpvar_12;
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
}
Fallback "Reflective/Bumped Diffuse"
}
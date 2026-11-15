Shader "MADFINGER/Characters/BRDFLit FX (Supports Backlight)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
 _NoiseTex ("Noise tex", 2D) = "white" {}
 _LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
 _FXColor ("FXColor", Color) = (0,0.97,0.89,1)
 _TimeOffs ("Time offs", Float) = 0
 _Duration ("Duration", Float) = 2
 _Invert ("Invert", Float) = 0
}
SubShader { 
 LOD 400
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
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
uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp float _Invert;
uniform highp float _Duration;
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
  mediump float tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  lowp float tmpvar_9;
  highp vec3 SHLighting;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = (tmpvar_10 * tmpvar_2);
  mediump vec3 tmpvar_12;
  mediump vec4 normal;
  normal = tmpvar_11;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAr, normal);
  x1.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAg, normal);
  x1.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAb, normal);
  x1.z = tmpvar_15;
  mediump vec4 tmpvar_16;
  tmpvar_16 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBr, tmpvar_16);
  x2.x = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBg, tmpvar_16);
  x2.y = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBb, tmpvar_16);
  x2.z = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (unity_SHC.xyz * vC);
  x3 = tmpvar_21;
  tmpvar_12 = ((x1 + x2) + x3);
  SHLighting = tmpvar_12;
  highp float tmpvar_22;
  tmpvar_22 = fract (((_TimeOffs + _Time.y) / _Duration));
  highp vec3 tmpvar_23;
  tmpvar_23 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_8 = tmpvar_23;
  highp float tmpvar_24;
  if ((_Invert > 0.0)) {
    tmpvar_24 = (1.0 - tmpvar_22);
  } else {
    tmpvar_24 = tmpvar_22;
  };
  tmpvar_9 = tmpvar_24;
  tmpvar_4 = tmpvar_8;
  tmpvar_5 = tmpvar_9;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
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
  tmpvar_6 = tmpvar_27;
  highp vec4 tmpvar_28;
  tmpvar_28.w = 1.0;
  tmpvar_28.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_29;
  tmpvar_29 = normalize ((tmpvar_26 * (((_World2Object * tmpvar_28).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_7 = tmpvar_29;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = vec3(0.0, 0.0, 0.0);
  xlv_TEXCOORD5 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _NoiseTex;
uniform sampler2D _MainTex;
uniform lowp vec4 _FXColor;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp float tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_4;
  tmpvar_4 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  tmpvar_2 = xlv_TEXCOORD2;
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_NoiseTex, (xlv_TEXCOORD0.xy * 2.0));
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_3);
  float tmpvar_7;
  tmpvar_7 = (1.0 - clamp (((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
  lowp float tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  lowp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
  lowp vec3 tmpvar_10;
  tmpvar_10 = ((texture2D (_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_11;
  tmpvar_11.x = ((dot (tmpvar_10, xlv_TEXCOORD3) * 0.5) + 0.5);
  tmpvar_11.y = dot (tmpvar_10, normalize ((xlv_TEXCOORD3 + xlv_TEXCOORD5)));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_BRDFTex, tmpvar_11);
  c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
  c_i0.w = float((tmpvar_5.x > tmpvar_2));
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_9 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
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

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
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
uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp float _Invert;
uniform highp float _Duration;
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
  mediump float tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  lowp float tmpvar_9;
  highp vec3 SHLighting;
  mat3 tmpvar_10;
  tmpvar_10[0] = _Object2World[0].xyz;
  tmpvar_10[1] = _Object2World[1].xyz;
  tmpvar_10[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = (tmpvar_10 * tmpvar_2);
  mediump vec3 tmpvar_12;
  mediump vec4 normal;
  normal = tmpvar_11;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAr, normal);
  x1.x = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAg, normal);
  x1.y = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAb, normal);
  x1.z = tmpvar_15;
  mediump vec4 tmpvar_16;
  tmpvar_16 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBr, tmpvar_16);
  x2.x = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBg, tmpvar_16);
  x2.y = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBb, tmpvar_16);
  x2.z = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (unity_SHC.xyz * vC);
  x3 = tmpvar_21;
  tmpvar_12 = ((x1 + x2) + x3);
  SHLighting = tmpvar_12;
  highp float tmpvar_22;
  tmpvar_22 = fract (((_TimeOffs + _Time.y) / _Duration));
  highp vec3 tmpvar_23;
  tmpvar_23 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_8 = tmpvar_23;
  highp float tmpvar_24;
  if ((_Invert > 0.0)) {
    tmpvar_24 = (1.0 - tmpvar_22);
  } else {
    tmpvar_24 = tmpvar_22;
  };
  tmpvar_9 = tmpvar_24;
  tmpvar_4 = tmpvar_8;
  tmpvar_5 = tmpvar_9;
  highp vec4 tmpvar_25;
  tmpvar_25 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  highp mat3 tmpvar_26;
  tmpvar_26[0] = tmpvar_1.xyz;
  tmpvar_26[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_26[2] = tmpvar_2;
  mat3 tmpvar_27;
  tmpvar_27[0].x = tmpvar_26[0].x;
  tmpvar_27[0].y = tmpvar_26[1].x;
  tmpvar_27[0].z = tmpvar_26[2].x;
  tmpvar_27[1].x = tmpvar_26[0].y;
  tmpvar_27[1].y = tmpvar_26[1].y;
  tmpvar_27[1].z = tmpvar_26[2].y;
  tmpvar_27[2].x = tmpvar_26[0].z;
  tmpvar_27[2].y = tmpvar_26[1].z;
  tmpvar_27[2].z = tmpvar_26[2].z;
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_27 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_6 = tmpvar_28;
  highp vec4 tmpvar_29;
  tmpvar_29.w = 1.0;
  tmpvar_29.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_30;
  tmpvar_30 = normalize ((tmpvar_27 * (((_World2Object * tmpvar_29).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_7 = tmpvar_30;
  highp vec4 o_i0_i1;
  highp vec4 tmpvar_31;
  tmpvar_31 = (tmpvar_25 * 0.5);
  o_i0_i1 = tmpvar_31;
  highp vec2 tmpvar_32;
  tmpvar_32.x = tmpvar_31.x;
  tmpvar_32.y = (tmpvar_31.y * _ProjectionParams.x);
  o_i0_i1.xy = (tmpvar_32 + tmpvar_31.w);
  o_i0_i1.zw = tmpvar_25.zw;
  gl_Position = tmpvar_25;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = vec3(0.0, 0.0, 0.0);
  xlv_TEXCOORD5 = tmpvar_7;
  xlv_TEXCOORD6 = o_i0_i1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _NoiseTex;
uniform sampler2D _MainTex;
uniform lowp vec4 _FXColor;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp float tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_4;
  tmpvar_4 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  tmpvar_2 = xlv_TEXCOORD2;
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_NoiseTex, (xlv_TEXCOORD0.xy * 2.0));
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_3);
  float tmpvar_7;
  tmpvar_7 = (1.0 - clamp (((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
  lowp float tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  lowp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
  lowp vec3 tmpvar_10;
  tmpvar_10 = ((texture2D (_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_11;
  tmpvar_11.x = ((dot (tmpvar_10, xlv_TEXCOORD3) * 0.5) + 0.5);
  tmpvar_11.y = dot (tmpvar_10, normalize ((xlv_TEXCOORD3 + xlv_TEXCOORD5)));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_BRDFTex, tmpvar_11);
  c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
  c_i0.w = float((tmpvar_5.x > tmpvar_2));
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_9 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
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

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
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
uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp float _Invert;
uniform highp float _Duration;
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
  mediump float tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  lowp vec3 tmpvar_9;
  lowp float tmpvar_10;
  highp vec3 SHLighting;
  mat3 tmpvar_11;
  tmpvar_11[0] = _Object2World[0].xyz;
  tmpvar_11[1] = _Object2World[1].xyz;
  tmpvar_11[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = (tmpvar_11 * tmpvar_2);
  mediump vec3 tmpvar_13;
  mediump vec4 normal;
  normal = tmpvar_12;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAr, normal);
  x1.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAg, normal);
  x1.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHAb, normal);
  x1.z = tmpvar_16;
  mediump vec4 tmpvar_17;
  tmpvar_17 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBr, tmpvar_17);
  x2.x = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBg, tmpvar_17);
  x2.y = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = dot (unity_SHBb, tmpvar_17);
  x2.z = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_21;
  highp vec3 tmpvar_22;
  tmpvar_22 = (unity_SHC.xyz * vC);
  x3 = tmpvar_22;
  tmpvar_13 = ((x1 + x2) + x3);
  SHLighting = tmpvar_13;
  highp float tmpvar_23;
  tmpvar_23 = fract (((_TimeOffs + _Time.y) / _Duration));
  highp vec3 tmpvar_24;
  tmpvar_24 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_9 = tmpvar_24;
  highp float tmpvar_25;
  if ((_Invert > 0.0)) {
    tmpvar_25 = (1.0 - tmpvar_23);
  } else {
    tmpvar_25 = tmpvar_23;
  };
  tmpvar_10 = tmpvar_25;
  tmpvar_4 = tmpvar_9;
  tmpvar_5 = tmpvar_10;
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  mat3 tmpvar_26;
  tmpvar_26[0] = _Object2World[0].xyz;
  tmpvar_26[1] = _Object2World[1].xyz;
  tmpvar_26[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_26 * (tmpvar_2 * unity_Scale.w));
  highp mat3 tmpvar_28;
  tmpvar_28[0] = tmpvar_1.xyz;
  tmpvar_28[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_28[2] = tmpvar_2;
  mat3 tmpvar_29;
  tmpvar_29[0].x = tmpvar_28[0].x;
  tmpvar_29[0].y = tmpvar_28[1].x;
  tmpvar_29[0].z = tmpvar_28[2].x;
  tmpvar_29[1].x = tmpvar_28[0].y;
  tmpvar_29[1].y = tmpvar_28[1].y;
  tmpvar_29[1].z = tmpvar_28[2].y;
  tmpvar_29[2].x = tmpvar_28[0].z;
  tmpvar_29[2].y = tmpvar_28[1].z;
  tmpvar_29[2].z = tmpvar_28[2].z;
  highp vec3 tmpvar_30;
  tmpvar_30 = (tmpvar_29 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_6 = tmpvar_30;
  highp vec4 tmpvar_31;
  tmpvar_31.w = 1.0;
  tmpvar_31.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_32;
  tmpvar_32 = normalize ((tmpvar_29 * (((_World2Object * tmpvar_31).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_8 = tmpvar_32;
  highp vec3 tmpvar_33;
  tmpvar_33 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosX0 - tmpvar_33.x);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosY0 - tmpvar_33.y);
  highp vec4 tmpvar_36;
  tmpvar_36 = (unity_4LightPosZ0 - tmpvar_33.z);
  highp vec4 tmpvar_37;
  tmpvar_37 = (((tmpvar_34 * tmpvar_34) + (tmpvar_35 * tmpvar_35)) + (tmpvar_36 * tmpvar_36));
  highp vec4 tmpvar_38;
  tmpvar_38 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_34 * tmpvar_27.x) + (tmpvar_35 * tmpvar_27.y)) + (tmpvar_36 * tmpvar_27.z)) * inversesqrt (tmpvar_37))) * (1.0/((1.0 + (tmpvar_37 * unity_4LightAtten0)))));
  highp vec3 tmpvar_39;
  tmpvar_39 = ((((unity_LightColor[0].xyz * tmpvar_38.x) + (unity_LightColor[1].xyz * tmpvar_38.y)) + (unity_LightColor[2].xyz * tmpvar_38.z)) + (unity_LightColor[3].xyz * tmpvar_38.w));
  tmpvar_7 = tmpvar_39;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _NoiseTex;
uniform sampler2D _MainTex;
uniform lowp vec4 _FXColor;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp float tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_4;
  tmpvar_4 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  tmpvar_2 = xlv_TEXCOORD2;
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_NoiseTex, (xlv_TEXCOORD0.xy * 2.0));
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_3);
  float tmpvar_7;
  tmpvar_7 = (1.0 - clamp (((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
  lowp float tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  lowp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
  lowp vec3 tmpvar_10;
  tmpvar_10 = ((texture2D (_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_11;
  tmpvar_11.x = ((dot (tmpvar_10, xlv_TEXCOORD3) * 0.5) + 0.5);
  tmpvar_11.y = dot (tmpvar_10, normalize ((xlv_TEXCOORD3 + xlv_TEXCOORD5)));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_BRDFTex, tmpvar_11);
  c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
  c_i0.w = float((tmpvar_5.x > tmpvar_2));
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_9 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
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

varying highp vec4 xlv_TEXCOORD6;
varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
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
uniform highp float _TimeOffs;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
uniform highp float _LightProbesLightingAmount;
uniform highp float _Invert;
uniform highp float _Duration;
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
  mediump float tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec3 tmpvar_7;
  lowp vec3 tmpvar_8;
  lowp vec3 tmpvar_9;
  lowp float tmpvar_10;
  highp vec3 SHLighting;
  mat3 tmpvar_11;
  tmpvar_11[0] = _Object2World[0].xyz;
  tmpvar_11[1] = _Object2World[1].xyz;
  tmpvar_11[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = (tmpvar_11 * tmpvar_2);
  mediump vec3 tmpvar_13;
  mediump vec4 normal;
  normal = tmpvar_12;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAr, normal);
  x1.x = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = dot (unity_SHAg, normal);
  x1.y = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHAb, normal);
  x1.z = tmpvar_16;
  mediump vec4 tmpvar_17;
  tmpvar_17 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBr, tmpvar_17);
  x2.x = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (unity_SHBg, tmpvar_17);
  x2.y = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = dot (unity_SHBb, tmpvar_17);
  x2.z = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_21;
  highp vec3 tmpvar_22;
  tmpvar_22 = (unity_SHC.xyz * vC);
  x3 = tmpvar_22;
  tmpvar_13 = ((x1 + x2) + x3);
  SHLighting = tmpvar_13;
  highp float tmpvar_23;
  tmpvar_23 = fract (((_TimeOffs + _Time.y) / _Duration));
  highp vec3 tmpvar_24;
  tmpvar_24 = clamp ((SHLighting + vec3((1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
  tmpvar_9 = tmpvar_24;
  highp float tmpvar_25;
  if ((_Invert > 0.0)) {
    tmpvar_25 = (1.0 - tmpvar_23);
  } else {
    tmpvar_25 = tmpvar_23;
  };
  tmpvar_10 = tmpvar_25;
  tmpvar_4 = tmpvar_9;
  tmpvar_5 = tmpvar_10;
  highp vec4 tmpvar_26;
  tmpvar_26 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_3.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3.zw = ((_glesMultiTexCoord0.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
  mat3 tmpvar_27;
  tmpvar_27[0] = _Object2World[0].xyz;
  tmpvar_27[1] = _Object2World[1].xyz;
  tmpvar_27[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_27 * (tmpvar_2 * unity_Scale.w));
  highp mat3 tmpvar_29;
  tmpvar_29[0] = tmpvar_1.xyz;
  tmpvar_29[1] = (cross (tmpvar_2, tmpvar_1.xyz) * _glesTANGENT.w);
  tmpvar_29[2] = tmpvar_2;
  mat3 tmpvar_30;
  tmpvar_30[0].x = tmpvar_29[0].x;
  tmpvar_30[0].y = tmpvar_29[1].x;
  tmpvar_30[0].z = tmpvar_29[2].x;
  tmpvar_30[1].x = tmpvar_29[0].y;
  tmpvar_30[1].y = tmpvar_29[1].y;
  tmpvar_30[1].z = tmpvar_29[2].y;
  tmpvar_30[2].x = tmpvar_29[0].z;
  tmpvar_30[2].y = tmpvar_29[1].z;
  tmpvar_30[2].z = tmpvar_29[2].z;
  highp vec3 tmpvar_31;
  tmpvar_31 = (tmpvar_30 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_6 = tmpvar_31;
  highp vec4 tmpvar_32;
  tmpvar_32.w = 1.0;
  tmpvar_32.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize ((tmpvar_30 * (((_World2Object * tmpvar_32).xyz * unity_Scale.w) - _glesVertex.xyz)));
  tmpvar_8 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosX0 - tmpvar_34.x);
  highp vec4 tmpvar_36;
  tmpvar_36 = (unity_4LightPosY0 - tmpvar_34.y);
  highp vec4 tmpvar_37;
  tmpvar_37 = (unity_4LightPosZ0 - tmpvar_34.z);
  highp vec4 tmpvar_38;
  tmpvar_38 = (((tmpvar_35 * tmpvar_35) + (tmpvar_36 * tmpvar_36)) + (tmpvar_37 * tmpvar_37));
  highp vec4 tmpvar_39;
  tmpvar_39 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_35 * tmpvar_28.x) + (tmpvar_36 * tmpvar_28.y)) + (tmpvar_37 * tmpvar_28.z)) * inversesqrt (tmpvar_38))) * (1.0/((1.0 + (tmpvar_38 * unity_4LightAtten0)))));
  highp vec3 tmpvar_40;
  tmpvar_40 = ((((unity_LightColor[0].xyz * tmpvar_39.x) + (unity_LightColor[1].xyz * tmpvar_39.y)) + (unity_LightColor[2].xyz * tmpvar_39.z)) + (unity_LightColor[3].xyz * tmpvar_39.w));
  tmpvar_7 = tmpvar_40;
  highp vec4 o_i0_i1;
  highp vec4 tmpvar_41;
  tmpvar_41 = (tmpvar_26 * 0.5);
  o_i0_i1 = tmpvar_41;
  highp vec2 tmpvar_42;
  tmpvar_42.x = tmpvar_41.x;
  tmpvar_42.y = (tmpvar_41.y * _ProjectionParams.x);
  o_i0_i1.xy = (tmpvar_42 + tmpvar_41.w);
  o_i0_i1.zw = tmpvar_26.zw;
  gl_Position = tmpvar_26;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
  xlv_TEXCOORD6 = o_i0_i1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying mediump float xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _NoiseTex;
uniform sampler2D _MainTex;
uniform lowp vec4 _FXColor;
uniform sampler2D _BumpMap;
uniform sampler2D _BRDFTex;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp float tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = xlv_TEXCOORD0.xy;
  highp vec2 tmpvar_4;
  tmpvar_4 = xlv_TEXCOORD0.zw;
  tmpvar_1 = xlv_TEXCOORD1;
  tmpvar_2 = xlv_TEXCOORD2;
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_NoiseTex, (xlv_TEXCOORD0.xy * 2.0));
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, tmpvar_3);
  float tmpvar_7;
  tmpvar_7 = (1.0 - clamp (((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
  lowp float tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_7);
  lowp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
  lowp vec3 tmpvar_10;
  tmpvar_10 = ((texture2D (_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp vec2 tmpvar_11;
  tmpvar_11.x = ((dot (tmpvar_10, xlv_TEXCOORD3) * 0.5) + 0.5);
  tmpvar_11.y = dot (tmpvar_10, normalize ((xlv_TEXCOORD3 + xlv_TEXCOORD5)));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_BRDFTex, tmpvar_11);
  c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
  c_i0.w = float((tmpvar_5.x > tmpvar_2));
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_9 * xlv_TEXCOORD4));
  c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
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
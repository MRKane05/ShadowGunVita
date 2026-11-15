Shader "MADFINGER/Environment/Lightprobes with VirtualGloss Per-Vertex Additive" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecOffset ("Specular Offset from Camera", Vector) = (1,10,2,0)
 _SpecRange ("Specular Range", Float) = 20
 _SpecColor ("Specular Color", Color) = (1,1,1,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
 _SHLightingScale ("LightProbe influence scale", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;


uniform highp float _SpecRange;
uniform highp vec3 _SpecOffset;
uniform highp vec3 _SpecColor;
uniform highp float _Shininess;
uniform highp float _SHLightingScale;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  mat3 tmpvar_6;
  tmpvar_6[0] = gl_ModelViewMatrix[0].xyz;
  tmpvar_6[1] = gl_ModelViewMatrix[1].xyz;
  tmpvar_6[2] = gl_ModelViewMatrix[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = ((gl_ModelViewMatrix * _glesVertex).xyz - (_SpecOffset * vec3(1.0, 1.0, -1.0)));
  highp vec3 tmpvar_8;
  tmpvar_8 = (((_SpecColor * pow (clamp (dot ((tmpvar_6 * tmpvar_1), normalize (((vec3(0.0, 0.0, 1.0) + normalize (-(tmpvar_7))) * 0.5))), 0.0, 1.0), (_Shininess * 128.0))) * 2.0) * (1.0 - clamp ((length (tmpvar_7) / _SpecRange), 0.0, 1.0)));
  tmpvar_3 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = (tmpvar_5 * tmpvar_1);
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
  highp vec3 tmpvar_20;
  tmpvar_20 = (tmpvar_10 * _SHLightingScale);
  tmpvar_4 = tmpvar_20;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  c = tmpvar_1;
  c.xyz = (tmpvar_1.xyz * xlv_TEXCOORD4);
  c.xyz = (c.xyz + (xlv_TEXCOORD3 * tmpvar_1.w));
  gl_FragData[0] = c;
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
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;


uniform highp float _SpecRange;
uniform highp vec3 _SpecOffset;
uniform highp vec3 _SpecColor;
uniform highp float _Shininess;
uniform highp float _SHLightingScale;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  mat3 tmpvar_6;
  tmpvar_6[0] = gl_ModelViewMatrix[0].xyz;
  tmpvar_6[1] = gl_ModelViewMatrix[1].xyz;
  tmpvar_6[2] = gl_ModelViewMatrix[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = ((gl_ModelViewMatrix * _glesVertex).xyz - (_SpecOffset * vec3(1.0, 1.0, -1.0)));
  highp vec3 tmpvar_8;
  tmpvar_8 = (((_SpecColor * pow (clamp (dot ((tmpvar_6 * tmpvar_1), normalize (((vec3(0.0, 0.0, 1.0) + normalize (-(tmpvar_7))) * 0.5))), 0.0, 1.0), (_Shininess * 128.0))) * 2.0) * (1.0 - clamp ((length (tmpvar_7) / _SpecRange), 0.0, 1.0)));
  tmpvar_3 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = (tmpvar_5 * tmpvar_1);
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
  highp vec3 tmpvar_20;
  tmpvar_20 = (tmpvar_10 * _SHLightingScale);
  tmpvar_4 = tmpvar_20;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD3 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  c = tmpvar_1;
  c.xyz = (tmpvar_1.xyz * xlv_TEXCOORD4);
  c.xyz = (c.xyz + (xlv_TEXCOORD3 * tmpvar_1.w));
  gl_FragData[0] = c;
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
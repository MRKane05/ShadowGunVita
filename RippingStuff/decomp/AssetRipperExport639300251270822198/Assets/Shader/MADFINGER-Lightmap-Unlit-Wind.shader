Shader "MADFINGER/Environment/Lightmap + Wind" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _Wind ("Wind params", Vector) = (1,1,1,1)
 _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
 _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp mat4 _World2Object;
uniform highp float _WindEdgeFlutterFreqScale;
uniform highp float _WindEdgeFlutter;
uniform highp vec4 _Wind;
uniform highp vec4 _Time;
uniform highp mat4 _Object2World;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 wind;
  highp float bendingFact;
  lowp float tmpvar_1;
  tmpvar_1 = _glesColor.w;
  bendingFact = tmpvar_1;
  mat3 tmpvar_2;
  tmpvar_2[0] = _World2Object[0].xyz;
  tmpvar_2[1] = _World2Object[1].xyz;
  tmpvar_2[2] = _World2Object[2].xyz;
  wind.xyz = (tmpvar_2 * _Wind.xyz);
  wind.w = (_Wind.w * bendingFact);
  highp vec2 tmpvar_3;
  tmpvar_3.y = 1.0;
  tmpvar_3.x = _WindEdgeFlutterFreqScale;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec3 bend;
  vec4 v_i0;
  v_i0.x = _Object2World[0].w;
  v_i0.y = _Object2World[1].w;
  v_i0.z = _Object2World[2].w;
  v_i0.w = _Object2World[3].w;
  highp float tmpvar_4;
  tmpvar_4 = dot (v_i0.xyz, vec3(1.0, 1.0, 1.0));
  highp vec2 tmpvar_5;
  tmpvar_5.x = dot (_glesVertex.xyz, vec3((_WindEdgeFlutter + tmpvar_4)));
  tmpvar_5.y = tmpvar_4;
  highp vec4 tmpvar_6;
  tmpvar_6 = abs (((fract ((((fract ((((_Time.y * tmpvar_3).xx + tmpvar_5).xxyy * vec4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  highp vec4 tmpvar_7;
  tmpvar_7 = ((tmpvar_6 * tmpvar_6) * (3.0 - (2.0 * tmpvar_6)));
  highp vec2 tmpvar_8;
  tmpvar_8 = (tmpvar_7.xz + tmpvar_7.yw);
  bend = ((_WindEdgeFlutter * 0.1) * normalize (_glesNormal));
  bend.y = (bendingFact * 0.3);
  pos.xyz = (_glesVertex.xyz + (((tmpvar_8.xyx * bend) + ((wind.xyz * tmpvar_8.y) * bendingFact)) * wind.w));
  pos.xyz = (pos.xyz + (bendingFact * wind.xyz));
  gl_Position = (gl_ModelViewProjectionMatrix * pos);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD2 = _glesColor.xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  c.xyz = tmpvar_1.xyz;
  c.w = tmpvar_1.w;
  c.xyz = (tmpvar_1.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz));
  gl_FragData[0] = c;
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
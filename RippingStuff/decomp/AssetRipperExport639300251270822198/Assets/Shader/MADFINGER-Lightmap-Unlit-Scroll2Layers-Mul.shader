Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative (Supports Lightmap)" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _MMultiplier ("Layer Multiplier", Float) = 2
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

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp float _ScrollY;
uniform highp float _ScrollX;
uniform highp float _Scroll2Y;
uniform highp float _Scroll2X;
uniform highp float _MMultiplier;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2.x = _ScrollX;
  tmpvar_2.y = _ScrollY;
  highp vec2 tmpvar_3;
  tmpvar_3.x = _Scroll2X;
  tmpvar_3.y = _Scroll2Y;
  highp vec4 tmpvar_4;
  tmpvar_4.x = _MMultiplier;
  tmpvar_4.y = _MMultiplier;
  tmpvar_4.z = _MMultiplier;
  tmpvar_4.w = _MMultiplier;
  tmpvar_1 = tmpvar_4;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = (_glesMultiTexCoord0.xy + fract ((tmpvar_2 * _Time.xy)));
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xy + fract ((tmpvar_3 * _Time.xy)));
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform sampler2D _DetailTex;
void main ()
{
  gl_FragData[0] = ((texture2D (_MainTex, xlv_TEXCOORD0) * texture2D (_DetailTex, xlv_TEXCOORD1)) * xlv_TEXCOORD2);
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

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

uniform highp vec4 _Time;
uniform highp float _ScrollY;
uniform highp float _ScrollX;
uniform highp float _Scroll2Y;
uniform highp float _Scroll2X;
uniform highp float _MMultiplier;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2.x = _ScrollX;
  tmpvar_2.y = _ScrollY;
  highp vec2 tmpvar_3;
  tmpvar_3.x = _Scroll2X;
  tmpvar_3.y = _Scroll2Y;
  highp vec4 tmpvar_4;
  tmpvar_4.x = _MMultiplier;
  tmpvar_4.y = _MMultiplier;
  tmpvar_4.z = _MMultiplier;
  tmpvar_4.w = _MMultiplier;
  tmpvar_1 = tmpvar_4;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = (_glesMultiTexCoord0.xy + fract ((tmpvar_2 * _Time.xy)));
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xy + fract ((tmpvar_3 * _Time.xy)));
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
uniform sampler2D _DetailTex;
void main ()
{
  lowp vec4 o;
  lowp vec4 tmpvar_1;
  tmpvar_1 = ((texture2D (_MainTex, xlv_TEXCOORD0) * texture2D (_DetailTex, xlv_TEXCOORD1)) * xlv_TEXCOORD2);
  o = tmpvar_1;
  o.xyz = (tmpvar_1.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD3).xyz));
  gl_FragData[0] = o;
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
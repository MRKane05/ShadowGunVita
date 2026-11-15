Shader "MADFINGER/FX/Menu - 3 layers" {
Properties {
 _BaseTex ("Base layer", 2D) = "white" {}
 _MulTex ("Mul layer", 2D) = "white" {}
 _AddTex ("Add layer", 2D) = "white" {}
 _ScrollBase ("Base layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollMul ("Mul layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollAdd ("Add layer scroll speed ", Vector) = (0,0,0,0)
 _Multiplier ("Multiplier", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Transparent" }
 Pass {
  Tags { "RenderType"="Transparent" }
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
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
varying highp vec4 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp vec4 _ScrollMul;
uniform highp vec4 _ScrollBase;
uniform highp vec4 _ScrollAdd;
uniform highp float _Multiplier;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2.xy = (_glesMultiTexCoord0.xy + fract ((_ScrollBase.xy * _Time.xy)));
  tmpvar_2.zw = (_glesMultiTexCoord0.xy + fract ((_ScrollMul.xy * _Time.xy)));
  tmpvar_1 = _glesColor;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesColor.xyz * _Multiplier);
  tmpvar_1.xyz = tmpvar_3;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xy + fract ((_ScrollAdd.xy * _Time.xy)));
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MulTex;
uniform sampler2D _BaseTex;
uniform sampler2D _AddTex;
void main ()
{
  gl_FragData[0] = (((texture2D (_BaseTex, xlv_TEXCOORD0.xy) + texture2D (_AddTex, xlv_TEXCOORD1)) * texture2D (_MulTex, xlv_TEXCOORD0.zw)) * xlv_TEXCOORD2);
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

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp vec4 _ScrollMul;
uniform highp vec4 _ScrollBase;
uniform highp vec4 _ScrollAdd;
uniform highp float _Multiplier;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2.xy = (_glesMultiTexCoord0.xy + fract ((_ScrollBase.xy * _Time.xy)));
  tmpvar_2.zw = (_glesMultiTexCoord0.xy + fract ((_ScrollMul.xy * _Time.xy)));
  tmpvar_1 = _glesColor;
  highp vec3 tmpvar_3;
  tmpvar_3 = (_glesColor.xyz * _Multiplier);
  tmpvar_1.xyz = tmpvar_3;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xy + fract ((_ScrollAdd.xy * _Time.xy)));
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MulTex;
uniform sampler2D _BaseTex;
uniform sampler2D _AddTex;
void main ()
{
  gl_FragData[0] = (((texture2D (_BaseTex, xlv_TEXCOORD0.xy) + texture2D (_AddTex, xlv_TEXCOORD1)) * texture2D (_MulTex, xlv_TEXCOORD0.zw)) * xlv_TEXCOORD2);
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
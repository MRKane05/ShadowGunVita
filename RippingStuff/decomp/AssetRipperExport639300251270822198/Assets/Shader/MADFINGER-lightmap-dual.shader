Shader "MADFINGER/Environment/Dual diffuse with lightmap" {
Properties {
 _MainTex0 ("Base layer0 (RGB)", 2D) = "white" {}
 _MainTex1 ("Base layer1 (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xy = _glesMultiTexCoord0.xy;
  tmpvar_1.zw = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_COLOR = _glesColor;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex0;
void main ()
{
  lowp vec4 res;
  lowp vec4 tmpvar_1;
  tmpvar_1 = mix (texture2D (_MainTex0, xlv_TEXCOORD0.xy), texture2D (_MainTex1, xlv_TEXCOORD0.xy), xlv_COLOR.wwww);
  res = tmpvar_1;
  res.xyz = (tmpvar_1.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD0.zw).xyz));
  gl_FragData[0] = res;
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

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_LightmapST;

attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xy = _glesMultiTexCoord0.xy;
  tmpvar_1.zw = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_COLOR = _glesColor;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex0;
void main ()
{
  lowp vec4 res;
  lowp vec4 tmpvar_1;
  tmpvar_1 = mix (texture2D (_MainTex0, xlv_TEXCOORD0.xy), texture2D (_MainTex1, xlv_TEXCOORD0.xy), xlv_COLOR.wwww);
  res = tmpvar_1;
  res.xyz = (tmpvar_1.xyz * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD0.zw).xyz));
  gl_FragData[0] = res;
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
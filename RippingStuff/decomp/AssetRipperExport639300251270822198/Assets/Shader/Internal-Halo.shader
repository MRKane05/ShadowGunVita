Shader "Hidden/Internal-Halo" {
SubShader { 
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend OneMinusDstColor One
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;

uniform highp vec4 _HaloFalloff_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _HaloFalloff_ST.xy) + _HaloFalloff_ST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _HaloFalloff;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float a;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_HaloFalloff, xlv_TEXCOORD0).w;
  a = tmpvar_2;
  mediump vec4 tmpvar_3;
  tmpvar_3.xyz = (xlv_COLOR.xyz * a);
  tmpvar_3.w = a;
  tmpvar_1 = tmpvar_3;
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
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord0
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend OneMinusDstColor One
  AlphaTest Greater 0
  ColorMask RGB
  SetTexture [_HaloFalloff] { combine primary * texture alpha, texture alpha }
 }
}
}
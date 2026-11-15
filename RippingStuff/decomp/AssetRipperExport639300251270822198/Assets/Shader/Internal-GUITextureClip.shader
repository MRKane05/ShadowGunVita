Shader "Hidden/Internal-GUITextureClip" {
Properties {
 _MainTex ("Texture", any) = "white" {}
}
SubShader { 
 Tags { "ForceSupported"="True" }
 Pass {
  Tags { "ForceSupported"="True" }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
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

varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;


uniform highp vec4 _MainTex_ST;
uniform highp mat4 _GUIClipTextureMatrix;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = (_GUIClipTextureMatrix * (gl_ModelViewMatrix * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
uniform sampler2D _MainTex;
uniform sampler2D _GUIClipTexture;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  col = tmpvar_1;
  col.w = (tmpvar_1.w * texture2D (_GUIClipTexture, xlv_TEXCOORD1).w);
  gl_FragData[0] = col;
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
 Tags { "ForceSupported"="True" }
 Pass {
  Tags { "ForceSupported"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine primary * texture }
  SetTexture [_GUIClipTexture] { combine previous, previous alpha * texture alpha }
 }
}
SubShader { 
 Tags { "ForceSupported"="True" }
 Pass {
  Tags { "ForceSupported"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  ColorMask A
  SetTexture [_MainTex] { combine primary * texture }
 }
 Pass {
  Tags { "ForceSupported"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend DstAlpha Zero
  ColorMask A
  SetTexture [_GUIClipTexture] { combine texture, texture alpha }
 }
 Pass {
  Tags { "ForceSupported"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend DstAlpha OneMinusDstAlpha
  ColorMask RGB
  SetTexture [_MainTex] { combine primary * texture }
 }
}
}
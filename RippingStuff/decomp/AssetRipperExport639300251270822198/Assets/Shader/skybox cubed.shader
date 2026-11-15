Shader "RenderFX/Skybox Cubed" {
Properties {
 _Tint ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _Tex ("Cubemap", CUBE) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Background" "RenderType"="Background" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD0;
uniform lowp vec4 unity_ColorSpaceGrey;
uniform lowp vec4 _Tint;
uniform samplerCube _Tex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = textureCube (_Tex, xlv_TEXCOORD0);
  col.xyz = ((tmpvar_1.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
  col.w = (tmpvar_1.w * _Tint.w);
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
 Tags { "QUEUE"="Background" "RenderType"="Background" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" }
  Color [_Tint]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_Tex] { combine texture +- primary, texture alpha * primary alpha }
 }
}
Fallback Off
}
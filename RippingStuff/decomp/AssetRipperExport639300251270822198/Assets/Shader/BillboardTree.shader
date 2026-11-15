Shader "Hidden/TerrainEngine/BillboardTree" {
Properties {
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent-100" "IGNOREPROJECTOR"="True" "RenderType"="TreeBillboard" }
 Pass {
  Tags { "QUEUE"="Transparent-100" "IGNOREPROJECTOR"="True" "RenderType"="TreeBillboard" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
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
varying lowp vec4 xlv_COLOR0;

uniform highp vec4 _TreeBillboardDistances;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardCameraFront;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec2 tmpvar_1;
  highp vec4 pos;
  pos = _glesVertex;
  highp vec2 offset;
  offset = _glesMultiTexCoord1.xy;
  highp float offsetz;
  offsetz = _glesMultiTexCoord0.y;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_3;
  tmpvar_3 = dot (tmpvar_2, tmpvar_2);
  if ((tmpvar_3 > _TreeBillboardDistances.x)) {
    offsetz = 0.0;
    offset = vec2(0.0, 0.0);
  };
  pos.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset.x));
  pos.xyz = (pos.xyz + (_TreeBillboardCameraUp.xyz * mix (offset.y, offsetz, _TreeBillboardCameraPos.w)));
  pos.xyz = (pos.xyz + ((_TreeBillboardCameraFront.xyz * abs (offset.x)) * _TreeBillboardCameraUp.w));
  tmpvar_1.x = _glesMultiTexCoord0.x;
  tmpvar_1.y = float((_glesMultiTexCoord0.y > 0.0));
  gl_Position = (gl_ModelViewProjectionMatrix * pos);
  xlv_COLOR0 = _glesColor;
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR0;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 col;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  col = tmpvar_1;
  col.xyz = (tmpvar_1.xyz * xlv_COLOR0.xyz);
  if ((tmpvar_1.w < 0.0)) {
    discard;
  };
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
 Tags { "QUEUE"="Transparent-100" "IGNOREPROJECTOR"="True" "RenderType"="TreeBillboard" }
 Pass {
  Tags { "QUEUE"="Transparent-100" "IGNOREPROJECTOR"="True" "RenderType"="TreeBillboard" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
}
  SetTexture [_MainTex] { combine texture * primary, texture alpha }
 }
}
Fallback Off
}
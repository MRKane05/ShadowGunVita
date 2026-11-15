Shader "MADFINGER/FX/Anim texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _NumTexTiles ("Num tex tiles", Vector) = (4,4,0,0)
 _ReplaySpeed ("Replay speed - FPS", Float) = 4
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp float _ReplaySpeed;
uniform highp vec4 _NumTexTiles;
uniform highp vec4 _Color;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 tile;
  highp float tmpvar_2;
  tmpvar_2 = (((_glesColor.w * 60.0) + _Time.y) * _ReplaySpeed);
  highp float tmpvar_3;
  tmpvar_3 = floor (tmpvar_2);
  highp float tmpvar_4;
  tmpvar_4 = (tmpvar_3 + 1.0);
  highp float tmpvar_5;
  tmpvar_5 = (tmpvar_2 - tmpvar_3);
  highp vec2 tmpvar_6;
  tmpvar_6 = (1.0/(_NumTexTiles.xy));
  highp vec2 tmpvar_7;
  tmpvar_7.x = tmpvar_3;
  tmpvar_7.y = floor ((tmpvar_3 / _NumTexTiles.x));
  tile.xy = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8.x = tmpvar_4;
  tmpvar_8.y = floor ((tmpvar_4 / _NumTexTiles.x));
  tile.zw = tmpvar_8;
  vec4 tmpvar_9;
  tmpvar_9 = (tile / _NumTexTiles.xyxy);
  vec4 tmpvar_10;
  tmpvar_10 = (fract (abs (tmpvar_9)) * _NumTexTiles.xyxy);
  float tmpvar_11;
  if ((tmpvar_9.x >= 0.0)) {
    tmpvar_11 = tmpvar_10.x;
  } else {
    tmpvar_11 = -(tmpvar_10.x);
  };
  float tmpvar_12;
  if ((tmpvar_9.y >= 0.0)) {
    tmpvar_12 = tmpvar_10.y;
  } else {
    tmpvar_12 = -(tmpvar_10.y);
  };
  float tmpvar_13;
  if ((tmpvar_9.z >= 0.0)) {
    tmpvar_13 = tmpvar_10.z;
  } else {
    tmpvar_13 = -(tmpvar_10.z);
  };
  float tmpvar_14;
  if ((tmpvar_9.w >= 0.0)) {
    tmpvar_14 = tmpvar_10.w;
  } else {
    tmpvar_14 = -(tmpvar_10.w);
  };
  vec4 tmpvar_15;
  tmpvar_15.x = tmpvar_11;
  tmpvar_15.y = tmpvar_12;
  tmpvar_15.z = tmpvar_13;
  tmpvar_15.w = tmpvar_14;
  tile = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (_Color.xyz * _glesColor.xyz);
  tmpvar_16.w = tmpvar_5;
  tmpvar_1 = tmpvar_16;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xyxy + tmpvar_15) * tmpvar_6.xyxy);
  xlv_COLOR = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec4 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (mix (texture2D (_MainTex, xlv_TEXCOORD0.xy), texture2D (_MainTex, xlv_TEXCOORD0.zw), xlv_COLOR.wwww) * xlv_COLOR);
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
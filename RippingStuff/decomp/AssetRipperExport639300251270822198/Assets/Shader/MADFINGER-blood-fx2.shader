Shader "MADFINGER/FX/Blood FX - alpha blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _FadeInSpeed ("Fade in speed", Float) = 5
 _DrippingSpeed ("Dripping speed", Float) = 0.1
}
SubShader { 
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Time;
uniform highp float _DrippingSpeed;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  highp float tmpvar_3;
  tmpvar_3 = (_Time.y + _glesVertex.z);
  highp vec4 tmpvar_4;
  tmpvar_4.zw = vec2(0.0, 1.0);
  tmpvar_4.xy = _glesVertex.xy;
  tmpvar_1 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = vec3(1.0, 1.0, 1.0);
  tmpvar_5.w = (1.0 - max ((tmpvar_3 - (0.25 * _glesMultiTexCoord1.x)), 0.0));
  tmpvar_2 = tmpvar_5;
  tmpvar_1.y = (_glesVertex.y - ((tmpvar_3 * _DrippingSpeed) * _glesMultiTexCoord1.y));
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_COLOR = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
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
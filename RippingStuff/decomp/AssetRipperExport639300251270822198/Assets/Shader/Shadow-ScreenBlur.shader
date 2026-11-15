Shader "Hidden/Shadow-ScreenBlur" {
Properties {
 _MainTex ("Base", 2D) = "white" {}
}
SubShader { 
 Pass {
  ZTest Always
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

varying mediump vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowBlurParams;
uniform sampler2D _MainTex;
uniform highp vec4 _BlurOffsets7;
uniform highp vec4 _BlurOffsets6;
uniform highp vec4 _BlurOffsets5;
uniform highp vec4 _BlurOffsets4;
uniform highp vec4 _BlurOffsets3;
uniform highp vec4 _BlurOffsets2;
uniform highp vec4 _BlurOffsets1;
uniform highp vec4 _BlurOffsets0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 sample_7;
  highp vec4 sample_6;
  highp vec4 sample_5;
  highp vec4 sample_4;
  highp vec4 sample_3;
  highp vec4 sample_2;
  highp vec4 sample_1;
  highp vec4 sample;
  highp vec4 mask;
  highp vec4 coord;
  mediump vec4 tmpvar_2;
  tmpvar_2.zw = vec2(0.0, 0.0);
  tmpvar_2.xy = xlv_TEXCOORD0;
  coord = tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, coord.xy);
  mask = tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (mask.z + (mask.w / 255.0));
  highp float tmpvar_5;
  tmpvar_5 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_4)), 0.0, 1.0);
  mask.xy = (mask.xy * unity_ShadowBlurParams.x);
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets0)).xy);
  sample = tmpvar_6;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample.z + (sample.w / 255.0))))), 0.0, 1.0) * sample.xy));
  lowp vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets1)).xy);
  sample_1 = tmpvar_7;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_1.z + (sample_1.w / 255.0))))), 0.0, 1.0) * sample_1.xy));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets2)).xy);
  sample_2 = tmpvar_8;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_2.z + (sample_2.w / 255.0))))), 0.0, 1.0) * sample_2.xy));
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets3)).xy);
  sample_3 = tmpvar_9;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_3.z + (sample_3.w / 255.0))))), 0.0, 1.0) * sample_3.xy));
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets4)).xy);
  sample_4 = tmpvar_10;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_4.z + (sample_4.w / 255.0))))), 0.0, 1.0) * sample_4.xy));
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets5)).xy);
  sample_5 = tmpvar_11;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_5.z + (sample_5.w / 255.0))))), 0.0, 1.0) * sample_5.xy));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets6)).xy);
  sample_6 = tmpvar_12;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_6.z + (sample_6.w / 255.0))))), 0.0, 1.0) * sample_6.xy));
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_MainTex, (coord + (tmpvar_5 * _BlurOffsets7)).xy);
  sample_7 = tmpvar_13;
  mask.xy = (mask.xy + (clamp ((unity_ShadowBlurParams.x - abs ((tmpvar_4 - (sample_7.z + (sample_7.w / 255.0))))), 0.0, 1.0) * sample_7.xy));
  highp vec4 tmpvar_14;
  tmpvar_14 = vec4((mask.x / mask.y));
  tmpvar_1 = tmpvar_14;
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
Fallback Off
}
Shader "Hidden/Shadow-ScreenBlurRotated" {
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
#define gl_TextureMatrix0 glstate_matrix_texture0
uniform mat4 glstate_matrix_texture0;

varying mediump vec2 xlv_TEXCOORD0;


attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec2 tmpvar_1;
  highp vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.zw = vec2(0.0, 0.0);
  tmpvar_3.x = _glesMultiTexCoord0.x;
  tmpvar_3.y = _glesMultiTexCoord0.y;
  tmpvar_2 = (gl_TextureMatrix0 * tmpvar_3).xy;
  tmpvar_1 = tmpvar_2;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowBlurParams;
uniform sampler2D unity_RandomRotation16;
uniform highp vec4 _ScreenParams;
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
  mediump vec4 sample_7;
  mediump vec4 sample_6;
  mediump vec4 sample_5;
  mediump vec4 sample_4;
  mediump vec4 sample_3;
  mediump vec4 sample_2;
  mediump vec4 sample_1;
  mediump vec4 sample;
  mediump float diffTolerance;
  mediump float radius;
  mediump vec4 mask;
  mediump vec4 rotation;
  highp vec4 coord;
  mediump vec4 tmpvar_2;
  tmpvar_2.zw = vec2(0.0, 0.0);
  tmpvar_2.xy = xlv_TEXCOORD0;
  coord = tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = ((2.0 * texture2D (unity_RandomRotation16, ((coord.xy * _ScreenParams.xy) / 16.0))) - 1.0);
  rotation = tmpvar_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, coord.xy);
  mask = tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = (mask.z + (mask.w / 255.0));
  highp float tmpvar_6;
  tmpvar_6 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_5)), 0.0, 1.0);
  radius = tmpvar_6;
  highp float tmpvar_7;
  tmpvar_7 = unity_ShadowBlurParams.x;
  diffTolerance = tmpvar_7;
  mask.xy = (mask.xy * diffTolerance);
  highp vec4 rotation_i0;
  rotation_i0 = rotation;
  highp vec2 offset;
  offset.x = dot (_BlurOffsets0.xy, rotation_i0.xy);
  offset.y = dot (_BlurOffsets0.xy, rotation_i0.zw);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, (coord.xy + (radius * offset)));
  sample = tmpvar_8;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample.z + (sample.w / 255.0))))), 0.0, 1.0) * sample.xy));
  highp vec4 rotation_i0_i1;
  rotation_i0_i1 = rotation;
  highp vec2 offset_i0;
  offset_i0.x = dot (_BlurOffsets1.xy, rotation_i0_i1.xy);
  offset_i0.y = dot (_BlurOffsets1.xy, rotation_i0_i1.zw);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, (coord.xy + (radius * offset_i0)));
  sample_1 = tmpvar_9;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_1.z + (sample_1.w / 255.0))))), 0.0, 1.0) * sample_1.xy));
  highp vec4 rotation_i0_i1_i2;
  rotation_i0_i1_i2 = rotation;
  highp vec2 offset_i0_i1;
  offset_i0_i1.x = dot (_BlurOffsets2.xy, rotation_i0_i1_i2.xy);
  offset_i0_i1.y = dot (_BlurOffsets2.xy, rotation_i0_i1_i2.zw);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1)));
  sample_2 = tmpvar_10;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_2.z + (sample_2.w / 255.0))))), 0.0, 1.0) * sample_2.xy));
  highp vec4 rotation_i0_i1_i2_i3;
  rotation_i0_i1_i2_i3 = rotation;
  highp vec2 offset_i0_i1_i2;
  offset_i0_i1_i2.x = dot (_BlurOffsets3.xy, rotation_i0_i1_i2_i3.xy);
  offset_i0_i1_i2.y = dot (_BlurOffsets3.xy, rotation_i0_i1_i2_i3.zw);
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1_i2)));
  sample_3 = tmpvar_11;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_3.z + (sample_3.w / 255.0))))), 0.0, 1.0) * sample_3.xy));
  highp vec4 rotation_i0_i1_i2_i3_i4;
  rotation_i0_i1_i2_i3_i4 = rotation;
  highp vec2 offset_i0_i1_i2_i3;
  offset_i0_i1_i2_i3.x = dot (_BlurOffsets4.xy, rotation_i0_i1_i2_i3_i4.xy);
  offset_i0_i1_i2_i3.y = dot (_BlurOffsets4.xy, rotation_i0_i1_i2_i3_i4.zw);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1_i2_i3)));
  sample_4 = tmpvar_12;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_4.z + (sample_4.w / 255.0))))), 0.0, 1.0) * sample_4.xy));
  highp vec4 rotation_i0_i1_i2_i3_i4_i5;
  rotation_i0_i1_i2_i3_i4_i5 = rotation;
  highp vec2 offset_i0_i1_i2_i3_i4;
  offset_i0_i1_i2_i3_i4.x = dot (_BlurOffsets5.xy, rotation_i0_i1_i2_i3_i4_i5.xy);
  offset_i0_i1_i2_i3_i4.y = dot (_BlurOffsets5.xy, rotation_i0_i1_i2_i3_i4_i5.zw);
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1_i2_i3_i4)));
  sample_5 = tmpvar_13;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_5.z + (sample_5.w / 255.0))))), 0.0, 1.0) * sample_5.xy));
  highp vec4 rotation_i0_i1_i2_i3_i4_i5_i6;
  rotation_i0_i1_i2_i3_i4_i5_i6 = rotation;
  highp vec2 offset_i0_i1_i2_i3_i4_i5;
  offset_i0_i1_i2_i3_i4_i5.x = dot (_BlurOffsets6.xy, rotation_i0_i1_i2_i3_i4_i5_i6.xy);
  offset_i0_i1_i2_i3_i4_i5.y = dot (_BlurOffsets6.xy, rotation_i0_i1_i2_i3_i4_i5_i6.zw);
  lowp vec4 tmpvar_14;
  tmpvar_14 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1_i2_i3_i4_i5)));
  sample_6 = tmpvar_14;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_6.z + (sample_6.w / 255.0))))), 0.0, 1.0) * sample_6.xy));
  highp vec4 rotation_i0_i1_i2_i3_i4_i5_i6_i7;
  rotation_i0_i1_i2_i3_i4_i5_i6_i7 = rotation;
  highp vec2 offset_i0_i1_i2_i3_i4_i5_i6;
  offset_i0_i1_i2_i3_i4_i5_i6.x = dot (_BlurOffsets7.xy, rotation_i0_i1_i2_i3_i4_i5_i6_i7.xy);
  offset_i0_i1_i2_i3_i4_i5_i6.y = dot (_BlurOffsets7.xy, rotation_i0_i1_i2_i3_i4_i5_i6_i7.zw);
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_MainTex, (coord.xy + (radius * offset_i0_i1_i2_i3_i4_i5_i6)));
  sample_7 = tmpvar_15;
  mask.xy = (mask.xy + (clamp ((diffTolerance - abs ((tmpvar_5 - (sample_7.z + (sample_7.w / 255.0))))), 0.0, 1.0) * sample_7.xy));
  mediump vec4 tmpvar_16;
  tmpvar_16 = vec4((mask.x / mask.y));
  tmpvar_1 = tmpvar_16;
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
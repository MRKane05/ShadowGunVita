Shader "Hidden/Internal-CombineDepthNormals" {
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

varying highp vec2 xlv_TEXCOORD0;

uniform highp vec4 _CameraNormalsTexture_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _CameraNormalsTexture_ST.xy) + _CameraNormalsTexture_ST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 _WorldToCamera;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec3 n;
  highp float d;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  d = tmpvar_2;
  lowp vec3 tmpvar_3;
  tmpvar_3 = ((texture2D (_CameraNormalsTexture, xlv_TEXCOORD0) * 2.0) - 1.0).xyz;
  n = tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * d) + _ZBufferParams.y)));
  d = tmpvar_4;
  mat3 tmpvar_5;
  tmpvar_5[0] = _WorldToCamera[0].xyz;
  tmpvar_5[1] = _WorldToCamera[1].xyz;
  tmpvar_5[2] = _WorldToCamera[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * n);
  n = tmpvar_6;
  n.z = -(tmpvar_6.z);
  highp vec4 tmpvar_7;
  if ((tmpvar_4 < 0.999985)) {
    highp vec4 enc;
    enc.xy = ((((tmpvar_6.xy / (n.z + 1.0)) / 1.7777) * 0.5) + 0.5);
    highp vec2 enc_i0_i1;
    highp vec2 tmpvar_8;
    tmpvar_8 = fract ((vec2(1.0, 255.0) * tmpvar_4));
    enc_i0_i1 = tmpvar_8;
    enc_i0_i1.x = (tmpvar_8.x - (tmpvar_8.y * 0.00392157));
    enc.zw = enc_i0_i1;
    tmpvar_7 = enc;
  } else {
    tmpvar_7 = vec4(0.5, 0.5, 1.0, 1.0);
  };
  tmpvar_1 = tmpvar_7;
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
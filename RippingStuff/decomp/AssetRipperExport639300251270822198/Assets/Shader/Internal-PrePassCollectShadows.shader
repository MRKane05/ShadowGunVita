Shader "Hidden/Internal-PrePassCollectShadows" {
Properties {
 _ShadowMapTexture ("", any) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp vec4 _LightShadowData;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res;
  highp float depth;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  depth = tmpvar_2;
  highp float tmpvar_3;
  tmpvar_3 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = (xlv_TEXCOORD1 * tmpvar_3);
  highp vec4 tmpvar_5;
  tmpvar_5 = (_CameraToWorld * tmpvar_4);
  mediump float shadow_i0;
  highp vec4 zFar;
  highp vec4 zNear;
  highp vec4 tmpvar_6;
  tmpvar_6 = tmpvar_4.zzzz;
  lowp vec4 tmpvar_7;
  tmpvar_7 = vec4(greaterThanEqual (tmpvar_6, _LightSplitsNear));
  zNear = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = tmpvar_4.zzzz;
  lowp vec4 tmpvar_9;
  tmpvar_9 = vec4(lessThan (tmpvar_8, _LightSplitsFar));
  zFar = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10 = (zNear * zFar);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = (((((unity_World2Shadow[0] * tmpvar_5).xyz * tmpvar_10.x) + ((unity_World2Shadow[1] * tmpvar_5).xyz * tmpvar_10.y)) + ((unity_World2Shadow[2] * tmpvar_5).xyz * tmpvar_10.z)) + ((unity_World2Shadow[3] * tmpvar_5).xyz * tmpvar_10.w));
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_ShadowMapTexture, tmpvar_11.xy);
  highp float tmpvar_13;
  if ((tmpvar_12.x < tmpvar_11.z)) {
    tmpvar_13 = _LightShadowData.x;
  } else {
    tmpvar_13 = 1.0;
  };
  shadow_i0 = tmpvar_13;
  res.x = shadow_i0;
  res.y = 1.0;
  highp vec2 enc;
  highp vec2 tmpvar_14;
  tmpvar_14 = fract ((vec2(1.0, 255.0) * (1.0 - tmpvar_3)));
  enc = tmpvar_14;
  enc.x = (tmpvar_14.x - (tmpvar_14.y * 0.00392157));
  res.zw = enc;
  tmpvar_1 = res;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;

attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _LightShadowData;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res;
  highp float depth;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  depth = tmpvar_2;
  highp float tmpvar_3;
  tmpvar_3 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = (xlv_TEXCOORD1 * tmpvar_3);
  highp vec4 tmpvar_5;
  tmpvar_5 = (_CameraToWorld * tmpvar_4);
  mediump float shadow_i0;
  highp vec4 weights;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xyz - unity_ShadowSplitSpheres[0].xyz);
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_5.xyz - unity_ShadowSplitSpheres[1].xyz);
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_5.xyz - unity_ShadowSplitSpheres[2].xyz);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_5.xyz - unity_ShadowSplitSpheres[3].xyz);
  highp vec4 tmpvar_10;
  tmpvar_10.x = dot (tmpvar_6, tmpvar_6);
  tmpvar_10.y = dot (tmpvar_7, tmpvar_7);
  tmpvar_10.z = dot (tmpvar_8, tmpvar_8);
  tmpvar_10.w = dot (tmpvar_9, tmpvar_9);
  lowp vec4 tmpvar_11;
  tmpvar_11 = vec4(lessThan (tmpvar_10, unity_ShadowSplitSqRadii));
  weights = tmpvar_11;
  weights.yzw = clamp ((weights.yzw - weights.xyz), 0.0, 1.0);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = (((((unity_World2Shadow[0] * tmpvar_5).xyz * weights.x) + ((unity_World2Shadow[1] * tmpvar_5).xyz * weights.y)) + ((unity_World2Shadow[2] * tmpvar_5).xyz * weights.z)) + ((unity_World2Shadow[3] * tmpvar_5).xyz * weights.w));
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_ShadowMapTexture, tmpvar_12.xy);
  highp float tmpvar_14;
  if ((tmpvar_13.x < tmpvar_12.z)) {
    tmpvar_14 = _LightShadowData.x;
  } else {
    tmpvar_14 = 1.0;
  };
  shadow_i0 = tmpvar_14;
  res.x = shadow_i0;
  res.y = 1.0;
  highp vec2 enc;
  highp vec2 tmpvar_15;
  tmpvar_15 = fract ((vec2(1.0, 255.0) * (1.0 - tmpvar_3)));
  enc = tmpvar_15;
  enc.x = (tmpvar_15.x - (tmpvar_15.y * 0.00392157));
  res.zw = enc;
  tmpvar_1 = res;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES"
}
}
 }
}
Fallback Off
}
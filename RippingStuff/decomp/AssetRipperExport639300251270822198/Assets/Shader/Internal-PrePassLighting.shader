Shader "Hidden/Internal-PrePassLighting" {
Properties {
 _LightTexture0 ("", any) = "" {}
 _LightTextureB0 ("", 2D) = "" {}
 _ShadowMapTexture ("", any) = "" {}
}
SubShader { 
 Pass {
  ZWrite Off
  Fog { Mode Off }
  Blend DstColor Zero
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (atten, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * atten));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  mediump vec4 tmpvar_22;
  tmpvar_22 = exp2 (-(tmpvar_21));
  tmpvar_1 = tmpvar_22;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_11;
  tmpvar_11 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_11;
  mediump float tmpvar_12;
  tmpvar_12 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_LightColor.xyz * tmpvar_10);
  res.xyz = tmpvar_13;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_14;
  tmpvar_14 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_15;
  tmpvar_15 = (spec * tmpvar_14);
  res.w = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_17;
  tmpvar_17 = (res * tmpvar_16);
  res = tmpvar_17;
  mediump vec4 tmpvar_18;
  tmpvar_18 = exp2 (-(tmpvar_17));
  tmpvar_1 = tmpvar_18;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_8);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_8;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  highp float tmpvar_16;
  tmpvar_16 = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  atten = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (spec * clamp (tmpvar_16, 0.0, 1.0));
  spec = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_LightColor.xyz * (tmpvar_17 * tmpvar_16));
  res.xyz = tmpvar_21;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_22;
  tmpvar_22 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_20 * tmpvar_22);
  res.w = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_25;
  tmpvar_25 = (res * tmpvar_24);
  res = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = exp2 (-(tmpvar_25));
  tmpvar_1 = tmpvar_26;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_8;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_13).xyz);
  highp float tmpvar_15;
  tmpvar_15 = (atten * tmpvar_14.w);
  atten = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = exp2 (-(tmpvar_24));
  tmpvar_1 = tmpvar_25;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_8;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_10).xy);
  highp float tmpvar_12;
  tmpvar_12 = tmpvar_11.w;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (tmpvar_12, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * tmpvar_12));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  mediump vec4 tmpvar_22;
  tmpvar_22 = exp2 (-(tmpvar_21));
  tmpvar_1 = tmpvar_22;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump float shadow;
  lowp vec4 tmpvar_20;
  tmpvar_20 = texture2DProj (_ShadowMapTexture, tmpvar_19);
  highp float tmpvar_21;
  if ((tmpvar_20.x < (tmpvar_19.z / tmpvar_19.w))) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  shadow = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = clamp ((shadow + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (atten * tmpvar_16);
  atten = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_25;
  tmpvar_25 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = (spec * clamp (tmpvar_23, 0.0, 1.0));
  spec = tmpvar_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (_LightColor.xyz * (tmpvar_24 * tmpvar_23));
  res.xyz = tmpvar_28;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_27 * tmpvar_29);
  res.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res * tmpvar_31);
  res = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2 (-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = tmpvar_10;
  mediump float tmpvar_14;
  tmpvar_14 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_15;
  tmpvar_15 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (spec * clamp (tmpvar_13, 0.0, 1.0));
  spec = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = (_LightColor.xyz * (tmpvar_14 * tmpvar_13));
  res.xyz = tmpvar_18;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_19;
  tmpvar_19 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_17 * tmpvar_19);
  res.w = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_22;
  tmpvar_22 = (res * tmpvar_21);
  res = tmpvar_22;
  mediump vec4 tmpvar_23;
  tmpvar_23 = exp2 (-(tmpvar_22));
  tmpvar_1 = tmpvar_23;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_7;
  lowp vec4 tmpvar_14;
  tmpvar_14 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_13).xy);
  highp float tmpvar_15;
  tmpvar_15 = (tmpvar_10 * tmpvar_14.w);
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = exp2 (-(tmpvar_24));
  tmpvar_1 = tmpvar_25;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (atten * tmpvar_14);
  atten = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec * clamp (tmpvar_18, 0.0, 1.0));
  spec = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res.xyz = tmpvar_23;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_24;
  tmpvar_24 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_25;
  tmpvar_25 = (tmpvar_22 * tmpvar_24);
  res.w = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_27;
  tmpvar_27 = (res * tmpvar_26);
  res = tmpvar_27;
  mediump vec4 tmpvar_28;
  tmpvar_28 = exp2 (-(tmpvar_27));
  tmpvar_1 = tmpvar_28;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  lowp vec4 tmpvar_19;
  tmpvar_19 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_18).xyz);
  highp float tmpvar_20;
  tmpvar_20 = ((atten * tmpvar_14) * tmpvar_19.w);
  atten = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_22;
  tmpvar_22 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = (spec * clamp (tmpvar_20, 0.0, 1.0));
  spec = tmpvar_24;
  highp vec3 tmpvar_25;
  tmpvar_25 = (_LightColor.xyz * (tmpvar_21 * tmpvar_20));
  res.xyz = tmpvar_25;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_24 * tmpvar_26);
  res.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res * tmpvar_28);
  res = tmpvar_29;
  mediump vec4 tmpvar_30;
  tmpvar_30 = exp2 (-(tmpvar_29));
  tmpvar_1 = tmpvar_30;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ShadowOffsets[4];
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 tmpvar_20;
  tmpvar_20 = (tmpvar_19.xyz / tmpvar_19.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[0].xy));
  shadowVals.x = tmpvar_21.x;
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[1].xy));
  shadowVals.y = tmpvar_22.x;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[2].xy));
  shadowVals.z = tmpvar_23.x;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[3].xy));
  shadowVals.w = tmpvar_24.x;
  bvec4 tmpvar_25;
  tmpvar_25 = lessThan (shadowVals, tmpvar_20.zzzz);
  highp float tmpvar_26;
  if (tmpvar_25.x) {
    tmpvar_26 = _LightShadowData.x;
  } else {
    tmpvar_26 = 1.0;
  };
  highp float tmpvar_27;
  if (tmpvar_25.y) {
    tmpvar_27 = _LightShadowData.x;
  } else {
    tmpvar_27 = 1.0;
  };
  highp float tmpvar_28;
  if (tmpvar_25.z) {
    tmpvar_28 = _LightShadowData.x;
  } else {
    tmpvar_28 = 1.0;
  };
  highp float tmpvar_29;
  if (tmpvar_25.w) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  highp vec4 tmpvar_30;
  tmpvar_30.x = tmpvar_26;
  tmpvar_30.y = tmpvar_27;
  tmpvar_30.z = tmpvar_28;
  tmpvar_30.w = tmpvar_29;
  shadows = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_32;
  tmpvar_32 = clamp ((tmpvar_31 + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (atten * tmpvar_16);
  atten = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_35;
  tmpvar_35 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_35;
  mediump float tmpvar_36;
  tmpvar_36 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_36;
  highp float tmpvar_37;
  tmpvar_37 = (spec * clamp (tmpvar_33, 0.0, 1.0));
  spec = tmpvar_37;
  highp vec3 tmpvar_38;
  tmpvar_38 = (_LightColor.xyz * (tmpvar_34 * tmpvar_33));
  res.xyz = tmpvar_38;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_39;
  tmpvar_39 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_40;
  tmpvar_40 = (tmpvar_37 * tmpvar_39);
  res.w = tmpvar_40;
  highp float tmpvar_41;
  tmpvar_41 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_42;
  tmpvar_42 = (res * tmpvar_41);
  res = tmpvar_42;
  mediump vec4 tmpvar_43;
  tmpvar_43 = exp2 (-(tmpvar_42));
  tmpvar_1 = tmpvar_43;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_25;
  tmpvar_25 = (atten * tmpvar_24);
  atten = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_27;
  tmpvar_27 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = (spec * clamp (tmpvar_25, 0.0, 1.0));
  spec = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (_LightColor.xyz * (tmpvar_26 * tmpvar_25));
  res.xyz = tmpvar_30;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_31;
  tmpvar_31 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_32;
  tmpvar_32 = (tmpvar_29 * tmpvar_31);
  res.w = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_34;
  tmpvar_34 = (res * tmpvar_33);
  res = tmpvar_34;
  mediump vec4 tmpvar_35;
  tmpvar_35 = exp2 (-(tmpvar_34));
  tmpvar_1 = tmpvar_35;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_7;
  lowp vec4 tmpvar_26;
  tmpvar_26 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_25).xyz);
  highp float tmpvar_27;
  tmpvar_27 = ((atten * tmpvar_24) * tmpvar_26.w);
  atten = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_29;
  tmpvar_29 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (spec * clamp (tmpvar_27, 0.0, 1.0));
  spec = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (_LightColor.xyz * (tmpvar_28 * tmpvar_27));
  res.xyz = tmpvar_32;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_31 * tmpvar_33);
  res.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res * tmpvar_35);
  res = tmpvar_36;
  mediump vec4 tmpvar_37;
  tmpvar_37 = exp2 (-(tmpvar_36));
  tmpvar_1 = tmpvar_37;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
}
 }
 Pass {
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (atten, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * atten));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  tmpvar_1 = tmpvar_21;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_11;
  tmpvar_11 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_11;
  mediump float tmpvar_12;
  tmpvar_12 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_LightColor.xyz * tmpvar_10);
  res.xyz = tmpvar_13;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_14;
  tmpvar_14 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_15;
  tmpvar_15 = (spec * tmpvar_14);
  res.w = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_17;
  tmpvar_17 = (res * tmpvar_16);
  res = tmpvar_17;
  tmpvar_1 = tmpvar_17;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_8);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_8;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  highp float tmpvar_16;
  tmpvar_16 = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  atten = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (spec * clamp (tmpvar_16, 0.0, 1.0));
  spec = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_LightColor.xyz * (tmpvar_17 * tmpvar_16));
  res.xyz = tmpvar_21;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_22;
  tmpvar_22 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_20 * tmpvar_22);
  res.w = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_25;
  tmpvar_25 = (res * tmpvar_24);
  res = tmpvar_25;
  tmpvar_1 = tmpvar_25;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_8;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_13).xyz);
  highp float tmpvar_15;
  tmpvar_15 = (atten * tmpvar_14.w);
  atten = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  tmpvar_1 = tmpvar_24;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_8;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_10).xy);
  highp float tmpvar_12;
  tmpvar_12 = tmpvar_11.w;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (tmpvar_12, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * tmpvar_12));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  tmpvar_1 = tmpvar_21;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump float shadow;
  lowp vec4 tmpvar_20;
  tmpvar_20 = texture2DProj (_ShadowMapTexture, tmpvar_19);
  highp float tmpvar_21;
  if ((tmpvar_20.x < (tmpvar_19.z / tmpvar_19.w))) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  shadow = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = clamp ((shadow + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (atten * tmpvar_16);
  atten = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_25;
  tmpvar_25 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = (spec * clamp (tmpvar_23, 0.0, 1.0));
  spec = tmpvar_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (_LightColor.xyz * (tmpvar_24 * tmpvar_23));
  res.xyz = tmpvar_28;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_27 * tmpvar_29);
  res.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res * tmpvar_31);
  res = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = tmpvar_10;
  mediump float tmpvar_14;
  tmpvar_14 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_15;
  tmpvar_15 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (spec * clamp (tmpvar_13, 0.0, 1.0));
  spec = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = (_LightColor.xyz * (tmpvar_14 * tmpvar_13));
  res.xyz = tmpvar_18;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_19;
  tmpvar_19 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_17 * tmpvar_19);
  res.w = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_22;
  tmpvar_22 = (res * tmpvar_21);
  res = tmpvar_22;
  tmpvar_1 = tmpvar_22;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_7;
  lowp vec4 tmpvar_14;
  tmpvar_14 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_13).xy);
  highp float tmpvar_15;
  tmpvar_15 = (tmpvar_10 * tmpvar_14.w);
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  tmpvar_1 = tmpvar_24;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (atten * tmpvar_14);
  atten = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec * clamp (tmpvar_18, 0.0, 1.0));
  spec = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res.xyz = tmpvar_23;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_24;
  tmpvar_24 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_25;
  tmpvar_25 = (tmpvar_22 * tmpvar_24);
  res.w = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_27;
  tmpvar_27 = (res * tmpvar_26);
  res = tmpvar_27;
  tmpvar_1 = tmpvar_27;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  lowp vec4 tmpvar_19;
  tmpvar_19 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_18).xyz);
  highp float tmpvar_20;
  tmpvar_20 = ((atten * tmpvar_14) * tmpvar_19.w);
  atten = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_22;
  tmpvar_22 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = (spec * clamp (tmpvar_20, 0.0, 1.0));
  spec = tmpvar_24;
  highp vec3 tmpvar_25;
  tmpvar_25 = (_LightColor.xyz * (tmpvar_21 * tmpvar_20));
  res.xyz = tmpvar_25;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_24 * tmpvar_26);
  res.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res * tmpvar_28);
  res = tmpvar_29;
  tmpvar_1 = tmpvar_29;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ShadowOffsets[4];
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 tmpvar_20;
  tmpvar_20 = (tmpvar_19.xyz / tmpvar_19.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[0].xy));
  shadowVals.x = tmpvar_21.x;
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[1].xy));
  shadowVals.y = tmpvar_22.x;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[2].xy));
  shadowVals.z = tmpvar_23.x;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[3].xy));
  shadowVals.w = tmpvar_24.x;
  bvec4 tmpvar_25;
  tmpvar_25 = lessThan (shadowVals, tmpvar_20.zzzz);
  highp float tmpvar_26;
  if (tmpvar_25.x) {
    tmpvar_26 = _LightShadowData.x;
  } else {
    tmpvar_26 = 1.0;
  };
  highp float tmpvar_27;
  if (tmpvar_25.y) {
    tmpvar_27 = _LightShadowData.x;
  } else {
    tmpvar_27 = 1.0;
  };
  highp float tmpvar_28;
  if (tmpvar_25.z) {
    tmpvar_28 = _LightShadowData.x;
  } else {
    tmpvar_28 = 1.0;
  };
  highp float tmpvar_29;
  if (tmpvar_25.w) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  highp vec4 tmpvar_30;
  tmpvar_30.x = tmpvar_26;
  tmpvar_30.y = tmpvar_27;
  tmpvar_30.z = tmpvar_28;
  tmpvar_30.w = tmpvar_29;
  shadows = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_32;
  tmpvar_32 = clamp ((tmpvar_31 + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (atten * tmpvar_16);
  atten = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_35;
  tmpvar_35 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_35;
  mediump float tmpvar_36;
  tmpvar_36 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_36;
  highp float tmpvar_37;
  tmpvar_37 = (spec * clamp (tmpvar_33, 0.0, 1.0));
  spec = tmpvar_37;
  highp vec3 tmpvar_38;
  tmpvar_38 = (_LightColor.xyz * (tmpvar_34 * tmpvar_33));
  res.xyz = tmpvar_38;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_39;
  tmpvar_39 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_40;
  tmpvar_40 = (tmpvar_37 * tmpvar_39);
  res.w = tmpvar_40;
  highp float tmpvar_41;
  tmpvar_41 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_42;
  tmpvar_42 = (res * tmpvar_41);
  res = tmpvar_42;
  tmpvar_1 = tmpvar_42;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_25;
  tmpvar_25 = (atten * tmpvar_24);
  atten = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_27;
  tmpvar_27 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = (spec * clamp (tmpvar_25, 0.0, 1.0));
  spec = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (_LightColor.xyz * (tmpvar_26 * tmpvar_25));
  res.xyz = tmpvar_30;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_31;
  tmpvar_31 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_32;
  tmpvar_32 = (tmpvar_29 * tmpvar_31);
  res.w = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_34;
  tmpvar_34 = (res * tmpvar_33);
  res = tmpvar_34;
  tmpvar_1 = tmpvar_34;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_7;
  lowp vec4 tmpvar_26;
  tmpvar_26 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_25).xyz);
  highp float tmpvar_27;
  tmpvar_27 = ((atten * tmpvar_24) * tmpvar_26.w);
  atten = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_29;
  tmpvar_29 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (spec * clamp (tmpvar_27, 0.0, 1.0));
  spec = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (_LightColor.xyz * (tmpvar_28 * tmpvar_27));
  res.xyz = tmpvar_32;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_31 * tmpvar_33);
  res.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res * tmpvar_35);
  res = tmpvar_36;
  tmpvar_1 = tmpvar_36;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
}
 }
 Pass {
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (atten, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * atten));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  tmpvar_1 = tmpvar_21.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  tmpvar_10 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_11;
  tmpvar_11 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_11;
  mediump float tmpvar_12;
  tmpvar_12 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_12;
  highp vec3 tmpvar_13;
  tmpvar_13 = (_LightColor.xyz * tmpvar_10);
  res.xyz = tmpvar_13;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_14;
  tmpvar_14 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_15;
  tmpvar_15 = (spec * tmpvar_14);
  res.w = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_17;
  tmpvar_17 = (res * tmpvar_16);
  res = tmpvar_17;
  tmpvar_1 = tmpvar_17.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_8);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_8;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  highp float tmpvar_16;
  tmpvar_16 = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  atten = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (spec * clamp (tmpvar_16, 0.0, 1.0));
  spec = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_LightColor.xyz * (tmpvar_17 * tmpvar_16));
  res.xyz = tmpvar_21;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_22;
  tmpvar_22 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_20 * tmpvar_22);
  res.w = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_25;
  tmpvar_25 = (res * tmpvar_24);
  res = tmpvar_25;
  tmpvar_1 = tmpvar_25.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_8;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_13).xyz);
  highp float tmpvar_15;
  tmpvar_15 = (atten * tmpvar_14.w);
  atten = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  tmpvar_1 = tmpvar_24.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_5;
  tmpvar_5 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_5;
  highp float tmpvar_6;
  tmpvar_6 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (_CameraToWorld * tmpvar_7).xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_8;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_10).xy);
  highp float tmpvar_12;
  tmpvar_12 = tmpvar_11.w;
  mediump float tmpvar_13;
  tmpvar_13 = max (0.0, dot (lightDir, tmpvar_4));
  highp vec3 tmpvar_14;
  tmpvar_14 = normalize ((lightDir - normalize ((tmpvar_8 - _WorldSpaceCameraPos))));
  h = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = pow (max (0.0, dot (h, tmpvar_4)), (nspec.w * 128.0));
  spec = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = (spec * clamp (tmpvar_12, 0.0, 1.0));
  spec = tmpvar_16;
  highp vec3 tmpvar_17;
  tmpvar_17 = (_LightColor.xyz * (tmpvar_13 * tmpvar_12));
  res.xyz = tmpvar_17;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_18;
  tmpvar_18 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_19;
  tmpvar_19 = (tmpvar_16 * tmpvar_18);
  res.w = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((1.0 - ((mix (tmpvar_7.z, distance (tmpvar_8, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_21;
  tmpvar_21 = (res * tmpvar_20);
  res = tmpvar_21;
  tmpvar_1 = tmpvar_21.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump float shadow;
  lowp vec4 tmpvar_20;
  tmpvar_20 = texture2DProj (_ShadowMapTexture, tmpvar_19);
  highp float tmpvar_21;
  if ((tmpvar_20.x < (tmpvar_19.z / tmpvar_19.w))) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  shadow = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = clamp ((shadow + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (atten * tmpvar_16);
  atten = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_25;
  tmpvar_25 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = (spec * clamp (tmpvar_23, 0.0, 1.0));
  spec = tmpvar_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (_LightColor.xyz * (tmpvar_24 * tmpvar_23));
  res.xyz = tmpvar_28;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_27 * tmpvar_29);
  res.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res * tmpvar_31);
  res = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = tmpvar_10;
  mediump float tmpvar_14;
  tmpvar_14 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_15;
  tmpvar_15 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_15;
  mediump float tmpvar_16;
  tmpvar_16 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (spec * clamp (tmpvar_13, 0.0, 1.0));
  spec = tmpvar_17;
  highp vec3 tmpvar_18;
  tmpvar_18 = (_LightColor.xyz * (tmpvar_14 * tmpvar_13));
  res.xyz = tmpvar_18;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_19;
  tmpvar_19 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_17 * tmpvar_19);
  res.w = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_22;
  tmpvar_22 = (res * tmpvar_21);
  res = tmpvar_22;
  tmpvar_1 = tmpvar_22.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = -(_LightDir.xyz);
  lightDir = tmpvar_9;
  mediump float tmpvar_10;
  lowp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_ShadowMapTexture, tmpvar_2);
  highp float tmpvar_12;
  tmpvar_12 = clamp ((tmpvar_11.x + clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0)), 0.0, 1.0);
  tmpvar_10 = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_7;
  lowp vec4 tmpvar_14;
  tmpvar_14 = texture2D (_LightTexture0, (_LightMatrix0 * tmpvar_13).xy);
  highp float tmpvar_15;
  tmpvar_15 = (tmpvar_10 * tmpvar_14.w);
  mediump float tmpvar_16;
  tmpvar_16 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = (spec * clamp (tmpvar_15, 0.0, 1.0));
  spec = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (_LightColor.xyz * (tmpvar_16 * tmpvar_15));
  res.xyz = tmpvar_20;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_19 * tmpvar_21);
  res.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res * tmpvar_23);
  res = tmpvar_24;
  tmpvar_1 = tmpvar_24.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (atten * tmpvar_14);
  atten = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec * clamp (tmpvar_18, 0.0, 1.0));
  spec = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res.xyz = tmpvar_23;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_24;
  tmpvar_24 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_25;
  tmpvar_25 = (tmpvar_22 * tmpvar_24);
  res.w = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_27;
  tmpvar_27 = (res * tmpvar_26);
  res = tmpvar_27;
  tmpvar_1 = tmpvar_27.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = ((length (tmpvar_9) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_14;
  highp vec4 packDist;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, tmpvar_9);
  packDist = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp float tmpvar_17;
  if ((tmpvar_16 < tmpvar_13)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  tmpvar_14 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  lowp vec4 tmpvar_19;
  tmpvar_19 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_18).xyz);
  highp float tmpvar_20;
  tmpvar_20 = ((atten * tmpvar_14) * tmpvar_19.w);
  atten = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_22;
  tmpvar_22 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = (spec * clamp (tmpvar_20, 0.0, 1.0));
  spec = tmpvar_24;
  highp vec3 tmpvar_25;
  tmpvar_25 = (_LightColor.xyz * (tmpvar_21 * tmpvar_20));
  res.xyz = tmpvar_25;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_24 * tmpvar_26);
  res.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res * tmpvar_28);
  res = tmpvar_29;
  tmpvar_1 = tmpvar_29.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 unity_World2Shadow;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ShadowOffsets[4];
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (_LightPos.xyz - tmpvar_7);
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize (tmpvar_9);
  lightDir = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = tmpvar_7;
  highp vec4 tmpvar_12;
  tmpvar_12 = (_LightMatrix0 * tmpvar_11);
  lowp float tmpvar_13;
  tmpvar_13 = texture2DProj (_LightTexture0, tmpvar_12).w;
  atten = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2D (_LightTextureB0, tmpvar_14);
  atten = ((atten * float((tmpvar_12.w < 0.0))) * tmpvar_15.w);
  mediump float tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = clamp (((tmpvar_8 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_7;
  highp vec4 tmpvar_19;
  tmpvar_19 = (unity_World2Shadow * tmpvar_18);
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 tmpvar_20;
  tmpvar_20 = (tmpvar_19.xyz / tmpvar_19.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[0].xy));
  shadowVals.x = tmpvar_21.x;
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[1].xy));
  shadowVals.y = tmpvar_22.x;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[2].xy));
  shadowVals.z = tmpvar_23.x;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture2D (_ShadowMapTexture, (tmpvar_20.xy + _ShadowOffsets[3].xy));
  shadowVals.w = tmpvar_24.x;
  bvec4 tmpvar_25;
  tmpvar_25 = lessThan (shadowVals, tmpvar_20.zzzz);
  highp float tmpvar_26;
  if (tmpvar_25.x) {
    tmpvar_26 = _LightShadowData.x;
  } else {
    tmpvar_26 = 1.0;
  };
  highp float tmpvar_27;
  if (tmpvar_25.y) {
    tmpvar_27 = _LightShadowData.x;
  } else {
    tmpvar_27 = 1.0;
  };
  highp float tmpvar_28;
  if (tmpvar_25.z) {
    tmpvar_28 = _LightShadowData.x;
  } else {
    tmpvar_28 = 1.0;
  };
  highp float tmpvar_29;
  if (tmpvar_25.w) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  highp vec4 tmpvar_30;
  tmpvar_30.x = tmpvar_26;
  tmpvar_30.y = tmpvar_27;
  tmpvar_30.z = tmpvar_28;
  tmpvar_30.w = tmpvar_29;
  shadows = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_32;
  tmpvar_32 = clamp ((tmpvar_31 + tmpvar_17), 0.0, 1.0);
  tmpvar_16 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (atten * tmpvar_16);
  atten = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_35;
  tmpvar_35 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_35;
  mediump float tmpvar_36;
  tmpvar_36 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_36;
  highp float tmpvar_37;
  tmpvar_37 = (spec * clamp (tmpvar_33, 0.0, 1.0));
  spec = tmpvar_37;
  highp vec3 tmpvar_38;
  tmpvar_38 = (_LightColor.xyz * (tmpvar_34 * tmpvar_33));
  res.xyz = tmpvar_38;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_39;
  tmpvar_39 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_40;
  tmpvar_40 = (tmpvar_37 * tmpvar_39);
  res.w = tmpvar_40;
  highp float tmpvar_41;
  tmpvar_41 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_42;
  tmpvar_42 = (res * tmpvar_41);
  res = tmpvar_42;
  tmpvar_1 = tmpvar_42.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_25;
  tmpvar_25 = (atten * tmpvar_24);
  atten = tmpvar_25;
  mediump float tmpvar_26;
  tmpvar_26 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_27;
  tmpvar_27 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = (spec * clamp (tmpvar_25, 0.0, 1.0));
  spec = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (_LightColor.xyz * (tmpvar_26 * tmpvar_25));
  res.xyz = tmpvar_30;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_31;
  tmpvar_31 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_32;
  tmpvar_32 = (tmpvar_29 * tmpvar_31);
  res.w = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_34;
  tmpvar_34 = (res * tmpvar_33);
  res = tmpvar_34;
  tmpvar_1 = tmpvar_34.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;


uniform highp vec4 _ProjectionParams;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  highp vec4 o_i0;
  highp vec4 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 0.5);
  o_i0 = tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = (tmpvar_2.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_3 + tmpvar_2.w);
  o_i0.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_i0;
  xlv_TEXCOORD1 = mix (((gl_ModelViewMatrix * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(float((_glesNormal.z != 0.0))));
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp vec4 unity_LightmapFade;
uniform highp vec4 _ZBufferParams;
uniform highp vec3 _WorldSpaceCameraPos;
uniform samplerCube _ShadowMapTexture;
uniform highp vec4 _ProjectionParams;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform highp vec4 _LightShadowData;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightPos;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _LightColor;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _CameraNormalsTexture;
uniform sampler2D _CameraDepthTexture;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res;
  highp float spec;
  mediump vec3 h;
  highp float atten;
  mediump vec3 lightDir;
  highp float depth;
  mediump vec3 normal;
  mediump vec4 nspec;
  highp vec2 tmpvar_2;
  tmpvar_2 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraNormalsTexture, tmpvar_2);
  nspec = tmpvar_3;
  normal = normalize (((nspec.xyz * 2.0) - 1.0));
  lowp float tmpvar_4;
  tmpvar_4 = texture2D (_CameraDepthTexture, tmpvar_2).x;
  depth = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * depth) + _ZBufferParams.y)));
  depth = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * tmpvar_5);
  highp vec3 tmpvar_7;
  tmpvar_7 = (_CameraToWorld * tmpvar_6).xyz;
  highp float tmpvar_8;
  tmpvar_8 = mix (tmpvar_6.z, distance (tmpvar_7, unity_ShadowFadeCenterAndType.xyz), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_7 - _LightPos.xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = -(normalize (tmpvar_9));
  lightDir = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = vec2((dot (tmpvar_9, tmpvar_9) * _LightPos.w));
  lowp float tmpvar_12;
  tmpvar_12 = texture2D (_LightTextureB0, tmpvar_11).w;
  atten = tmpvar_12;
  mediump vec4 shadows;
  highp vec4 shadowVals;
  highp vec3 vec_i0_i0;
  vec_i0_i0 = (tmpvar_9 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist;
  lowp vec4 tmpvar_13;
  tmpvar_13 = textureCube (_ShadowMapTexture, vec_i0_i0);
  packDist = tmpvar_13;
  shadowVals.x = dot (packDist, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1;
  vec_i0_i1 = (tmpvar_9 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_i0;
  lowp vec4 tmpvar_14;
  tmpvar_14 = textureCube (_ShadowMapTexture, vec_i0_i1);
  packDist_i0 = tmpvar_14;
  shadowVals.y = dot (packDist_i0, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2;
  vec_i0_i1_i2 = (tmpvar_9 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1;
  lowp vec4 tmpvar_15;
  tmpvar_15 = textureCube (_ShadowMapTexture, vec_i0_i1_i2);
  packDist_i0_i1 = tmpvar_15;
  shadowVals.z = dot (packDist_i0_i1, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec3 vec_i0_i1_i2_i3;
  vec_i0_i1_i2_i3 = (tmpvar_9 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_i0_i1_i2;
  lowp vec4 tmpvar_16;
  tmpvar_16 = textureCube (_ShadowMapTexture, vec_i0_i1_i2_i3);
  packDist_i0_i1_i2 = tmpvar_16;
  shadowVals.w = dot (packDist_i0_i1_i2, vec4(1.0, 0.00392157, 1.53787e-005, 0.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = vec4(((length (tmpvar_9) * _LightPositionRange.w) * 0.97));
  bvec4 tmpvar_18;
  tmpvar_18 = lessThan (shadowVals, tmpvar_17);
  highp float tmpvar_19;
  if (tmpvar_18.x) {
    tmpvar_19 = _LightShadowData.x;
  } else {
    tmpvar_19 = 1.0;
  };
  highp float tmpvar_20;
  if (tmpvar_18.y) {
    tmpvar_20 = _LightShadowData.x;
  } else {
    tmpvar_20 = 1.0;
  };
  highp float tmpvar_21;
  if (tmpvar_18.z) {
    tmpvar_21 = _LightShadowData.x;
  } else {
    tmpvar_21 = 1.0;
  };
  highp float tmpvar_22;
  if (tmpvar_18.w) {
    tmpvar_22 = _LightShadowData.x;
  } else {
    tmpvar_22 = 1.0;
  };
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_19;
  tmpvar_23.y = tmpvar_20;
  tmpvar_23.z = tmpvar_21;
  tmpvar_23.w = tmpvar_22;
  shadows = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = dot (shadows, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_7;
  lowp vec4 tmpvar_26;
  tmpvar_26 = textureCube (_LightTexture0, (_LightMatrix0 * tmpvar_25).xyz);
  highp float tmpvar_27;
  tmpvar_27 = ((atten * tmpvar_24) * tmpvar_26.w);
  atten = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = max (0.0, dot (lightDir, normal));
  highp vec3 tmpvar_29;
  tmpvar_29 = normalize ((lightDir - normalize ((tmpvar_7 - _WorldSpaceCameraPos))));
  h = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = pow (max (0.0, dot (h, normal)), (nspec.w * 128.0));
  spec = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (spec * clamp (tmpvar_27, 0.0, 1.0));
  spec = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (_LightColor.xyz * (tmpvar_28 * tmpvar_27));
  res.xyz = tmpvar_32;
  lowp vec3 c;
  c = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_31 * tmpvar_33);
  res.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - ((tmpvar_8 * unity_LightmapFade.z) + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res * tmpvar_35);
  res = tmpvar_36;
  tmpvar_1 = tmpvar_36.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
}
 }
}
Fallback Off
}
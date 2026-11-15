Shader "MADFINGER/FX/Lightning bolt FX" {
Properties {
 _MainTex ("Main tex", 2D) = "white" {}
 _Duration ("Duration (x - ON, y - OFF)", Vector) = (0.5,2,0,0)
 _Amplitude ("Amplitude (x - horizontal, y - vertical)", Vector) = (0.2,0.01,0,0)
 _NoiseFreqs ("NoiseFreqs", Vector) = (4,8,16,32)
 _NoiseSpeeds ("NoiseSpeeds", Vector) = (3.2,2.3,0.5,1)
 _NoiseAmps ("NoiseAmps", Vector) = (2,1,0.5,0.125)
 _OtherParams ("x - line width, y - inv wave speed, z - aspect ratio", Vector) = (0.2,0.025,0,0)
 _Color ("Color", Color) = (0.7,0.8,1.3,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD;

uniform highp vec4 _Time;
uniform highp vec4 _OtherParams;
uniform highp vec4 _NoiseSpeeds;
uniform highp vec4 _NoiseFreqs;
uniform highp vec4 _NoiseAmps;
uniform highp vec4 _Duration;
uniform highp vec4 _Color;
uniform highp vec4 _Amplitude;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec4 tmpvar_2;
  tmpvar_2 = _glesVertex;
  highp vec2 offsDir2D;
  highp vec4 tmpvar_3;
  lowp vec4 tmpvar_4;
  highp vec4 sdir;
  offsDir2D = vec2(1.0, -1.0);
  highp vec3 tmpvar_5;
  tmpvar_5 = normalize (tmpvar_1);
  highp float tmpvar_6;
  tmpvar_6 = abs (tmpvar_5.y);
  vec3 tmpvar_7;
  if ((tmpvar_6 > 0.999)) {
    tmpvar_7 = vec3(0.0, 0.0, 1.0);
  } else {
    tmpvar_7 = vec3(0.0, 1.0, 0.0);
  };
  highp vec3 tmpvar_8;
  tmpvar_8 = normalize (cross (tmpvar_7, tmpvar_5));
  highp vec3 tmpvar_9;
  tmpvar_9 = cross (tmpvar_5, tmpvar_8);
  highp float tmpvar_10;
  tmpvar_10 = (_glesMultiTexCoord0.x * 0.2);
  highp float tmpvar_11;
  tmpvar_11 = ((_Time.y * 0.5) + (_glesMultiTexCoord1.x * 536.375));
  highp vec4 tmpvar_12;
  tmpvar_12 = ((fract ((((0.159155 * 78.233) * vec4((_glesMultiTexCoord1.x * 63.2771))) + 0.5)) * 2.0) - 1.0);
  highp vec4 tmpvar_13;
  tmpvar_13 = (vec4(0.7, 0.7, 0.7, 0.7) + (fract (((4.0 * 43758.5) * (tmpvar_12 - (tmpvar_12 * abs (tmpvar_12))))) * 0.3));
  highp float tmpvar_14;
  tmpvar_14 = (_Duration.x * tmpvar_13.x);
  highp float y;
  y = (tmpvar_14 + (_Duration.y * tmpvar_13.y));
  float tmpvar_15;
  tmpvar_15 = ((tmpvar_11 - (tmpvar_10 * _OtherParams.y)) / y);
  float tmpvar_16;
  tmpvar_16 = (fract (abs (tmpvar_15)) * y);
  float tmpvar_17;
  if ((tmpvar_15 >= 0.0)) {
    tmpvar_17 = tmpvar_16;
  } else {
    tmpvar_17 = -(tmpvar_16);
  };
  highp float tmpvar_18;
  tmpvar_18 = (smoothstep (0.0, (tmpvar_14 * 0.1), tmpvar_17) * (1.0 - smoothstep ((tmpvar_14 * 0.9), tmpvar_14, tmpvar_17)));
  highp vec2 offs_i0;
  highp vec4 tmpvar_19;
  tmpvar_19 = (((vec4(tmpvar_10) + vec4(183.818, 1041.63, 3860.17, 5575.81)) + (tmpvar_11 * _NoiseSpeeds)) * _NoiseFreqs);
  highp vec4 tmpvar_20;
  tmpvar_20 = (((vec4(tmpvar_10) + vec4(5575.81, 183.818, 1041.63, 3860.17)) + (tmpvar_11 * _NoiseSpeeds)) * _NoiseFreqs);
  highp vec4 tmpvar_21;
  tmpvar_21 = fract (tmpvar_19);
  highp vec4 tmpvar_22;
  tmpvar_22 = ((fract ((((0.159155 * 78.233) * (tmpvar_19 - tmpvar_21)) + 0.5)) * 2.0) - 1.0);
  highp vec4 tmpvar_23;
  tmpvar_23 = ((fract ((((0.159155 * 78.233) * ((tmpvar_19 - tmpvar_21) + 1.0)) + 0.5)) * 2.0) - 1.0);
  highp vec4 tmpvar_24;
  tmpvar_24 = fract (tmpvar_20);
  highp vec4 tmpvar_25;
  tmpvar_25 = ((fract ((((0.159155 * 78.233) * (tmpvar_20 - tmpvar_24)) + 0.5)) * 2.0) - 1.0);
  highp vec4 tmpvar_26;
  tmpvar_26 = ((fract ((((0.159155 * 78.233) * ((tmpvar_20 - tmpvar_24) + 1.0)) + 0.5)) * 2.0) - 1.0);
  offs_i0.x = dot (((mix (fract (((4.0 * 43758.5) * (tmpvar_22 - (tmpvar_22 * abs (tmpvar_22))))), fract (((4.0 * 43758.5) * (tmpvar_23 - (tmpvar_23 * abs (tmpvar_23))))), ((tmpvar_21 * tmpvar_21) * (vec4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_21)))) * 2.0) - 1.0), _NoiseAmps);
  offs_i0.y = dot (((mix (fract (((4.0 * 43758.5) * (tmpvar_25 - (tmpvar_25 * abs (tmpvar_25))))), fract (((4.0 * 43758.5) * (tmpvar_26 - (tmpvar_26 * abs (tmpvar_26))))), ((tmpvar_24 * tmpvar_24) * (vec4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_24)))) * 2.0) - 1.0), _NoiseAmps);
  highp vec2 tmpvar_27;
  tmpvar_27 = ((offs_i0 * _Amplitude.xy) * tmpvar_18);
  tmpvar_2.xyz = ((_glesVertex.xyz + (tmpvar_8 * tmpvar_27.x)) + (tmpvar_9 * tmpvar_27.y));
  highp vec4 tmpvar_28;
  tmpvar_28.w = 1.0;
  tmpvar_28.xyz = tmpvar_2.xyz;
  highp vec4 tmpvar_29;
  tmpvar_29 = (gl_ModelViewProjectionMatrix * tmpvar_28);
  highp vec4 tmpvar_30;
  tmpvar_30.w = 1.0;
  tmpvar_30.xyz = (tmpvar_2.xyz + tmpvar_1);
  highp vec4 tmpvar_31;
  tmpvar_31 = (gl_ModelViewProjectionMatrix * tmpvar_30);
  sdir.xy = normalize (((tmpvar_31.xy / tmpvar_31.w) - (tmpvar_29.xy / tmpvar_29.w)));
  highp vec4 tmpvar_32;
  tmpvar_32 = (gl_ModelViewProjectionMatrix * tmpvar_2);
  tmpvar_3 = tmpvar_32;
  offsDir2D.y = (-1.0 * _OtherParams.z);
  tmpvar_3.xy = (tmpvar_32.xy + ((((sdir.yx * offsDir2D) * _OtherParams.x) * _glesMultiTexCoord0.y) * tmpvar_18));
  highp vec4 tmpvar_33;
  tmpvar_33.xyz = (((_Color.xyz * tmpvar_18) * clamp ((tmpvar_32.w - 1.0), 0.0, 1.0)) * 2.0);
  tmpvar_33.w = clamp ((1.0 - pow ((2.0 * abs ((0.5 - _glesMultiTexCoord1.y))), 8.0)), 0.0, 1.0);
  tmpvar_4 = tmpvar_33;
  gl_Position = tmpvar_3;
  xlv_TEXCOORD = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD;
void main ()
{
  lowp float c;
  highp float tmpvar_1;
  tmpvar_1 = ((1.0 - abs (xlv_TEXCOORD.y)) * xlv_TEXCOORD1.w);
  c = tmpvar_1;
  gl_FragData[0] = ((c * c) * xlv_TEXCOORD1);
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
Shader "MADFINGER/Environment/Blinking emissive" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _IntensityScaleBias ("Intensity scale X / bias Y", Vector) = (1,0.1,0,0)
 _SwitchOnOffDuration ("Switch ON (X) / OFF (Y) duration", Vector) = (1,3,0,0)
 _BlinkingRate ("Blinking rate", Float) = 10
 _RndGridSize ("Randomization grid size", Float) = 5
}
SubShader { 
 LOD 100
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

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;

uniform highp vec4 _Time;
uniform highp vec2 _SwitchOnOffDuration;
uniform highp vec2 _IntensityScaleBias;
uniform highp float _BlinkingRate;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec4 tmpvar_1;
  highp float seed;
  lowp float tmpvar_2;
  tmpvar_2 = (dot (_glesColor, _glesColor) * 40.0);
  seed = tmpvar_2;
  highp float time_i0;
  time_i0 = (_Time.y * _BlinkingRate);
  highp float tmpvar_3;
  tmpvar_3 = float((abs (cos (((17.0 * sin ((time_i0 * 5.0))) + (10.0 * sin (((seed + (time_i0 * 3.0)) + 7.993)))))) > 0.5));
  highp vec2 tmpvar_4;
  tmpvar_4 = (_SwitchOnOffDuration * (0.8 + (0.4 * fract (seed))));
  highp float y;
  y = (tmpvar_4.x + tmpvar_4.y);
  float tmpvar_5;
  tmpvar_5 = ((_Time.y + seed) / y);
  float tmpvar_6;
  tmpvar_6 = (fract (abs (tmpvar_5)) * y);
  float tmpvar_7;
  if ((tmpvar_5 >= 0.0)) {
    tmpvar_7 = tmpvar_6;
  } else {
    tmpvar_7 = -(tmpvar_6);
  };
  highp vec4 tmpvar_8;
  tmpvar_8 = (((_glesColor * (tmpvar_3 * float((tmpvar_7 < tmpvar_4.x)))) * _IntensityScaleBias.x) + _IntensityScaleBias.y);
  tmpvar_1 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_TEXCOORD1);
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
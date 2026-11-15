Shader "MADFINGER/FX/Blood FX blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Params ("Params", Vector) = (0,1,0,0)
 _ColorBooster ("Color booster", Color) = (0.5,0,0,0)
}
SubShader { 
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend DstColor SrcColor
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX

varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Time;
uniform highp vec4 _Params;
uniform highp vec4 _ColorBooster;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_3 = normalize (-(_glesVertex.xy));
  highp float tmpvar_4;
  tmpvar_4 = _Params.x;
  float tmpvar_5;
  tmpvar_5 = ((_Time.y * _Params.y) / 1.25);
  float tmpvar_6;
  tmpvar_6 = (fract (abs (tmpvar_5)) * 1.25);
  float tmpvar_7;
  if ((tmpvar_5 >= 0.0)) {
    tmpvar_7 = tmpvar_6;
  } else {
    tmpvar_7 = -(tmpvar_6);
  };
  highp float tmpvar_8;
  if ((tmpvar_7 < 0.5)) {
    float tmpvar_9;
    tmpvar_9 = (tmpvar_7 / 0.5);
    float tmpvar_10;
    tmpvar_10 = (fract (abs (tmpvar_9)) * 0.5);
    float tmpvar_11;
    if ((tmpvar_9 >= 0.0)) {
      tmpvar_11 = tmpvar_10;
    } else {
      tmpvar_11 = -(tmpvar_10);
    };
    highp float tmpvar_12;
    tmpvar_12 = (20.0 * tmpvar_11);
    tmpvar_8 = (tmpvar_12 * exp ((1.0 - tmpvar_12)));
  } else {
    tmpvar_8 = 0.0;
  };
  highp float tmpvar_13;
  if ((_Params.x > 0.0)) {
    tmpvar_13 = mix (0.1, 0.0, tmpvar_4);
  } else {
    tmpvar_13 = 0.0;
  };
  highp float tmpvar_14;
  if ((_Params.x > 0.0)) {
    tmpvar_14 = mix (1.0, 0.0, tmpvar_4);
  } else {
    tmpvar_14 = 0.0;
  };
  highp vec4 tmpvar_15;
  tmpvar_15.zw = vec2(0.0, 1.0);
  tmpvar_15.xy = (_glesVertex.xy * 2.0);
  tmpvar_1 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.xyz = (((_ColorBooster.xyz * tmpvar_14) * _glesColor.w) * tmpvar_8);
  tmpvar_16.w = _glesColor.w;
  tmpvar_2 = tmpvar_16;
  tmpvar_1.xy = (tmpvar_15.xy - ((tmpvar_3 * (_Params.x + (tmpvar_8 * tmpvar_13))) * _glesColor.w));
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_COLOR = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD0);
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
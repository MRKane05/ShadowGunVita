Shader "MADFINGER/FX/Blood FX nonblended" {
Properties {
 _Params ("Params", Vector) = (0,1,0,0)
 _Color ("Color", Color) = (0,0,0,0)
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
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX

uniform highp vec4 _Time;
uniform highp vec4 _Params;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = normalize (-(_glesVertex.xy));
  highp float tmpvar_3;
  tmpvar_3 = _Params.x;
  float tmpvar_4;
  tmpvar_4 = ((_Time.y * _Params.y) / 1.25);
  float tmpvar_5;
  tmpvar_5 = (fract (abs (tmpvar_4)) * 1.25);
  float tmpvar_6;
  if ((tmpvar_4 >= 0.0)) {
    tmpvar_6 = tmpvar_5;
  } else {
    tmpvar_6 = -(tmpvar_5);
  };
  highp float tmpvar_7;
  if ((tmpvar_6 < 0.5)) {
    float tmpvar_8;
    tmpvar_8 = (tmpvar_6 / 0.5);
    float tmpvar_9;
    tmpvar_9 = (fract (abs (tmpvar_8)) * 0.5);
    float tmpvar_10;
    if ((tmpvar_8 >= 0.0)) {
      tmpvar_10 = tmpvar_9;
    } else {
      tmpvar_10 = -(tmpvar_9);
    };
    highp float tmpvar_11;
    tmpvar_11 = (20.0 * tmpvar_10);
    tmpvar_7 = (tmpvar_11 * exp ((1.0 - tmpvar_11)));
  } else {
    tmpvar_7 = 0.0;
  };
  highp float tmpvar_12;
  if ((_Params.x > 0.0)) {
    tmpvar_12 = mix (0.1, 0.0, tmpvar_3);
  } else {
    tmpvar_12 = 0.0;
  };
  if ((_Params.x > 0.0)) {
  };
  highp vec4 tmpvar_13;
  tmpvar_13.zw = vec2(0.0, 1.0);
  tmpvar_13.xy = (_glesVertex.xy * 2.0);
  tmpvar_1 = tmpvar_13;
  tmpvar_1.xy = (tmpvar_13.xy - ((tmpvar_2 * (_Params.x + (tmpvar_7 * tmpvar_12))) * _glesColor.w));
  gl_Position = tmpvar_1;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Color;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _Color;
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
}
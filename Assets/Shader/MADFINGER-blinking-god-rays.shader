// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Transparent/Blinking GodRays" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _TimeOnDuration ("ON duration", Float) = 0.5
 _TimeOffDuration ("OFF duration", Float) = 0.5
 _BlinkingTimeOffsScale ("Blinking time offset scale (seconds)", Float) = 5
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _NoiseAmount ("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
 _Color ("Color", Color) = (1,1,1,1)
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
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float _Bias;
  float _BlinkingTimeOffsScale;
  float4 _Color;
  float _FadeOutDistFar;
  float _FadeOutDistNear;
  sampler2D _MainTex;
  float _MaxGrowSize;
  float _Multiplier;
  float _NoiseAmount;
  float _SizeGrowEndDist;
  float _SizeGrowStartDist;
  float _TimeOffDuration;
  float _TimeOnDuration;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 tmpvar_1;
    tmpvar_1 = normalize(v.normal);
    half4 tmpvar_2;
    float4 mdlPos;
    float tmpvar_3;
    tmpvar_3 = (_Time.y + (_BlinkingTimeOffsScale * v.color.z));
    float tmpvar_4;
    tmpvar_4 = length((mul(UNITY_MATRIX_MV, v.vertex)).xyz);
    float tmpvar_5;
    tmpvar_5 = clamp((tmpvar_4 / _FadeOutDistNear), 0.0, 1.0);
    float tmpvar_6;
    tmpvar_6 = (1.0 - clamp((max((tmpvar_4 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
    float y;
    y = (_TimeOnDuration + _TimeOffDuration);
    float tmpvar_7;
    tmpvar_7 = (tmpvar_3 / y);
    float tmpvar_8;
    tmpvar_8 = (frac(abs(tmpvar_7)) * y);
    float tmpvar_9;
    if (tmpvar_7 >= 0.0) {
      tmpvar_9 = tmpvar_8;
    } else {
      tmpvar_9 = -(tmpvar_8);
    }
    float tmpvar_10;
    tmpvar_10 = (smoothstep(0.0, (_TimeOnDuration * 0.25), tmpvar_9) * (1.0 - smoothstep((_TimeOnDuration * 0.75), _TimeOnDuration, tmpvar_9)));
    float tmpvar_11;
    tmpvar_11 = (tmpvar_3 * (6.28319 / _TimeOnDuration));
    float tmpvar_12;
    tmpvar_12 = ((_NoiseAmount * (sin(tmpvar_11) * ((0.5 * cos(((tmpvar_11 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
    float tmpvar_13;
    tmpvar_13 = min((max((tmpvar_4 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
    float tmpvar_14;
    if (_NoiseAmount < 0.01) {
      tmpvar_14 = tmpvar_10;
    } else {
      tmpvar_14 = tmpvar_12;
    }
    float tmpvar_15;
    tmpvar_15 = (tmpvar_5 * tmpvar_5);
    mdlPos = v.vertex;
    mdlPos.xyz = (v.vertex.xyz + ((((tmpvar_13 * tmpvar_13) * _MaxGrowSize) * v.color.w) * tmpvar_1));
    float4 tmpvar_16;
    tmpvar_16 = (((((tmpvar_15 * tmpvar_15) * (tmpvar_6 * tmpvar_6)) * _Color) * _Multiplier) * (tmpvar_14 + _Bias));
    tmpvar_2 = tmpvar_16;
    o.pos = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, mdlPos)));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
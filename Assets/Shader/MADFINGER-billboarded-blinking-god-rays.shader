// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Transparent/Blinking GodRays Billboarded" {
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
 _VerticalBillboarding ("Vertical billboarding amount", Range(0,1)) = 1
 _ViewerOffset ("Viewer offset", Float) = 0
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
  // uniform scale of the object (Unity 3.5 kept it apart from _World2Object)
  inline float sg_ObjScale () { return length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); }
  // Unity 3.5 _World2Object = inverse world matrix with the scale removed (code multiplies by unity_Scale.w)
  inline float4x4 sg_World2Object () { float s = sg_ObjScale(); float4x4 m = unity_WorldToObject; m[0] *= s; m[1] *= s; m[2] *= s; return m; }
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
  float _VerticalBillboarding;
  float _ViewerOffset;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
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
    float3 localDir;
    half3 tmpvar_3;
    tmpvar_3.z = 0.0;
    tmpvar_3.xy = (float2(0.5, 0.5) - v.color.xy);
    float3 tmpvar_4;
    tmpvar_4 = (tmpvar_3 * v.texcoord1.xyy);
    float3 tmpvar_5;
    tmpvar_5 = (v.vertex.xyz + tmpvar_4);
    float4 tmpvar_6;
    tmpvar_6.w = 1.0;
    tmpvar_6.xyz = _WorldSpaceCameraPos;
    float3 tmpvar_7;
    tmpvar_7 = ((mul(sg_World2Object(), tmpvar_6)).xyz - tmpvar_5);
    localDir = tmpvar_7;
    localDir.y = lerp(0.0, tmpvar_7.y, _VerticalBillboarding);
    float tmpvar_8;
    tmpvar_8 = length(localDir);
    float3 dir;
    dir = (localDir / tmpvar_8);
    float tmpvar_9;
    tmpvar_9 = abs(dir.y);
    float3 tmpvar_10;
    if (tmpvar_9 > 0.999) {
      tmpvar_10 = float3(0.0, 0.0, 1.0);
    } else {
      tmpvar_10 = float3(0.0, 1.0, 0.0);
    }
    float3 tmpvar_11;
    tmpvar_11 = normalize(cross(tmpvar_10, dir));
    float3 tmpvar_12;
    tmpvar_12 = cross(dir, tmpvar_11);
    float tmpvar_13;
    tmpvar_13 = min((max((tmpvar_8 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
    float3 tmpvar_14;
    tmpvar_14 = (((tmpvar_5 - ((tmpvar_11 * tmpvar_4.x) + (tmpvar_12 * tmpvar_4.y))) + (((tmpvar_11 * tmpvar_1.x) + (tmpvar_12 * tmpvar_1.y)) * (((tmpvar_13 * tmpvar_13) * _MaxGrowSize) * v.color.w))) + (_ViewerOffset * localDir));
    float tmpvar_15;
    tmpvar_15 = (_Time.y + (_BlinkingTimeOffsScale * v.color.z));
    float y;
    y = (_TimeOnDuration + _TimeOffDuration);
    float tmpvar_16;
    tmpvar_16 = (tmpvar_15 / y);
    float tmpvar_17;
    tmpvar_17 = (frac(abs(tmpvar_16)) * y);
    float tmpvar_18;
    if (tmpvar_16 >= 0.0) {
      tmpvar_18 = tmpvar_17;
    } else {
      tmpvar_18 = -(tmpvar_17);
    }
    float tmpvar_19;
    tmpvar_19 = (smoothstep(0.0, (_TimeOnDuration * 0.25), tmpvar_18) * (1.0 - smoothstep((_TimeOnDuration * 0.75), _TimeOnDuration, tmpvar_18)));
    float tmpvar_20;
    tmpvar_20 = (tmpvar_15 * (6.28319 / _TimeOnDuration));
    float tmpvar_21;
    tmpvar_21 = ((_NoiseAmount * (sin(tmpvar_20) * ((0.5 * cos(((tmpvar_20 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
    float tmpvar_22;
    if (_NoiseAmount < 0.01) {
      tmpvar_22 = tmpvar_19;
    } else {
      tmpvar_22 = tmpvar_21;
    }
    float4 tmpvar_23;
    tmpvar_23.w = 1.0;
    tmpvar_23.xyz = tmpvar_14;
    float tmpvar_24;
    tmpvar_24 = clamp((tmpvar_8 / _FadeOutDistNear), 0.0, 1.0);
    float tmpvar_25;
    tmpvar_25 = (1.0 - clamp((max((tmpvar_8 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
    float tmpvar_26;
    tmpvar_26 = (tmpvar_24 * tmpvar_24);
    float4 tmpvar_27;
    tmpvar_27 = (((((tmpvar_26 * tmpvar_26) * (tmpvar_25 * tmpvar_25)) * _Color) * _Multiplier) * (tmpvar_22 + _Bias));
    tmpvar_2 = tmpvar_27;
    o.pos = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, tmpvar_23)));
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
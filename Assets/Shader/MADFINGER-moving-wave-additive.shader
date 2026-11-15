// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Transparent/Moving waves FX" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _WaveSpeed ("Waves speed", Vector) = (0.25,0.25,0.25,0.25)
 _WaveSize ("Waves size", Vector) = (0.25,0.25,0.25,0.25)
 _WaveAmplitude ("Waves amp", Vector) = (1,0,0,0)
 _WaveBias ("Waves bias", Vector) = (0,0,0,0)
 _WaveTimeOffs ("Wave time offs", Vector) = (0,0,0,0)
 _Color0 ("Color0", Color) = (1,1,1,1)
 _Color1 ("Color1", Color) = (1,0,0,1)
 _Color2 ("Color2", Color) = (0,1,0,1)
 _Color3 ("Color3", Color) = (0,0,1,1)
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
  float4 _Color0;
  float4 _Color1;
  float4 _Color2;
  float4 _Color3;
  float _FadeOutDistFar;
  float _FadeOutDistNear;
  sampler2D _MainTex;
  float _MaxGrowSize;
  float _Multiplier;
  float _SizeGrowEndDist;
  float _SizeGrowStartDist;
  float4 _WaveAmplitude;
  float4 _WaveBias;
  float4 _WaveSize;
  float4 _WaveSpeed;
  float4 _WaveTimeOffs;
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
    tmpvar_3 = length((mul(UNITY_MATRIX_MV, v.vertex)).xyz);
    float tmpvar_4;
    tmpvar_4 = clamp((tmpvar_3 / _FadeOutDistNear), 0.0, 1.0);
    float tmpvar_5;
    tmpvar_5 = (1.0 - clamp((max((tmpvar_3 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
    float tmpvar_6;
    tmpvar_6 = min((max((tmpvar_3 - _SizeGrowStartDist), 0.0) / _SizeGrowEndDist), 1.0);
    float4 y;
    y = (float4(1.0, 1.0, 1.0, 1.0) + (2.0 * _WaveSize));
    float4 tmpvar_7;
    tmpvar_7 = (((_Time.y + _WaveTimeOffs) * _WaveSpeed) / y);
    float4 tmpvar_8;
    tmpvar_8 = (frac(abs(tmpvar_7)) * y);
    float tmpvar_9;
    if (tmpvar_7.x >= 0.0) {
      tmpvar_9 = tmpvar_8.x;
    } else {
      tmpvar_9 = -(tmpvar_8.x);
    }
    float tmpvar_10;
    if (tmpvar_7.y >= 0.0) {
      tmpvar_10 = tmpvar_8.y;
    } else {
      tmpvar_10 = -(tmpvar_8.y);
    }
    float tmpvar_11;
    if (tmpvar_7.z >= 0.0) {
      tmpvar_11 = tmpvar_8.z;
    } else {
      tmpvar_11 = -(tmpvar_8.z);
    }
    float tmpvar_12;
    if (tmpvar_7.w >= 0.0) {
      tmpvar_12 = tmpvar_8.w;
    } else {
      tmpvar_12 = -(tmpvar_8.w);
    }
    float4 tmpvar_13;
    tmpvar_13.x = tmpvar_9;
    tmpvar_13.y = tmpvar_10;
    tmpvar_13.z = tmpvar_11;
    tmpvar_13.w = tmpvar_12;
    float4 tmpvar_14;
    tmpvar_14 = abs((v.color.xxxx - (-(_WaveSize) + tmpvar_13)));
    float4 tmpvar_15;
    tmpvar_15 = (tmpvar_14 / _WaveSize);
    bool4 tmpvar_16;
    tmpvar_16 = (tmpvar_14 > _WaveSize);
    float4 c_i0;
    c_i0 = (float4(1.0, 1.0, 1.0, 1.0) - ((tmpvar_15 * tmpvar_15) * (float4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_15))));
    float tmpvar_17;
    if (tmpvar_16.x) {
      tmpvar_17 = 0.0;
    } else {
      tmpvar_17 = c_i0.x;
    }
    float tmpvar_18;
    if (tmpvar_16.y) {
      tmpvar_18 = 0.0;
    } else {
      tmpvar_18 = c_i0.y;
    }
    float tmpvar_19;
    if (tmpvar_16.z) {
      tmpvar_19 = 0.0;
    } else {
      tmpvar_19 = c_i0.z;
    }
    float tmpvar_20;
    if (tmpvar_16.w) {
      tmpvar_20 = 0.0;
    } else {
      tmpvar_20 = c_i0.w;
    }
    float4 tmpvar_21;
    tmpvar_21.x = tmpvar_17;
    tmpvar_21.y = tmpvar_18;
    tmpvar_21.z = tmpvar_19;
    tmpvar_21.w = tmpvar_20;
    float4 tmpvar_22;
    tmpvar_22 = ((tmpvar_21 * _WaveAmplitude) + _WaveBias);
    float tmpvar_23;
    tmpvar_23 = (tmpvar_4 * tmpvar_4);
    mdlPos = v.vertex;
    mdlPos.xyz = (v.vertex.xyz + ((((tmpvar_6 * tmpvar_6) * _MaxGrowSize) * v.color.w) * tmpvar_1));
    float4 tmpvar_24;
    tmpvar_24 = ((((tmpvar_23 * tmpvar_23) * (tmpvar_5 * tmpvar_5)) * ((((_Color0 * tmpvar_22.x) + (_Color1 * tmpvar_22.y)) + (_Color2 * tmpvar_22.z)) + (_Color3 * tmpvar_22.w))) * _Multiplier);
    tmpvar_2 = tmpvar_24;
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
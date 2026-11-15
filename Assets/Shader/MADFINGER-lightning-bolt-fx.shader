// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
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
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float4 _Amplitude;
  float4 _Color;
  float4 _Duration;
  float4 _NoiseAmps;
  float4 _NoiseFreqs;
  float4 _NoiseSpeeds;
  float4 _OtherParams;
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 tmpvar_1;
    tmpvar_1 = normalize(v.normal);
    float4 tmpvar_2;
    tmpvar_2 = v.vertex;
    float2 offsDir2D;
    float4 tmpvar_3;
    half4 tmpvar_4;
    float4 sdir;
    offsDir2D = float2(1.0, -1.0);
    float3 tmpvar_5;
    tmpvar_5 = normalize(tmpvar_1);
    float tmpvar_6;
    tmpvar_6 = abs(tmpvar_5.y);
    float3 tmpvar_7;
    if (tmpvar_6 > 0.999) {
      tmpvar_7 = float3(0.0, 0.0, 1.0);
    } else {
      tmpvar_7 = float3(0.0, 1.0, 0.0);
    }
    float3 tmpvar_8;
    tmpvar_8 = normalize(cross(tmpvar_7, tmpvar_5));
    float3 tmpvar_9;
    tmpvar_9 = cross(tmpvar_5, tmpvar_8);
    float tmpvar_10;
    tmpvar_10 = (v.texcoord.x * 0.2);
    float tmpvar_11;
    tmpvar_11 = ((_Time.y * 0.5) + (v.texcoord1.x * 536.375));
    float4 tmpvar_12;
    tmpvar_12 = ((frac((((0.159155 * 78.233) * ((float4)(v.texcoord1.x * 63.2771))) + 0.5)) * 2.0) - 1.0);
    float4 tmpvar_13;
    tmpvar_13 = (float4(0.7, 0.7, 0.7, 0.7) + (frac(((4.0 * 43758.5) * (tmpvar_12 - (tmpvar_12 * abs(tmpvar_12))))) * 0.3));
    float tmpvar_14;
    tmpvar_14 = (_Duration.x * tmpvar_13.x);
    float y;
    y = (tmpvar_14 + (_Duration.y * tmpvar_13.y));
    float tmpvar_15;
    tmpvar_15 = ((tmpvar_11 - (tmpvar_10 * _OtherParams.y)) / y);
    float tmpvar_16;
    tmpvar_16 = (frac(abs(tmpvar_15)) * y);
    float tmpvar_17;
    if (tmpvar_15 >= 0.0) {
      tmpvar_17 = tmpvar_16;
    } else {
      tmpvar_17 = -(tmpvar_16);
    }
    float tmpvar_18;
    tmpvar_18 = (smoothstep(0.0, (tmpvar_14 * 0.1), tmpvar_17) * (1.0 - smoothstep((tmpvar_14 * 0.9), tmpvar_14, tmpvar_17)));
    float2 offs_i0;
    float4 tmpvar_19;
    tmpvar_19 = (((((float4)tmpvar_10) + float4(183.818, 1041.63, 3860.17, 5575.81)) + (tmpvar_11 * _NoiseSpeeds)) * _NoiseFreqs);
    float4 tmpvar_20;
    tmpvar_20 = (((((float4)tmpvar_10) + float4(5575.81, 183.818, 1041.63, 3860.17)) + (tmpvar_11 * _NoiseSpeeds)) * _NoiseFreqs);
    float4 tmpvar_21;
    tmpvar_21 = frac(tmpvar_19);
    float4 tmpvar_22;
    tmpvar_22 = ((frac((((0.159155 * 78.233) * (tmpvar_19 - tmpvar_21)) + 0.5)) * 2.0) - 1.0);
    float4 tmpvar_23;
    tmpvar_23 = ((frac((((0.159155 * 78.233) * ((tmpvar_19 - tmpvar_21) + 1.0)) + 0.5)) * 2.0) - 1.0);
    float4 tmpvar_24;
    tmpvar_24 = frac(tmpvar_20);
    float4 tmpvar_25;
    tmpvar_25 = ((frac((((0.159155 * 78.233) * (tmpvar_20 - tmpvar_24)) + 0.5)) * 2.0) - 1.0);
    float4 tmpvar_26;
    tmpvar_26 = ((frac((((0.159155 * 78.233) * ((tmpvar_20 - tmpvar_24) + 1.0)) + 0.5)) * 2.0) - 1.0);
    offs_i0.x = dot(((lerp(frac(((4.0 * 43758.5) * (tmpvar_22 - (tmpvar_22 * abs(tmpvar_22))))), frac(((4.0 * 43758.5) * (tmpvar_23 - (tmpvar_23 * abs(tmpvar_23))))), ((tmpvar_21 * tmpvar_21) * (float4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_21)))) * 2.0) - 1.0), _NoiseAmps);
    offs_i0.y = dot(((lerp(frac(((4.0 * 43758.5) * (tmpvar_25 - (tmpvar_25 * abs(tmpvar_25))))), frac(((4.0 * 43758.5) * (tmpvar_26 - (tmpvar_26 * abs(tmpvar_26))))), ((tmpvar_24 * tmpvar_24) * (float4(3.0, 3.0, 3.0, 3.0) - (2.0 * tmpvar_24)))) * 2.0) - 1.0), _NoiseAmps);
    float2 tmpvar_27;
    tmpvar_27 = ((offs_i0 * _Amplitude.xy) * tmpvar_18);
    tmpvar_2.xyz = ((v.vertex.xyz + (tmpvar_8 * tmpvar_27.x)) + (tmpvar_9 * tmpvar_27.y));
    float4 tmpvar_28;
    tmpvar_28.w = 1.0;
    tmpvar_28.xyz = tmpvar_2.xyz;
    float4 tmpvar_29;
    tmpvar_29 = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, tmpvar_28)));
    float4 tmpvar_30;
    tmpvar_30.w = 1.0;
    tmpvar_30.xyz = (tmpvar_2.xyz + tmpvar_1);
    float4 tmpvar_31;
    tmpvar_31 = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, tmpvar_30)));
    sdir.xy = normalize(((tmpvar_31.xy / tmpvar_31.w) - (tmpvar_29.xy / tmpvar_29.w)));
    float4 tmpvar_32;
    tmpvar_32 = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, tmpvar_2)));
    tmpvar_3 = tmpvar_32;
    offsDir2D.y = (-1.0 * _OtherParams.z);
    tmpvar_3.xy = (tmpvar_32.xy + ((((sdir.yx * offsDir2D) * _OtherParams.x) * v.texcoord.y) * tmpvar_18));
    float4 tmpvar_33;
    tmpvar_33.xyz = (((_Color.xyz * tmpvar_18) * clamp((tmpvar_32.w - 1.0), 0.0, 1.0)) * 2.0);
    tmpvar_33.w = clamp((1.0 - pow((2.0 * abs((0.5 - v.texcoord1.y))), 8.0)), 0.0, 1.0);
    tmpvar_4 = tmpvar_33;
    o.pos = tmpvar_3;
    o.xlv_TEXCOORD = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_4;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half c;
    float tmpvar_1;
    tmpvar_1 = ((1.0 - abs(i.xlv_TEXCOORD.y)) * i.xlv_TEXCOORD1.w);
    c = tmpvar_1;
    sg_FragColor = ((c * c) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
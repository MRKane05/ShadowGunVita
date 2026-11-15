// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Characters/BRDFLit FX (Supports Backlight)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
 _NoiseTex ("Noise tex", 2D) = "white" {}
 _LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
 _FXColor ("FXColor", Color) = (0,0.97,0.89,1)
 _TimeOffs ("Time offs", Float) = 0
 _Duration ("Duration", Float) = 2
 _Invert ("Invert", Float) = 0
}
SubShader { 
 LOD 400
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ VERTEXLIGHT_ON
  #include "UnityCG.cginc"
  // uniform scale of the object (Unity 3.5 kept it apart from _World2Object)
  inline float sg_ObjScale () { return length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); }
  // Unity 3.5 _World2Object = inverse world matrix with the scale removed (code multiplies by unity_Scale.w)
  inline float4x4 sg_World2Object () { float s = sg_ObjScale(); float4x4 m = unity_WorldToObject; m[0] *= s; m[1] *= s; m[2] *= s; return m; }
  // Unity 3.5 unity_Scale: w = 1 / object scale
  inline float4 sg_unity_Scale () { float s = sg_ObjScale(); return float4(s, s, s, 1.0 / s); }
  sampler2D _BRDFTex;
  sampler2D _BumpMap;
  float4 _BumpMap_ST;
  float _Duration;
  half4 _FXColor;
  float _Invert;
  float _LightProbesLightingAmount;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  sampler2D _NoiseTex;
  float _TimeOffs;
  #if defined(VERTEXLIGHT_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD5 : TEXCOORD5;
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    half xlv_TEXCOORD2 : TEXCOORD2;
    half3 xlv_TEXCOORD1 : TEXCOORD1;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    tmpvar_1.xyz = normalize(v.tangent.xyz);
    tmpvar_1.w = v.tangent.w;
    float3 tmpvar_2;
    tmpvar_2 = normalize(v.normal);
    float4 tmpvar_3;
    half3 tmpvar_4;
    half tmpvar_5;
    half3 tmpvar_6;
    half3 tmpvar_7;
    half3 tmpvar_8;
    half3 tmpvar_9;
    half tmpvar_10;
    float3 SHLighting;
    float3x3 tmpvar_11;
    tmpvar_11[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_11[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_11[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float4 tmpvar_12;
    tmpvar_12.w = 1.0;
    tmpvar_12.xyz = (mul(tmpvar_2, tmpvar_11));
    half3 tmpvar_13;
    half4 normal;
    normal = tmpvar_12;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_14;
    tmpvar_14 = dot(unity_SHAr, normal);
    x1.x = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = dot(unity_SHAg, normal);
    x1.y = tmpvar_15;
    float tmpvar_16;
    tmpvar_16 = dot(unity_SHAb, normal);
    x1.z = tmpvar_16;
    half4 tmpvar_17;
    tmpvar_17 = (normal.xyzz * normal.yzzx);
    float tmpvar_18;
    tmpvar_18 = dot(unity_SHBr, tmpvar_17);
    x2.x = tmpvar_18;
    float tmpvar_19;
    tmpvar_19 = dot(unity_SHBg, tmpvar_17);
    x2.y = tmpvar_19;
    float tmpvar_20;
    tmpvar_20 = dot(unity_SHBb, tmpvar_17);
    x2.z = tmpvar_20;
    half tmpvar_21;
    tmpvar_21 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_21;
    float3 tmpvar_22;
    tmpvar_22 = (unity_SHC.xyz * vC);
    x3 = tmpvar_22;
    tmpvar_13 = ((x1 + x2) + x3);
    SHLighting = tmpvar_13;
    float tmpvar_23;
    tmpvar_23 = frac(((_TimeOffs + _Time.y) / _Duration));
    float3 tmpvar_24;
    tmpvar_24 = clamp((SHLighting + ((float3)(1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
    tmpvar_9 = tmpvar_24;
    float tmpvar_25;
    if (_Invert > 0.0) {
      tmpvar_25 = (1.0 - tmpvar_23);
    } else {
      tmpvar_25 = tmpvar_23;
    }
    tmpvar_10 = tmpvar_25;
    tmpvar_4 = tmpvar_9;
    tmpvar_5 = tmpvar_10;
    tmpvar_3.xy = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    tmpvar_3.zw = ((v.texcoord.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
    float3x3 tmpvar_26;
    tmpvar_26[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_26[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_26[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_27;
    tmpvar_27 = (mul((tmpvar_2 * sg_unity_Scale().w), tmpvar_26));
    float3x3 tmpvar_28;
    tmpvar_28[0] = tmpvar_1.xyz;
    tmpvar_28[1] = (cross(tmpvar_2, tmpvar_1.xyz) * v.tangent.w);
    tmpvar_28[2] = tmpvar_2;
    float3x3 tmpvar_29;
    tmpvar_29[0].x = tmpvar_28[0].x;
    tmpvar_29[0].y = tmpvar_28[1].x;
    tmpvar_29[0].z = tmpvar_28[2].x;
    tmpvar_29[1].x = tmpvar_28[0].y;
    tmpvar_29[1].y = tmpvar_28[1].y;
    tmpvar_29[1].z = tmpvar_28[2].y;
    tmpvar_29[2].x = tmpvar_28[0].z;
    tmpvar_29[2].y = tmpvar_28[1].z;
    tmpvar_29[2].z = tmpvar_28[2].z;
    float3 tmpvar_30;
    tmpvar_30 = (mul((mul(sg_World2Object(), _WorldSpaceLightPos0)).xyz, tmpvar_29));
    tmpvar_6 = tmpvar_30;
    float4 tmpvar_31;
    tmpvar_31.w = 1.0;
    tmpvar_31.xyz = _WorldSpaceCameraPos;
    float3 tmpvar_32;
    tmpvar_32 = normalize((mul((((mul(sg_World2Object(), tmpvar_31)).xyz * sg_unity_Scale().w) - v.vertex.xyz), tmpvar_29)));
    tmpvar_8 = tmpvar_32;
    float3 tmpvar_33;
    tmpvar_33 = (mul(unity_ObjectToWorld, v.vertex)).xyz;
    float4 tmpvar_34;
    tmpvar_34 = (unity_4LightPosX0 - tmpvar_33.x);
    float4 tmpvar_35;
    tmpvar_35 = (unity_4LightPosY0 - tmpvar_33.y);
    float4 tmpvar_36;
    tmpvar_36 = (unity_4LightPosZ0 - tmpvar_33.z);
    float4 tmpvar_37;
    tmpvar_37 = (((tmpvar_34 * tmpvar_34) + (tmpvar_35 * tmpvar_35)) + (tmpvar_36 * tmpvar_36));
    float4 tmpvar_38;
    tmpvar_38 = (max(float4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_34 * tmpvar_27.x) + (tmpvar_35 * tmpvar_27.y)) + (tmpvar_36 * tmpvar_27.z)) * rsqrt(tmpvar_37))) * (1.0 / (1.0 + (tmpvar_37 * unity_4LightAtten0))));
    float3 tmpvar_39;
    tmpvar_39 = ((((unity_LightColor[0].xyz * tmpvar_38.x) + (unity_LightColor[1].xyz * tmpvar_38.y)) + (unity_LightColor[2].xyz * tmpvar_38.z)) + (unity_LightColor[3].xyz * tmpvar_38.w));
    tmpvar_7 = tmpvar_39;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_3;
    o.xlv_TEXCOORD1 = tmpvar_4;
    o.xlv_TEXCOORD2 = tmpvar_5;
    o.xlv_TEXCOORD3 = tmpvar_6;
    o.xlv_TEXCOORD4 = tmpvar_7;
    o.xlv_TEXCOORD5 = tmpvar_8;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half3 tmpvar_1;
    half tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3 = i.xlv_TEXCOORD0.xy;
    float2 tmpvar_4;
    tmpvar_4 = i.xlv_TEXCOORD0.zw;
    tmpvar_1 = i.xlv_TEXCOORD1;
    tmpvar_2 = i.xlv_TEXCOORD2;
    half4 tmpvar_5;
    tmpvar_5 = tex2D(_NoiseTex, (i.xlv_TEXCOORD0.xy * 2.0));
    half4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, tmpvar_3);
    float tmpvar_7;
    tmpvar_7 = (1.0 - clamp(((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
    half tmpvar_8;
    tmpvar_8 = (tmpvar_7 * tmpvar_7);
    half3 tmpvar_9;
    tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
    half3 tmpvar_10;
    tmpvar_10 = ((tex2D(_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
    half4 c_i0;
    half2 tmpvar_11;
    tmpvar_11.x = ((dot(tmpvar_10, i.xlv_TEXCOORD3) * 0.5) + 0.5);
    tmpvar_11.y = dot(tmpvar_10, normalize((i.xlv_TEXCOORD3 + i.xlv_TEXCOORD5)));
    half4 tmpvar_12;
    tmpvar_12 = tex2D(_BRDFTex, tmpvar_11);
    c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
    c_i0.w = ((float)(tmpvar_5.x > tmpvar_2));
    c = c_i0;
    c.xyz = (c_i0.xyz + (tmpvar_9 * i.xlv_TEXCOORD4));
    c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
    sg_FragColor = c;
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD5 : TEXCOORD5;
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    half xlv_TEXCOORD2 : TEXCOORD2;
    half3 xlv_TEXCOORD1 : TEXCOORD1;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 tmpvar_1;
    tmpvar_1.xyz = normalize(v.tangent.xyz);
    tmpvar_1.w = v.tangent.w;
    float3 tmpvar_2;
    tmpvar_2 = normalize(v.normal);
    float4 tmpvar_3;
    half3 tmpvar_4;
    half tmpvar_5;
    half3 tmpvar_6;
    half3 tmpvar_7;
    half3 tmpvar_8;
    half tmpvar_9;
    float3 SHLighting;
    float3x3 tmpvar_10;
    tmpvar_10[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_10[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_10[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float4 tmpvar_11;
    tmpvar_11.w = 1.0;
    tmpvar_11.xyz = (mul(tmpvar_2, tmpvar_10));
    half3 tmpvar_12;
    half4 normal;
    normal = tmpvar_11;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHAr, normal);
    x1.x = tmpvar_13;
    float tmpvar_14;
    tmpvar_14 = dot(unity_SHAg, normal);
    x1.y = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = dot(unity_SHAb, normal);
    x1.z = tmpvar_15;
    half4 tmpvar_16;
    tmpvar_16 = (normal.xyzz * normal.yzzx);
    float tmpvar_17;
    tmpvar_17 = dot(unity_SHBr, tmpvar_16);
    x2.x = tmpvar_17;
    float tmpvar_18;
    tmpvar_18 = dot(unity_SHBg, tmpvar_16);
    x2.y = tmpvar_18;
    float tmpvar_19;
    tmpvar_19 = dot(unity_SHBb, tmpvar_16);
    x2.z = tmpvar_19;
    half tmpvar_20;
    tmpvar_20 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_20;
    float3 tmpvar_21;
    tmpvar_21 = (unity_SHC.xyz * vC);
    x3 = tmpvar_21;
    tmpvar_12 = ((x1 + x2) + x3);
    SHLighting = tmpvar_12;
    float tmpvar_22;
    tmpvar_22 = frac(((_TimeOffs + _Time.y) / _Duration));
    float3 tmpvar_23;
    tmpvar_23 = clamp((SHLighting + ((float3)(1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
    tmpvar_8 = tmpvar_23;
    float tmpvar_24;
    if (_Invert > 0.0) {
      tmpvar_24 = (1.0 - tmpvar_22);
    } else {
      tmpvar_24 = tmpvar_22;
    }
    tmpvar_9 = tmpvar_24;
    tmpvar_4 = tmpvar_8;
    tmpvar_5 = tmpvar_9;
    tmpvar_3.xy = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    tmpvar_3.zw = ((v.texcoord.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
    float3x3 tmpvar_25;
    tmpvar_25[0] = tmpvar_1.xyz;
    tmpvar_25[1] = (cross(tmpvar_2, tmpvar_1.xyz) * v.tangent.w);
    tmpvar_25[2] = tmpvar_2;
    float3x3 tmpvar_26;
    tmpvar_26[0].x = tmpvar_25[0].x;
    tmpvar_26[0].y = tmpvar_25[1].x;
    tmpvar_26[0].z = tmpvar_25[2].x;
    tmpvar_26[1].x = tmpvar_25[0].y;
    tmpvar_26[1].y = tmpvar_25[1].y;
    tmpvar_26[1].z = tmpvar_25[2].y;
    tmpvar_26[2].x = tmpvar_25[0].z;
    tmpvar_26[2].y = tmpvar_25[1].z;
    tmpvar_26[2].z = tmpvar_25[2].z;
    float3 tmpvar_27;
    tmpvar_27 = (mul((mul(sg_World2Object(), _WorldSpaceLightPos0)).xyz, tmpvar_26));
    tmpvar_6 = tmpvar_27;
    float4 tmpvar_28;
    tmpvar_28.w = 1.0;
    tmpvar_28.xyz = _WorldSpaceCameraPos;
    float3 tmpvar_29;
    tmpvar_29 = normalize((mul((((mul(sg_World2Object(), tmpvar_28)).xyz * sg_unity_Scale().w) - v.vertex.xyz), tmpvar_26)));
    tmpvar_7 = tmpvar_29;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_3;
    o.xlv_TEXCOORD1 = tmpvar_4;
    o.xlv_TEXCOORD2 = tmpvar_5;
    o.xlv_TEXCOORD3 = tmpvar_6;
    o.xlv_TEXCOORD4 = float3(0.0, 0.0, 0.0);
    o.xlv_TEXCOORD5 = tmpvar_7;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half3 tmpvar_1;
    half tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3 = i.xlv_TEXCOORD0.xy;
    float2 tmpvar_4;
    tmpvar_4 = i.xlv_TEXCOORD0.zw;
    tmpvar_1 = i.xlv_TEXCOORD1;
    tmpvar_2 = i.xlv_TEXCOORD2;
    half4 tmpvar_5;
    tmpvar_5 = tex2D(_NoiseTex, (i.xlv_TEXCOORD0.xy * 2.0));
    half4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, tmpvar_3);
    float tmpvar_7;
    tmpvar_7 = (1.0 - clamp(((tmpvar_5.x - tmpvar_2) * 4.0), 0.0, 1.0));
    half tmpvar_8;
    tmpvar_8 = (tmpvar_7 * tmpvar_7);
    half3 tmpvar_9;
    tmpvar_9 = (tmpvar_6.xyz * tmpvar_1);
    half3 tmpvar_10;
    tmpvar_10 = ((tex2D(_BumpMap, tmpvar_4).xyz * 2.0) - 1.0);
    half4 c_i0;
    half2 tmpvar_11;
    tmpvar_11.x = ((dot(tmpvar_10, i.xlv_TEXCOORD3) * 0.5) + 0.5);
    tmpvar_11.y = dot(tmpvar_10, normalize((i.xlv_TEXCOORD3 + i.xlv_TEXCOORD5)));
    half4 tmpvar_12;
    tmpvar_12 = tex2D(_BRDFTex, tmpvar_11);
    c_i0.xyz = ((tmpvar_9 * (tmpvar_12.xyz + (tmpvar_6.w * tmpvar_12.w))) * 2.0);
    c_i0.w = ((float)(tmpvar_5.x > tmpvar_2));
    c = c_i0;
    c.xyz = (c_i0.xyz + (tmpvar_9 * i.xlv_TEXCOORD4));
    c.xyz = (c.xyz + (_FXColor * (tmpvar_8 * tmpvar_8)).xyz);
    sg_FragColor = c;
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
Fallback "Diffuse"
}
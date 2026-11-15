// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Characters/BRDFLit (Supports Backlight)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
 _LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
}
SubShader { 
 LOD 400
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
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
  float _LightProbesLightingAmount;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  #if defined(VERTEXLIGHT_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
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
    half3 tmpvar_5;
    half3 tmpvar_6;
    half3 tmpvar_7;
    half3 tmpvar_8;
    float3 SHLighting;
    float3x3 tmpvar_9;
    tmpvar_9[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_9[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_9[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float4 tmpvar_10;
    tmpvar_10.w = 1.0;
    tmpvar_10.xyz = (mul(tmpvar_2, tmpvar_9));
    half3 tmpvar_11;
    half4 normal;
    normal = tmpvar_10;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_12;
    tmpvar_12 = dot(unity_SHAr, normal);
    x1.x = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHAg, normal);
    x1.y = tmpvar_13;
    float tmpvar_14;
    tmpvar_14 = dot(unity_SHAb, normal);
    x1.z = tmpvar_14;
    half4 tmpvar_15;
    tmpvar_15 = (normal.xyzz * normal.yzzx);
    float tmpvar_16;
    tmpvar_16 = dot(unity_SHBr, tmpvar_15);
    x2.x = tmpvar_16;
    float tmpvar_17;
    tmpvar_17 = dot(unity_SHBg, tmpvar_15);
    x2.y = tmpvar_17;
    float tmpvar_18;
    tmpvar_18 = dot(unity_SHBb, tmpvar_15);
    x2.z = tmpvar_18;
    half tmpvar_19;
    tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_19;
    float3 tmpvar_20;
    tmpvar_20 = (unity_SHC.xyz * vC);
    x3 = tmpvar_20;
    tmpvar_11 = ((x1 + x2) + x3);
    SHLighting = tmpvar_11;
    float3 tmpvar_21;
    tmpvar_21 = clamp((SHLighting + ((float3)(1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
    tmpvar_8 = tmpvar_21;
    tmpvar_4 = tmpvar_8;
    tmpvar_3.xy = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    tmpvar_3.zw = ((v.texcoord.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
    float3x3 tmpvar_22;
    tmpvar_22[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_22[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_22[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_23;
    tmpvar_23 = (mul((tmpvar_2 * sg_unity_Scale().w), tmpvar_22));
    float3x3 tmpvar_24;
    tmpvar_24[0] = tmpvar_1.xyz;
    tmpvar_24[1] = (cross(tmpvar_2, tmpvar_1.xyz) * v.tangent.w);
    tmpvar_24[2] = tmpvar_2;
    float3x3 tmpvar_25;
    tmpvar_25[0].x = tmpvar_24[0].x;
    tmpvar_25[0].y = tmpvar_24[1].x;
    tmpvar_25[0].z = tmpvar_24[2].x;
    tmpvar_25[1].x = tmpvar_24[0].y;
    tmpvar_25[1].y = tmpvar_24[1].y;
    tmpvar_25[1].z = tmpvar_24[2].y;
    tmpvar_25[2].x = tmpvar_24[0].z;
    tmpvar_25[2].y = tmpvar_24[1].z;
    tmpvar_25[2].z = tmpvar_24[2].z;
    float3 tmpvar_26;
    tmpvar_26 = (mul((mul(sg_World2Object(), _WorldSpaceLightPos0)).xyz, tmpvar_25));
    tmpvar_5 = tmpvar_26;
    float4 tmpvar_27;
    tmpvar_27.w = 1.0;
    tmpvar_27.xyz = _WorldSpaceCameraPos;
    float3 tmpvar_28;
    tmpvar_28 = normalize((mul((((mul(sg_World2Object(), tmpvar_27)).xyz * sg_unity_Scale().w) - v.vertex.xyz), tmpvar_25)));
    tmpvar_7 = tmpvar_28;
    float3 tmpvar_29;
    tmpvar_29 = (mul(unity_ObjectToWorld, v.vertex)).xyz;
    float4 tmpvar_30;
    tmpvar_30 = (unity_4LightPosX0 - tmpvar_29.x);
    float4 tmpvar_31;
    tmpvar_31 = (unity_4LightPosY0 - tmpvar_29.y);
    float4 tmpvar_32;
    tmpvar_32 = (unity_4LightPosZ0 - tmpvar_29.z);
    float4 tmpvar_33;
    tmpvar_33 = (((tmpvar_30 * tmpvar_30) + (tmpvar_31 * tmpvar_31)) + (tmpvar_32 * tmpvar_32));
    float4 tmpvar_34;
    tmpvar_34 = (max(float4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_30 * tmpvar_23.x) + (tmpvar_31 * tmpvar_23.y)) + (tmpvar_32 * tmpvar_23.z)) * rsqrt(tmpvar_33))) * (1.0 / (1.0 + (tmpvar_33 * unity_4LightAtten0))));
    float3 tmpvar_35;
    tmpvar_35 = ((((unity_LightColor[0].xyz * tmpvar_34.x) + (unity_LightColor[1].xyz * tmpvar_34.y)) + (unity_LightColor[2].xyz * tmpvar_34.z)) + (unity_LightColor[3].xyz * tmpvar_34.w));
    tmpvar_6 = tmpvar_35;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_3;
    o.xlv_TEXCOORD1 = tmpvar_4;
    o.xlv_TEXCOORD2 = tmpvar_5;
    o.xlv_TEXCOORD3 = tmpvar_6;
    o.xlv_TEXCOORD4 = tmpvar_7;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half3 tmpvar_1;
    float2 tmpvar_2;
    tmpvar_2 = i.xlv_TEXCOORD0.xy;
    float2 tmpvar_3;
    tmpvar_3 = i.xlv_TEXCOORD0.zw;
    tmpvar_1 = i.xlv_TEXCOORD1;
    half4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, tmpvar_2);
    half3 tmpvar_5;
    tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
    half3 tmpvar_6;
    tmpvar_6 = ((tex2D(_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
    half4 c_i0;
    half2 tmpvar_7;
    tmpvar_7.x = ((dot(tmpvar_6, i.xlv_TEXCOORD2) * 0.5) + 0.5);
    tmpvar_7.y = dot(tmpvar_6, normalize((i.xlv_TEXCOORD2 + i.xlv_TEXCOORD4)));
    half4 tmpvar_8;
    tmpvar_8 = tex2D(_BRDFTex, tmpvar_7);
    c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
    c_i0.w = 0.0;
    c = c_i0;
    c.xyz = (c_i0.xyz + (tmpvar_5 * i.xlv_TEXCOORD3));
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
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
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
    half3 tmpvar_5;
    half3 tmpvar_6;
    half3 tmpvar_7;
    float3 SHLighting;
    float3x3 tmpvar_8;
    tmpvar_8[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_8[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_8[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float4 tmpvar_9;
    tmpvar_9.w = 1.0;
    tmpvar_9.xyz = (mul(tmpvar_2, tmpvar_8));
    half3 tmpvar_10;
    half4 normal;
    normal = tmpvar_9;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_11;
    tmpvar_11 = dot(unity_SHAr, normal);
    x1.x = tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = dot(unity_SHAg, normal);
    x1.y = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHAb, normal);
    x1.z = tmpvar_13;
    half4 tmpvar_14;
    tmpvar_14 = (normal.xyzz * normal.yzzx);
    float tmpvar_15;
    tmpvar_15 = dot(unity_SHBr, tmpvar_14);
    x2.x = tmpvar_15;
    float tmpvar_16;
    tmpvar_16 = dot(unity_SHBg, tmpvar_14);
    x2.y = tmpvar_16;
    float tmpvar_17;
    tmpvar_17 = dot(unity_SHBb, tmpvar_14);
    x2.z = tmpvar_17;
    half tmpvar_18;
    tmpvar_18 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_18;
    float3 tmpvar_19;
    tmpvar_19 = (unity_SHC.xyz * vC);
    x3 = tmpvar_19;
    tmpvar_10 = ((x1 + x2) + x3);
    SHLighting = tmpvar_10;
    float3 tmpvar_20;
    tmpvar_20 = clamp((SHLighting + ((float3)(1.0 - _LightProbesLightingAmount))), 0.0, 1.0);
    tmpvar_7 = tmpvar_20;
    tmpvar_4 = tmpvar_7;
    tmpvar_3.xy = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    tmpvar_3.zw = ((v.texcoord.xy * _BumpMap_ST.xy) + _BumpMap_ST.zw);
    float3x3 tmpvar_21;
    tmpvar_21[0] = tmpvar_1.xyz;
    tmpvar_21[1] = (cross(tmpvar_2, tmpvar_1.xyz) * v.tangent.w);
    tmpvar_21[2] = tmpvar_2;
    float3x3 tmpvar_22;
    tmpvar_22[0].x = tmpvar_21[0].x;
    tmpvar_22[0].y = tmpvar_21[1].x;
    tmpvar_22[0].z = tmpvar_21[2].x;
    tmpvar_22[1].x = tmpvar_21[0].y;
    tmpvar_22[1].y = tmpvar_21[1].y;
    tmpvar_22[1].z = tmpvar_21[2].y;
    tmpvar_22[2].x = tmpvar_21[0].z;
    tmpvar_22[2].y = tmpvar_21[1].z;
    tmpvar_22[2].z = tmpvar_21[2].z;
    float3 tmpvar_23;
    tmpvar_23 = (mul((mul(sg_World2Object(), _WorldSpaceLightPos0)).xyz, tmpvar_22));
    tmpvar_5 = tmpvar_23;
    float4 tmpvar_24;
    tmpvar_24.w = 1.0;
    tmpvar_24.xyz = _WorldSpaceCameraPos;
    float3 tmpvar_25;
    tmpvar_25 = normalize((mul((((mul(sg_World2Object(), tmpvar_24)).xyz * sg_unity_Scale().w) - v.vertex.xyz), tmpvar_22)));
    tmpvar_6 = tmpvar_25;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = tmpvar_3;
    o.xlv_TEXCOORD1 = tmpvar_4;
    o.xlv_TEXCOORD2 = tmpvar_5;
    o.xlv_TEXCOORD3 = float3(0.0, 0.0, 0.0);
    o.xlv_TEXCOORD4 = tmpvar_6;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half3 tmpvar_1;
    float2 tmpvar_2;
    tmpvar_2 = i.xlv_TEXCOORD0.xy;
    float2 tmpvar_3;
    tmpvar_3 = i.xlv_TEXCOORD0.zw;
    tmpvar_1 = i.xlv_TEXCOORD1;
    half4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, tmpvar_2);
    half3 tmpvar_5;
    tmpvar_5 = (tmpvar_4.xyz * tmpvar_1);
    half3 tmpvar_6;
    tmpvar_6 = ((tex2D(_BumpMap, tmpvar_3).xyz * 2.0) - 1.0);
    half4 c_i0;
    half2 tmpvar_7;
    tmpvar_7.x = ((dot(tmpvar_6, i.xlv_TEXCOORD2) * 0.5) + 0.5);
    tmpvar_7.y = dot(tmpvar_6, normalize((i.xlv_TEXCOORD2 + i.xlv_TEXCOORD4)));
    half4 tmpvar_8;
    tmpvar_8 = tex2D(_BRDFTex, tmpvar_7);
    c_i0.xyz = ((tmpvar_5 * (tmpvar_8.xyz + (tmpvar_4.w * tmpvar_8.w))) * 2.0);
    c_i0.w = 0.0;
    c = c_i0;
    c.xyz = (c_i0.xyz + (tmpvar_5 * i.xlv_TEXCOORD3));
    sg_FragColor = c;
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
Fallback "Diffuse"
}
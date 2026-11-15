// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
// Port of the Unity 3.5 built-in "Diffuse" that shipped with the game. Renamed "Compat/Diffuse" so it does not clash with
// Unity 2018's own built-in shaders; materials reach it through this file's GUID.
Shader "Compat/Diffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #pragma multi_compile _ VERTEXLIGHT_ON
  #include "UnityCG.cginc"
  // uniform scale of the object (Unity 3.5 kept it apart from _World2Object)
  inline float sg_ObjScale () { return length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); }
  // Unity 3.5 unity_Scale: w = 1 / object scale
  inline float4 sg_unity_Scale () { float s = sg_ObjScale(); return float4(s, s, s, 1.0 / s); }
  half4 _Color;
  half4 _LightColor0;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.xlv_TEXCOORD1 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, i.xlv_TEXCOORD0) * _Color);
    c = float4(0.0, 0.0, 0.0, 0.0);
    c.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1).xyz));
    c.w = tmpvar_1.w;
    sg_FragColor = c;
    return sg_FragColor;
  }
  #else
  #if defined(VERTEXLIGHT_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
    half3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 shlight;
    half3 tmpvar_1;
    half3 tmpvar_2;
    float3x3 tmpvar_3;
    tmpvar_3[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_3[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_3[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_4;
    tmpvar_4 = (mul((normalize(v.normal) * sg_unity_Scale().w), tmpvar_3));
    tmpvar_1 = tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5.w = 1.0;
    tmpvar_5.xyz = tmpvar_4;
    half3 tmpvar_6;
    half4 normal;
    normal = tmpvar_5;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_7;
    tmpvar_7 = dot(unity_SHAr, normal);
    x1.x = tmpvar_7;
    float tmpvar_8;
    tmpvar_8 = dot(unity_SHAg, normal);
    x1.y = tmpvar_8;
    float tmpvar_9;
    tmpvar_9 = dot(unity_SHAb, normal);
    x1.z = tmpvar_9;
    half4 tmpvar_10;
    tmpvar_10 = (normal.xyzz * normal.yzzx);
    float tmpvar_11;
    tmpvar_11 = dot(unity_SHBr, tmpvar_10);
    x2.x = tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = dot(unity_SHBg, tmpvar_10);
    x2.y = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHBb, tmpvar_10);
    x2.z = tmpvar_13;
    half tmpvar_14;
    tmpvar_14 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_14;
    float3 tmpvar_15;
    tmpvar_15 = (unity_SHC.xyz * vC);
    x3 = tmpvar_15;
    tmpvar_6 = ((x1 + x2) + x3);
    shlight = tmpvar_6;
    tmpvar_2 = shlight;
    float3 tmpvar_16;
    tmpvar_16 = (mul(unity_ObjectToWorld, v.vertex)).xyz;
    float4 tmpvar_17;
    tmpvar_17 = (unity_4LightPosX0 - tmpvar_16.x);
    float4 tmpvar_18;
    tmpvar_18 = (unity_4LightPosY0 - tmpvar_16.y);
    float4 tmpvar_19;
    tmpvar_19 = (unity_4LightPosZ0 - tmpvar_16.z);
    float4 tmpvar_20;
    tmpvar_20 = (((tmpvar_17 * tmpvar_17) + (tmpvar_18 * tmpvar_18)) + (tmpvar_19 * tmpvar_19));
    float4 tmpvar_21;
    tmpvar_21 = (max(float4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_17 * tmpvar_4.x) + (tmpvar_18 * tmpvar_4.y)) + (tmpvar_19 * tmpvar_4.z)) * rsqrt(tmpvar_20))) * (1.0 / (1.0 + (tmpvar_20 * unity_4LightAtten0))));
    float3 tmpvar_22;
    tmpvar_22 = (tmpvar_2 + ((((unity_LightColor[0].xyz * tmpvar_21.x) + (unity_LightColor[1].xyz * tmpvar_21.y)) + (unity_LightColor[2].xyz * tmpvar_21.z)) + (unity_LightColor[3].xyz * tmpvar_21.w)));
    tmpvar_2 = tmpvar_22;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.xlv_TEXCOORD1 = tmpvar_1;
    o.xlv_TEXCOORD2 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, i.xlv_TEXCOORD0) * _Color);
    half4 c_i0_i1;
    c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max(0.0, dot(i.xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
    c_i0_i1.w = tmpvar_1.w;
    c = c_i0_i1;
    c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * i.xlv_TEXCOORD2));
    sg_FragColor = c;
    return sg_FragColor;
  }
  #else
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
    half3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 shlight;
    half3 tmpvar_1;
    half3 tmpvar_2;
    float3x3 tmpvar_3;
    tmpvar_3[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_3[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_3[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_4;
    tmpvar_4 = (mul((normalize(v.normal) * sg_unity_Scale().w), tmpvar_3));
    tmpvar_1 = tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5.w = 1.0;
    tmpvar_5.xyz = tmpvar_4;
    half3 tmpvar_6;
    half4 normal;
    normal = tmpvar_5;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_7;
    tmpvar_7 = dot(unity_SHAr, normal);
    x1.x = tmpvar_7;
    float tmpvar_8;
    tmpvar_8 = dot(unity_SHAg, normal);
    x1.y = tmpvar_8;
    float tmpvar_9;
    tmpvar_9 = dot(unity_SHAb, normal);
    x1.z = tmpvar_9;
    half4 tmpvar_10;
    tmpvar_10 = (normal.xyzz * normal.yzzx);
    float tmpvar_11;
    tmpvar_11 = dot(unity_SHBr, tmpvar_10);
    x2.x = tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = dot(unity_SHBg, tmpvar_10);
    x2.y = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHBb, tmpvar_10);
    x2.z = tmpvar_13;
    half tmpvar_14;
    tmpvar_14 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_14;
    float3 tmpvar_15;
    tmpvar_15 = (unity_SHC.xyz * vC);
    x3 = tmpvar_15;
    tmpvar_6 = ((x1 + x2) + x3);
    shlight = tmpvar_6;
    tmpvar_2 = shlight;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.xlv_TEXCOORD1 = tmpvar_1;
    o.xlv_TEXCOORD2 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, i.xlv_TEXCOORD0) * _Color);
    half4 c_i0_i1;
    c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max(0.0, dot(i.xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)) * 2.0));
    c_i0_i1.w = tmpvar_1.w;
    c = c_i0_i1;
    c.xyz = (c_i0_i1.xyz + (tmpvar_1.xyz * i.xlv_TEXCOORD2));
    sg_FragColor = c;
    return sg_FragColor;
  }
  #endif
  #endif
  ENDCG
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
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
  // Unity 3.5 unity_Scale: w = 1 / object scale
  inline float4 sg_unity_Scale () { float s = sg_ObjScale(); return float4(s, s, s, 1.0 / s); }
  half4 _Color;
  half4 _LightColor0;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
    half3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half3 tmpvar_1;
    half3 tmpvar_2;
    float3x3 tmpvar_3;
    tmpvar_3[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_3[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_3[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_4;
    tmpvar_4 = (mul((normalize(v.normal) * sg_unity_Scale().w), tmpvar_3));
    tmpvar_1 = tmpvar_4;
    float3 tmpvar_5;
    tmpvar_5 = _WorldSpaceLightPos0.xyz;
    tmpvar_2 = tmpvar_5;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.xlv_TEXCOORD1 = tmpvar_1;
    o.xlv_TEXCOORD2 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half3 lightDir;
    half4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, i.xlv_TEXCOORD0) * _Color);
    lightDir = i.xlv_TEXCOORD2;
    half4 c_i0_i1;
    c_i0_i1.xyz = ((tmpvar_1.xyz * _LightColor0.xyz) * (max(0.0, dot(i.xlv_TEXCOORD1, lightDir)) * 2.0));
    c_i0_i1.w = tmpvar_1.w;
    c = c_i0_i1;
    c.w = 0.0;
    sg_FragColor = c;
    return sg_FragColor;
  }
  ENDCG
 }
}
Fallback "VertexLit"
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Lightprobes with Gloss Per-Vertex Additive" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecDir ("Specular Direction", Vector) = (1,1,0,0)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
 _SHLightingScale ("LightProbe influence scale", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile _ LIGHTMAP_ON
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  float _SHLightingScale;
  float _Shininess;
  float3 _SpecColor;
  float3 _SpecDir;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
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
    tmpvar_4 = (mul(normalize(v.normal), tmpvar_3));
    float3 tmpvar_5;
    tmpvar_5 = reflect(normalize(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos)), tmpvar_4);
    float3 tmpvar_6;
    tmpvar_6 = ((_SpecColor * pow(clamp(dot(normalize(_SpecDir), tmpvar_5), 0.0, 1.0), (_Shininess * 128.0))) * 2.0);
    tmpvar_1 = tmpvar_6;
    float4 tmpvar_7;
    tmpvar_7.w = 1.0;
    tmpvar_7.xyz = tmpvar_4;
    half3 tmpvar_8;
    half4 normal;
    normal = tmpvar_7;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_9;
    tmpvar_9 = dot(unity_SHAr, normal);
    x1.x = tmpvar_9;
    float tmpvar_10;
    tmpvar_10 = dot(unity_SHAg, normal);
    x1.y = tmpvar_10;
    float tmpvar_11;
    tmpvar_11 = dot(unity_SHAb, normal);
    x1.z = tmpvar_11;
    half4 tmpvar_12;
    tmpvar_12 = (normal.xyzz * normal.yzzx);
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    x2.x = tmpvar_13;
    float tmpvar_14;
    tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    half tmpvar_16;
    tmpvar_16 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_16;
    float3 tmpvar_17;
    tmpvar_17 = (unity_SHC.xyz * vC);
    x3 = tmpvar_17;
    tmpvar_8 = ((x1 + x2) + x3);
    float3 tmpvar_18;
    tmpvar_18 = (tmpvar_8 * _SHLightingScale);
    tmpvar_2 = tmpvar_18;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_5;
    o.xlv_TEXCOORD3 = tmpvar_1;
    o.xlv_TEXCOORD4 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    c.xyz = (tmpvar_1.xyz * i.xlv_TEXCOORD4);
    c.xyz = (c.xyz + (i.xlv_TEXCOORD3 * tmpvar_1.w));
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
    half3 xlv_TEXCOORD4 : TEXCOORD4;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
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
    tmpvar_4 = (mul(normalize(v.normal), tmpvar_3));
    float3 tmpvar_5;
    tmpvar_5 = reflect(normalize(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos)), tmpvar_4);
    float3 tmpvar_6;
    tmpvar_6 = ((_SpecColor * pow(clamp(dot(normalize(_SpecDir), tmpvar_5), 0.0, 1.0), (_Shininess * 128.0))) * 2.0);
    tmpvar_1 = tmpvar_6;
    float4 tmpvar_7;
    tmpvar_7.w = 1.0;
    tmpvar_7.xyz = tmpvar_4;
    half3 tmpvar_8;
    half4 normal;
    normal = tmpvar_7;
    half3 x3;
    float vC;
    half3 x2;
    half3 x1;
    float tmpvar_9;
    tmpvar_9 = dot(unity_SHAr, normal);
    x1.x = tmpvar_9;
    float tmpvar_10;
    tmpvar_10 = dot(unity_SHAg, normal);
    x1.y = tmpvar_10;
    float tmpvar_11;
    tmpvar_11 = dot(unity_SHAb, normal);
    x1.z = tmpvar_11;
    half4 tmpvar_12;
    tmpvar_12 = (normal.xyzz * normal.yzzx);
    float tmpvar_13;
    tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    x2.x = tmpvar_13;
    float tmpvar_14;
    tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    half tmpvar_16;
    tmpvar_16 = ((normal.x * normal.x) - (normal.y * normal.y));
    vC = tmpvar_16;
    float3 tmpvar_17;
    tmpvar_17 = (unity_SHC.xyz * vC);
    x3 = tmpvar_17;
    tmpvar_8 = ((x1 + x2) + x3);
    float3 tmpvar_18;
    tmpvar_18 = (tmpvar_8 * _SHLightingScale);
    tmpvar_2 = tmpvar_18;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_5;
    o.xlv_TEXCOORD3 = tmpvar_1;
    o.xlv_TEXCOORD4 = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    c.xyz = (tmpvar_1.xyz * i.xlv_TEXCOORD4);
    c.xyz = (c.xyz + (i.xlv_TEXCOORD3 * tmpvar_1.w));
    sg_FragColor = c;
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Cube env map (Supports LightProbes)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _EnvTex ("Cube env tex", CUBE) = "black" {}
 _Spread ("Spread", Range(0.1,0.5)) = 0.5
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
  #include "UnityCG.cginc"
  samplerCUBE _EnvTex;
  sampler2D _MainTex;
  float _SHLightingScale;
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    float3 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_2[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_2[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_3;
    tmpvar_3 = (mul(normalize(v.normal), tmpvar_2));
    float3 tmpvar_4;
    tmpvar_4 = reflect(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos), tmpvar_3);
    tmpvar_1 = tmpvar_4;
    tmpvar_1.x = -(tmpvar_4.x);
    float4 tmpvar_5;
    tmpvar_5.w = 1.0;
    tmpvar_5.xyz = tmpvar_3;
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
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_1;
    o.xlv_COLOR = (tmpvar_6 * _SHLightingScale);
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    c.xyz = (tmpvar_1.xyz + (texCUBE(_EnvTex, i.xlv_TEXCOORD1) * tmpvar_1.w).xyz);
    float3 tmpvar_2;
    tmpvar_2 = (c.xyz * i.xlv_COLOR);
    c.xyz = tmpvar_2;
    sg_FragColor = c;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Gloss Per-Vertex Additive (Supports Lightmap)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecDir ("Specular Direction", Vector) = (1,1,0,0)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
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
  float _Shininess;
  float3 _SpecColor;
  float3 _SpecDir;
  #if defined(LIGHTMAP_ON)
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    float2 xlv_TEXCOORD2 : TEXCOORD2;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half3 tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_2[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_2[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_3;
    tmpvar_3 = reflect(normalize(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos)), (mul(normalize(v.normal), tmpvar_2)));
    float3 tmpvar_4;
    tmpvar_4 = ((_SpecColor * pow(clamp(dot(normalize(_SpecDir), tmpvar_3), 0.0, 1.0), (_Shininess * 128.0))) * 2.0);
    tmpvar_1 = tmpvar_4;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_3;
    o.xlv_TEXCOORD2 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.xlv_TEXCOORD3 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c.xyz = tmpvar_1.xyz;
    c.w = tmpvar_1.w;
    c.xyz = (tmpvar_1.xyz + (i.xlv_TEXCOORD3 * tmpvar_1.w));
    c.xyz = (c.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD2).xyz));
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
    half3 xlv_TEXCOORD3 : TEXCOORD3;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half3 tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_2[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_2[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_3;
    tmpvar_3 = reflect(normalize(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos)), (mul(normalize(v.normal), tmpvar_2)));
    float3 tmpvar_4;
    tmpvar_4 = ((_SpecColor * pow(clamp(dot(normalize(_SpecDir), tmpvar_3), 0.0, 1.0), (_Shininess * 128.0))) * 2.0);
    tmpvar_1 = tmpvar_4;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_3;
    o.xlv_TEXCOORD3 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c.xyz = tmpvar_1.xyz;
    c.w = tmpvar_1.w;
    c.xyz = (tmpvar_1.xyz + (i.xlv_TEXCOORD3 * tmpvar_1.w));
    sg_FragColor = c;
    return sg_FragColor;
  }
  #endif
  ENDCG
 }
}
}
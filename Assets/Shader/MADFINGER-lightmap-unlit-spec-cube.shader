// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Cubemap specular + Lightmap" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecCubeTex ("SpecCube", CUBE) = "black" {}
 _SpecularStrength ("Specular strength", Float) = 1
 _ScrollingSpeed ("Scrolling speed", Vector) = (0,0,0,0)
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
  sampler2D _MainTex;
  float4 _ScrollingSpeed;
  samplerCUBE _SpecCubeTex;
  float _SpecularStrength;
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float3 xlv_TEXCOORD2 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3x3 tmpvar_1;
    tmpvar_1[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_1[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_1[2] = transpose(unity_ObjectToWorld)[2].xyz;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (v.texcoord + frac((_ScrollingSpeed * _Time.y))).xy;
    o.xlv_TEXCOORD1 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.xlv_TEXCOORD2 = reflect(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos), normalize((mul(normalize(v.normal), tmpvar_1))));
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half3 spec;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    half4 tmpvar_2;
    tmpvar_2 = texCUBE(_SpecCubeTex, i.xlv_TEXCOORD2);
    float3 tmpvar_3;
    tmpvar_3 = ((tmpvar_2 * _SpecularStrength) * tmpvar_1.w).xyz;
    spec = tmpvar_3;
    c.xyz = (tmpvar_1.xyz + spec);
    c.xyz = (c.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1).xyz));
    sg_FragColor = c;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Cube env map modulated by vertex color + lightmap" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _EnvTex ("Cube env tex", CUBE) = "black" {}
 _Spread ("Spread", Range(0.1,0.5)) = 0.5
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
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float3 xlv_TEXCOORD2 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
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
    tmpvar_3 = reflect(((mul(unity_ObjectToWorld, v.vertex)).xyz - _WorldSpaceCameraPos), (mul(normalize(v.normal), tmpvar_2)));
    tmpvar_1 = tmpvar_3;
    tmpvar_1.x = -(tmpvar_3.x);
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.xlv_TEXCOORD2 = tmpvar_1;
    o.xlv_COLOR = v.color;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 c;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    c = tmpvar_1;
    c.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1).xyz));
    c.xyz = (c.xyz + (texCUBE(_EnvTex, i.xlv_TEXCOORD2) * tmpvar_1.w).xyz);
    half4 tmpvar_2;
    tmpvar_2 = (c * i.xlv_COLOR);
    c = tmpvar_2;
    sg_FragColor = tmpvar_2;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
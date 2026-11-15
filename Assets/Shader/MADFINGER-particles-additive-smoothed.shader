// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Particles/Additive + fadeout" {
Properties {
 _MainTex ("Main tex", 2D) = "white" {}
 _Color ("Tint color", Color) = (0,1,0,0)
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
  float4 _Color;
  sampler2D _MainTex;
  struct sg_appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_2[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_2[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float4 tmpvar_3;
    tmpvar_3.xyz = _Color.xyz;
    tmpvar_3.w = clamp((4.0 * abs(dot(normalize((mul(normalize(v.normal), tmpvar_2))), normalize((_WorldSpaceCameraPos - (mul(unity_ObjectToWorld, v.vertex)).xyz))))), 0.0, 1.0);
    tmpvar_1 = tmpvar_3;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_COLOR = tmpvar_1;
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half4 tex;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0);
    tex = tmpvar_1;
    tex.xyz = (tmpvar_1.xyz * i.xlv_COLOR.xyz);
    sg_FragColor = ((tex * i.xlv_COLOR.w) * i.xlv_COLOR.w);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
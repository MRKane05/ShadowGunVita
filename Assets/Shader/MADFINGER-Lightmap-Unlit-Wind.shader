// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Lightmap + Wind" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _Wind ("Wind params", Vector) = (1,1,1,1)
 _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
 _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  // uniform scale of the object (Unity 3.5 kept it apart from _World2Object)
  inline float sg_ObjScale () { return length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); }
  // Unity 3.5 _World2Object = inverse world matrix with the scale removed (code multiplies by unity_Scale.w)
  inline float4x4 sg_World2Object () { float s = sg_ObjScale(); float4x4 m = unity_WorldToObject; m[0] *= s; m[1] *= s; m[2] *= s; return m; }
  sampler2D _MainTex;
  float4 _MainTex_ST;
  float4 _Wind;
  float _WindEdgeFlutter;
  float _WindEdgeFlutterFreqScale;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half3 xlv_TEXCOORD2 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float4 wind;
    float bendingFact;
    half tmpvar_1;
    tmpvar_1 = v.color.w;
    bendingFact = tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = transpose(sg_World2Object())[0].xyz;
    tmpvar_2[1] = transpose(sg_World2Object())[1].xyz;
    tmpvar_2[2] = transpose(sg_World2Object())[2].xyz;
    wind.xyz = (mul(_Wind.xyz, tmpvar_2));
    wind.w = (_Wind.w * bendingFact);
    float2 tmpvar_3;
    tmpvar_3.y = 1.0;
    tmpvar_3.x = _WindEdgeFlutterFreqScale;
    float4 pos;
    pos = v.vertex;
    float3 bend;
    float4 v_i0;
    v_i0.x = transpose(unity_ObjectToWorld)[0].w;
    v_i0.y = transpose(unity_ObjectToWorld)[1].w;
    v_i0.z = transpose(unity_ObjectToWorld)[2].w;
    v_i0.w = transpose(unity_ObjectToWorld)[3].w;
    float tmpvar_4;
    tmpvar_4 = dot(v_i0.xyz, float3(1.0, 1.0, 1.0));
    float2 tmpvar_5;
    tmpvar_5.x = dot(v.vertex.xyz, ((float3)(_WindEdgeFlutter + tmpvar_4)));
    tmpvar_5.y = tmpvar_4;
    float4 tmpvar_6;
    tmpvar_6 = abs(((frac((((frac((((_Time.y * tmpvar_3).xx + tmpvar_5).xxyy * float4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
    float4 tmpvar_7;
    tmpvar_7 = ((tmpvar_6 * tmpvar_6) * (3.0 - (2.0 * tmpvar_6)));
    float2 tmpvar_8;
    tmpvar_8 = (tmpvar_7.xz + tmpvar_7.yw);
    bend = ((_WindEdgeFlutter * 0.1) * normalize(v.normal));
    bend.y = (bendingFact * 0.3);
    pos.xyz = (v.vertex.xyz + (((tmpvar_8.xyx * bend) + ((wind.xyz * tmpvar_8.y) * bendingFact)) * wind.w));
    pos.xyz = (pos.xyz + (bendingFact * wind.xyz));
    o.pos = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos)));
    o.xlv_TEXCOORD0 = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    o.xlv_TEXCOORD1 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    o.xlv_TEXCOORD2 = v.color.xyz;
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
    c.xyz = (tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1).xyz));
    sg_FragColor = c;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
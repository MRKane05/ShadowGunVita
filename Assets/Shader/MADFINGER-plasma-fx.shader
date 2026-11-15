// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Plasma FX" {
Properties {
 _MainTex ("Noise tex", 2D) = "white" {}
 _ColorStart ("Color start", Color) = (1,0.8,0,1)
 _ColorEnd ("Color end", Color) = (0.5,0.3,0,1)
 _Speed ("Speed", Float) = 2
 _BorderWidth ("Border width", Float) = 4
 _TimeOffs ("TimeOffs", Float) = 9999
 _ExpansionSpeed ("ExpansionSpeed", Float) = 1
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float _BorderWidth;
  float4 _ColorEnd;
  float4 _ColorStart;
  float _ExpansionSpeed;
  sampler2D _MainTex;
  float4 _MainTex_ST;
  float _Speed;
  float _TimeOffs;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float3 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    float3 tmpvar_1;
    half4 tmpvar_2;
    float tmpvar_3;
    tmpvar_3 = ((_Time.y + _TimeOffs) * _Speed);
    float tmpvar_4;
    tmpvar_4 = clamp(tmpvar_3, 0.0, 1.0);
    float4 tmpvar_5;
    tmpvar_5.w = 1.0;
    tmpvar_5.xyz = (v.vertex.xyz + ((normalize(v.normal) * tmpvar_4) * _ExpansionSpeed));
    tmpvar_1.xy = ((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    tmpvar_1.z = tmpvar_3;
    float4 tmpvar_6;
    tmpvar_6.xyz = lerp(_ColorStart.xyz, _ColorEnd.xyz, ((float3)tmpvar_4));
    tmpvar_6.w = (v.color.w * pow((1.0 - tmpvar_4), 3.0));
    tmpvar_2 = tmpvar_6;
    o.pos = (mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, tmpvar_5)));
    o.xlv_TEXCOORD0 = tmpvar_1;
    o.xlv_COLOR = tmpvar_2;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    half threshold;
    half4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, i.xlv_TEXCOORD0.xy);
    float tmpvar_2;
    tmpvar_2 = i.xlv_TEXCOORD0.z;
    threshold = tmpvar_2;
    float tmpvar_3;
    tmpvar_3 = (1.0 - clamp(((tmpvar_1 - threshold).x * _BorderWidth), 0.0, 1.0));
    half tmpvar_4;
    tmpvar_4 = (tmpvar_3 * tmpvar_3);
    sg_FragColor = ((i.xlv_COLOR + ((float4)(tmpvar_4 * tmpvar_4))) * (i.xlv_COLOR.w * step(threshold, tmpvar_1.x)));
    return sg_FragColor;
  }
  ENDCG
 }
}
}
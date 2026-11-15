// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Mine-FX" {
Properties {
 _MainTex ("MainTex", 2D) = "white" {}
 _Params ("X = period, Y = max scale, Z = time offs, W = delay", Vector) = (1,5,0,0)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  float4 _Params;
  struct sg_appdata {
    float4 vertex : POSITION;
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
    float tmpvar_2;
    tmpvar_2 = (_Params.x + _Params.w);
    float tmpvar_3;
    tmpvar_3 = ((_Time.y + _Params.z) / tmpvar_2);
    float tmpvar_4;
    tmpvar_4 = (frac(abs(tmpvar_3)) * tmpvar_2);
    float tmpvar_5;
    if (tmpvar_3 >= 0.0) {
      tmpvar_5 = tmpvar_4;
    } else {
      tmpvar_5 = -(tmpvar_4);
    }
    float tmpvar_6;
    tmpvar_6 = clamp((tmpvar_5 / _Params.x), 0.0, 1.0);
    float4 tmpvar_7;
    tmpvar_7.xyz = float3(1.0, 1.0, 1.0);
    tmpvar_7.w = (1.0 - pow(tmpvar_6, 3.0));
    tmpvar_1 = tmpvar_7;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = (((v.texcoord.xy - float2(0.5, 0.5)) * (1.0 / ((tmpvar_6 * _Params.y) + 0.05))) + float2(0.5, 0.5));
    o.xlv_COLOR = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_COLOR);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
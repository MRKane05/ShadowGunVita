// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Environment/Blinking emissive" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _IntensityScaleBias ("Intensity scale X / bias Y", Vector) = (1,0.1,0,0)
 _SwitchOnOffDuration ("Switch ON (X) / OFF (Y) duration", Vector) = (1,3,0,0)
 _BlinkingRate ("Blinking rate", Float) = 10
 _RndGridSize ("Randomization grid size", Float) = 5
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
  float _BlinkingRate;
  float2 _IntensityScaleBias;
  sampler2D _MainTex;
  float2 _SwitchOnOffDuration;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float seed;
    half tmpvar_2;
    tmpvar_2 = (dot(v.color, v.color) * 40.0);
    seed = tmpvar_2;
    float time_i0;
    time_i0 = (_Time.y * _BlinkingRate);
    float tmpvar_3;
    tmpvar_3 = ((float)(abs(cos(((17.0 * sin((time_i0 * 5.0))) + (10.0 * sin(((seed + (time_i0 * 3.0)) + 7.993)))))) > 0.5));
    float2 tmpvar_4;
    tmpvar_4 = (_SwitchOnOffDuration * (0.8 + (0.4 * frac(seed))));
    float y;
    y = (tmpvar_4.x + tmpvar_4.y);
    float tmpvar_5;
    tmpvar_5 = ((_Time.y + seed) / y);
    float tmpvar_6;
    tmpvar_6 = (frac(abs(tmpvar_5)) * y);
    float tmpvar_7;
    if (tmpvar_5 >= 0.0) {
      tmpvar_7 = tmpvar_6;
    } else {
      tmpvar_7 = -(tmpvar_6);
    }
    float4 tmpvar_8;
    tmpvar_8 = (((v.color * (tmpvar_3 * ((float)(tmpvar_7 < tmpvar_4.x)))) * _IntensityScaleBias.x) + _IntensityScaleBias.y);
    tmpvar_1 = tmpvar_8;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = v.texcoord.xy;
    o.xlv_TEXCOORD1 = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_TEXCOORD1);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
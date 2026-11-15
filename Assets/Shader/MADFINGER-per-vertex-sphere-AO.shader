// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/Characters/Character shadow plane - sphere AO" {
Properties {
 _Sphere0 ("S0", Vector) = (0,0,0,0)
 _Sphere1 ("S0", Vector) = (0,0,0,0)
 _Sphere2 ("S0", Vector) = (0,0,0,0)
 _Intensity ("Intensity", Float) = 0.9
}
SubShader { 
 Tags { "QUEUE"="Transparent-15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent-15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend DstColor Zero
CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"
  float _Intensity;
  float4 _Sphere0;
  float4 _Sphere1;
  float4 _Sphere2;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float3 normal : NORMAL;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float3 tmpvar_2;
    tmpvar_2 = (mul(unity_ObjectToWorld, v.vertex)).xyz;
    float3x3 tmpvar_3;
    tmpvar_3[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_3[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_3[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_4;
    tmpvar_4 = (mul(normalize(v.normal), tmpvar_3));
    float3 tmpvar_5;
    tmpvar_5 = (_Sphere0.xyz - tmpvar_2);
    float tmpvar_6;
    tmpvar_6 = length(tmpvar_5);
    float tmpvar_7;
    tmpvar_7 = (_Sphere0.w / tmpvar_6);
    float3 tmpvar_8;
    tmpvar_8 = (_Sphere1.xyz - tmpvar_2);
    float tmpvar_9;
    tmpvar_9 = length(tmpvar_8);
    float tmpvar_10;
    tmpvar_10 = (_Sphere1.w / tmpvar_9);
    float3 tmpvar_11;
    tmpvar_11 = (_Sphere2.xyz - tmpvar_2);
    float tmpvar_12;
    tmpvar_12 = length(tmpvar_11);
    float tmpvar_13;
    tmpvar_13 = (_Sphere2.w / tmpvar_12);
    float tmpvar_14;
    tmpvar_14 = (max((1.0 - clamp(((((dot(tmpvar_4, (tmpvar_5 / tmpvar_6)) * tmpvar_7) * tmpvar_7) + ((dot(tmpvar_4, (tmpvar_8 / tmpvar_9)) * tmpvar_10) * tmpvar_10)) + ((dot(tmpvar_4, (tmpvar_11 / tmpvar_12)) * tmpvar_13) * tmpvar_13)), 0.0, 1.0)), (1.0 - _Intensity)) + (1.0 - v.color.x));
    float4 tmpvar_15;
    tmpvar_15.x = tmpvar_14;
    tmpvar_15.y = tmpvar_14;
    tmpvar_15.z = tmpvar_14;
    tmpvar_15.w = tmpvar_14;
    tmpvar_1 = tmpvar_15;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_COLOR = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = i.xlv_COLOR;
    return sg_FragColor;
  }
  ENDCG
 }
}
}
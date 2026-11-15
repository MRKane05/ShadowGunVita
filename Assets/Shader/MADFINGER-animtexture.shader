// Rebuilt from the shipped Shadowgun 1.3.5 (Unity 3.5.7) GLES2 programs by Tools/ShaderPort/sg_glsl2hlsl.py.
// Exact transliteration of the original maths; edit the generator rather than this file where possible.
Shader "MADFINGER/FX/Anim texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _NumTexTiles ("Num tex tiles", Vector) = (4,4,0,0)
 _ReplaySpeed ("Replay speed - FPS", Float) = 4
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
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
  float4 _NumTexTiles;
  float _ReplaySpeed;
  struct sg_appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD0;
  };
  struct sg_v2f {
    float4 pos : SV_POSITION;
    float4 xlv_TEXCOORD0 : TEXCOORD0;
    half4 xlv_COLOR : COLOR0;
  };
  sg_v2f vert (sg_appdata v)
  {
    sg_v2f o;
    UNITY_INITIALIZE_OUTPUT(sg_v2f, o);
    half4 tmpvar_1;
    float4 tile;
    float tmpvar_2;
    tmpvar_2 = (((v.color.w * 60.0) + _Time.y) * _ReplaySpeed);
    float tmpvar_3;
    tmpvar_3 = floor(tmpvar_2);
    float tmpvar_4;
    tmpvar_4 = (tmpvar_3 + 1.0);
    float tmpvar_5;
    tmpvar_5 = (tmpvar_2 - tmpvar_3);
    float2 tmpvar_6;
    tmpvar_6 = (1.0 / (_NumTexTiles.xy));
    float2 tmpvar_7;
    tmpvar_7.x = tmpvar_3;
    tmpvar_7.y = floor((tmpvar_3 / _NumTexTiles.x));
    tile.xy = tmpvar_7;
    float2 tmpvar_8;
    tmpvar_8.x = tmpvar_4;
    tmpvar_8.y = floor((tmpvar_4 / _NumTexTiles.x));
    tile.zw = tmpvar_8;
    float4 tmpvar_9;
    tmpvar_9 = (tile / _NumTexTiles.xyxy);
    float4 tmpvar_10;
    tmpvar_10 = (frac(abs(tmpvar_9)) * _NumTexTiles.xyxy);
    float tmpvar_11;
    if (tmpvar_9.x >= 0.0) {
      tmpvar_11 = tmpvar_10.x;
    } else {
      tmpvar_11 = -(tmpvar_10.x);
    }
    float tmpvar_12;
    if (tmpvar_9.y >= 0.0) {
      tmpvar_12 = tmpvar_10.y;
    } else {
      tmpvar_12 = -(tmpvar_10.y);
    }
    float tmpvar_13;
    if (tmpvar_9.z >= 0.0) {
      tmpvar_13 = tmpvar_10.z;
    } else {
      tmpvar_13 = -(tmpvar_10.z);
    }
    float tmpvar_14;
    if (tmpvar_9.w >= 0.0) {
      tmpvar_14 = tmpvar_10.w;
    } else {
      tmpvar_14 = -(tmpvar_10.w);
    }
    float4 tmpvar_15;
    tmpvar_15.x = tmpvar_11;
    tmpvar_15.y = tmpvar_12;
    tmpvar_15.z = tmpvar_13;
    tmpvar_15.w = tmpvar_14;
    tile = tmpvar_15;
    float4 tmpvar_16;
    tmpvar_16.xyz = (_Color.xyz * v.color.xyz);
    tmpvar_16.w = tmpvar_5;
    tmpvar_1 = tmpvar_16;
    o.pos = (UnityObjectToClipPos(v.vertex));
    o.xlv_TEXCOORD0 = ((v.texcoord.xyxy + tmpvar_15) * tmpvar_6.xyxy);
    o.xlv_COLOR = tmpvar_1;
    return o;
  }
  fixed4 frag (sg_v2f i) : SV_Target
  {
    fixed4 sg_FragColor = 0;
    sg_FragColor = (lerp(tex2D(_MainTex, i.xlv_TEXCOORD0.xy), tex2D(_MainTex, i.xlv_TEXCOORD0.zw), i.xlv_COLOR.wwww) * i.xlv_COLOR);
    return sg_FragColor;
  }
  ENDCG
 }
}
}
// Readable version of "MADFINGER/Environment/Scroll 2 Layers Sine AlphaBlended".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Two scrolling textures multiplied together. Each layer's UV also wobbles on a sine wave
// (per axis amplitude and frequency, driven by _Time.x). The result is tinted by
// _MMultiplier * _Color * vertex colour, and alpha blended.
//
// Changes from the generated version:
//  - The LIGHTMAP_ON and default variants were identical (neither uses the lightmap), so they
//    are merged and the multi_compile is removed.

Shader "MADFINGER/Environment/Scroll 2 Layers Sine AlphaBlended"
{
    Properties
    {
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
        _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1
        _ScrollY ("Base layer Scroll speed Y", Float) = 0
        _Scroll2X ("2nd layer Scroll speed X", Float) = 1
        _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
        _SineAmplX ("Base layer sine amplitude X", Float) = 0.5
        _SineAmplY ("Base layer sine amplitude Y", Float) = 0.5
        _SineFreqX ("Base layer sine freq X", Float) = 10
        _SineFreqY ("Base layer sine freq Y", Float) = 10
        _SineAmplX2 ("2nd layer sine amplitude X", Float) = 0.5
        _SineAmplY2 ("2nd layer sine amplitude Y", Float) = 0.5
        _SineFreqX2 ("2nd layer sine freq X", Float) = 10
        _SineFreqY2 ("2nd layer sine freq Y", Float) = 10
        _Color ("Color", Color) = (1,1,1,1)
        _MMultiplier ("Layer Multiplier", Float) = 2
    }

    SubShader
    {
        LOD 100
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _DetailTex;
            float4    _DetailTex_ST;
            float     _ScrollX;
            float     _ScrollY;
            float     _Scroll2X;
            float     _Scroll2Y;
            float     _SineAmplX;
            float     _SineAmplY;
            float     _SineFreqX;
            float     _SineFreqY;
            float     _SineAmplX2;
            float     _SineAmplY2;
            float     _SineFreqX2;
            float     _SineFreqY2;
            float     _MMultiplier;
            float4    _Color;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv  : TEXCOORD0; // xy = base layer UV, zw = 2nd layer UV
                half4  tint : TEXCOORD1; // _MMultiplier * _Color * vertex colour
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);

                // Scrolling
                o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex)   + frac(float2(_ScrollX, _ScrollY)   * _Time.xy);
                o.uv.zw = TRANSFORM_TEX(v.texcoord.xy, _DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time.xy);

                // Sine wobble, one wave per axis and layer
                o.uv += sin(_Time.x * float4(_SineFreqX, _SineFreqY, _SineFreqX2, _SineFreqY2))
                      * float4(_SineAmplX, _SineAmplY, _SineAmplX2, _SineAmplY2);

                o.tint = _MMultiplier * _Color * v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 layers = tex2D(_MainTex, i.uv.xy) * tex2D(_DetailTex, i.uv.zw);
                return layers * i.tint;
            }
            ENDCG
        }
    }
}

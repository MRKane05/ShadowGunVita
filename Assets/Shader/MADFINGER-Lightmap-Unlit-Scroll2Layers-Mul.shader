// Readable version of "MADFINGER/Environment/Scroll 2 Layers Multiplicative (Supports Lightmap)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Two independently scrolling textures multiplied together and scaled by _MMultiplier,
// then optionally multiplied by the baked lightmap. Note: the material tiling/offset fields
// are not used (the original ignored them too).
//
// Changes from the generated version:
//  - _MMultiplier is read directly in the fragment stage. The generated code copied this constant
//    into an interpolator, which costs a varying on the Vita for no benefit.

Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
        _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1
        _ScrollY ("Base layer Scroll speed Y", Float) = 0
        _Scroll2X ("2nd layer Scroll speed X", Float) = 1
        _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
        _MMultiplier ("Layer Multiplier", Float) = 2
    }

    SubShader
    {
        LOD 100
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ LIGHTMAP_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DetailTex;
            float     _ScrollX;
            float     _ScrollY;
            float     _Scroll2X;
            float     _Scroll2Y;
            float     _MMultiplier;

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float4 texcoord  : TEXCOORD0;
            #if defined(LIGHTMAP_ON)
                float4 texcoord1 : TEXCOORD1;
            #endif
            };

            struct v2f
            {
                float4 pos        : SV_POSITION;
                float2 uvBase     : TEXCOORD0;
                float2 uvDetail   : TEXCOORD1;
            #if defined(LIGHTMAP_ON)
                float2 uvLightmap : TEXCOORD3;
            #endif
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBase   = v.texcoord.xy + frac(float2(_ScrollX, _ScrollY)   * _Time.xy);
                o.uvDetail = v.texcoord.xy + frac(float2(_Scroll2X, _Scroll2Y) * _Time.xy);

            #if defined(LIGHTMAP_ON)
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uvBase) * tex2D(_DetailTex, i.uvDetail) * (half)_MMultiplier;

            #if defined(LIGHTMAP_ON)
                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;
            #endif
                return color;
            }
            ENDCG
        }
    }
}

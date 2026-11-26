// Readable version of "MADFINGER/Environment/Scroll 1 Layer (Supports Lightmap)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Texture scrolled over time, optionally multiplied by the baked lightmap.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Environment/Scroll 1 Layer (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ScrollX ("Scroll speed X", Float) = 1
        _ScrollY ("Scroll speed Y", Float) = 0
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
            float     _ScrollX;
            float     _ScrollY;

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
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0; // scrolled base UV
            #if defined(LIGHTMAP_ON)
                float2 uvLightmap : TEXCOORD1;
            #endif
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);

                // frac() keeps the offset small so UV precision does not degrade over time
                o.uv = v.texcoord.xy + frac(float2(_ScrollX, _ScrollY) * _Time.xy);

            #if defined(LIGHTMAP_ON)
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);

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

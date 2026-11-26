// Readable version of "MADFINGER/Environment/Lightmap + emissivity".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Lightmapped texture with an emissive term: the texture's alpha marks the glowing areas,
// tinted by _EmisColor, and is added to the lightmap before it multiplies the albedo.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Environment/Lightmap + emissivity"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _EmisColor ("Emisive color", Color) = (1,1,1,1)
    }

    SubShader
    {
        LOD 100
        Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float3    _EmisColor;

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos        : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 uvLightmap : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);

                // Lightmap (stored at half intensity) plus emission where the alpha marks it
                half3 lightmap = 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;
                half3 light = lightmap + color.a * _EmisColor;

                color.rgb *= light;
                return color;
            }
            ENDCG
        }
    }
}

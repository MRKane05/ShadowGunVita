// Readable version of "MADFINGER/Environment/Dual diffuse with lightmap".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Blends between two textures using the vertex alpha (0 = layer 0, 1 = layer 1), then
// multiplies by the baked lightmap.
//
// Changes from the generated version:
//  - The LIGHTMAP_ON and default variants were identical (both sample the lightmap), so they are
//    merged and the multi_compile is removed. The lightmap is always applied, as before.

Shader "MADFINGER/Environment/Dual diffuse with lightmap"
{
    Properties
    {
        _MainTex0 ("Base layer0 (RGB)", 2D) = "white" {}
        _MainTex1 ("Base layer1 (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        LOD 100
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Tags { "RenderType" = "Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex0;
            sampler2D _MainTex1;

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float4 color     : COLOR;    // a = blend between the two layers
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float4 uv    : TEXCOORD0; // xy = base UV, zw = lightmap UV
                half4  color : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy;
                o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = lerp(tex2D(_MainTex0, i.uv.xy), tex2D(_MainTex1, i.uv.xy), i.color.a);

                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw).rgb;
                return color;
            }
            ENDCG
        }
    }
}

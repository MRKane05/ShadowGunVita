// Readable version of "MADFINGER/Environment/Cube env map (Supports Lightmap)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Lightmapped albedo plus a cubemap reflection weighted by the texture alpha (gloss).
// _Spread is exposed in the material but was not used by the original either.
//
// Normal fix:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code multiplied by the object-to-world matrix without
//    renormalising, so reflections were wrong on scaled objects. For unscaled objects the result
//    is unchanged.
//
// The reflection's X is negated, as in the original, to match how the cubemaps were authored.

Shader "MADFINGER/Environment/Cube env map (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _EnvTex ("Cube env tex", CUBE) = "black" {}
        _Spread ("Spread", Range(0.1,0.5)) = 0.5
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

            samplerCUBE _EnvTex;
            sampler2D   _MainTex;

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float3 normal    : NORMAL;
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos        : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 uvLightmap : TEXCOORD1;
                float3 reflDir    : TEXCOORD2; // world-space reflection, X mirrored
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.reflDir = reflect(worldPos - _WorldSpaceCameraPos, worldNormal);
                o.reflDir.x = -o.reflDir.x;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                half gloss = color.a;

                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;

                // Reflection is added after lighting, so it is not darkened by the lightmap
                color.rgb += texCUBE(_EnvTex, i.reflDir).rgb * gloss;
                return color;
            }
            ENDCG
        }
    }
}

// Readable version of "MADFINGER/Environment/Gloss Per-Vertex Additive (Supports Lightmap)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Albedo plus a per-vertex specular highlight from a fixed direction (_SpecDir), weighted by
// the texture's alpha (gloss), then optionally multiplied by the baked lightmap.
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code multiplied by the object-to-world matrix and did not
//    renormalise, which bends the reflection vector (and the highlight) on scaled objects.
//    For unscaled and uniformly scaled objects the result is unchanged.
//
// Other changes:
//  - The generated code also sent the reflection vector to the fragment stage, which never used
//    it. That interpolator is removed.

Shader "MADFINGER/Environment/Gloss Per-Vertex Additive (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecDir ("Specular Direction", Vector) = (1,1,0,0)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.01,1)) = 0.078125
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
            #pragma multi_compile _ LIGHTMAP_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float3    _SpecDir;
            float3    _SpecColor;
            float     _Shininess;

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float3 normal    : NORMAL;
                float4 texcoord  : TEXCOORD0;
            #if defined(LIGHTMAP_ON)
                float4 texcoord1 : TEXCOORD1;
            #endif
            };

            struct v2f
            {
                float4 pos        : SV_POSITION;
                float2 uv         : TEXCOORD0;
            #if defined(LIGHTMAP_ON)
                float2 uvLightmap : TEXCOORD2;
            #endif
                half3  specular   : TEXCOORD3; // per-vertex specular colour
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;

            #if defined(LIGHTMAP_ON)
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif

                // Inverse transpose of the object-to-world matrix, normalised
                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                float3 reflected = reflect(viewDir, worldNormal);

                // Specular lobe towards _SpecDir (shininess 0.01..1 maps to an exponent of 1.28..128)
                float specLobe = pow(saturate(dot(normalize(_SpecDir), reflected)), _Shininess * 128.0);
                o.specular = _SpecColor * specLobe * 2.0;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 albedoGloss = tex2D(_MainTex, i.uv);

                half4 color;
                color.rgb = albedoGloss.rgb + i.specular * albedoGloss.a;
                color.a = albedoGloss.a;

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

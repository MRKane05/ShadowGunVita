// Readable version of "Compat/Diffuse".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Port of the Unity 3.5 built-in "Diffuse" that shipped with the game. Renamed "Compat/Diffuse" so
// it does not clash with Unity 2018's own built-in shaders; materials reach it through this file's
// GUID.
//
// ForwardBase: lightmap if the object has one, otherwise the main directional light (lit per
// pixel with an interpolated normal) plus light probes and up to four per-vertex point lights.
// ForwardAdd: one extra directional light per pass, added on top.
// Lighting uses Unity 3.5's convention of doubling the light (* 2).
//
// Normal fix:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code emulated Unity 3.5 (object-to-world matrix divided by the
//    object's X scale), which is only correct for uniform scale. For unscaled and uniformly
//    scaled objects the result is unchanged.
//
// Other changes:
//  - The hand-expanded spherical harmonics code is replaced by Unity's SHEvalLinearL0L1 +
//    SHEvalLinearL2, which is the same maths. ShadeSH9 is deliberately not used: in Unity 2018
//    it also applies a gamma conversion when the project is in linear space.
//  - The four-point-light code is written out in a helper (the same maths as the original,
//    which differs slightly from Unity 2018's Shade4PointLights).
//  - The ForwardAdd pass read the light direction from an interpolator; it is now read
//    directly in the fragment stage.

Shader "Compat/Diffuse"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        LOD 200
        Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        half4     _Color;
        half4     _LightColor0;
        sampler2D _MainTex;
        float4    _MainTex_ST;

        float3 WorldNormal(float3 normal)
        {
            // Inverse transpose of the object-to-world matrix, normalised
            return normalize(mul(normalize(normal), (float3x3)unity_WorldToObject));
        }

        // Unity 3.5 style diffuse: light colour * N.L * 2
        half3 DiffuseLight(half3 albedo, half3 normal, half3 lightDir)
        {
            return albedo * _LightColor0.rgb * (max(0.0, dot(normal, lightDir)) * 2.0);
        }
        ENDCG

        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ VERTEXLIGHT_ON

        #if defined(LIGHTMAP_ON)

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
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, i.uv) * _Color;

                // The lightmap is stored at half intensity, so it is doubled
                half4 color;
                color.rgb = albedo.rgb * (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb);
                color.a = albedo.a;
                return color;
            }

        #else // no lightmap: realtime lighting

        #if defined(VERTEXLIGHT_ON)
            // Up to four point lights evaluated per vertex (Unity 3.5's Shade4PointLights)
            float3 FourPointLights(float3 worldPos, float3 worldNormal)
            {
                float4 toLightX = unity_4LightPosX0 - worldPos.x;
                float4 toLightY = unity_4LightPosY0 - worldPos.y;
                float4 toLightZ = unity_4LightPosZ0 - worldPos.z;
                float4 lengthSq = toLightX * toLightX + toLightY * toLightY + toLightZ * toLightZ;

                float4 nDotL = toLightX * worldNormal.x + toLightY * worldNormal.y + toLightZ * worldNormal.z;
                float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
                float4 diff = max(float4(0.0, 0.0, 0.0, 0.0), nDotL * rsqrt(lengthSq)) * atten;

                return unity_LightColor[0].rgb * diff.x
                     + unity_LightColor[1].rgb * diff.y
                     + unity_LightColor[2].rgb * diff.z
                     + unity_LightColor[3].rgb * diff.w;
            }
        #endif

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
                half3  vertexLight : TEXCOORD2; // light probes + vertex point lights
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float3 worldNormal = WorldNormal(v.normal);

                half4 n = half4(worldNormal, 1.0);
                float3 vertexLight = SHEvalLinearL0L1(n) + SHEvalLinearL2(n);
            #if defined(VERTEXLIGHT_ON)
                vertexLight += FourPointLights(mul(unity_ObjectToWorld, v.vertex).xyz, worldNormal);
            #endif

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.worldNormal = worldNormal;
                o.vertexLight = vertexLight;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, i.uv) * _Color;

                half4 color;
                color.rgb = DiffuseLight(albedo.rgb, i.worldNormal, _WorldSpaceLightPos0.xyz);
                color.rgb += albedo.rgb * i.vertexLight;
                color.a = albedo.a;
                return color;
            }

        #endif
            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE" = "ForwardAdd" "RenderType" = "Opaque" }
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  worldNormal : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.worldNormal = WorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, i.uv) * _Color;

                // Directional light only (the original had no point/spot attenuation)
                half4 color;
                color.rgb = DiffuseLight(albedo.rgb, i.worldNormal, (half3)_WorldSpaceLightPos0.xyz);
                color.a = 0.0;
                return color;
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}

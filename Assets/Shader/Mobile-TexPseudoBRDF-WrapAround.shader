// Readable, corrected version of "MADFINGER/Characters/BRDFLit (Supports Backlight)".
// Rewritten from the Shadowgun 1.3.5 GLES2 programs for Unity 2018.
//
// Normal-direction fixes compared with the generated code:
//  1. The normal map is unpacked with UnpackNormal(), which handles compressed normal maps
//     (X stored in alpha). The raw (x * 2 - 1) read gives the wrong X on those maps, so some
//     faces get a normal facing away from the light and go black.
//  2. The tangent is made perpendicular to the normal (Gram-Schmidt). Mesh tangents are often
//     slightly skewed, which tilts the tangent-space basis.
//  3. The world normal is transformed with the inverse transpose and normalised. The generated
//     code used the object-to-world matrix and did not renormalise, so the probe lighting
//     scaled with object scale and could clip to white.
//  4. The light direction is normalised in object space. This keeps it unit length for
//     non-uniform scale.
//
// Lighting model, probe handling and output are otherwise unchanged.

Shader "MADFINGER/Characters/BRDFLit (Supports Backlight) (Readable)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
        _LightProbesLightingAmount ("Light probes lighting amount", Range(0,1)) = 0.9
    }

    SubShader
    {
        LOD 400
        Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ VERTEXLIGHT_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _BumpMap;
            float4    _BumpMap_ST;
            sampler2D _BRDFTex;
            float     _LightProbesLightingAmount;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float4 uv          : TEXCOORD0; // xy = main texture, zw = bump texture
                half3  probeLight  : TEXCOORD1; // light probe lighting, clamped
                half3  lightDirTS  : TEXCOORD2; // main light direction, tangent space
                half3  vertexLight : TEXCOORD3; // extra point-light lighting (zero unless VERTEXLIGHT_ON)
                half3  viewDirTS   : TEXCOORD4; // view direction, tangent space
            };

            // Four point lights, evaluated per vertex (VERTEXLIGHT_ON only).
            // Normal must be unit length.
            float3 VertexPointLights(float3 worldPos, float3 worldNormal)
            {
                float4 toLightX = unity_4LightPosX0 - worldPos.x;
                float4 toLightY = unity_4LightPosY0 - worldPos.y;
                float4 toLightZ = unity_4LightPosZ0 - worldPos.z;

                float4 distSq = toLightX * toLightX + toLightY * toLightY + toLightZ * toLightZ;

                // N.L for each light, divided by distance (rsqrt), clamped at zero
                float4 nDotL = toLightX * worldNormal.x + toLightY * worldNormal.y + toLightZ * worldNormal.z;
                float4 diffuse = max(float4(0.0, 0.0, 0.0, 0.0), nDotL * rsqrt(distSq));

                // Distance attenuation
                float4 attenuation = 1.0 / (1.0 + distSq * unity_4LightAtten0);
                diffuse *= attenuation;

                return unity_LightColor[0].rgb * diffuse.x
                     + unity_LightColor[1].rgb * diffuse.y
                     + unity_LightColor[2].rgb * diffuse.z
                     + unity_LightColor[3].rgb * diffuse.w;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                // Object-space tangent frame
                float3 objNormal  = normalize(v.normal);
                float3 objTangent = normalize(v.tangent.xyz);

                // Make the tangent perpendicular to the normal (Gram-Schmidt)
                objTangent = normalize(objTangent - objNormal * dot(objNormal, objTangent));
                float3 objBinormal = cross(objNormal, objTangent) * v.tangent.w;

                // Rows are T, B, N, so mul(objectToTangent, dir) gives the direction in tangent space
                float3x3 objectToTangent = float3x3(objTangent, objBinormal, objNormal);

                // World-space normal. Normals transform with the inverse transpose of the
                // object-to-world matrix, which is mul(normal, unity_WorldToObject).
                // The result is normalised, so probe lighting does not depend on object scale.
                float3 worldNormal = normalize(mul(objNormal, (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Light probes: spherical harmonics for the unit normal, clamped and lifted by the amount
                float3 probe = ShadeSH9(float4(worldNormal, 1.0));
                o.probeLight = saturate(probe + (1.0 - _LightProbesLightingAmount));

                // Main light: direction into object space, normalised (the light is a direction, w = 0)
                float3 lightObj = normalize(mul((float3x3)unity_WorldToObject, _WorldSpaceLightPos0.xyz));
                o.lightDirTS = mul(objectToTangent, lightObj);

                // View direction: camera in object space minus the vertex, into tangent space
                float3 cameraObj = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;
                o.viewDirTS = mul(objectToTangent, normalize(cameraObj - v.vertex.xyz));

                // Point lights
                o.vertexLight = float3(0.0, 0.0, 0.0);
            #if defined(VERTEXLIGHT_ON)
                o.vertexLight = VertexPointLights(worldPos, worldNormal);
            #endif

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 albedoGloss = tex2D(_MainTex, i.uv.xy);

                // Unpack the normal map. UnpackNormal handles compressed (X in alpha) maps.
                half3 normalTS = normalize(UnpackNormal(tex2D(_BumpMap, i.uv.zw)));

                half3 L = normalize(i.lightDirTS);
                half3 V = normalize(i.viewDirTS);
                half3 H = normalize(L + V);

                // BRDF lookup: x = N.L remapped to 0..1, y = N.H
                half2 brdfUV;
                brdfUV.x = dot(normalTS, L) * 0.5 + 0.5;
                brdfUV.y = dot(normalTS, H);
                half4 brdf = tex2D(_BRDFTex, brdfUV);

                // Probe-lit albedo, shaded by the BRDF, with gloss from the main texture alpha
                half3 albedoLit = albedoGloss.rgb * i.probeLight;
                half3 color = albedoLit * (brdf.rgb + albedoGloss.a * brdf.a) * 2.0;

                // Point-light contribution
                color += albedoLit * i.vertexLight;

                // Alpha is zero, as in the original (opaque geometry, no blending)
                return fixed4(color, 0.0);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
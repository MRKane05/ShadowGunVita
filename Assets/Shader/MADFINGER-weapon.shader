// Readable, corrected version of "MADFINGER/Environment/weapon".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Normal fixes, the same as the other MADFINGER shaders:
//  1. The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//     and normalised. The generated code multiplied by the object-to-world matrix, which skews
//     normals under non-uniform scale, and did not renormalise. That changes both the light
//     probe colour and the specular direction.
//  2. The normal is normalised again in the fragment stage, after interpolation. Interpolated
//     normals shrink, which made the specular highlight dim and shift across large faces.
//
// Other changes:
//  - The two LIGHTMAP_ON / default variants in the original were identical, so they are merged.
//  - The specular term is computed per pixel. The original computed it per vertex, which
//    produces a blotchy highlight on long, flat parts such as a weapon barrel.
//  - The specular colour uses the probe colour's direction (normalised), as the original did.
//    A zero-length probe colour is guarded against, so it cannot produce NaN on the GPU.

Shader "MADFINGER/Environment/Weapon (Readable)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _Shininess ("Shininess", Range(0.01, 1)) = 0.078125
        _SHLightingScale ("Light probe influence scale", Float) = 1
    }

    SubShader
    {
        LOD 100
        Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float     _Shininess;
            float     _SHLightingScale;

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
                half3  probe       : TEXCOORD1; // unscaled light probe (spherical harmonics) colour
                float3 worldNormal : TEXCOORD2; // unit world-space normal
                float3 worldPos    : TEXCOORD3;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;

                // Inverse transpose of the object-to-world matrix, normalised
                o.worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Light probes for the unit normal (the original fed the unnormalised normal to SH)
                o.probe = ShadeSH9(float4(o.worldNormal, 1.0));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 albedoGloss = tex2D(_MainTex, i.uv);

                float3 N = normalize(i.worldNormal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 V = normalize(i.worldPos - _WorldSpaceCameraPos);
                float3 R = reflect(V, N);

                // Specular lobe, with the exponent from the shininess range (0.01..1 maps to 1.28..128)
                float specLobe = pow(saturate(dot(L, R)), _Shininess * 128.0);

                // Specular colour: the probe colour's direction, so the highlight picks up the environment's hue
                half3 probeDir = i.probe / max(length(i.probe), 1e-4);
                half3 specular = probeDir * specLobe * 2.0;

                // Diffuse: albedo lit by the probe, scaled. Specular is weighted by the gloss in alpha.
                half3 color = albedoGloss.rgb * i.probe * _SHLightingScale
                            + specular * albedoGloss.a;

                return fixed4(color, albedoGloss.a);
            }
            ENDCG
        }
    }
}
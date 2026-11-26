// Readable version of "MADFINGER/Environment/Cube env map (Supports LightProbes)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Albedo plus a cubemap reflection weighted by the texture alpha (gloss), lit per-vertex by
// the light probes (scaled by _SHLightingScale). For dynamic objects that cannot use a lightmap.
// _Spread is exposed in the material but was not used by the original either.
//
// Normal fix:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code multiplied by the object-to-world matrix without
//    renormalising, so both the reflection and the probe lighting were wrong on scaled objects
//    (a 2x scale made probe lighting up to 2x too bright). For unscaled objects the result is
//    unchanged.
//
// Other changes:
//  - The hand-expanded spherical harmonics code is replaced by Unity's SHEvalLinearL0L1 +
//    SHEvalLinearL2, which is the same maths. ShadeSH9 is deliberately not used: in Unity 2018
//    it also applies a gamma conversion when the project is in linear space, which would change
//    the brightness.
//  - The reflection's X is negated, as in the original, to match how the cubemaps were authored.

Shader "MADFINGER/Environment/Cube env map (Supports LightProbes)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _EnvTex ("Cube env tex", CUBE) = "black" {}
        _Spread ("Spread", Range(0.1,0.5)) = 0.5
        _SHLightingScale ("LightProbe influence scale", Float) = 1
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
            float       _SHLightingScale;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 reflDir  : TEXCOORD1; // world-space reflection, X mirrored
                float3 lighting : COLOR0;    // light probe colour
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;

                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.reflDir = reflect(worldPos - _WorldSpaceCameraPos, worldNormal);
                o.reflDir.x = -o.reflDir.x;

                half4 n = half4(worldNormal, 1.0);
                o.lighting = (SHEvalLinearL0L1(n) + SHEvalLinearL2(n)) * _SHLightingScale;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                color.rgb += texCUBE(_EnvTex, i.reflDir).rgb * color.a;
                color.rgb *= i.lighting;
                return color;
            }
            ENDCG
        }
    }
}

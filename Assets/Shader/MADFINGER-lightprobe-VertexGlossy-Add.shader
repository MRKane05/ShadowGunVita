// Readable version of "MADFINGER/Environment/Lightprobes with Gloss Per-Vertex Additive".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Per-vertex specular highlight from a fixed world direction (_SpecDir).
// The albedo is lit per-vertex by the light probes (scaled by _SHLightingScale), then the
// highlight, weighted by the texture alpha (gloss), is added on top.
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code multiplied by the object-to-world matrix without
//    renormalising, so the highlight and probe lighting were wrong on scaled objects (a 2x scale
//    made probe lighting up to 2x too bright). For unscaled objects the result is unchanged.
//
// Other changes:
//  - The LIGHTMAP_ON and default variants were identical (neither uses a lightmap), so they are
//    merged and the multi_compile is removed.
//  - The generated code sent an unused vector to the fragment stage. That interpolator is removed.
//  - The hand-expanded spherical harmonics code is replaced by Unity's SHEvalLinearL0L1 +
//    SHEvalLinearL2, which is the same maths. ShadeSH9 is deliberately not used: in Unity 2018
//    it also applies a gamma conversion when the project is in linear space.

Shader "MADFINGER/Environment/Lightprobes with Gloss Per-Vertex Additive"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecDir ("Specular Direction", Vector) = (1,1,0,0)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.01,1)) = 0.078125
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

            sampler2D _MainTex;
            float     _SHLightingScale;
            float     _Shininess;
            float3    _SpecColor;
            float3    _SpecDir;

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
                half3  specular : TEXCOORD3; // per-vertex specular colour
                half3  lighting : TEXCOORD4; // light probe colour
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;

                // Inverse transpose of the object-to-world matrix, normalised
                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                float3 reflected = reflect(viewDir, worldNormal);

                // Specular lobe towards _SpecDir (shininess 0.01..1 maps to an exponent of 1.28..128)
                float specLobe = pow(saturate(dot(normalize(_SpecDir), reflected)), _Shininess * 128.0);
                o.specular = _SpecColor * specLobe * 2.0;

                half4 n = half4(worldNormal, 1.0);
                o.lighting = (SHEvalLinearL0L1(n) + SHEvalLinearL2(n)) * _SHLightingScale;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                color.rgb = color.rgb * i.lighting + i.specular * color.a;
                return color;
            }
            ENDCG
        }
    }
}

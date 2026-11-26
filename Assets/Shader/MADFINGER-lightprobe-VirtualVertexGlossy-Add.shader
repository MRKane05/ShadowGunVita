// Readable version of "MADFINGER/Environment/Lightprobes with VirtualGloss Per-Vertex Additive".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// "Virtual" gloss: a per-vertex specular highlight from a light placed at a fixed offset from
// the camera (_SpecOffset, in view space), fading out with distance (_SpecRange).
// The albedo is lit per-vertex by the light probes (scaled by _SHLightingScale), then the
// highlight, weighted by the texture alpha (gloss), is added on top.
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The world normal (for the probes) uses the inverse transpose (mul(normal, unity_WorldToObject))
//    and the view normal (for the highlight) uses UNITY_MATRIX_IT_MV, both normalised. The
//    generated code used the plain object-to-world and model-view matrices without renormalising,
//    which skews lighting on scaled objects. For unscaled objects the result is unchanged.
//
// Other changes:
//  - The LIGHTMAP_ON and default variants were identical (neither uses a lightmap), so they are
//    merged and the multi_compile is removed.
//  - The generated code sent an unused vector to the fragment stage. That interpolator is removed.
//  - The hand-expanded spherical harmonics code is replaced by Unity's SHEvalLinearL0L1 +
//    SHEvalLinearL2, which is the same maths. ShadeSH9 is deliberately not used: in Unity 2018
//    it also applies a gamma conversion when the project is in linear space.

Shader "MADFINGER/Environment/Lightprobes with VirtualGloss Per-Vertex Additive"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecOffset ("Specular Offset from Camera", Vector) = (1,10,2,0)
        _SpecRange ("Specular Range", Float) = 20
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
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
            float3    _SpecOffset;
            float     _SpecRange;

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

                // View space: the camera is at the origin looking down -Z
                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.normal)));
                float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;

                // Vector from the virtual light (offset from the camera) to the vertex
                float3 fromLight = viewPos - _SpecOffset * float3(1.0, 1.0, -1.0);

                // Blinn half vector between the light direction and the view axis (+Z)
                float3 halfDir = normalize((float3(0.0, 0.0, 1.0) + normalize(-fromLight)) * 0.5);

                float specLobe = pow(saturate(dot(viewNormal, halfDir)), _Shininess * 128.0);
                float rangeFade = 1.0 - saturate(length(fromLight) / _SpecRange);

                o.specular = _SpecColor * specLobe * 2.0 * rangeFade;

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

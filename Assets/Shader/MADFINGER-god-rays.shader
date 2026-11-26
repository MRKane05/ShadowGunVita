// Readable version of "MADFINGER/Transparent/GodRays".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive light shafts. They are invisible right next to the camera (fully visible from
// _FadeOutDistNear) and fade out over 5 units beyond _FadeOutDistFar. As they fade near the
// camera, vertices pull inwards along the normal (by vertex alpha * _ContractionAmount), so the
// shaft narrows rather than clipping through the camera.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Transparent/GodRays"
{
    Properties
    {
        _MainTex ("Base texture", 2D) = "white" {}
        _FadeOutDistNear ("Near fadeout dist", Float) = 10
        _FadeOutDistFar ("Far fadeout dist", Float) = 10000
        _Multiplier ("Multiplier", Float) = 1
        _ContractionAmount ("Near contraction amount", Float) = 5
    }

    SubShader
    {
        LOD 100
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float     _FadeOutDistNear;
            float     _FadeOutDistFar;
            float     _Multiplier;
            float     _ContractionAmount;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;    // rgb = colour, a = contraction amount
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                half4  color : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float dist = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);

                // (dist / near)^4 close to the camera, falling to 0 over 5 units past far
                float nearFade = saturate(dist / _FadeOutDistNear);
                float farFade = 1.0 - saturate(max(dist - _FadeOutDistFar, 0.0) * 0.2);
                nearFade *= nearFade;
                float fade = nearFade * nearFade * farFade * farFade;

                // Contract along the normal as the shaft fades out
                float4 pos = v.vertex;
                pos.xyz -= normalize(v.normal) * saturate(1.0 - fade) * v.color.a * _ContractionAmount;

                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                o.uv = v.texcoord.xy;
                o.color = fade * v.color * _Multiplier;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }
            ENDCG
        }
    }
}

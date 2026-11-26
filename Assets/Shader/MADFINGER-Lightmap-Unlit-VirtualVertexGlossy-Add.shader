// Readable version of "MADFINGER/Environment/Virtual Gloss Per-Vertex Additive (Supports Lightmap)".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// "Virtual" gloss: a per-vertex specular highlight from a light placed at a fixed offset from
// the camera (_SpecOffset, in view space), fading out with distance (_SpecRange). The highlight
// is weighted by the texture's alpha (gloss), added to the albedo, and the result is multiplied
// by the baked lightmap.
// The base UV scrolls by _ScrollingSpeed (the material tiling/offset fields are not used).
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The view-space normal is transformed with the inverse transpose of the model-view matrix
//    (UNITY_MATRIX_IT_MV) and normalised, as Unity's COMPUTE_VIEW_NORMAL does. The generated code
//    used the model-view matrix itself and did not renormalise, which skews the highlight on
//    scaled objects. For unscaled and uniformly scaled objects the result is unchanged.

Shader "MADFINGER/Environment/Virtual Gloss Per-Vertex Additive (Supports Lightmap)"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecOffset ("Specular Offset from Camera", Vector) = (1,10,2,0)
        _SpecRange ("Specular Range", Float) = 20
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.01,1)) = 0.078125
        _ScrollingSpeed ("Scrolling speed", Vector) = (0,0,0,0)
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
            float4    _ScrollingSpeed;
            float3    _SpecOffset;
            float     _SpecRange;
            float3    _SpecColor;
            float     _Shininess;

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
                half3  specular   : TEXCOORD2; // per-vertex specular colour
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = (v.texcoord + frac(_ScrollingSpeed * _Time.y)).xy;
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

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
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);

                color.rgb += i.specular * color.a;

                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;
                return color;
            }
            ENDCG
        }
    }
}

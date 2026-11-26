// Readable version of "MADFINGER/Environment/Cubemap specular + Lightmap".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Albedo (scrolling) plus a cubemap reflection weighted by the texture alpha (gloss) and
// _SpecularStrength, then multiplied by the baked lightmap.
//
// Normal fix:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject)).
//    The generated code multiplied by the object-to-world matrix, which bends the reflection on
//    non-uniformly scaled objects. For unscaled and uniformly scaled objects the result is unchanged.

Shader "MADFINGER/Environment/Cubemap specular + Lightmap"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _SpecCubeTex ("SpecCube", CUBE) = "black" {}
        _SpecularStrength ("Specular strength", Float) = 1
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

            sampler2D   _MainTex;
            float4      _ScrollingSpeed;   // xy = UV scroll speed
            samplerCUBE _SpecCubeTex;
            float       _SpecularStrength;

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
                float3 reflDir    : TEXCOORD2; // world-space reflection vector
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy + frac(_ScrollingSpeed.xy * _Time.y);
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.reflDir = reflect(worldPos - _WorldSpaceCameraPos, worldNormal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                half3 spec = texCUBE(_SpecCubeTex, i.reflDir).rgb * (half)_SpecularStrength * color.a;

                color.rgb += spec;

                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;
                return color;
            }
            ENDCG
        }
    }
}

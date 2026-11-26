// Readable version of "MADFINGER/Environment/Lightmap + Wind".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Lightmapped, alpha-blended foliage bent by wind. The vertex alpha is the bending factor
// (0 = rigid, 1 = fully bent). The motion is Unity's standard vegetation wind: a main bend in
// the wind direction plus an edge flutter along the normal, both driven by smoothed tri
// waves.
//
// Behaviour is unchanged. Two things worth knowing:
//  - The per-object wave phase reads unity_ObjectToWorld[3].xyz, the bottom row of the matrix,
//    which is (0,0,0) for every object, so all objects wave in sync. It was probably meant to be
//    the object's position (unity_ObjectToWorld._14_24_34). Kept as-is to preserve the look.
//  - The generated code sent the vertex colour to the fragment stage, which never used it. That
//    interpolator is removed.

Shader "MADFINGER/Environment/Lightmap + Wind"
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _Wind ("Wind params", Vector) = (1,1,1,1)
        _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
        _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
    }

    SubShader
    {
        LOD 100
        Tags { "LIGHTMODE" = "ForwardBase" "QUEUE" = "Transparent" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "LIGHTMODE" = "ForwardBase" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _Wind;                     // xyz = world direction/strength, w = bend amount
            float     _WindEdgeFlutter;
            float     _WindEdgeFlutterFreqScale;

            // Unity 3.5's _World2Object: the inverse world matrix with the object's uniform scale
            // removed. The wind vector is taken into object space with it.
            float4x4 World2ObjectUnscaled()
            {
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));
                float4x4 m = unity_WorldToObject;
                m[0] *= scale;
                m[1] *= scale;
                m[2] *= scale;
                return m;
            }

            // Smoothed tri wave in 0..1 (the same as Unity's TerrainEngine SmoothTriangleWave)
            float4 SmoothTriangleWave(float4 x)
            {
                float4 tri = abs(frac(x + 0.5) * 2.0 - 1.0);
                return tri * tri * (3.0 - 2.0 * tri);
            }

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float4 color     : COLOR;   // a = bending factor
                float3 normal    : NORMAL;
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

                float bendingFact = v.color.a;

                // Wind in object space; w scales the whole effect by the bending factor
                float4 wind;
                wind.xyz = mul(_Wind.xyz, (float3x3)transpose(World2ObjectUnscaled()));
                wind.w = _Wind.w * bendingFact;

                // Wave phases: x = edge flutter (varies along the mesh), y = main bend
                float objPhase = dot(unity_ObjectToWorld[3].xyz, float3(1.0, 1.0, 1.0)); // always 0, see header
                float2 phase = float2(dot(v.vertex.xyz, (float3)(_WindEdgeFlutter + objPhase)), objPhase);
                float2 waveIn = (_Time.y * float2(_WindEdgeFlutterFreqScale, 1.0)).xx + phase;

                float4 waves = frac(waveIn.xxyy * float4(1.975, 0.793, 0.375, 0.193)) * 2.0 - 1.0;
                float4 smoothWaves = SmoothTriangleWave(waves);
                float2 wavesSum = smoothWaves.xz + smoothWaves.yw;

                // Edge flutter along the normal, vertical bob from the bending factor
                float3 bend = (_WindEdgeFlutter * 0.1) * normalize(v.normal);
                bend.y = bendingFact * 0.3;

                float4 pos = v.vertex;
                pos.xyz += (wavesSum.xyx * bend + wind.xyz * wavesSum.y * bendingFact) * wind.w;
                pos.xyz += bendingFact * wind.xyz; // static lean into the wind

                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.uvLightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);

                // The lightmap is stored at half intensity, so it is doubled
                color.rgb *= 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).rgb;
                return color;
            }
            ENDCG
        }
    }
}

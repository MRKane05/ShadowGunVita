// Readable version of "MADFINGER/Transparent/Moving waves FX".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive light bands that sweep along a mesh. Vertex red is the position along the sweep
// (0..1). Four independent waves (one per x/y/z/w channel of the _Wave* vectors) travel from
// -size to 1 + size at _WaveSpeed. Each is a smooth bump of half-width _WaveSize, scaled by
// _WaveAmplitude and offset by _WaveBias. Each wave adds its own colour (_Color0.._Color3).
// Distance fade and growth work like the god-ray shaders. _Bias is exposed in the material but
// was not used by the original either.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Transparent/Moving waves FX"
{
    Properties
    {
        _MainTex ("Base texture", 2D) = "white" {}
        _FadeOutDistNear ("Near fadeout dist", Float) = 10
        _FadeOutDistFar ("Far fadeout dist", Float) = 10000
        _Multiplier ("Color multiplier", Float) = 1
        _Bias ("Bias", Float) = 0
        _SizeGrowStartDist ("Size grow start dist", Float) = 5
        _SizeGrowEndDist ("Size grow end dist", Float) = 50
        _MaxGrowSize ("Max grow size", Float) = 2.5
        _WaveSpeed ("Waves speed", Vector) = (0.25,0.25,0.25,0.25)
        _WaveSize ("Waves size", Vector) = (0.25,0.25,0.25,0.25)
        _WaveAmplitude ("Waves amp", Vector) = (1,0,0,0)
        _WaveBias ("Waves bias", Vector) = (0,0,0,0)
        _WaveTimeOffs ("Wave time offs", Vector) = (0,0,0,0)
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,0,0,1)
        _Color2 ("Color2", Color) = (0,1,0,1)
        _Color3 ("Color3", Color) = (0,0,1,1)
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
            float     _SizeGrowStartDist;
            float     _SizeGrowEndDist;
            float     _MaxGrowSize;
            float4    _WaveSpeed;
            float4    _WaveSize;
            float4    _WaveAmplitude;
            float4    _WaveBias;
            float4    _WaveTimeOffs;
            float4    _Color0;
            float4    _Color1;
            float4    _Color2;
            float4    _Color3;

            // GLSL-style fmod that keeps the sign of x (written out as in the original)
            float4 FMod(float4 x, float4 y)
            {
                float4 q = x / y;
                float4 r = frac(abs(q)) * y;
                return float4(q.x >= 0.0 ? r.x : -r.x,
                              q.y >= 0.0 ? r.y : -r.y,
                              q.z >= 0.0 ? r.z : -r.z,
                              q.w >= 0.0 ? r.w : -r.w);
            }

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;    // r = position along the sweep, a = grow amount
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

                float3 normal = normalize(v.normal);
                float dist = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);

                // (dist / near)^4 close to the camera, falling to 0 over 5 units past far
                float nearFade = saturate(dist / _FadeOutDistNear);
                float farFade = 1.0 - saturate(max(dist - _FadeOutDistFar, 0.0) * 0.2);
                float grow = min(max(dist - _SizeGrowStartDist, 0.0) / _SizeGrowEndDist, 1.0);

                // Each wave's centre runs from -size to 1 + size, then repeats
                float4 travel = float4(1.0, 1.0, 1.0, 1.0) + 2.0 * _WaveSize;
                float4 waveCentre = FMod((_Time.y + _WaveTimeOffs) * _WaveSpeed, travel) - _WaveSize;

                // Smooth bump: 1 at the centre, 0 at +-size and beyond
                float4 offset = abs(v.color.xxxx - waveCentre);
                float4 s = offset / _WaveSize;
                float4 bump = float4(1.0, 1.0, 1.0, 1.0) - s * s * (float4(3.0, 3.0, 3.0, 3.0) - 2.0 * s);
                bump = float4(offset.x > _WaveSize.x ? 0.0 : bump.x,
                              offset.y > _WaveSize.y ? 0.0 : bump.y,
                              offset.z > _WaveSize.z ? 0.0 : bump.z,
                              offset.w > _WaveSize.w ? 0.0 : bump.w);
                float4 waves = bump * _WaveAmplitude + _WaveBias;

                float4 pos = v.vertex;
                pos.xyz += grow * grow * _MaxGrowSize * v.color.a * normal;

                nearFade *= nearFade;
                float fade = nearFade * nearFade * farFade * farFade;
                float4 waveColor = _Color0 * waves.x + _Color1 * waves.y + _Color2 * waves.z + _Color3 * waves.w;

                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                o.uv = v.texcoord.xy;
                o.color = fade * waveColor * _Multiplier;
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

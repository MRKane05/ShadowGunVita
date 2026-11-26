// Readable version of "MADFINGER/FX/Lightning bolt FX".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive electric arc. The mesh is a strip along the bolt:
//  - normal: the bolt direction
//  - TEXCOORD0.x: distance along the bolt; TEXCOORD0.y: -1..1 across the strip
//  - TEXCOORD1.x: a per-bolt random seed; TEXCOORD1.y: 0..1 along the bolt (fades the ends)
// Each bolt is on for _Duration.x and off for _Duration.y seconds (both randomised 70-100%
// per bolt), and switches on as a wave travelling along it (_OtherParams.y). Vertices wobble
// sideways by two 4-octave value noises (_NoiseFreqs/_NoiseSpeeds/_NoiseAmps, scaled by
// _Amplitude), and the strip is widened in screen space to _OtherParams.x (corrected by the
// aspect ratio in _OtherParams.z).
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/FX/Lightning bolt FX"
{
    Properties
    {
        _MainTex ("Main tex", 2D) = "white" {}
        _Duration ("Duration (x - ON, y - OFF)", Vector) = (0.5,2,0,0)
        _Amplitude ("Amplitude (x - horizontal, y - vertical)", Vector) = (0.2,0.01,0,0)
        _NoiseFreqs ("NoiseFreqs", Vector) = (4,8,16,32)
        _NoiseSpeeds ("NoiseSpeeds", Vector) = (3.2,2.3,0.5,1)
        _NoiseAmps ("NoiseAmps", Vector) = (2,1,0.5,0.125)
        _OtherParams ("x - line width, y - inv wave speed, z - aspect ratio", Vector) = (0.2,0.025,0,0)
        _Color ("Color", Color) = (0.7,0.8,1.3,1)
    }

    SubShader
    {
        LOD 100
        Tags { "QUEUE" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Amplitude;
            float4 _Color;
            float4 _Duration;
            float4 _NoiseAmps;
            float4 _NoiseFreqs;
            float4 _NoiseSpeeds;
            float4 _OtherParams;

            // Pseudo-random 0..1: frac(sin(x * 78.233) * 43758.5), using a parabola instead of sin.
            // The constants are kept exactly as compiled.
            float4 Hash(float4 x)
            {
                float4 s = frac((0.159155 * 78.233) * x + 0.5) * 2.0 - 1.0;
                return frac((4.0 * 43758.5) * (s - s * abs(s)));
            }

            // Four octaves of 1D value noise in -1..1, weighted by _NoiseAmps
            float ValueNoise(float4 x)
            {
                float4 cell = frac(x);
                float4 smoothCell = cell * cell * (float4(3.0, 3.0, 3.0, 3.0) - 2.0 * cell);
                float4 corner = x - cell;
                return dot(lerp(Hash(corner), Hash(corner + 1.0), smoothCell) * 2.0 - 1.0, _NoiseAmps);
            }

            float4 ClipPos(float3 objPos)
            {
                return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(objPos, 1.0)));
            }

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float3 normal    : NORMAL;    // bolt direction
                float4 texcoord  : TEXCOORD0; // x = distance along, y = -1..1 across
                float4 texcoord1 : TEXCOORD1; // x = random seed, y = 0..1 along
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                half4  color : TEXCOORD1; // rgb = colour, a = end fade
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float3 boltDir = normalize(v.normal);

                // Basis perpendicular to the bolt (switch up vector when the bolt is vertical)
                float3 forward = normalize(boltDir);
                float3 up = (abs(forward.y) > 0.999) ? float3(0.0, 0.0, 1.0) : float3(0.0, 1.0, 0.0);
                float3 side = normalize(cross(up, forward));
                up = cross(forward, side);

                float along = v.texcoord.x * 0.2;
                float time = _Time.y * 0.5 + v.texcoord1.x * 536.375;

                // Per-bolt on/off durations, 70-100% of _Duration
                float4 durationScale = float4(0.7, 0.7, 0.7, 0.7) + Hash((float4)(v.texcoord1.x * 63.2771)) * 0.3;
                float onTime = _Duration.x * durationScale.x;
                float period = onTime + _Duration.y * durationScale.y;

                // Time within the cycle, delayed along the bolt (fmod written out, as in the original)
                float cycles = (time - along * _OtherParams.y) / period;
                float cycleTime = frac(abs(cycles)) * period;
                if (cycles < 0.0)
                    cycleTime = -cycleTime;

                // Fade in over the first 10% of ON, out over the last 10%
                float intensity = smoothstep(0.0, onTime * 0.1, cycleTime)
                                * (1.0 - smoothstep(onTime * 0.9, onTime, cycleTime));

                // Sideways wobble
                float2 wobble;
                wobble.x = ValueNoise(((float4)along + float4(183.818, 1041.63, 3860.17, 5575.81) + time * _NoiseSpeeds) * _NoiseFreqs);
                wobble.y = ValueNoise(((float4)along + float4(5575.81, 183.818, 1041.63, 3860.17) + time * _NoiseSpeeds) * _NoiseFreqs);
                wobble *= _Amplitude.xy * intensity;

                float4 pos = v.vertex;
                pos.xyz = v.vertex.xyz + side * wobble.x + up * wobble.y;

                // Screen-space direction of the bolt, used to widen the strip across it
                float4 clipA = ClipPos(pos.xyz);
                float4 clipB = ClipPos(pos.xyz + boltDir);
                float2 screenDir = normalize(clipB.xy / clipB.w - clipA.xy / clipA.w);

                float4 clipPos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                float2 perpendicular = screenDir.yx * float2(1.0, -1.0 * _OtherParams.z);
                o.pos = clipPos;
                o.pos.xy = clipPos.xy + perpendicular * _OtherParams.x * v.texcoord.y * intensity;

                o.uv = v.texcoord.xy;
                // Hidden within 1 unit of the camera; ends fade out
                o.color.rgb = _Color.rgb * intensity * saturate(clipPos.w - 1.0) * 2.0;
                o.color.a = saturate(1.0 - pow(2.0 * abs(0.5 - v.texcoord1.y), 8.0));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Bright core falling off to the strip edges
                half core = (1.0 - abs(i.uv.y)) * i.color.a;
                return core * core * i.color;
            }
            ENDCG
        }
    }
}

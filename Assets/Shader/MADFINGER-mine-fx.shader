// Readable version of "MADFINGER/FX/Mine-FX".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Repeating "ping" ring for mines: the texture grows from the centre of the quad to
// _Params.y times its size over _Params.x seconds, fading out as it grows (alpha = 1 - t^3),
// then waits _Params.w seconds before the next ping. _Params.z offsets the timing.
//
// Changes from the generated version:
//  - The fade is passed as a single value instead of a colour with RGB fixed at 1.

Shader "MADFINGER/FX/Mine-FX"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Params ("X = period, Y = max scale, Z = time offs, W = delay", Vector) = (1,5,0,0)
    }

    SubShader
    {
        LOD 100
        Tags { "QUEUE" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" }
            ZWrite Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _Params; // x = grow time, y = max scale, z = time offset, w = delay

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                half   alpha : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                // Time within the cycle (fmod written out, as in the original)
                float period = _Params.x + _Params.w;
                float cycles = (_Time.y + _Params.z) / period;
                float cycleTime = frac(abs(cycles)) * period;
                if (cycles < 0.0)
                    cycleTime = -cycleTime;

                float t = saturate(cycleTime / _Params.x); // 0..1 while growing, 1 during the delay

                o.pos = UnityObjectToClipPos(v.vertex);
                // Scale the UVs about the centre so the texture appears to grow
                o.uv = (v.texcoord.xy - float2(0.5, 0.5)) * (1.0 / (t * _Params.y + 0.05)) + float2(0.5, 0.5);
                o.alpha = 1.0 - pow(t, 3.0);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                color.a *= i.alpha;
                return color;
            }
            ENDCG
        }
    }
}

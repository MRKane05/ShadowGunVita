// Readable version of "MADFINGER/Environment/Blinking emissive".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive emissive lights that switch on and off. Each light flickers while "on" and stays
// dark while "off". The vertex colour is both the light colour and the random seed, so lights
// with different colours blink out of step.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Environment/Blinking emissive"
{
    Properties
    {
        _MainTex ("Base texture", 2D) = "white" {}
        _IntensityScaleBias ("Intensity scale X / bias Y", Vector) = (1,0.1,0,0)
        _SwitchOnOffDuration ("Switch ON (X) / OFF (Y) duration", Vector) = (1,3,0,0)
        _BlinkingRate ("Blinking rate", Float) = 10
        _RndGridSize ("Randomization grid size", Float) = 5
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
            float2    _IntensityScaleBias;  // x = scale, y = bias
            float2    _SwitchOnOffDuration; // x = on time, y = off time (seconds)
            float     _BlinkingRate;
            // _RndGridSize is declared in Properties but was not used by the original either

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0;
                half4  intensity : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                // Per-light random seed from the vertex colour
                float seed = (half)(dot(v.color, v.color) * 40.0);

                // Flicker: pseudo-random on/off at _BlinkingRate
                float t = _Time.y * _BlinkingRate;
                float flicker = (float)(abs(cos(17.0 * sin(t * 5.0) + 10.0 * sin(seed + t * 3.0 + 7.993))) > 0.5);

                // On/off cycle, durations jittered by +-20% per light
                float2 onOff = _SwitchOnOffDuration * (0.8 + 0.4 * frac(seed));
                float period = onOff.x + onOff.y;

                // Time within the current cycle (fmod written out, as in the original)
                float cycles = (_Time.y + seed) / period;
                float cycleTime = frac(abs(cycles)) * period;
                if (cycles < 0.0)
                    cycleTime = -cycleTime;
                float isOn = (float)(cycleTime < onOff.x);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.intensity = v.color * (flicker * isOn) * _IntensityScaleBias.x + _IntensityScaleBias.y;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.intensity;
            }
            ENDCG
        }
    }
}

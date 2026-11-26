// Readable version of "MADFINGER/Transparent/Blinking GodRays".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive light shafts that pulse on and off.
//  - Fade: invisible right next to the camera (fully visible from _FadeOutDistNear) and fades out
//    over 5 units beyond _FadeOutDistFar.
//  - Grow: from _SizeGrowStartDist the shaft widens by up to _MaxGrowSize (scaled by vertex alpha).
//  - Blink: vertex blue offsets each shaft's timing by up to _BlinkingTimeOffsScale seconds.
//    With _NoiseAmount below 0.01 it is a smooth on/off pulse; otherwise a noisy flicker.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Transparent/Blinking GodRays"
{
    Properties
    {
        _MainTex ("Base texture", 2D) = "white" {}
        _FadeOutDistNear ("Near fadeout dist", Float) = 10
        _FadeOutDistFar ("Far fadeout dist", Float) = 10000
        _Multiplier ("Color multiplier", Float) = 1
        _Bias ("Bias", Float) = 0
        _TimeOnDuration ("ON duration", Float) = 0.5
        _TimeOffDuration ("OFF duration", Float) = 0.5
        _BlinkingTimeOffsScale ("Blinking time offset scale (seconds)", Float) = 5
        _SizeGrowStartDist ("Size grow start dist", Float) = 5
        _SizeGrowEndDist ("Size grow end dist", Float) = 50
        _MaxGrowSize ("Max grow size", Float) = 2.5
        _NoiseAmount ("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
        _Color ("Color", Color) = (1,1,1,1)
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
            float     _Bias;
            float     _TimeOnDuration;
            float     _TimeOffDuration;
            float     _BlinkingTimeOffsScale;
            float     _SizeGrowStartDist;
            float     _SizeGrowEndDist;
            float     _MaxGrowSize;
            float     _NoiseAmount;
            float4    _Color;

            // 0..1 fade: (dist / near)^4 close to the camera, falling to 0 over 5 units past far
            float DistanceFade(float dist)
            {
                float nearFade = saturate(dist / _FadeOutDistNear);
                float farFade = 1.0 - saturate(max(dist - _FadeOutDistFar, 0.0) * 0.2);
                nearFade *= nearFade;
                return nearFade * nearFade * farFade * farFade;
            }

            // 0..1 growth factor between _SizeGrowStartDist and _SizeGrowEndDist, squared
            float GrowFactor(float dist)
            {
                float grow = min(max(dist - _SizeGrowStartDist, 0.0) / _SizeGrowEndDist, 1.0);
                return grow * grow;
            }

            float Blink(float time)
            {
                // Time within the on/off cycle (fmod written out, as in the original)
                float period = _TimeOnDuration + _TimeOffDuration;
                float cycles = time / period;
                float cycleTime = frac(abs(cycles)) * period;
                if (cycles < 0.0)
                    cycleTime = -cycleTime;

                // Smooth pulse: fade in over the first quarter of ON, out over the last quarter
                float pulse = smoothstep(0.0, _TimeOnDuration * 0.25, cycleTime)
                            * (1.0 - smoothstep(_TimeOnDuration * 0.75, _TimeOnDuration, cycleTime));

                // Noisy flicker
                float phase = time * (6.28319 / _TimeOnDuration);
                float noise = _NoiseAmount * (sin(phase) * (0.5 * cos(phase * 0.6366 + 56.7272) + 0.5)) + (1.0 - _NoiseAmount);

                return (_NoiseAmount < 0.01) ? pulse : noise;
            }

            struct appdata_t
            {
                float4 vertex    : POSITION;
                float4 color     : COLOR;    // b = blink time offset, a = grow amount
                float3 normal    : NORMAL;
                float4 texcoord  : TEXCOORD0;
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

                // Grow outwards along the normal with distance
                float4 pos = v.vertex;
                pos.xyz += GrowFactor(dist) * _MaxGrowSize * v.color.a * normal;

                float blink = Blink(_Time.y + _BlinkingTimeOffsScale * v.color.z);

                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                o.uv = v.texcoord.xy;
                o.color = DistanceFade(dist) * _Color * _Multiplier * (blink + _Bias);
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

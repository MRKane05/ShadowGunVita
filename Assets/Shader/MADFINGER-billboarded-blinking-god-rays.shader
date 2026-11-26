// Readable version of "MADFINGER/Transparent/Blinking GodRays Billboarded".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive light shafts that pulse on and off.
//  - Fade: invisible right next to the camera (fully visible from _FadeOutDistNear) and fades out
//    over 5 units beyond _FadeOutDistFar.
//  - Grow: from _SizeGrowStartDist the shaft widens by up to _MaxGrowSize (scaled by vertex alpha).
//  - Blink: vertex blue offsets each shaft's timing by up to _BlinkingTimeOffsScale seconds.
//    With _NoiseAmount below 0.01 it is a smooth on/off pulse; otherwise a noisy flicker.
//  - Billboarding: each quad is stored collapsed to its centre. Vertex red/green (0 or 1) mark the
//    corner and TEXCOORD1.xy holds the quad size. The quad is rebuilt facing the camera;
//    _VerticalBillboarding 0 keeps it upright, 1 lets it tilt fully towards the camera.
//    _ViewerOffset pushes it towards (positive) or away from the camera. The normal's xy is the
//    grow direction in the billboard plane.
//  - The camera position is taken into object space with Unity 3.5's scale-free _World2Object
//    (emulated by World2ObjectUnscaled), as in the original.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/Transparent/Blinking GodRays Billboarded"
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
        _VerticalBillboarding ("Vertical billboarding amount", Range(0,1)) = 1
        _ViewerOffset ("Viewer offset", Float) = 0
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
            float     _VerticalBillboarding;
            float     _ViewerOffset;

            // Unity 3.5's _World2Object: the inverse world matrix with the object's uniform scale removed
            float4x4 World2ObjectUnscaled()
            {
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));
                float4x4 m = unity_WorldToObject;
                m[0] *= scale;
                m[1] *= scale;
                m[2] *= scale;
                return m;
            }

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
                float4 color     : COLOR;    // rg = corner, b = blink time offset, a = grow amount
                float3 normal    : NORMAL;
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1; // xy = quad size
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

                // Corner offset from the quad centre, in the quad's own plane
                half3 corner = half3(float2(0.5, 0.5) - v.color.xy, 0.0);
                float3 cornerOffset = corner * v.texcoord1.xyy;
                float3 centre = v.vertex.xyz + cornerOffset;

                // Direction from the quad centre to the camera, in object space
                float3 localDir = mul(World2ObjectUnscaled(), float4(_WorldSpaceCameraPos, 1.0)).xyz - centre;
                localDir.y = lerp(0.0, localDir.y, _VerticalBillboarding);
                float dist = length(localDir);
                float3 dir = localDir / dist;

                // Billboard basis (switch the up vector when looking straight up or down)
                float3 up = (abs(dir.y) > 0.999) ? float3(0.0, 0.0, 1.0) : float3(0.0, 1.0, 0.0);
                float3 right = normalize(cross(up, dir));
                up = cross(dir, right);

                float4 pos;
                pos.xyz = centre - (right * cornerOffset.x + up * cornerOffset.y)
                        + (right * normal.x + up * normal.y) * (GrowFactor(dist) * _MaxGrowSize * v.color.a)
                        + _ViewerOffset * localDir;
                pos.w = 1.0;

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

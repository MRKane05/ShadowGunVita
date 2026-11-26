// Readable version of "MADFINGER/FX/Blood FX nonblended".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Full-screen blood overlay shown when the player is hurt. The mesh is a ring of quads already
// laid out in screen space (vertex xy in -0.5..0.5); vertices with alpha 1 are the inner edge.
//  - _Params.x = damage amount 0..1. It pulls the inner edge in towards the centre of the screen.
//  - _Params.y = heartbeat rate. A sharp pulse every 1.25 / rate seconds briefly pushes the
//    edge further in (only while _Params.x > 0, and less as damage increases).
// Draws the overlay as a flat, opaque _Color.
// Changes from the generated version:
//  - Layout and naming only; output is identical.

Shader "MADFINGER/FX/Blood FX nonblended"
{
    Properties
    {
        _Params ("Params", Vector) = (0,1,0,0)
        _Color ("Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "QUEUE" = "Overlay" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Overlay" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4    _Color;
            float4    _Params; // x = damage 0..1, y = heartbeat rate

            // The vertex shader writes GL clip space directly (no matrix): z in [-w, w], never
            // flipped. Convert to this platform's depth range (reversed Z on D3D11; the Vita is
            // GL-like) and apply Unity's projection flip when rendering into a texture.
            float4 FromGLClip(float4 p)
            {
            #if defined(UNITY_REVERSED_Z)
                p.z = (p.w - p.z) * 0.5;
            #endif
                p.y *= _ProjectionParams.x;
                return p;
            }

            // GLSL-style fmod that keeps the sign of x (written out as in the original)
            float FMod(float x, float y)
            {
                float q = x / y;
                float r = frac(abs(q)) * y;
                return (q >= 0.0) ? r : -r;
            }

            // Heartbeat: a sharp x * e^(1 - x) spike (peak 1) in the first 0.5 s of each 1.25 s beat
            float Heartbeat()
            {
                float beatTime = FMod(_Time.y * _Params.y, 1.25);
                if (beatTime < 0.5)
                {
                    float x = 20.0 * FMod(beatTime, 0.5);
                    return x * exp(1.0 - x);
                }
                return 0.0;
            }

            struct appdata_t
            {
                float4 vertex   : POSITION; // xy = screen position (-0.5..0.5)
                float4 color    : COLOR;    // a = 1 on the inner (moving) edge
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float damage = _Params.x;
                float pulse = Heartbeat();
                float pulseAmount = (damage > 0.0) ? lerp(0.1, 0.0, damage) : 0.0;

                // Pull the inner edge towards the screen centre
                float2 towardsCentre = normalize(-v.vertex.xy);
                float4 pos = float4(v.vertex.xy * 2.0, 0.0, 1.0);
                pos.xy -= towardsCentre * (damage + pulse * pulseAmount) * v.color.a;

                o.pos = FromGLClip(pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return (half4)_Color;
            }
            ENDCG
        }
    }
}

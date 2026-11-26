// Readable version of "MADFINGER/FX/Blood FX - alpha blended".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Screen-space blood splats that drip down and fade out. The mesh is already in screen space.
//  - vertex.z: start-time offset for each splat (added to _Time.y)
//  - TEXCOORD1.x: fade delay. Alpha starts falling after 0.25 * x seconds and takes 1 second.
//  - TEXCOORD1.y: how much this vertex drips (0 = fixed, 1 = moves at _DrippingSpeed)
// _FadeInSpeed is exposed in the material but was not used by the original either.
//
// Changes from the generated version:
//  - The fade is passed as a single value instead of a colour with RGB fixed at 1.

Shader "MADFINGER/FX/Blood FX - alpha blended"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FadeInSpeed ("Fade in speed", Float) = 5
        _DrippingSpeed ("Dripping speed", Float) = 0.1
    }

    SubShader
    {
        Tags { "QUEUE" = "Overlay" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Overlay" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float     _DrippingSpeed;
            sampler2D _MainTex;

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

            struct appdata_t
            {
                float4 vertex    : POSITION;  // xy = screen position, z = time offset
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1; // x = fade delay, y = drip amount
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

                float time = _Time.y + v.vertex.z;

                float4 pos = float4(v.vertex.xy, 0.0, 1.0);
                pos.y -= time * _DrippingSpeed * v.texcoord1.y;

                o.pos = FromGLClip(pos);
                o.uv = v.texcoord.xy;
                o.alpha = 1.0 - max(time - 0.25 * v.texcoord1.x, 0.0);
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

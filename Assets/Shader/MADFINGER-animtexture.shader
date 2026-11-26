// Readable version of "MADFINGER/FX/Anim texture".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive flipbook. The texture is a grid of _NumTexTiles.x by _NumTexTiles.y frames played at
// _ReplaySpeed frames per second, cross-fading between the current and next frame. Vertex alpha
// offsets the start time (alpha 1 = 60 seconds), so instances do not play in sync. Vertex RGB
// tints the result.
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/FX/Anim texture"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NumTexTiles ("Num tex tiles", Vector) = (4,4,0,0)
        _ReplaySpeed ("Replay speed - FPS", Float) = 4
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
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

            float4    _Color;
            sampler2D _MainTex;
            float4    _NumTexTiles;  // xy = columns, rows
            float     _ReplaySpeed;

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
                float4 color    : COLOR;    // rgb = tint, a = time offset
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float4 uv    : TEXCOORD0; // xy = current frame, zw = next frame
                half4  color : COLOR0;    // rgb = tint, a = blend to next frame
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float frameTime = (v.color.a * 60.0 + _Time.y) * _ReplaySpeed;
                float frame = floor(frameTime);
                float nextFrame = frame + 1.0;

                // Frame index -> (column, row) for this frame and the next
                float4 tile;
                tile.xy = float2(frame, floor(frame / _NumTexTiles.x));
                tile.zw = float2(nextFrame, floor(nextFrame / _NumTexTiles.x));
                tile = FMod(tile, _NumTexTiles.xyxy);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = (v.texcoord.xyxy + tile) * (1.0 / _NumTexTiles.xy).xyxy;
                o.color.rgb = _Color.rgb * v.color.rgb;
                o.color.a = frameTime - frame;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return lerp(tex2D(_MainTex, i.uv.xy), tex2D(_MainTex, i.uv.zw), i.color.aaaa) * i.color;
            }
            ENDCG
        }
    }
}

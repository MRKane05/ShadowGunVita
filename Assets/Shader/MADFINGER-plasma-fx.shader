// Readable version of "MADFINGER/FX/Plasma FX".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// One-shot additive plasma burst. Progress t = (time + _TimeOffs) * _Speed, clamped to 0..1.
// As t rises:
//  - the mesh expands along its normals by up to _ExpansionSpeed
//  - the colour moves from _ColorStart to _ColorEnd and fades out as (1 - t)^3 * vertex alpha
//  - the noise texture dissolves it: texels below the threshold (t, unclamped) disappear, with
//    a bright rim _BorderWidth sharp at the dissolve edge
// The script drives _TimeOffs. Its default of 9999 keeps the effect finished (invisible).
// Output is identical to the generated version; only the layout and naming changed.

Shader "MADFINGER/FX/Plasma FX"
{
    Properties
    {
        _MainTex ("Noise tex", 2D) = "white" {}
        _ColorStart ("Color start", Color) = (1,0.8,0,1)
        _ColorEnd ("Color end", Color) = (0.5,0.3,0,1)
        _Speed ("Speed", Float) = 2
        _BorderWidth ("Border width", Float) = 4
        _TimeOffs ("TimeOffs", Float) = 9999
        _ExpansionSpeed ("ExpansionSpeed", Float) = 1
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
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _ColorStart;
            float4    _ColorEnd;
            float     _Speed;
            float     _BorderWidth;
            float     _TimeOffs;
            float     _ExpansionSpeed;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;    // a = opacity
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float3 uv    : TEXCOORD0; // xy = noise UV, z = dissolve threshold
                half4  color : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float progress = (_Time.y + _TimeOffs) * _Speed;
                float t = saturate(progress);

                float4 pos = float4(v.vertex.xyz + normalize(v.normal) * t * _ExpansionSpeed, 1.0);

                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, pos));
                o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.uv.z = progress;
                o.color.rgb = lerp(_ColorStart.rgb, _ColorEnd.rgb, (float3)t);
                o.color.a = v.color.a * pow(1.0 - t, 3.0);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half noise = tex2D(_MainTex, i.uv.xy).r;
                half threshold = i.uv.z;

                // Bright rim just above the dissolve threshold: (1 - (noise - threshold) * width)^4
                half rim = 1.0 - saturate((noise - threshold) * _BorderWidth);
                rim *= rim;

                // Texels below the threshold are dissolved away
                return (i.color + (half4)(rim * rim)) * (i.color.a * step(threshold, noise));
            }
            ENDCG
        }
    }
}

// Readable version of "MADFINGER/Environment/Scroll 1 Layer".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Unlit texture scrolled over time, with a per-material time offset.
//
// Changes from the generated version:
//  - The LIGHTMAP_ON and default variants were identical (neither uses the lightmap), so they
//    are merged and the multi_compile is removed. One shader variant instead of two.

Shader "MADFINGER/Environment/Scroll 1 Layer"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ScrollX ("Scroll speed X", Float) = 1
        _ScrollY ("Scroll speed Y", Float) = 0
        _TimeOffs ("Time offs", Float) = 0
    }

    SubShader
    {
        LOD 100
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float     _ScrollX;
            float     _ScrollY;
            float     _TimeOffs;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0; // scrolled UV
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);

                float2 time = _Time.xy + _TimeOffs;
                o.uv = v.texcoord.xy + frac(float2(_ScrollX, _ScrollY) * time);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}

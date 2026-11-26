// Readable version of "MADFINGER/Environment/Scroll 2 Layers Multiplicative - Skybox".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Two independently scrolling textures multiplied together, then scaled by _AMultiplier.
// Drawn just after opaque geometry with depth writes off, for sky domes.
//
// Changes from the generated version:
//  - The LIGHTMAP_ON and default variants were identical (neither uses the lightmap), so they
//    are merged and the multi_compile is removed.
//  - _AMultiplier is read directly in the fragment stage. The generated code copied this constant
//    into an interpolator, which costs a varying on the Vita for no benefit.

Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative - Skybox"
{
    Properties
    {
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
        _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1
        _ScrollY ("Base layer Scroll speed Y", Float) = 0
        _Scroll2X ("2nd layer Scroll speed X", Float) = 1
        _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
        _AMultiplier ("Layer Multiplier", Float) = 0.5
    }

    SubShader
    {
        LOD 100
        Tags { "QUEUE" = "Geometry+10" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "QUEUE" = "Geometry+10" "RenderType" = "Opaque" }
            ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            sampler2D _DetailTex;
            float4    _DetailTex_ST;
            float     _ScrollX;
            float     _ScrollY;
            float     _Scroll2X;
            float     _Scroll2Y;
            float     _AMultiplier;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uvBase   : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBase   = TRANSFORM_TEX(v.texcoord.xy, _MainTex)   + frac(float2(_ScrollX, _ScrollY)   * _Time.xy);
                o.uvDetail = TRANSFORM_TEX(v.texcoord.xy, _DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time.xy);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 layers = tex2D(_MainTex, i.uvBase) * tex2D(_DetailTex, i.uvDetail);
                return layers * (half)_AMultiplier;
            }
            ENDCG
        }
    }
}

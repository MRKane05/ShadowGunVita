// Readable version of "MADFINGER/FX/Menu - 3 layers".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Menu background: (base + add) * mul, each layer scrolling on its own, tinted by vertex colour.
// _Multiplier scales the RGB of the vertex colour, not its alpha. Alpha blended.
//
// Changes from the generated version:
//  - The LIGHTMAP_ON and default variants were identical (neither uses a lightmap), so they are
//    merged and the multi_compile is removed.

Shader "MADFINGER/FX/Menu - 3 layers"
{
    Properties
    {
        _BaseTex ("Base layer", 2D) = "white" {}
        _MulTex ("Mul layer", 2D) = "white" {}
        _AddTex ("Add layer", 2D) = "white" {}
        _ScrollBase ("Base layer scroll speed ", Vector) = (0,0,0,0)
        _ScrollMul ("Mul layer scroll speed ", Vector) = (0,0,0,0)
        _ScrollAdd ("Add layer scroll speed ", Vector) = (0,0,0,0)
        _Multiplier ("Multiplier", Float) = 1
    }

    SubShader
    {
        LOD 100
        Tags { "RenderType" = "Transparent" }

        Pass
        {
            Tags { "RenderType" = "Transparent" }
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseTex;
            sampler2D _MulTex;
            sampler2D _AddTex;
            float4    _ScrollBase;  // xy = UV scroll speed
            float4    _ScrollMul;
            float4    _ScrollAdd;
            float     _Multiplier;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float4 uvBaseMul : TEXCOORD0; // xy = base, zw = mul
                float2 uvAdd     : TEXCOORD1;
                half4  color     : TEXCOORD2;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBaseMul.xy = v.texcoord.xy + frac(_ScrollBase.xy * _Time.xy);
                o.uvBaseMul.zw = v.texcoord.xy + frac(_ScrollMul.xy * _Time.xy);
                o.uvAdd        = v.texcoord.xy + frac(_ScrollAdd.xy * _Time.xy);

                o.color = v.color;
                o.color.rgb = v.color.rgb * _Multiplier;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 layers = tex2D(_BaseTex, i.uvBaseMul.xy) + tex2D(_AddTex, i.uvAdd);
                return layers * tex2D(_MulTex, i.uvBaseMul.zw) * i.color;
            }
            ENDCG
        }
    }
}

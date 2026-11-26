// Readable version of "Compat/GUI/Text Shader".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Port of the Unity 3.5 built-in "GUI/Text Shader" that shipped with the game. Renamed
// "Compat/GUI/Text Shader" so it does not clash with Unity 2018's own built-in shaders; materials
// reach it through this file's GUID.
//
// Text in _Color, with the font texture's alpha as coverage. The second SubShader is the
// original fixed-function fallback, unchanged.
// Output is identical to the generated version; only the layout and naming changed.

Shader "Compat/GUI/Text Shader"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            half4     _Color;
            sampler2D _MainTex;
            float4    _MainTex_ST;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = _Color;
                color.a *= tex2D(_MainTex, i.uv).a;
                return color;
            }
            ENDCG
        }
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            Color [_Color]
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine primary, texture alpha * primary alpha }
        }
    }
}

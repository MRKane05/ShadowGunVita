// Readable version of "Compat/Particles/Alpha Blended".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Port of the Unity 3.5 built-in "Particles/Alpha Blended" that shipped with the game. Renamed
// "Compat/Particles/Alpha Blended" so it does not clash with Unity 2018's own built-in shaders;
// materials reach it through this file's GUID.
//
// Texture * vertex colour * tint * 2, alpha blended. _InvFade (soft particles) is exposed but was
// not used by this version either. The last two SubShaders are the original fixed-function
// fallbacks, unchanged.
// Output is identical to the generated version; only the layout and naming changed.

Shader "Compat/Particles/Alpha Blended"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            BindChannels
            {
                Bind "vertex", Vertex
                Bind "color", Color
                Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.01
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            half4     _TintColor;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                half4  color : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // A 0.5 grey tint leaves the texture unchanged (Unity's "double" convention)
                return 2.0 * i.color * _TintColor * tex2D(_MainTex, i.uv);
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
            BindChannels
            {
                Bind "vertex", Vertex
                Bind "color", Color
                Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.01
            ColorMask RGB
            SetTexture [_MainTex] { ConstantColor [_TintColor] combine constant * primary }
            SetTexture [_MainTex] { combine texture * previous double }
        }
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            BindChannels
            {
                Bind "vertex", Vertex
                Bind "color", Color
                Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.01
            ColorMask RGB
            SetTexture [_MainTex] { combine texture * primary }
        }
    }
}

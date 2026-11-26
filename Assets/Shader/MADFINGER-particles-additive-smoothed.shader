// Readable version of "MADFINGER/Particles/Additive + fadeout".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Additive texture tinted by _Color that fades out where the surface turns edge-on to the camera
// (fully visible when facing within about 75 degrees of the camera, gone at 90). This hides the
// hard edges of flat effect cards. Both sides fade the same way.
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject)).
//    The generated code multiplied by the object-to-world matrix, which tilts the normal on
//    non-uniformly scaled objects and changes where the fade happens. For unscaled and uniformly
//    scaled objects the result is unchanged.
//
// Other changes:
//  - The fade is passed as a single value; the tint is read from _Color in the fragment stage
//    instead of being copied into an interpolator.

Shader "MADFINGER/Particles/Additive + fadeout"
{
    Properties
    {
        _MainTex ("Main tex", 2D) = "white" {}
        _Color ("Tint color", Color) = (0,1,0,0)
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

            float4    _Color;
            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                half   fade : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 toCamera = normalize(_WorldSpaceCameraPos - worldPos);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.fade = saturate(4.0 * abs(dot(worldNormal, toCamera)));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                color.rgb *= (half3)_Color.rgb;
                // The fade is applied twice (squared), as in the original
                return color * i.fade * i.fade;
            }
            ENDCG
        }
    }
}

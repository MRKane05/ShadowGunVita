// Readable version of "MADFINGER/Characters/Character shadow plane - sphere AO".
// Rebuilt from the Shadowgun 1.3.5 GLES2 programs for Unity 2018 (Vita).
//
// Blob shadow under characters: a ground mesh darkened (multiply blend) by up to three spheres
// (_SphereN.xyz = world centre, .w = radius) that approximate the character's body. Each sphere
// occludes by (radius / distance)^2 * cos(angle to sphere). _Intensity limits how dark it gets.
// Vertex red fades the mesh edges (0 = no darkening).
//
// Normal fix, the same as the other MADFINGER shaders:
//  - The world normal is transformed with the inverse transpose (mul(normal, unity_WorldToObject))
//    and normalised. The generated code multiplied by the object-to-world matrix without
//    renormalising, so on a scaled shadow plane the occlusion was scaled too. For unscaled
//    objects the result is unchanged.

Shader "MADFINGER/Characters/Character shadow plane - sphere AO"
{
    Properties
    {
        _Sphere0 ("S0", Vector) = (0,0,0,0)
        _Sphere1 ("S0", Vector) = (0,0,0,0)
        _Sphere2 ("S0", Vector) = (0,0,0,0)
        _Intensity ("Intensity", Float) = 0.9
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent-15" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent-15" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }
            ZWrite Off
            Cull Off
            Fog { Color (0,0,0,0) }
            Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float  _Intensity;
            float4 _Sphere0; // xyz = world centre, w = radius
            float4 _Sphere1;
            float4 _Sphere2;

            // Occlusion from one sphere: cos(angle) * (radius / distance)^2
            float SphereOcclusion(float4 sphere, float3 worldPos, float3 worldNormal)
            {
                float3 toSphere = sphere.xyz - worldPos;
                float dist = length(toSphere);
                float size = sphere.w / dist;
                return dot(worldNormal, toSphere / dist) * size * size;
            }

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;  // r = 0 at the mesh edges
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                half4  color : COLOR0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(mul(normalize(v.normal), (float3x3)unity_WorldToObject));

                float occlusion = SphereOcclusion(_Sphere0, worldPos, worldNormal)
                                + SphereOcclusion(_Sphere1, worldPos, worldNormal)
                                + SphereOcclusion(_Sphere2, worldPos, worldNormal);

                float shade = max(1.0 - saturate(occlusion), 1.0 - _Intensity) + (1.0 - v.color.r);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = (half4)shade;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}

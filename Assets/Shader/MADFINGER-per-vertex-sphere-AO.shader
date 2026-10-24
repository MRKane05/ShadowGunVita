Shader "MADFINGER/Characters/Character shadow plane - sphere AO" {
Properties {
 _Sphere0 ("S0", Vector) = (0,0,0,0)
 _Sphere1 ("S0", Vector) = (0,0,0,0)
 _Sphere2 ("S0", Vector) = (0,0,0,0)
 _Intensity ("Intensity", Float) = 0.9
}
	// Reconstructed: the exporter left an opaque stub, which is why this drew a solid disc
	// following the player instead of a soft contact shadow. CharacterShadow.cs feeds three
	// world-space spheres as (x,y,z,radius) - pelvis plus both feet - and _Intensity as the
	// darkening amount, so this multiplies the ground by their combined occlusion.
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Blend DstColor Zero
		ZWrite Off
		Offset -1, -1
		Cull Off
		Lighting Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Sphere0;
			float4 _Sphere1;
			float4 _Sphere2;
			float _Intensity;

			struct v2f { float4 pos : SV_POSITION; float3 wpos : TEXCOORD0; };

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			// 1 = unoccluded, 0 = directly beneath the sphere centre
			float SphereAO(float3 p, float4 s)
			{
				float r = max(s.w, 0.0001);
				return smoothstep(0.0, 1.0, saturate(distance(p, s.xyz) / r));
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float ao = SphereAO(i.wpos, _Sphere0)
				         * SphereAO(i.wpos, _Sphere1)
				         * SphereAO(i.wpos, _Sphere2);
				float shade = 1.0 - saturate(_Intensity) * (1.0 - ao);
				return fixed4(shade, shade, shade, 1.0);
			}
			ENDCG
		}
	}
	Fallback off
}

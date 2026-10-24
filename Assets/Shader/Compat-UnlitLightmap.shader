// Compatibility shader for materials that referenced Unity built-in "Diffuse" by numeric id
// (fileID 7). That is a LIT shader; this game bakes its lighting and has almost no runtime
// lights, so those surfaces rendered black/wrong. Unlit base texture modulated by the baked
// lightmap, matching the game's own Mobile-Lightmap-Unlit (plain sample x2, dLDR).
Shader "Compat/UnlitLightmap" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Lighting Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ LIGHTMAP_ON
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 lmuv : TEXCOORD1;
			};
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv) * _Color;
				#ifdef LIGHTMAP_ON
				c.rgb *= UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb * 2.0;
				#endif
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}

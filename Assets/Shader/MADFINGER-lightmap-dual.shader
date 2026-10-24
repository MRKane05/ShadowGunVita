Shader "MADFINGER/Environment/Dual diffuse with lightmap" {
Properties {
 _MainTex0 ("Base layer0 (RGB)", 2D) = "white" {}
 _MainTex1 ("Base layer1 (RGB)", 2D) = "white" {}
}
	// Reconstructed: the exporter left a stub using a *lit* Lambert surface shader.
	// This game bakes its lighting and has almost no runtime lights, so that rendered
	// nearly black. The name says Unlit/Lightmap, so this samples the base texture and
	// modulates it by the baked lightmap instead.
	// Vertex colour is deliberately NOT used: 421 of 561 meshes have no colour channel,
	// and reading it there yields zero, which made geometry vanish.
	// Scrolling uses _Time.x (t/20), not _Time.y (t): _ScrollX 0.5 on the sky meant half
	// a UV per second, which span far too fast.
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ LIGHTMAP_ON
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 lmuv : TEXCOORD2;
			};
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv);
				#ifdef LIGHTMAP_ON
				c.rgb *= UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb * 2.0;   // dLDR: the game's own
				// recovered Mobile-Lightmap-Unlit shader does 'combine texture * previous double',
				// i.e. a plain sample x2. DecodeLightmap() assumes RGBM and shifted colours blue.
				#endif
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}
Shader "MADFINGER/Environment/Scroll 2 Layers Multiplicative - alphablended" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _AMultiplier ("Layer Multiplier", Float) = 0.5
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
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
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
			sampler2D _DetailTex;
			float4 _DetailTex_ST;
			float _ScrollX;
			float _ScrollY;
			float _Scroll2X;
			float _Scroll2Y;
			float _AMultiplier;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 lmuv : TEXCOORD2;
				fixed4 col : COLOR;
			};
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv += float2(_ScrollX, _ScrollY) * _Time.x;
				o.uv2 = TRANSFORM_TEX(v.texcoord, _DetailTex);
				o.uv2 += float2(_Scroll2X, _Scroll2Y) * _Time.x;
				o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.col = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed4 dt = tex2D(_DetailTex, i.uv2);
				c.rgb *= dt.rgb;
				#ifdef LIGHTMAP_ON
				c.rgb *= UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb * 2.0;   // dLDR: the game's own
				// recovered Mobile-Lightmap-Unlit shader does 'combine texture * previous double',
				// i.e. a plain sample x2. DecodeLightmap() assumes RGBM and shifted colours blue.
				#endif
				// vertex alpha carries the smoke/fade density here. Verified every mesh
				// using this shader has a colour channel - unlike most of the project,
				// where 421 of 561 meshes have none and reading it yields zero.
				c *= i.col;
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}
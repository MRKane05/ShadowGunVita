Shader "MADFINGER/FX/Bullet trail" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _AMultiplier ("Layer Multiplier", Float) = 0.5
 _TintColor ("Tint color", Color) = (1,1,1,1)
}
	// Reconstructed (the exporter left a stub). The property block is the original one and
	// matches MADFINGER's "Scroll 2 Layers" family exactly, so this follows that family's
	// source (Tools/original_shaders/MADFINGER-Unlit-Scroll2Layers-Mul-Sine.shader):
	// two scrolling layers multiplied together, additive blend, scaled by the multiplier.
	// The tracer texture is a dark noise band with a bright streak, so multiplying the two
	// scrolled copies leaves only a thin flickering line of the 0.4 m wide LineRenderer
	// ribbon; drawing one layer flat (the earlier reconstruction) showed the whole ribbon.
	// _TintColor.a is driven by ProjectileTrail.cs to fade the trail out.
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		Blend One One
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Color (0,0,0,0) }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			sampler2D _DetailTex;
			float4 _MainTex_ST;
			float4 _DetailTex_ST;
			float _ScrollX;
			float _ScrollY;
			float _Scroll2X;
			float _Scroll2Y;
			float _AMultiplier;
			fixed4 _TintColor;
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time.y);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.xy, _DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time.y);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D(_DetailTex, i.uv.zw);
				fixed4 o = tex * tex2 * _AMultiplier;
				o.rgb *= _TintColor.rgb * _TintColor.a;
				return o;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}

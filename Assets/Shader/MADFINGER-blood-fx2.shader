Shader "MADFINGER/FX/Blood FX - alpha blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _FadeInSpeed ("Fade in speed", Float) = 5
 _DrippingSpeed ("Dripping speed", Float) = 0.1
}
	// Reconstructed: the exporter could not recover this shader and left an opaque
	// stub, which is why these effects rendered as flat black. Properties above are
	// the original interface. Brightness/falloff params are declared but only the
	// core blending is implemented.
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off
		Lighting Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 t = tex2D(_MainTex, i.uv);
				fixed4 c = t;   // NOTE: no vertex colour - 421 of 561 meshes have no colour channel
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}
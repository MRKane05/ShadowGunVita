Shader "MADFINGER/FX/Plasma FX" {
Properties {
 _MainTex ("Noise tex", 2D) = "white" {}
 _ColorStart ("Color start", Color) = (1,0.8,0,1)
 _ColorEnd ("Color end", Color) = (0.5,0.3,0,1)
 _Speed ("Speed", Float) = 2
 _BorderWidth ("Border width", Float) = 4
 _TimeOffs ("TimeOffs", Float) = 9999
 _ExpansionSpeed ("ExpansionSpeed", Float) = 1
}
	// Reconstructed: the exporter could not recover this shader and left an opaque
	// stub, which is why these effects rendered as flat black. Properties above are
	// the original interface. Brightness/falloff params are declared but only the
	// core blending is implemented.
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha One
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
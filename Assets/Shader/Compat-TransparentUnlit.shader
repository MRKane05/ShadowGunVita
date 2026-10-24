// Compatibility shader for materials that referenced a Unity built-in shader by numeric id
// (fileID 203). Used by the character shadow blob, explosion decals and the firefly sprite -
// all of which are transparent. The remapped built-in gave a hard-edged, wrongly coloured
// disc; this is a plain alpha-blended unlit pass.
Shader "Compat/TransparentUnlit" {
Properties {
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		Lighting Off
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				return tex2D(_MainTex, i.uv) * _Color;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}

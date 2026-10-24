Shader "MADFINGER/FX/Lightning bolt FX" {
Properties {
 _MainTex ("Main tex", 2D) = "white" {}
 _Duration ("Duration (x - ON, y - OFF)", Vector) = (0.5,2,0,0)
 _Amplitude ("Amplitude (x - horizontal, y - vertical)", Vector) = (0.2,0.01,0,0)
 _NoiseFreqs ("NoiseFreqs", Vector) = (4,8,16,32)
 _NoiseSpeeds ("NoiseSpeeds", Vector) = (3.2,2.3,0.5,1)
 _NoiseAmps ("NoiseAmps", Vector) = (2,1,0.5,0.125)
 _OtherParams ("x - line width, y - inv wave speed, z - aspect ratio", Vector) = (0.2,0.025,0,0)
 _Color ("Color", Color) = (0.7,0.8,1.3,1)
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
			fixed4 _Color;
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
				c *= _Color;
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}
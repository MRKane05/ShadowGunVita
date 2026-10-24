Shader "MADFINGER/Transparent/Moving waves FX" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 10
 _FadeOutDistFar ("Far fadeout dist", Float) = 10000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _WaveSpeed ("Waves speed", Vector) = (0.25,0.25,0.25,0.25)
 _WaveSize ("Waves size", Vector) = (0.25,0.25,0.25,0.25)
 _WaveAmplitude ("Waves amp", Vector) = (1,0,0,0)
 _WaveBias ("Waves bias", Vector) = (0,0,0,0)
 _WaveTimeOffs ("Wave time offs", Vector) = (0,0,0,0)
 _Color0 ("Color0", Color) = (1,1,1,1)
 _Color1 ("Color1", Color) = (1,0,0,1)
 _Color2 ("Color2", Color) = (0,1,0,1)
 _Color3 ("Color3", Color) = (0,0,1,1)
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
			float _Multiplier;
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
				c.rgb *= _Multiplier;
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}
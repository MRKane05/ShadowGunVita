Shader "MADFINGER/FX/Blood FX blended" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Params ("Params", Vector) = (0,1,0,0)
 _ColorBooster ("Color booster", Color) = (0.5,0,0,0)
}
	// Reconstructed. BloodFXManager drives this every frame with
	//   _Params.x = currentHealth * 0.8   (so 0.8 == full health, 0 == dead)
	//   _Params.y = heartbeat frequency scale
	// The first reconstruction ignored _Params, so the overlay sat at full opacity and
	// showed up as a permanent disc in front of the player. Alpha now scales with damage
	// taken, which means it is invisible at full health.
	SubShader {
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always
		Cull Off
		Lighting Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Params;
			fixed4 _ColorBooster;
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv);
				float health = saturate(_Params.x / 0.8);
				float hurt = saturate(1.0 - health);
				// gentle pulse while hurt; _Params.y is the heartbeat rate
				float pulse = 1.0 + 0.15 * sin(_Time.y * max(_Params.y, 0.0001) * 6.2831853);
				c.rgb += _ColorBooster.rgb * hurt;
				c.a *= hurt * pulse;
				return c;
			}
			ENDCG
		}
	}
	Fallback off
}

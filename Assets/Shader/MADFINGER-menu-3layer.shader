Shader "MADFINGER/FX/Menu - 3 layers" {
Properties {
 _BaseTex ("Base layer", 2D) = "white" {}
 _MulTex ("Mul layer", 2D) = "white" {}
 _AddTex ("Add layer", 2D) = "white" {}
 _ScrollBase ("Base layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollMul ("Mul layer scroll speed ", Vector) = (0,0,0,0)
 _ScrollAdd ("Add layer scroll speed ", Vector) = (0,0,0,0)
 _Multiplier ("Multiplier", Float) = 1
}
	// Reconstructed body: the original shader code was not recovered by the
	// exporter, which left a //DummyShaderTextExporter stub that sampled an
	// undeclared _MainTex and rendered flat. The Properties block above and the
	// values in menubg.mat describe the real interface, so this reimplements the
	// obvious reading of it: three scrolling layers composited base*mul + add.
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _BaseTex;
			sampler2D _MulTex;
			sampler2D _AddTex;
			float4 _BaseTex_ST;
			float4 _MulTex_ST;
			float4 _AddTex_ST;
			float4 _ScrollBase;
			float4 _ScrollMul;
			float4 _ScrollAdd;
			float _Multiplier;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uvBase : TEXCOORD0;
				float2 uvMul : TEXCOORD1;
				float2 uvAdd : TEXCOORD2;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvBase = TRANSFORM_TEX(v.texcoord, _BaseTex) + _ScrollBase.xy * _Time.y;
				o.uvMul  = TRANSFORM_TEX(v.texcoord, _MulTex)  + _ScrollMul.xy  * _Time.y;
				o.uvAdd  = TRANSFORM_TEX(v.texcoord, _AddTex)  + _ScrollAdd.xy  * _Time.y;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 b = tex2D(_BaseTex, i.uvBase);
				fixed4 m = tex2D(_MulTex, i.uvMul);
				fixed4 a = tex2D(_AddTex, i.uvAdd);
				fixed4 c = b * m * _Multiplier + a;
				c.a = 1;
				return c;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}

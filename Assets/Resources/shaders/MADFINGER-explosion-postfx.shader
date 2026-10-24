Shader "MADFINGER/PostFX/ExplosionFX" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _UVOffsAndAspectScale ("UVOffsAndAspectScale", Vector) = (0,0,0,0)
 _Wave0ParamSet0 ("Wave0ParamSet0", Vector) = (0,0,0,0)
 _Wave0ParamSet1 ("Wave0ParamSet1", Vector) = (0,0,0,0)
 _Wave1ParamSet0 ("Wave1ParamSet0", Vector) = (0,0,0,0)
 _Wave1ParamSet1 ("Wave1ParamSet1", Vector) = (0,0,0,0)
 _Wave2ParamSet0 ("Wave2ParamSet0", Vector) = (0,0,0,0)
 _Wave2ParamSet1 ("Wave2ParamSet1", Vector) = (0,0,0,0)
 _Wave3ParamSet0 ("Wave3ParamSet0", Vector) = (0,0,0,0)
 _Wave3ParamSet1 ("Wave3ParamSet1", Vector) = (0,0,0,0)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}
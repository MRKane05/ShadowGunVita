Shader "MADFINGER/FX/Plasma FX" {
	Properties{
	 _MainTex("Noise tex", 2D) = "white" {}
	 _ColorStart("Color start", Color) = (1,0.8,0,1)
	 _ColorEnd("Color end", Color) = (0.5,0.3,0,1)
	 _Speed("Speed", Float) = 2
	 _BorderWidth("Border width", Float) = 4
	 _TimeOffs("TimeOffs", Float) = 9999
	 _ExpansionSpeed("ExpansionSpeed", Float) = 1

		 // Added: basic Fresnel rim for the shield
		 _FresnelColor("Fresnel color", Color) = (1,1,1,1)
		 _FresnelPower("Fresnel power", Range(0.5, 8)) = 3
		 _FresnelIntensity("Fresnel intensity", Range(0, 4)) = 1
	}
		// Reconstructed: the exporter could not recover this shader and left an opaque
		// stub, which is why these effects rendered as flat black. Properties above are
		// the original interface plus the Fresnel controls. The Fresnel term is computed
		// per vertex and passed to the fragment stage through the vertex colour.
		 SubShader{
			 Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
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
				 fixed4 _FresnelColor;
				 float _FresnelPower;
				 float _FresnelIntensity;
				 uniform float _TimeOffs;

				 struct v2f {
					 float4 pos   : SV_POSITION;
					 float2 uv    : TEXCOORD0;
					 fixed4 color : COLOR0;    // rim term from the vertex stage
				 };

				 v2f vert(appdata_full v) {
					 v2f o;
					 o.pos = UnityObjectToClipPos(v.vertex);
					 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

					 // Fresnel: how much the surface faces away from the camera.
					 // Use the direction from the vertex to the camera, normalised here,
					 // so the result depends on viewing angle and not on distance.
					 float3 worldNormal = UnityObjectToWorldNormal(v.normal);
					 float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					 float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

					 float NdotV = clamp(abs((dot(worldNormal, viewDir))), -1.0, 1.0);
					 float fresnel = abs(pow(1.0 - NdotV, _FresnelPower) * _FresnelIntensity);

					 // Pass the rim colour through the vertex colour
					 o.color = _FresnelColor * saturate(fresnel);
					 return o;
				 }

				 fixed4 frag(v2f i) : SV_Target {
					 fixed4 t = tex2D(_MainTex, i.uv + float2(_TimeOffs, 0));
						t = t + tex2D(_MainTex, i.uv - float2(_TimeOffs, 0));
						t = t * 0.5;
					 // Add the rim to the plasma texture. The blend mode is additive,
					 // so the rim brightens the shield edges.
					 //fixed4 c = t;
					 //c.rgb += i.color.rgb * t.a;
						t.a = t.a * i.color.r;
				 return t;
			 }
			 ENDCG
		 }
	 }
		 Fallback "Unlit/Transparent"
}

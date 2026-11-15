Shader "MADFINGER/Environment/Skybox - opaque - no fog" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("HACK: temporary to fix lightmap bouncing light (will be fixed in RC1)", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Geometry+10" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "QUEUE"="Geometry+10" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
  ZWrite Off
  Fog { Mode Off }
  SetTexture [_MainTex] { combine texture }
 }
}
}
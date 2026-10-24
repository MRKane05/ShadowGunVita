// Compatibility shader. "Font Material" referenced a Unity built-in shader by numeric id
// (fileID 10101 in unity_builtin_extra). Built-in shader ids changed between Unity 3.5 and
// 2018, so that now resolves to the wrong shader - text drew as a solid block because the
// glyph alpha was never applied. This is the classic GUI/Text behaviour: tint by vertex
// colour, take coverage from the font texture's alpha.
Shader "Compat/Font" {
Properties {
 _MainTex ("Font Texture", 2D) = "white" {}
 _Color ("Text Color", Color) = (1,1,1,1)
}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		Cull Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Color [_Color]
			SetTexture [_MainTex] {
				combine primary, texture * primary
			}
		}
	}
}

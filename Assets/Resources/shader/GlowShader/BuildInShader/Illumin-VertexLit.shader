Shader "Effect/Glow/Self-Illumin/VertexLit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Spec Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.1, 1)) = 0.7
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
    _GlowTex ("Glow", 2D) = "" {}
    _GlowColor ("Glow Color", Color)  = (1,1,1,1)
    _GlowStrength ("Glow Strength", Float) = 1.0
}

// ------------------------------------------------------------------
// Dual texture cards

SubShader {
	LOD 100
	Tags { "RenderEffect"="Glow" "RenderType"="Glow" }
	
	Pass {
		Name "BASE"
		Tags {"LightMode" = "Vertex"}
		Material {
			Diffuse [_Color]
			Shininess [_Shininess]
			Specular [_SpecColor]
		}
		SeparateSpecular On
		Lighting On
		SetTexture [_Illum] {
			constantColor [_Color]
			combine constant lerp (texture) previous
		}
		SetTexture [_MainTex] {
			Combine texture * previous, texture*primary
		}
	}
}

FallBack Off
CustomEditor "GlowMatInspector"
}

# Shadowgun shader report

Rip compared: `RippingStuff/decomp/AssetRipperExport639300251270822198` (Shadowgun 1.3.5 Android, Unity 3.5.7).
Project compared as committed at `6d501a8` (before this port).

## What was changed

- **43 shaders rebuilt from the game's own code.** Every programmable game shader in the project was replaced by an
  exact transliteration of its shipped GLES2 programs (`Tools/ShaderPort/sg_glsl2hlsl.py`). Shader names, properties,
  pass state and `.meta` GUIDs are unchanged, so materials keep pointing at them.
  - 24 of them were generic placeholders ("Reconstructed" unlit/lightmap stand-ins) and 3 were AssetRipper dummy
    stubs (lit Lambert); none of those had the original effect.
  - 16 were hand ports or the Shadowgun sample sources; close, but not the shipped versions (lightmap decode,
    properties and blend state differ in places, see the table).
- **10 fixed-function shaders** now hold the game's shipped ShaderLab text verbatim (6 files changed, 4 already
  matched). They behaved the same before; this only makes the files identical to the game's.
- **3 `Compat/*` stand-ins replaced by ports of the Unity 3.5 built-ins the game used.** Each keeps its `.meta` GUID,
  so its materials stay linked. Each is renamed `Compat/<original name>` so it cannot clash with Unity 2018's own
  built-ins:
  - `Compat-Font.shader`: `Compat/Font` became `Compat/GUI/Text Shader` (Font Material).
  - `Compat-TransparentUnlit.shader` became `Compat-ParticlesAlphaBlended.shader`: `Compat/Particles/Alpha Blended`
    (m_explo_decal, m_stin_01, svetluska).
  - `Compat-UnlitLightmap.shader` became `Compat-Diffuse.shader`: `Compat/Diffuse` (Default-Diffuse,
    i1_podlaha01-Material #73, m_MINE_road, traverza-m_tech_01).
- **Materials.** `m_bloodscreen_2black.mat` points at `MADFINGER/FX/Blood FX nonblended` again (the game's
  assignment); it had been switched to a Unity built-in shader (fileID 10703) to get the placeholder damage screen to
  show. `Gui3G`, `Gui4G`, `GuiiPad`, `Score3G` and `Score4G` move from a Unity 2018 built-in (fileID 203) to
  `Compat/Particles/Alpha Blended`, which is what they used in the game. No other material changed.
- Not ported: `Particles/Alpha Blended Premultiply`, because its only material (Default-Particle) is not in the project.

Checked: all 56 shaders in the project (43 game ports, 10 fixed-function, 3 Compat ports) compile with no errors or
warnings in the Unity 2018.2.19f1 editor and in a PSP2 player build. This was done in a scratch project, covering every
LIGHTMAP_ON / VERTEXLIGHT_ON variant. **Not checked:** how they look in the editor or on the Vita.

### Porting rules (what the generator does)

- Maths copied 1:1 from the GLSL (`tmpvar_N` names kept, so a port can be read side by side with the rip).
- GLSL matrices are column-major; they are stored transposed, so `M[i]` keeps its meaning and `M * v` becomes
  `mul(v, M)`. Unity matrices use `mul(U, v)`.
- Unity 3.5 built-ins that no longer exist are emulated: `_World2Object` (inverse world matrix without scale) and
  `unity_Scale` (w = 1 / object scale).
- Lightmaps: the game's own `2.0 * sample` (dLDR), which matches the PNG lightmaps `LightmapRestorer` loads.
- `LIGHTMAP_OFF/ON` variants become `#pragma multi_compile _ LIGHTMAP_ON` (works on untagged passes too);
  `VERTEXLIGHT_ON` is kept; realtime-shadow variants are dropped (the game uses blob shadows).
- Precision: highp becomes float; mediump and lowp become half. The game relies on Android's lowp being fp16:
  Blinking emissive builds a random seed as `dot(color, color) * 40` in lowp, which a ±2 `fixed` would clamp.
- The 4 shaders that write clip space themselves (the three Blood FX and ExplosionFX) go through `sg_FromGLClip`:
  reversed Z in the D3D11 editor, and Unity's render-texture y-flip (`_ProjectionParams.x`).
- `mul(UNITY_MATRIX_MVP, x)` is written as `mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, x))`, so Unity's import
  upgrader does not rewrite the files.

Regenerate one shader:

    python Tools/ShaderPort/sg_glsl2hlsl.py "<rip>/Assets/Shader/X.shader" Assets/Shader/X.shader

## Things to look at in game

- **Vertex colours.** The real shaders read vertex colour where the placeholders did not. A few meshes had no colour
  channel in the original game either, so they depend on the default colour Unity supplies:
  `MADFINGER/Transparent/Blinking GodRays` on brain, Enemy_eyes, glow, m_flyereye, m_MINE_light, m_MINE_wall,
  m_mouthglow and m_mouthglow 1 (alpha drives the size growth, blue the blink offset), and
  `Scroll 2 Layers Multiplicative No Lightmap Sine2)` on m_DLC_electricity_RED (output times alpha, so it is invisible
  if the default is 0). Every other colour-reading material's meshes have colours.
- **Explosion post-effect.** `MFExplosionPostFX.OnRenderImage` is commented out ("Disabled post-effect"); it broke
  because the project only had a dummy stub for its shader. The real shader is in now, so it can be turned back on.
  If the screen comes out upside down, the y-flip in `sg_FromGLClip` is the place to look.
- **Light probes.** The character and lightprobe shaders evaluate SH exactly as Unity 3.5 did. The Deus Ex port found
  Unity 4 probe data needs a sign flip on SH coefficients 1, 3, 5 and 7 for 2018; the tool that made
  `Resources/lightprobes/*.asset` is not in this repo, so check characters are not lit from the mirrored side.
- **Compat/Diffuse is lit**, as it was in the game: lightmap times texture when the object has a lightmap, realtime
  lights otherwise. The old stand-in was unlit because those surfaces rendered black without a lightmap. If they go
  dark, the objects are not getting their lightmap index from `LightmapRestorer`.
- **Compat/GUI/Text Shader has `ZTest Always`**, like the game's: text is drawn over geometry.

## Game shaders that are in the project (53)

Status of each project shader **before** this port, compared with the shipped 1.3.5 shader in the rip.

| Shader | File | Old project version | Materials (rip) | Differences found |
|---|---|---|---|---|
| MADFINGER/Diffuse/Simple | `Shader/TextureSimple.shader` | Fixed function | 11 | none (same behaviour) |
| MADFINGER/Environment/Skybox - opaque - no fog | `Shader/MADFINGER-skybox.shader` | Fixed function | 3 | none (same behaviour) |
| MADFINGER/Environment/Skybox - transparent - no fog | `Shader/MADFINGER-skybox-transparent.shader` | Fixed function | 6 | none (same behaviour) |
| MADFINGER/Environment/Unlit (Supports Lightmap) | `Shader/Mobile-Lightmap-Unlit.shader` | Fixed function | 9 | none (same behaviour) |
| MADFINGER/Particles/Additive TwoSide | `Shader/Particles Additive Culled.shader` | Fixed function | 17 | none (same behaviour) |
| MADFINGER/Particles/Alpha Blended | `Shader/Particle Alpha Blend.shader` | Fixed function | 14 | none (same behaviour) |
| MADFINGER/Particles/Alpha Blended Z-enabled | `Shader/Particle Alpha Blend-Z.shader` | Fixed function | 8 | none (same behaviour) |
| MADFINGER/Particles/Multiply Culled | `Shader/Partocle Multiply Culled.shader` | Fixed function | 1 | none (same behaviour) |
| Mobile/Particles/Alpha Blended | `Shader/Mobile-Particle-Alpha.shader` | Fixed function | 1 | none (same behaviour) |
| iPhone/Particles/Multiply Culled | `Shader/iPhone Multiply Culled.shader` | Fixed function | 1 | none (same behaviour) |
| MADFINGER/Characters/Character shadow plane - sphere AO | `Shader/MADFINGER-per-vertex-sphere-AO.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Queue transparent-15<br>project had: Offset -1, -1; Queue transparent |
| MADFINGER/Environment/Blinking emissive | `Shader/MADFINGER-blinking-emissive.shader` | Placeholder (generic stand-in) | 3 | generic shader, none of the original effect<br>game has: Blend one one; Cull off; Queue transparent; ZWrite off |
| MADFINGER/Environment/Cube env map (Supports LightProbes) | `Shader/MADFINGER-cube-env-lightprobes.shader` | Placeholder (generic stand-in) | 2 | generic shader, none of the original effect<br>game has: LightMode forwardbase |
| MADFINGER/Environment/Cube env map modulated by vertex color + lightmap | `Shader/MADFINGER-cube-env-vtx-color.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: LightMode forwardbase |
| MADFINGER/Environment/Cubemap specular + Lightmap | `Shader/MADFINGER-lightmap-unlit-spec-cube.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: LightMode forwardbase |
| MADFINGER/Environment/Dual diffuse with lightmap | `Shader/MADFINGER-lightmap-dual.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect |
| MADFINGER/Environment/Gloss Per-Vertex Additive (Supports Lightmap) | `Shader/MADFINGER-Lightmap-Unlit-VertexGlossy-Add.shader` | Placeholder (generic stand-in) | 2 | generic shader, none of the original effect<br>game has: LightMode forwardbase |
| MADFINGER/Environment/Scroll 1 Layer | `Shader/MADFINGER-Unlit-Scroll.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect |
| MADFINGER/Environment/Scroll 1 Layer (Supports Lightmap) | `Shader/MADFINGER-Lightmap-Unlit-Scroll.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect |
| MADFINGER/Environment/Scroll 2 Layers Additive | `Shader/MADFINGER-Unlit-Scroll2Layers-Add.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative | `Shader/MADFINGER-Unlit-Scroll2Layers-Mul.shader` | Placeholder (generic stand-in) | 2 | generic shader, none of the original effect |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative (Supports Lightmap) | `Shader/MADFINGER-Lightmap-Unlit-Scroll2Layers-Mul.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative - alphablended | `Shader/MADFINGER-Unlit-Scroll2Layers-Mul-alphablended.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>project had: Queue transparent; ZWrite off |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative No Lightmap Sine2) | `Shader/MADFINGER-Unlit-Scroll2Layers-Mul-Sine2.shader` | Placeholder (generic stand-in) | 3 | generic shader, none of the original effect<br>game has: Blend one one; Cull off; Queue transparent; ZWrite off |
| MADFINGER/Environment/Virtual Gloss Per-Vertex Additive AlphaKeyed (Supports Lightmap) | `Shader/MADFINGER-VirtualVertexGlossy-Add-alphakeyed.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: LightMode forwardbase |
| MADFINGER/FX/Blood FX - alpha blended | `Shader/MADFINGER-blood-fx2.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Queue overlay<br>project had: Queue transparent |
| MADFINGER/FX/Blood FX blended | `Shader/MADFINGER-blood-fx-blended.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Blend dstcolor srccolor<br>project had: Blend srcalpha oneminussrcalpha |
| MADFINGER/FX/Blood FX nonblended | `Shader/MADFINGER-blood-fx-nonblended.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>missing properties `_Color`<br>extra properties `_MainTex`, `_ColorBooster`<br>project had: Blend srcalpha oneminussrcalpha |
| MADFINGER/FX/Bullet trail | `Shader/MADFINGER-bullet-trail-fx.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>project had: Cull off |
| MADFINGER/FX/Lightning bolt FX | `Shader/MADFINGER-lightning-bolt-fx.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Blend one one<br>project had: Blend srcalpha one |
| MADFINGER/FX/Mine-FX | `Shader/MADFINGER-mine-fx.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Blend srcalpha oneminussrcalpha<br>project had: Blend srcalpha one; Cull off |
| MADFINGER/FX/Plasma FX | `Shader/MADFINGER-plasma-fx.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>extra properties `_FresnelColor`, `_FresnelPower`, `_FresnelIntensity`<br>game has: Blend one one<br>project had: Blend srcalpha one; Cull off |
| MADFINGER/Particles/Additive + fadeout | `Shader/MADFINGER-particles-additive-smoothed.shader` | Placeholder (generic stand-in) | 1 | generic shader, none of the original effect<br>game has: Blend one one<br>project had: Blend srcalpha one |
| MADFINGER/Transparent/Moving waves FX | `Shader/MADFINGER-moving-wave-additive.shader` | Placeholder (generic stand-in) | 2 | generic shader, none of the original effect<br>game has: Blend one one<br>project had: Blend srcalpha one |
| MADFINGER/Characters/BRDFLit (Supports Backlight) | `Shader/Mobile-TexPseudoBRDF-WrapAround.shader` | Hand port / sample source | 7 | same state and properties; maths not compared line by line |
| MADFINGER/Characters/BRDFLit FX (Supports Backlight) | `Shader/Mobile-TexPseudoBRDF-WrapAround-FX.shader` | Hand port / sample source | 5 | game has: Blend srcalpha oneminussrcalpha; Queue transparent |
| MADFINGER/Environment/Cube env map (Supports Lightmap) | `Shader/MADFINGER-cube-env.shader` | Hand port / sample source | 7 | lightmap through DecodeLightmap() (game: plain sample x2; same only with dLDR lightmap encoding) |
| MADFINGER/Environment/Lightmap + Wind | `Shader/MADFINGER-Lightmap-Unlit-Wind.shader` | Hand port / sample source | 3 | lightmap through DecodeLightmap() (game: plain sample x2; same only with dLDR lightmap encoding) |
| MADFINGER/Environment/Lightmap + emissivity | `Shader/MADFINGER-Lightmap-Unlit-emis.shader` | Hand port / sample source | 2 | missing properties `_EmisColor`<br>extra properties `_Emissivity`<br>lightmap through DecodeLightmap() (game: plain sample x2; same only with dLDR lightmap encoding) |
| MADFINGER/Environment/Lightprobes with Gloss Per-Vertex Additive | `Shader/MADFINGER-lightprobe-VertexGlossy-Add.shader` | Hand port / sample source | 21 | same state and properties; maths not compared line by line |
| MADFINGER/Environment/Lightprobes with VirtualGloss Per-Vertex Additive | `Shader/MADFINGER-lightprobe-VirtualVertexGlossy-Add.shader` | Hand port / sample source | 7 | same state and properties; maths not compared line by line |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative - Skybox | `Shader/MADFINGER-Unlit-Scroll2Layers-Mul-skybox.shader` | Hand port / sample source | 6 | property type/default differs: `_ScrollX`, `_ScrollY`, `_Scroll2X`, `_Scroll2Y` |
| MADFINGER/Environment/Scroll 2 Layers Multiplicative No Lightmap Sine) | `Shader/MADFINGER-Unlit-Scroll2Layers-Mul-Sine.shader` | Hand port / sample source | 18 | property type/default differs: `_ScrollX`, `_ScrollY`, `_Scroll2X`, `_Scroll2Y`, `_MMultiplier` |
| MADFINGER/Environment/Scroll 2 Layers Sine AlphaBlended | `Shader/MADFINGER-Unlit-Scroll2Layers-Sine-AlphaBlend.shader` | Hand port / sample source | 1 | property type/default differs: `_ScrollX`, `_ScrollY`, `_Scroll2X`, `_Scroll2Y`, `_MMultiplier` |
| MADFINGER/Environment/Virtual Gloss Per-Vertex Additive (Supports Lightmap) | `Shader/MADFINGER-Lightmap-Unlit-VirtualVertexGlossy-Add.shader` | Hand port / sample source | 53 | lightmap through DecodeLightmap() (game: plain sample x2; same only with dLDR lightmap encoding) |
| MADFINGER/Environment/weapon | `Shader/MADFINGER-weapon.shader` | Hand port / sample source | 6 | same state and properties; maths not compared line by line |
| MADFINGER/FX/Anim texture | `Shader/MADFINGER-animtexture.shader` | Hand port / sample source | 4 | same state and properties; maths not compared line by line |
| MADFINGER/Transparent/Blinking GodRays | `Shader/MADFINGER-blinking-god-rays.shader` | Hand port / sample source | 67 | same state and properties; maths not compared line by line |
| MADFINGER/Transparent/Blinking GodRays Billboarded | `Shader/MADFINGER-billboarded-blinking-god-rays.shader` | Hand port / sample source | 16 | same state and properties; maths not compared line by line |
| MADFINGER/Transparent/GodRays | `Shader/MADFINGER-god-rays.shader` | Hand port / sample source | 2 | same state and properties; maths not compared line by line |
| MADFINGER/FX/Menu - 3 layers | `Shader/MADFINGER-menu-3layer.shader` | AssetRipper dummy (Lambert stub) | 1 | renders a lit diffuse surface, none of the original effect<br>game has: Blend srcalpha oneminussrcalpha |
| MADFINGER/PostFX/ColorCorrection | `Resources/shaders/MADFINGER-color-correction.shader` | AssetRipper dummy (Lambert stub) | 0 | renders a lit diffuse surface, none of the original effect<br>game has: Cull off; ZTest always; ZWrite off |
| MADFINGER/PostFX/ExplosionFX | `Resources/shaders/MADFINGER-explosion-postfx.shader` | AssetRipper dummy (Lambert stub) | 0 | renders a lit diffuse surface, none of the original effect<br>game has: Cull off; ZTest always; ZWrite off |

## Project-only shaders (3)

These were the stand-ins before the port. They now hold ports of the built-ins listed in the next section.

- **Compat/Font** (`Shader/Compat-Font.shader`)
- **Compat/TransparentUnlit** (`Shader/Compat-TransparentUnlit.shader`)
- **Compat/UnlitLightmap** (`Shader/Compat-UnlitLightmap.shader`)

## Shaders in the rip that the project does not have (71)

All are Unity 3.5 built-in shaders that shipped inside the game's data.

Used by the game's materials (4):

| Shader | Rip file | Game materials | What the project uses instead |
|---|---|---|---|
| Diffuse | `Shader\Normal-Diffuse.shader` | Default-Diffuse, i1_podlaha01-Material #73, m_MINE_road, traverza-m_tech_01 | Compat/UnlitLightmap |
| GUI/Text Shader | `Shader\Font.shader` | Font Material | Compat/Font |
| Particles/Alpha Blended | `Shader\0\Particle Alpha Blend.shader` | Gui3G, Gui4G, GuiiPad, Score3G, Score4G, m_explo_decal, m_stin_01, svetluska | Compat/TransparentUnlit, Unity built-in (fileID 203) |
| Particles/Alpha Blended Premultiply | `Shader\Particle Premultiply Blend.shader` | Default-Particle | material missing |

The last column is the state before the port. The first three are now ported: see "What was changed".

Not used by any material (67): Bumped Diffuse (`Normal-Bumped.shader`), Bumped Specular (`Normal-BumpSpec.shader`), Decal (`Shader.shader`), Diffuse Detail (`Normal-DiffuseDetail.shader`), FX/Flare (`Flare.shader`), Hidden/BlitCopy (`Internal-BlitCopy.shader`), Hidden/Camera-DepthNormalTexture (`Camera-DepthNormalTexture.shader`), Hidden/Camera-DepthTexture (`Camera-DepthTexture.shader`), Hidden/Internal-CombineDepthNormals (`Internal-CombineDepthNormals.shader`), Hidden/Internal-Flare (`Internal-Flare.shader`), Hidden/Internal-GUITexture (`Internal-GUITexture.shader`), Hidden/Internal-GUITextureBlit (`Internal-GUITextureBlit.shader`), Hidden/Internal-GUITextureClip (`Internal-GUITextureClip.shader`), Hidden/Internal-GUITextureClipText (`Internal-GUITextureClipText.shader`), Hidden/Internal-Halo (`Internal-Halo.shader`), Hidden/Internal-PrePassCollectShadows (`Internal-PrePassCollectShadows.shader`), Hidden/Internal-PrePassLighting (`Internal-PrePassLighting.shader`), Hidden/Shadow-ScreenBlur (`Shadow-ScreenBlur.shader`), Hidden/Shadow-ScreenBlurRotated (`Shadow-ScreenBlurRotated.shader`), Hidden/TerrainEngine/BillboardTree (`BillboardTree.shader`), Hidden/TerrainEngine/Details/BillboardWavingDoublePass (`WavingGrassBillboard.shader`), Hidden/TerrainEngine/Details/Vertexlit (`VertexLit.shader`), Hidden/TerrainEngine/Details/WavingDoublePass (`WavingGrass.shader`), Hidden/TerrainEngine/Splatmap/Lightmap-AddPass (`AddPass.shader`), Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass (`FirstPass.shader`), Legacy Shaders/Diffuse Fast (`Normal-DiffuseFast.shader`), Parallax Diffuse (`Normal-Parallax.shader`), Parallax Specular (`Normal-ParallaxSpec.shader`), Particles/Additive (`Particle Add.shader`), Particles/Additive (Soft) (`Particle AddSmooth.shader`), Particles/Multiply (`Particle Multiply.shader`), Particles/Multiply (Double) (`Particle MultiplyDouble.shader`), Particles/VertexLit Blended (`Particle VertexLit Blended.shader`), Particles/~Additive-Multiply (`Particle AddMultiply.shader`), Reflective/Bumped Diffuse (`Reflect-Bumped.shader`), Reflective/Bumped Specular (`Reflect-BumpSpec.shader`), Reflective/Bumped Unlit (`Reflect-BumpNolight.shader`), Reflective/Bumped VertexLit (`Reflect-BumpVertexLit.shader`), Reflective/Diffuse (`Reflect-Diffuse.shader`), Reflective/Parallax Diffuse (`Reflect-Parallax.shader`), Reflective/Parallax Specular (`Reflect-ParallaxSpec.shader`), Reflective/Specular (`Reflect-Glossy.shader`), Reflective/VertexLit (`Reflect-VertexLit.shader`), RenderFX/Skybox (`Skybox.shader`), RenderFX/Skybox Cubed (`skybox cubed.shader`), Self-Illumin/Bumped Diffuse (`Illumin-Bumped.shader`), Self-Illumin/Bumped Specular (`Illumin-BumpSpec.shader`), Self-Illumin/Diffuse (`Illumin-Diffuse.shader`), Self-Illumin/Parallax Diffuse (`Illumin-Parallax.shader`), Self-Illumin/Parallax Specular (`Illumin-ParallaxSpec.shader`), Self-Illumin/Specular (`Illumin-Glossy.shader`), Self-Illumin/VertexLit (`Illumin-VertexLit.shader`), Specular (`Normal-Glossy.shader`), Transparent/Bumped Diffuse (`Alpha-Bumped.shader`), Transparent/Bumped Specular (`Alpha-BumpSpec.shader`), Transparent/Cutout/Bumped Diffuse (`AlphaTest-Bumped.shader`), Transparent/Cutout/Bumped Specular (`AlphaTest-BumpSpec.shader`), Transparent/Cutout/Diffuse (`AlphaTest-Diffuse.shader`), Transparent/Cutout/Soft Edge Unlit (`AlphaTest-SoftEdgeUnlit.shader`), Transparent/Cutout/Specular (`AlphaTest-Glossy.shader`), Transparent/Cutout/VertexLit (`AlphaTest-VertexLit.shader`), Transparent/Diffuse (`Alpha-Diffuse.shader`), Transparent/Parallax Diffuse (`Alpha-Parallax.shader`), Transparent/Parallax Specular (`Alpha-ParallaxSpec.shader`), Transparent/Specular (`Alpha-Glossy.shader`), Transparent/VertexLit (`Alpha-VertexLit.shader`), VertexLit (`Normal-VertexLit.shader`)


# Art Style Guide

GIMP filters
1) Mean Curvature Blur
2) Pixelize
3) Waterpixels
4) Softglow
5) Spread


===

Final:
-grading R top up bottom down, B top down bottom up
->filter->noise->HSV 3 ///
??? Blur->GHaussianm
->size to 400px, LINEAR
->scale up 1920x1080 NONE


---

## 1. Rendering technique

- **Method:** painterly pixel-mosaic. Not clean/uniform pixel-art (no fixed grid, no hard 1px outlines). Cell size varies by region — larger, looser clusters in flat/shadow areas (curtains, floor, walls), tighter clusters in focal detail (face, hands, flame).
- **No clean vector linework, no cel-shading, no flat color fills.** Every surface carries dither/noise texture — this is the load-bearing element of the style. 

- **Edges:** soft, broken, slightly irregular — never a crisp anti-aliased line. Mosaic edge noise substitutes for line quality.

**Rule:** define a fixed process (filter/plugin/prompt+seed) that produces this mosaic effect, and apply it as the **last step** on every asset, uniformly. Do not hand-vary the dither by scene.

## 2. Lighting model

- **Maximum two light-source color temperatures per scene**, established here as warm (fire/lamp, ~2000–2500K equivalent, amber/orange) vs cool (exterior window light, ~7000K+, blue).
- **Value distribution is heavily dark-weighted.** Estimate ~70% of frame at low value (near-black to dark umber), ~25% midtone, ~5% bright highlight pools directly at light sources. This is a night-interior chiaroscuro rule, not a universal rule — see Gaps.
- **Light falloff is local and small-radius.** Lamps/candles illuminate a tight pool (face, hands, immediate surface), not the room. No ambient fill light beyond source glow.
- **No flat/even lighting anywhere in frame.** Every surface should read as lit-from-somewhere or in shadow — flat mid-grey areas are a tell for inconsistency.

## 3. Palette

Sample and lock actual hex values from this reference before producing more assets — do not eyeball new palettes from memory. Approximate families present:

| Role | Family | Notes |
|---|---|---|
| Fire/lamp highlight | amber/orange, high saturation | small area, highest chroma in image |
| Skin/midtone warm | desaturated ochre-brown | |
| Shadow/base | near-black brown, blue-black | majority of canvas |
| Cool exterior | desaturated blue-grey | window only, low chroma |
| Wood/furniture | mid brown, low saturation | |

**Action item:** extract a 12–16 color locked swatch from this file (not from your head) and reuse it as your base ramp for every future asset. Deviation from this swatch = the #1 cause of scene-to-scene mismatch in this kind of style.

## 4. Composition & camera

- **Single fixed interior perspective**, slight elevated/eye-level angle, no dramatic foreshortening.
- **Prop density is high but subordinate** — background objects (books, bottles, clock, candlesticks) are numerous but rendered at lower contrast/detail than the focal subject, so they don't compete.
- **Focal subject occupies center-left to center**, with a secondary light anchor (fireplace) offset right. This asymmetric-but-balanced framing is a compositional habit worth repeating, not a hard rule — call it a default, not a law.

## 5. Detail hierarchy

- **Face/hands (points of narrative focus)** get the tightest mosaic resolution — most "readable" detail in the image.
- **Everything else** — walls, floor, distant furniture — gets looser, noisier treatment.
- **Rule:** detail resolution should track narrative importance, not literal distance from camera. This is different from photographic depth-of-field logic — it's an attention-directing rule.

## 6. Canvas / technical

- Confirm and lock: resolution, aspect ratio, and whether the mosaic effect is resolution-dependent (it likely is — re-check at your actual output resolution before mass-producing, since dither density will look wrong if generated at a different scale than this reference).

---

## Gaps — this guide does NOT yet cover

Flagging these explicitly because a one-image style guide is a trap if you don't:

1. **Daytime / exterior scenes.** Zero data here. Warm/cool dual-source lighting logic may not translate to open sky or daylight interiors.
2. **High-key or emotionally "light" scenes.** Your whole sampled palette is dark/moody. If your VN has any tonal range at all, you need a second reference for contrast.
3. **Character variety.** One old man, one skin tone, one clothing era. No data on how the mosaic/dither treats varied skin tones, hair colors, or modern dress if that's in scope.
4. **Multi-character scenes / dialogue framing.** This is a solo-figure establishing shot. No data on two-shots, close crops, or how the style holds up at different subject scales.
5. **Text/UI legibility.** If dialogue boxes or UI sit over these backgrounds, contrast and noise levels here may fight with text readability — untested.

**Do not generate your full background/scene set against this guide alone.** Produce 2–3 more references covering a bright scene, a mid-tone scene, and a two-character scene, extract shared rules, then lock v1.0.

---


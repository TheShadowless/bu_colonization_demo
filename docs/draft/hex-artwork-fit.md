# Hex artwork fit

The terrain illustrations had different silhouettes and transparent margins. Although the map Grid had zero cell gap, those margins exposed the camera background between adjacent tiles.

The map now fits each illustration to the same point-up hex footprint at render time. The original PNG artwork, Grid cell positions, colliders, movement and resource data are preserved. Both terrain and forest sprite assignments use shared rendering profiles, including a terrain change after clearing a forest.

- `HexArtworkProfiles` loads the 18 material profiles once and selects one when a sprite is assigned.
- `Hex Artwork Fit` maps each filled source row onto the canonical hex. It uses sprite UV transforms so SpriteRenderer batching and rotated atlas coordinates do not change the fit.
- `Tools > Colonization > Rebuild Hex Artwork Fit` rebuilds the profiles from the existing PNGs after replacing the artwork. It does not edit the PNG files.
- Original assignment PNG exports still contain the original illustration silhouettes; the fitting is part of this Unity project.

## Verification

The same 1600 × 900 map view was rendered with a magenta camera background before and after fitting. The original view exposed 99,009 background pixels; the fitted view exposed zero. A separate view repeating all 18 terrain and forest profiles also exposed zero background pixels. These checks cover the tested map views, rather than every possible camera configuration.

The updated live gameplay recording also checks sailing, landing, clearing a forest, building a settlement, opening the town UI and assigning/returning a worker.

# BU Colonization — Project Draft

Unity project for the GIS334 Project Draft assignment.

## Open the project

1. Clone or download this repository.
2. In Unity Hub, add the repository folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. Open with Unity **6000.3.19f1** and let Unity import assets and restore packages.
4. Open `Assets/Scenes/Map01.unity` and press Play.

## Project contents

- `Assets/`: scenes, scripts, prefabs, textures, and ScriptableObjects.
- `Packages/`: package manifest and lock file.
- `ProjectSettings/`: Unity project configuration.

Unity caches, logs, local editor preferences, and generated IDE files are excluded from Git.

## Source

Based on the course demo: https://github.com/watcharayui/bu_colonization_demo
Original commit history is preserved. Assets remain subject to their original authors' terms.

## Project Draft completed

The student artwork is connected through the original Texture2D/Sprite and ScriptableObject workflow:

- 11 base terrains and 7 forest variants.
- 20 land-unit icons and 6 ship icons, with catalogue ScriptableObjects.
- 13 Europe/Native faction flags and 8 Native settlement icons.
- 8 transparent Native leader illustrations and the Week 3–6 design catalogue.

[Week 3–6 Canva presentation](https://www.canva.com/d/HSXQdkDSmvqu63D) contains 27 pages. [Week 1–2 original work](https://www.canva.com/d/YCCt4hQw3R_Nm4H) is preserved separately.

[Download the gameplay video, assignment PNGs and presentation](https://github.com/TheShadowless/bu_colonization_demo/releases/tag/draft-2026-10-17).

The live Unity demonstration covers sailing, disembarking, forest clearing, founding a town, opening the town panel, dragging a worker into a surrounding terrain slot, and returning the worker to the idle area. Both fog renderers are disabled in the Hex prefab for this Draft demonstration.

### Controls

- Move camera: WASD / arrow keys. Zoom: mouse wheel or Z/X.
- Select unit: left click or Tab. Move to an adjacent hex: right click.
- Pioneer clears forest: P. Pioneer founds a settlement on cleared land: B.
- End turn: Backspace. Enter a town while standing on it: Enter.
- Inside town: drag a unit between the idle area and one of the six surrounding hex slots.

### Scope

Gameplay structure is retained. Existing enum IDs are preserved; the missing profession labels are appended to store the complete icon catalogue. Existing starting units and their behavior remain in place. Additional professions/ships are data entries, not new recruitment systems. The optional Week 7 production extension has not been added.

Native faction emblems are original game designs. Fictional leaders, scenario rivals, aggression and economic preferences are clearly identified as design choices. Historical context and the distinction between research and gameplay values are documented in [design notes](docs/draft/design-notes.md), [catalogue data](docs/draft/design-catalogue.json), and the Canva speaker notes.

See [submission links](docs/draft/submission.md) and [asset manifest](docs/draft/asset-manifest.json).

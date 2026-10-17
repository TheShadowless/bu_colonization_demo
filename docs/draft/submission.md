# Draft submission through DOC Week 7

- Git: https://github.com/TheShadowless/bu_colonization_demo
- Canva expanded editable version, 36 pages: https://www.canva.com/d/P0G2pnZW7p8jqBb
- Release: https://github.com/TheShadowless/bu_colonization_demo/releases/tag/draft-week7-2026-10-17
- Original Week 1–2 design is preserved: https://www.canva.com/d/YCCt4hQw3R_Nm4H

Open Unity 6000.3.19f1 and Assets/Scenes/Map01.unity. The release contains the editable PPTX, 92 artwork exports plus the full-size Assignment 7 Town Panel, an 84.5-second gameplay MP4, and the updated project skill ZIP.

## Completed

Assignment 7: Town Panel 1920 × 1080, seven sections, 16 reserved building sites, header/year/gold, workers, seven fitted Hexes, nearby ships, special output, 16 warehouse entries, Exit and Escape.

Assignment 8 / DOC Week 7: 16 product types with 17 transparent resource images, corn/fish Food variants, eight professions, center Food, per-slot production and accurate per-turn totals. Returning a worker or changing professions updates the previous output. Ship stats and Scout/equipment fields match the manual. Base Week 2 terrain potentials are preserved; forest values follow the DOC formula.

## Evidence and controls

The recording uses normal game APIs for sailing, landing, clearing and settlement creation. Its second portion drives virtual Mouse/Keyboard input through the real UI event system for dragging, choosing professions, returning workers and closing/reopening. Additional checks cover null/boundary slots, city switching, input isolation, correct data references and formula results. See ../week7/validation.json and ../week7/screenshots/.

World: WASD/arrows, wheel or Z/X zoom, Tab/left click selects units, right click moves, P clears forest, B builds a settlement, Backspace ends turn. Enter on an owned town opens it; left-clicking its town icon also opens it. Town: drag an idle worker onto a surrounding Hex, choose a producing profession, click an assigned worker to change it, or drag it back to idle. Exit/Escape closes; Escape cancels an open profession chooser first.

## Scope

The warehouse displays stored amounts and per-turn production. Automatic stock transfer starts in DOC Week 9. Manufacturing, recruitment, trade and the DOC Week 8 main menu are outside this Week 7 delivery. Building positions are reserved design slots. Native economic preferences and rivalry remain design settings. No standalone player build or Teams submission is claimed.

The original Canva remains separate. The new presentation contains no original process-note pages. Pages 28–36 cover Town Panel, resource icons, production and the geography supplement. DOC and Teams Week labels differ; assignment numbers define the match.

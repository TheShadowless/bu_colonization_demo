# Work plan through DOC Week 7

Started 17 October 2026. Authorized by the user to update the project skill, address learned mistakes, plan and implement through Week 7. Starting commit: `953baec6dd50bc113cc5e423bbccb86d8cc87470`. Main project: `E:/git/bu_colonization_demo`.

## Scope and source

The production manual controls the implementation and assignment acceptance. Teams Week labels are offset: Teams Week 6 is Assignment 6 (Native design), while DOC Week 6 contains Town UI and Assignment 7. DOC Week 7 adds Yield production, selection, aggregate Food output and Assignment 8 resource icons.

DOC Week 7's accumulation is the sum in `TotalYieldThisTurn`. Automatic transfer into `Warehouse` is introduced in DOC Week 9 (manual paragraphs 2322–2334), and is not implemented as an unrequested economy extension here. A 16-entry warehouse model/display is included as required by the Week 7 data setup.

## Delivery phases

- [x] P0 Read actual Week 7 text and code screenshots; identify rules and boundary with Week 9. Inspect Git, current source, Unity CLI and available app tools.
- [x] P1 Update the installed project skill with source/Week mapping, audit lessons, UI verification and tool discovery. Preserve existing six local user configuration changes.
- [x] P2 Correct ship names/stats and Scout/equipment data using DOC evidence. Resolve existing TownSlot reset/null behavior, close flow and unsafe UI/world input overlap.
- [x] P3 Create Assignment 7 town design at 1920 × 1080 with seven sections and 16 reserved building locations. Reuse the existing Canvas/TerrainSlots and replace the lecturer mockup. Use opaque readable navy/parchment/gold styling, coherent town imagery and working Exit/Escape.
- [x] P4 Create Assignment 8's 16 resource types plus the second Food (fish) image, transparent PNGs at 256 × 256. Create 16 ProductData assets in exact ID order and bind to GameManager. Record generation prompts, dimensions and alpha.
- [x] P5 Implement Week 7 yields and profession chooser. Display corn on land/fish at sea, eight yield choices, center Food, per-slot icons/counts and accurate aggregate Food. Keep model totals idempotent across opening, closing, changing professions and removing workers.
- [x] P6 Extend the authored resource data with forest Fur/Lumber potential, preserving Week 2 values. Apply the manual's forest reduction/town rounding rules and align presentation values with running calculations. Fix Town UI Hex gaps independently of world SpriteRenderer fitting.
- [x] P7 Update the design presentation/Canva to cover Assignment 7–8, keep original Week 1–2 work, exclude chat-note slides from the submission export, and provide corrected geography/implementation notes where relevant.
- [x] P8 Verify data, compilation, UI layout and actual player-facing events. Test assignment, profession changes, zero yields, occupied/null slots, city switching, close/reopen and totals; verify new art is used. Capture fresh screenshots and video, then prepare Git/release and submission notes.

## Week 7 rules

IDs: 0 Food, 1 Sugar, 2 Tobacco, 3 Cotton, 4 Furs, 5 Lumber, 6 Iron Ores, 7 Silver, 8 Horses, 9 Rum, 10 Cigars, 11 Clothes, 12 Coats, 13 Trade Goods, 14 Tools, 15 Musket. Food has corn and fish icons.

For normal yield, use the authored terrain potential. Fur/Lumber require forest; the other six IDs lose 2 in forest. A town halves the result with ceiling rounding, and negative results clamp to zero. Actual yield currently equals normal yield; specialist multipliers/manufacturing are later work. Center Hex automatically produces Food. Six surrounding Hexes produce only with an assigned resident worker and a selected resource.

Recompute totals from assignments, rather than incrementing UI counters on open, to avoid duplicated production. Dropping a worker opens the profession chooser. Returning to idle clears its old assignment/yield and refreshes totals. No background warehouse growth is claimed for this Week.

## Acceptance evidence

1. Exact six-ship stats, names and ranges; Scout range and equipment consistent with its icon.
2. Sixteen products, seventeen valid PNGs and resolved data references.
3. Town screen has seven sections and sixteen reserved building positions at the 1920 × 1080 reference size; can exit via Button and Escape.
4. Seven map Hexes in the town view, with consistent fitting and no stale forest/town overlay after new/null data.
5. Profession selection works through the actual buttons, shows zero-output options clearly and updates slot/total output correctly.
6. Open/close/reopen and city switching do not inflate totals or duplicate worker icons; no errors on boundary/null/occupied slots.
7. Inputs used for town interaction do not simultaneously move the camera, order world units or end a world turn behind the panel.
8. Fresh runtime screenshots/video and readable presentation exports. Distinguish direct API checks from input/UI interaction. Standalone builds are claimed only if one is actually produced and run.

## Preserved work and delivery

Preserve original enum integers, established movement/AI, Week 2 base terrain values and submitted original Canva. The six pre-existing modified configuration/IDE files are excluded from implementation commits. No Teams submission or instructor message is part of this authorization.

Progress and fresh evidence will be recorded here and in `docs/week7/validation.json` and `submission.md` as implementation finishes.

Completed evidence: `validation.json`, `screenshots/`, checked 36-page Canva DAHYOWR2Viw and prepared Week 7 release artifacts. Skill updated and validated with eight packaged files.

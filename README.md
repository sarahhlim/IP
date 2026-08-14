# README — Dead End Deductions


This document covers application controls, known limitations and bugs, the
finite state machines (FSM) driving game flow and NPC behaviour, answers to
each case's puzzle, and credits for all external assets used in the project.

---

## 1. Controls

The application is designed for keyboard and mouse. Controller support is
not implemented (see Section 2).

| Action                 | Input             | Context                                              |
|-------------------------|-------------------|-------------------------------------------------------|
| Move                    | W / A / S / D     | Office hub & crash-site scenes                        |
| Look / Camera           | Mouse movement    | First-person view, all scenes                         |
| Interact / Examine      | E                 | Talk to spirit, inspect evidence, use computer/case folder |
| Pause / Menu            | Esc               | Any scene                                              |

---

## 2. Known Limitations & Bugs

### Known limitations
- No controller/gamepad support — keyboard and mouse only.
- No save/load system yet — progress resets if the application is closed mid-session.
- Only 3 of 6 planned cases are fully scripted with evidence and dialogue at time of writing.
- No audio mixing/volume settings menu — system volume only.

### Known bugs

| Bug | Where it occurs | Severity | Workaround |
|---|---|---|---|
| Case folder UI can be reopened while a flashback is still loading, causing camera clipping through geometry. | Office Hub → Case transition | Medium | Wait for the loading fade to fully complete before pressing E again. |
| Frame rate drops noticeably in the Office Hub once all 6 case folders are present. | Office Hub | Medium | Lighting/lightmap optimisation planned; not yet resolved. |

---

## 3. FSM Diagrams & AI Implementation

The project uses two separate finite state machines: one driving overall
game/UI flow, and one driving individual spirit NPC behaviour during an
investigation.

┌─────────────┐   destination reached    ┌─────────────┐
│  Wandering   │ ────────────────────────▶│   Waiting    │
│ (moving to   │                           │ (idle, timer │
│  destination)│◀──────────────────────────│  counting down)│
└─────────────┘   timer expires,          └─────────────┘
                   new destination picked


---
## 4. Puzzle Answers
| Case | Correct Cause | Key Evidence | Red Herring(s) |
|---|---|---|---|
| 01 — Bus Incident | Driver slept on the job, sped the bus and bang into a tree | Tissue pack, Soda, Medicine bottle | phone: implies distraction, but there's no battery on the phone |
| 02 — Jaywalking (jWalking) | pedestrian crossed a red man while wearing full black. driver didn't see and crashed into them | Jacket, Phone, Headphones | none |
| 03 — PMD Incident | PMD on the road despite red man, causing the car to crash into it | Drugs, Phone, Cigarettes | None |


---

## 5. References & Credits

Assets: 
Cars and truck: https://assetstore.unity.com/packages/3d/vehicles/mobile-optimize-free-low-poly-cars-327313 

Ghost glow: https://assetstore.unity.com/packages/vfx/shaders/ghost-effect-shader-282923 

Terrain textures: https://assetstore.unity.com/packages/2d/textures-materials/nature/yughues-free-ground-materials-13001 

NPCs: https://assetstore.unity.com/packages/3d/characters/easy-primitive-people-161846 

City pack: https://assetstore.unity.com/packages/3d/environments/urban/city-package-107224 

Office pack: https://assetstore.unity.com/packages/3d/environments/office-environment-329079 

Buildings: https://assetstore.unity.com/packages/3d/props/versatile-building-kit-15-medium-poly-models-for-game-developmen-303398 

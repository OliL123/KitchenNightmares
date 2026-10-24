# Cult of the Chef — Design Doc

Halloween game jam entry. Cosmic-horror cooking game: Overcooked's chaotic station juggling crossed with Cooking Mama's minigames. You're a cultist cooking sacrifices for a sleeping eldritch god. Feed it well and fast enough, or it eats you.

- **Working title:** Cult of the Chef (alt: R'lyeh's Kitchen; first level could be called "Hell's Kitchen")
- **Team:** One programmer, one art partner
- **Engine:** Unity 6.3, 2D URP, new Input System
- **View:** Top-down 3/4 view like *The Binding of Isaac*. One kitchen that fits on a single screen.
- **Art:** Pixel art. Pixels-per-unit and reference resolution still to agree (suggestion: 32 PPU at 480×270).
- **Players:** 1–2, local co-op. Must be fully playable solo. Controller support is a priority.

> This version replaces the original doc. The "Changes from the original" section at the end lists what moved.

## Tone

Cheerful, cozy cooking-show framing over grim ingredients. Menu items use fake restaurant language ("Hand-Picked Finger Tenders", "Spaghetti al Sangue"). Cartoony, not gory. Optional: a screaming-chef cultist host ("IT'S RAW! THE GOD HUNGERS!").

## Core loop

1. The god shows an order (one of the dishes below).
2. Players grab raw ingredients from supply spots.
3. They move items through active and passive stations.
4. They build the dish on a plate and drop the plate into the maw at the center of the map.
5. Miss too many orders or deliver too slowly → the god's patience meter fills → game over (the god eats you).

**Key tension:** Active stations lock a player into a minigame while passive stations keep ticking and can ruin food. Players keep choosing between finishing a minigame cleanly or bailing out to save something else.

## Layout

A single kitchen. The **maw** sits in the center and every station is arranged around it, so every delivery crosses the most dangerous part of the map. The maw should be a pit in the floor rather than something tall, so it doesn't hide what's behind it.

## Carrying and plates

- Players carry one item at a time, **above their head**.
- Items can go on counters, on stations, or be **dropped on the floor**. Interact targets the closest station or item **in front of** the player.
- **Plates** come from a plate stack (unlimited for now). A plate holds **up to 4 ingredients**, added in any order.
  - Only prepared ingredients can go on a plate (each item has a *Can Plate* setting). Raw food can't.
  - Duplicates are allowed (double cheese), but then it's the wrong dish and has to be thrown out.
  - Nothing comes back off a plate.
  - Plating works both ways: hold the plate and take an ingredient, or hold the ingredient and put it on a plate. Supply spots and cooking stations can put food straight onto a held plate.
  - A plate shows icons for its contents above it. There's no "dish ready" signal: players read the plate themselves.
  - The trash empties a plate (you keep the plate) and destroys anything else.
- A dish matches **exactly**: the same ingredients and the same number of each.

## Dishes

| Dish | On the plate |
|---|---|
| Spaghetti al Sangue | Boiled Noodles + Blood |
| Spaghetti al Sangue e Formaggio | Boiled Noodles + Blood + Cheese |
| The Full Sacrament | Boiled Noodles + Blood + Cheese + Meatballs |
| Brain Mash al Sangue | Chanted Mash + Blood |
| Hand-Picked Finger Tenders | Finger Tenders |
| Tenders & Mash | Finger Tenders + Chanted Mash |
| Blood-Glazed Arm Roast | Roasted Arm + Blood |

Names are placeholders in the fake-menu style. Order tickets should show ingredient icons, not just the name.

## How each plate ingredient is made

| Ingredient | Made from |
|---|---|
| Boiled Noodles | Brain → **butcher block** (slice) → Brain Noodles → **cauldron** (boil) |
| Blood | **Bleeding rack**, which fills on its own over time |
| Cheese | **Cheese box** (raw, ready to use) |
| Meatballs | Arm → **grinder** |
| Chanted Mash | Brain → **grinder** (mash) → Brain Mash → **altar** |
| Finger Tenders | Arm → **butcher block** (snip) → Fingers → coat in bone dust (Bones → **grinder**) → **brazier** (fry) |
| Roasted Arm | Arm, whole or fingerless → **altar** (bless) → **brazier** (roast) |

The fingerless arm left over from snipping fingers can go into the roast.

**Bottlenecks to watch in playtesting:** blood is in 5 of the 7 dishes; the grinder makes three things and locks a player in; the altar serves three dishes.

## Stations

### Supply (unlimited)
- **Sacrifice pen:** one wide station with two hatches, arms on one side and brains on the other. Face the side you want.
- **Bone pile:** bones
- **Cheese box:** cheese
- **Plate stack:** plates

### Active (minigame, the player is locked in for 2–5 s)
- **Butcher block:** cutting minigame. Brain → brain noodles. Arm → fingers + fingerless arm.
- **Grinder:** mash or rotate minigame. Brain → brain mash. Bones → bone dust. Arm → meatballs.
- **Altar** (the chanting pedestal and ritual circle merged into one): Brain mash → chanted mash. Arm (whole or fingerless) → blessed arm. Suggested minigame: rhythm button prompts, which work the same on keyboard and controller.

Minigames are **graded**. A sloppy result still works but lowers dish quality; higher quality pleases the god more. Cap it at these three minigames.

### Passive (timer; food is ruined if left too long)
- **Cauldron:** one item at a time. Brain noodles → boiled noodles.
- **Brazier:** breaded fingers → finger tenders. Blessed arm → roasted arm.
- **Bleeding rack:** fills with blood on its own over time; a player collects it. Idea: it overflows and wastes the blood if it's left full too long.

Passive stations cook one item. You can take the food out at any point and it keeps its progress. Food left too long becomes **Overcooked**, which can't be plated and has to go in the trash (a possible future mechanic could use it).

### Other
- **Counters:** park one item or a plate.
- **Trash**
- **The maw:** plates go in here.

## The god

A sleeping god represented by an open maw in the center of the map. **God states are a stretch goal.** The core game only needs orders, the patience meter and game over.

If there's time: its state changes on a **timer** (not based on player performance), and every state gets a readable tell about 2 seconds before it takes effect (maw color, rumble, eyes opening, breathing change). Its animations are effectively the game's UI.

| State | Effect |
|---|---|
| Sleeping | Baseline, nothing changes |
| Stirring | Telegraphed tentacle sweeps across part of the floor. A hit interrupts any minigame in progress. |
| Dreaming | Stations drift or swap positions |
| Craving | Demands a specific dish for a bonus |
| Restless | Orders arrive faster but are worth more |
| Deep sleep | Calm period, recovery and prep time |

## Items

- **Raw:** arm, brain, bones, blood, cheese
- **In progress:** brain noodles, boiled noodles, fingers, breaded fingers, fingerless arm, blessed arm, roasted arm, brain mash, chanted mash, bone dust, meatballs, finger tenders
- **Other:** plate, Overcooked

**Art note:** many in-progress states can reuse a base sprite with a tint, overlay or particles (boiled = tint + steam, blessed = glow, roasted = darker tint). Every item also needs to read at icon size, because it's carried above someone's head and shown as an icon on plates.

## Build order

1. ✅ Movement in a bounded room, camera
2. ✅ Pick up, put down, floor drops, depth sorting
3. ✅ Supply spots
4. ✅ Plates, dishes, trash
5. ✅ First passive station: the cauldron
6. ⬜ Butcher block (first active station and minigame)
7. ⬜ Maw, orders, patience meter, game over → **first playtest: does the juggling feel tense?**
8. ⬜ Co-op and controllers
9. ⬜ Remaining stations (grinder, altar, brazier, bleeding rack, bone pile) and dishes
10. ⬜ Minigame grading and quality scoring
11. ⬜ Polish: host dialogue, audio, screen shake, pixel-perfect camera
12. ⬜ Stretch: god states, limited plates with washing, a shop between rounds, two players on one keyboard

## Open questions

- Exact jam length and deadline
- Patience meter tuning; order speed scaling with player count
- What a wrong delivery costs: just time, or patience too?
- Pixels per unit and reference resolution
- Butcher block rules: what the cutting minigame looks like, whether you can bail out mid-cut, where the second output goes
- Altar minigame: rhythm prompts (suggested) or something else
- How fingers get coated in bone dust

## Changes from the original

- Engine decided (Unity); view is Isaac-style 3/4, not isometric; pixel art.
- Co-op moved from stretch to core (1–2 players), plus controller support.
- Plates added, with exact-match recipes. Dishes now go on plates.
- Blood ketchup dropped; the mash dish uses plain blood.
- New ingredients: cheese (raw), meatballs (grinder).
- Spaghetti split into three variants; tenders gained a "with mash" variant (7 dishes total).
- The chanting pedestal and ritual circle merged into one altar.
- The sacrifice pen has two hatches; the bleeding rack no longer needs victims.
- Ruined food becomes "Overcooked" and goes in the trash.
- God states moved from MVP to stretch.

# 🔨 Soulbonk

> **"Explore with Soul. Fight with Bonk."**
>
> A hybrid 2D Action-Roguelite built with Unity. Blending the deliberate exploration of *Elden Ring* with the chaotic horde-survival intensity of *Vampire Survivors*.

![Gameplay Demo](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjEx...INSERT_YOUR_GIF_LINK_HERE) 
*(Note: A gameplay GIF showcasing the combat impact will be added here soon)*

## 🎮 The Concept

**Soulbonk** is a genre-bending "Extraction Survivor" game featuring a unique **Dual-Phase Loop**:

### Phase I: The Preparation (Daylight) ☀️
*Genre: Open World / Souls-like*
* **Explore:** Roam a handcrafted open world to find hidden chests and resources.
* **Prepare:** Hunt elite enemies to gather "Soul Shards".
* **Upgrade:** Rest at **Campfires** to permanently increase your base stats (Movement Speed, Damage, Health).
* **Risk:** Venture deeper for better loot, but remember: time is ticking.

### Phase II: The Eclipse (Nightfall) 🌑
*Genre: Horde Survival / Bullet Heaven*
* **Survive:** The sky turns dark. The map borders close in. You can no longer hide.
* **The Bonk:** Endless hordes of enemies swarm your location.
* **Evolve:** Use the power gathered during the day to unlock chaotic abilities and massive area-of-effect attacks.
* **Meta:** Survive as long as possible. Death is not the end; use Meta-Currency to unlock new skills in the Skill Tree.

## 🛠 Tech Stack & Architecture

* **Engine:** Unity 6.3 LTS
* **Language:** C#
* **Architecture:**
    * **VectorViolet Core:** Custom modular architecture for Stat Systems and Audio Management.
    * **ScriptableObject Workflow:** Data-driven design for Enemy Stats, Items, and Skills.
    * **State Machine:** Finite State Machine (FSM) for Enemy AI behaviors (Patrol, Chase, Attack).
* **Tools:**
  * GitHub Projects (Kanban) [https://github.com/users/umutcanekinci/projects/6]
  * Figma Whiteboarding (Game Design & UI/UX) [https://www.figma.com/board/tN4s3aQBngW0V2z9BegVQW/Soulbonk?node-id=0-1&t=TRBo3Wkng3GCbL2K-1]
  * Nano Banana Pro with Adobe Express (Background Remover)
  * Visual Studio Code

## 📅 Roadmap & Kanban

We follow an Agile methodology. You can track our live progress on our [**GitHub Project Board**](https://github.com/users/umutcanekinci/projects/6).

### Sprint 1: Foundation (Completed) ✅
- [x] **Core Movement:** Fluid 2D Movement & Physics-based Dash.
- [x] **Architecture:** Implemented `StatHolder` and `EntityMovement` systems.
- [x] **Audio:** SoundManager Core implementation.
- [x] **Visuals:** Basic Rendering and Tilemap setup.

### Sprint 2: The Enemy (Current) 🚧
- [ ] **AI:** Implement NavMesh-based Enemy AI (Chase & Attack States).
- [ ] **Combat:** "Impact Frame" logic for satisfying melee hits.
- [ ] **Animation:** Syncing Attack Speed stats with Animator multipliers.
- [ ] **UI:** Dynamic Health Bars and Floating Damage Numbers.

### Sprint 3: The Game Loop (Upcoming) ⏳
- [ ] **Phase System:** Implementing the "Eclipse" Timer and Phase switching logic.
- [ ] **Campfire System:** UI for spending resources to upgrade stats.
- [ ] **Horde Logic:** Enemy spawner system for Phase II.

## 🔧 Installation / How to Run

1.  Clone the repository:
    ```bash
    git clone [https://github.com/umutcanekinci/soulbonk.git](https://github.com/umutcanekinci/soulbonk.git)
    ```
2.  Open **Unity Hub**.
3.  Add the project folder and open with **Unity 6.2 (6000.2.9.f1)** (or newer).
4.  Navigate to `Assets/Scenes` and open **MainScene**.
5.  Press **Play**!

## 🤝 Assets & Credits

This project is developed by **Umutcan Ekinci**.

**Third-Party Assets:**
- [Character Sprite Pack by Xzany](https://xzany.itch.io/top-down-adventurer-character)
- [Input Prompts by Kenney](https://www.kenney.nl/assets/input-prompts)
- [Mobile Controls by Kenney](https://www.kenney.nl/assets/mobile-controls)
- [Environment: Small Forest by Rowdy41](https://rowdy41.itch.io/small-forest)
- [Environment: Top Down Tileset by Itch.io Community](https://itch.io/c/1574579/top-down)
- [Environment: Top Down Assets Collection by Itch.io Community](https://itch.io/queue/c/1574579/top-down?game_id=3307044&password=)
- [Enemies: Monster Packs by Admurin](https://admurin.itch.io/monster-pack-40), [Monster Pack 21](https://admurin.itch.io/monster-pack-21), [Monster Pack Collection](https://admurin.itch.io/monster-pack)

Future Usage Plans
- https://admurin.itch.io/free-monster-pack-character
- https://admurin.itch.io/free-monster-pack-character-2
- https://admurin.itch.io/top-down-mobs-warhog
- https://admurin.itch.io/top-down-mobs-dog
- https://admurin.itch.io/top-down-mobs-bee
- https://admurin.itch.io/monster-pack-85
- https://admurin.itch.io/monster-pack-83
- https://admurin.itch.io/monster-pack-82
- https://admurin.itch.io/monster-pack-66

---
*License: MIT*

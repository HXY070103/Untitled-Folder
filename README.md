# Evolve or Fall

**Evolve or Fall** is a 2D action roguelite game developed with Unity.

The title reflects the core idea of the game: the player can become stronger by exploring more rooms, but the Boss will also grow stronger at the same time. Every choice affects the final fight.

> You grow. The Boss grows. Every choice shapes the fight.

---

## Short Introduction

**Evolve or Fall** is a fast-paced 2D action roguelite game inspired by games such as *BlazBlue Entropy Effect*. The game focuses on action combat, build-making, and meaningful risk-reward decisions.

At the start of each run, the player chooses one combat style: **Time Control**, **Life Steal**, or **Phase**. Each style has its own core skill, upgrade options, and evolution paths.

During the run, the player can choose to fight the Boss directly or explore more rooms to gain more upgrades, purification chances, and evolutions. However, every extra room also makes the Boss stronger. This means the player must decide whether to challenge the Boss early or take the risk of becoming stronger while also empowering the Boss.

---

## Core Features

### Three Combat Skill

The game includes three different combat skills. Each skill gives the player a different way to fight and survive.

#### Time Control

Time Control slows down enemies and projectiles, giving the player more time to dodge, aim, and attack.

Main ideas:

- Slow enemies
- Slow projectiles
- Create openings for attacks
- Control the pace of combat

#### Life Steal

Life Steal allows the player to recover health by dealing damage. This style encourages aggressive and risky combat.

Main ideas:

- Deal damage to recover health
- Fight more aggressively
- Survive dangerous situations
- Become stronger when taking risks

#### Phase

Phase allows the player to become temporarily untouchable, avoid attacks, and strike back with a powerful counterattack.

Main ideas:

- Avoid enemy attacks
- Become temporarily untouchable
- Create counterattack chances
- Reward precise timing

The more details about the three skills will be introduced later.

---

### Skill Decay System

Each combat style has a core skill that can be used repeatedly. However, every use increases the skill decay level, making the skill weaker over time.

This creates an important decision during combat:

- Use the skill now to survive.
- Save the skill strength for a harder fight later.

The player can reduce skill decay by clearing rooms or choosing purification rewards. This system prevents the player from using powerful skills without thinking and makes every skill activation more meaningful.

Example:

| Decay Level | Skill Effect |
|---|---|
| 0 | 100% |
| 1 | 85% |
| 2 | 70% |
| 3 | 55% |
| 4 | 40% |
| 5+ | 30% |

Ways to reduce decay:

| Method | Effect |
|---|---|
| Clear Room | -1 Decay |
| Purify | -2 Decay |

---

### Boss Growth System

Before fighting the Boss, the player can choose to enter more normal or elite rooms. These rooms can give the player more upgrades or purification.

However, every extra room also strengthens the Boss. The Boss may gain:

- More health
- Higher damage
- Faster attacks
- Stronger abilities

This creates the main risk-reward loop of the game:

> The player becomes stronger, but the Boss also evolves.

---

### Upgrade, Purification, and Evolution

After clearing rooms, the player can receive different types of rewards.

#### Upgrades

Upgrades improve the player's current combat style. The player usually chooses one upgrade from several options.

#### Purification

Purification reduces skill decay. Choosing purification can help the player keep their core skill powerful, but it may also mean giving up an upgrade.

The player must choose rewards based on their current health, skill decay level, combat style, and the strength of the upcoming Boss.

---

## Gameplay Overview

In each run, the player begins by choosing one combat skill. After that, the player enters combat rooms, defeats enemies, and collects rewards to improve their build.

The player needs to manage two things at the same time: their own growth and the Boss's growth. Exploring more rooms gives the player more chances to become stronger, but it also makes the Boss more dangerous.

A typical run follows this structure:

1. Choose a combat skill.
2. Enter normal or elite combat rooms.
3. Defeat enemies and collect rewards.
4. Manage skill decay through careful skill usage and purification.
5. Decide whether to continue exploring or fight the Boss.
6. Challenge the Boss before it becomes too powerful.

The game is designed to make every decision matter. Players must think carefully about when to use their core skill, which rewards to choose, and when to stop exploring and face the Boss.

---

## Built With

- Unity
- C#
- GitHub

## Character Controller

The player controller is implemented using a finite state machine (FSM) architecture.

### Movement

| Key | Action |
|---|---|
| [A] / [D] | Move left / Move right |
| [Space] (Ground) | Jump |
| [K] | Dash |

### Wall Movement

When the player is close to a wall while falling, the character enters the `WallSlide` state.

The player exits the `WallSlide` state after moving away from the wall.

| Key | Action |
|---|---|
| [S] (WallSlide) | Slide down faster |
| [Space] (WallSlide) | Wall jump |

### Combat

| Key | Action |
|---|---|
| [J] (Ground) | Three-hit combo attack |
| [J] (Air) | Downward slash attack |
| [L] (Ground) | Counter attack |

### State Machine

The controller uses a modular FSM system with states. 
Each state handles animation transitions and physics behavior independently for better scalability and maintainability.

Main States: 

- Grounded State  
  - Idle State: The default state when the character is standing still on the ground with no movement input.  
  - Move State: The state when the character is moving horizontally on the ground based on player input.  

- Air State  
  - Jump State: The initial rising state when the character leaves the ground.  
  - Fall State: The state when the character is descending after reaching the peak of a jump or falling off a platform.  

- Dash State: A fast, short-duration movement state.  

- Wall Slide State: A state triggered when the character is touching a wall in the air, causing a controlled slide along the wall.  

- Wall Jump State: A movement state where the character pushes off from a wall to gain vertical or horizontal velocity.  

- Attack State: The basic ground attack state where the character performs a standard ranged attack.  

- Jump Attack State: A downward air attack performed while falling, dealing damage upon impact with enemies below.  

- Counter Attack State: A special reactive state triggered in response to an enemy attack.

Transition bewteen states:

![FSM Diagram](Images/StateTransition.png)

---

## Attribute Stats

### Resource Stats

| Attribute | Default Value | Description |
|---|---|---|
| maxHP | 100 | Maximum Health |

### Offensive Stats

| Attribute | Default Value | Description |
|---|---|---|
| damage | 10 | Base damage |
| critChance | 20% | Critical hit chance |
| critPower | 200% | Critical damage multiplier |

### Defensive Stats

| Attribute | Default Value | Description |
|---|---|---|
| armor | 0 | Damage reduction factor |

### Special Stats

| Attribute | Default Value | Description |
|---|---|---|
| skillEffectiveness | 100% | Multiplier applied to skill effects |

### Damage Calculation

Produced damage:

```math
outputDamage =
\begin{cases}
damage \times critPower, & \text{critical hit}\\
damage, & \text{normal hit}
\end{cases}
```

Produced mitigation:

```math
damageMitigation = \frac{armor}{armor + 100}
```

Final damage taken:

```math
finalDamage = producedDamage \times (1 - damageMitigation)
```

### Attribute Upgrades

- **Vitality**  
  - Increases survivability by enhancing health and defense.
  - Gives +5 Max HP and +10 Armor.

- **Strength**  
  - Focuses on raw physical power.
  - Gives +10 Damage.

- **Precision**  
  - Improves critical performance and consistency.
  - Gives +3% Crit Chance and +0.2 Crit Power.

- **Technique**  
  - Enhances overall skill efficiency and scaling.
  - Gives +0.2 Skill Effectiveness.

---

## Skill System

The player begins the game by selecting one of three core skills:

- Time Control

- Life Steal

- Phase

Each skill defines a distinct playstyle and has its own augment paths and scaling mechanics.

[Q] is used as the primary skill activation input.

___

### Time Control

#### Skill Effect

Creates a domain that slows down enemies and enemy projectiles within its area.

#### Base Parameters

| Parameter | Value |
|---|---|
| Duration | 3 seconds |
| Slowdown Percentage | 80% |
| Domain Range | 20 units |

#### Skill Upgrade

| Upgrade Name | Rarity | Effect |
|---|---|---|
| Temporal Freeze | Rare | Enemies within the field are immobilized |
| Hunting Time | Uncommon | Killing enemies during the skill extends its duration |
| Synchronized Field | Uncommon | The field follows the player’s movement |
| Temporal Corrosion | Common | Enemies within the field take continuous damage over time |
| Precision Targeting | Common | The first attack against each enemy during the skill deals increased damage |

#### Decay Resistance Upgrade

| Upgrade Name | Effect |
|---------------|--------|
| Time Stabilization | Each skill activation increases skill decay by 50% less |
| Precision Control | If only 1 enemy is killed during the skill and the player takes no damage, no skill decay is gained |
| Low Temperature Domain *(Mutually Exclusive with Compression Field)* | Skill decay only reduces duration |
| Compression Domain *(Mutually Exclusive with Low Temperature Field)* | Skill decay only reduces slow strength and domain range |

---

### Life Steal

#### Skill Effect

Records all damage dealt during the skill and converts it into healing after the duration ends

#### Base Parameters

| Parameter | Value |
|---|---|
| Duration | 3 seconds |
| Conversion Rate | 30% |

#### Skill Upgrade

| Upgrade Name | Rarity | Effect |
|---|---|---|
| Unshakable Resolve | Rare | If the player takes fatal damage during the skill, instantly restore 20% max HP, but cannot heal after the skill ends |
| Stable Gain | Uncommon | Gradually restores HP in real time during the skill |
| Desperate Gambit | Uncommon | Damage conversion ratio increases as HP decreases during the skill |
| Blood Growth | Common | If post-skill healing exceeds 20% max HP, permanently increases max HP |
| Blood Frenzy | Common | Increases damage taken during the skill, but also increases damage and attack speed |

#### Decay Resistance Upgrade

| Upgrade name | Effect |
|---|---|
| Battle Stabilization | Each skill activation increases skill decay by 50% less |
| Blood Reparation | If sufficient damage is dealt during the skill, no skill decay is gained for that activation |
| War Discipline *(Mutually Exclusive with Bloodlust)* | Skill decay no longer reduces duration |
| Blood Pact *(Mutually Exclusive with War Discipline)* | Skill decay no longer reduces healing conversion ratio |

---

### Phase

#### Skill Effect
Enter a phased state where the next attack becomes a heavy strike. During the phase state, the player is immune to damage and gains increased movement speed. If the player attacks, the phase state ends immediately. If the player times the phase just before an enemy attack, a perfect phase is triggered.

#### Base Parameters

| Parameter | Value |
|---|---|
| Duration | 3 seconds |
| Heavy Strike Multiplier | 200% |

#### Skill Upgrade

| Upgrade Name | Rarity | Effect |
|---|---|---|
| Shadow Assassination | Rare | Converts heavy strike into a lethal strike |
| Weakness Marking | Uncommon | Passing through enemies during phase reduces their damage and armor |
| Shadow Execution | Uncommon | After killing an enemy with a heavy strike, briefly re-enter phase |
| Chain Strikes | Common | After triggering Perfect Phase, the next 3 attacks become heavy strikes |
| Perfect Echo | Common | Restores a portion of HP when Perfect Phase is triggered |

#### Decay Resistance Upgrade

| Upgrade Name | Effect |
|---|---|
| Phase Stabilization | Each skill activation increases skill decay by 50% less |
| Perfect Rhythm | Triggering Perfect Phase prevents skill decay gain for that activation |
| Sustained Shift *(Mutually Exclusive with Shadow Strike)* | Skill decay no longer reduces phase duration |
| Shadow Strike *(Mutually Exclusive with Sustained Shift)* | Skill decay no longer reduces heavy strike multiplier |

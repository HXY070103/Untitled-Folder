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

### Boss Growth System

Before fighting the Boss, the player can choose to enter more normal or elite rooms. These rooms can give the player more upgrades or purification.

However, every extra room also strengthens the Boss. The Boss may gain:

- More health
- Higher damage
- Faster attacks
- Stronger abilities

This creates the main risk-reward loop of the game:

> The player becomes stronger, but the Boss also evolves.

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

---

## StateMachine

`StateMachine` is responsible for managing the currently active state and handling state transitions.

It serves as the central controller of the finite state machine architecture while delegating behavior execution to the active state.

### Active State Management

The currently active state is stored through:

```csharp
EntityState currentState
```

This reference determines which state receives update calls and executes behavior logic.

### State Initialization

#### `Initialize(EntityState startState)`

- Assigns the starting state.
- Calls the state's `Enter()` method.
- Enables state transitions.

### State Transitions

#### `ChangeState(EntityState newState)`

- Verifies whether state transitions are currently allowed.
- Calls `Exit()` on the current state.
- Updates the active state reference.
- Calls `Enter()` on the new state.

When a transition occurs:
1. The current state's `Exit()` method is called.
2. The active state reference is updated.
3. The new state's `Enter()` method is called.

```text
Current State -> Exit -> Change State -> Enter -> New State
```

### State Updates

#### `UpdateActiveState()`

- Updates the currently active state.

### Transition Control

State transition availability is controlled through:

```csharp
bool canChangeState
```

#### `SwitchOff()`

- Disables all future state transitions.

---

## EntityState

`EntityState` represents a single character behavior within the state machine.

Each state contains only the logic required for its specific behavior while sharing a common lifecycle and interface.

### State References

Every state stores references to:

```csharp
Entity entity
StateMachine stateMachine
```

These references allow the state to access character functionality and request state transitions.

### State Lifecycle

Each state follows the same lifecycle:

```text
Enter() -> Update() -> Exit()
```

### Enter

#### `Enter()`

- Executed once when the state becomes active.
- Starts the state's animation.
- Resets state-specific variables.


### Update

#### `Update()`

- Executed every frame while the state remains active.
- Executes state-specific behavior.

The base implementation also updates:

```csharp
stateTimer
```

which can be used for cooldowns, delays, and timed transitions.

### Exit

#### `Exit()`

- Executed once before leaving the state.
- Stops the state's animation.

### Animation Events

Animation events are forwarded through:

#### `AnimationTrigger()`

- Marks that an animation event has occurred.
- Sets the internal trigger flag.

The result is stored in:

```csharp
bool triggerCalled
```

This allows states to react to animation events without directly coupling gameplay logic to animation clips.

---

## Entity

`Entity` serves as the base class for all living characters in the game, including the player, enemies, and bosses.

It provides a common framework for state management, movement control, character orientation, and collision detection. By centralizing these shared systems, derived classes can focus on implementing character-specific behavior while reusing the same core functionality.

### State Reference

Each entity owns a state machine through:

```csharp
StateMachine stateMachine
```

This reference is responsible for managing the entity's current behavior and handling state transitions. The entity updates the state machine every frame, allowing the currently active state to execute its behavior.

### State Machine Management

Each entity owns a `StateMachine` instance responsible for controlling its current behavior.

#### `Update()`

- Serves as the main update loop of the entity.
- Updates collision detection information through `HandleCollisionDetection()`.
- Updates the currently active state through `stateMachine.UpdateActiveState()`.

### Movement System

Movement is performed through a unified velocity interface built on top of `Rigidbody2D`.

#### `SetVelocity(float xVelocity, float yVelocity)`

- Updates the entity's velocity.
- Automatically updates character orientation through `HandleFlip()`.

### Character Orientation

The entity maintains its current facing direction through:

```csharp
int facingDir
```

- `1` indicates facing right.
- `-1` indicates facing left.

This value is commonly used by movement, AI, and combat systems to determine the entity's current orientation.

#### `HandleFlip(float xVelocity)`

- Determines whether the entity should change its facing direction.
- Calls `Flip()` when movement direction differs from the current orientation.

#### `Flip()`

- Rotates the character horizontally.
- Updates internal facing direction values.

### Collision Detection

Collision detection is continuously updated through raycasts and exposed through public properties.

#### `HandleCollisionDetection()`

- Updates ground detection status.
- Updates wall detection status.
- Stores results in `groundDetected` and `wallDetected`.

#### `OnDrawGizmos()`

- Draws ground detection raycasts.
- Draws wall detection raycasts.

#### Ground Detection

Ground detection is performed using a downward raycast.

```csharp
bool groundDetected
```

Responsibilities:

- Determines whether the entity is standing on valid ground.
- Supports movement, jumping, and state transitions.

#### Wall Detection

Wall detection is performed using two forward raycasts.

```csharp
bool wallDetected
```

Responsibilities:

- Detects obstacles in front of the entity.
- Supports AI navigation and movement decisions.
- Improves detection reliability compared to a single raycast.

### Animation Events

The entity provides a bridge between animation events and the state machine.

#### `CurrentStateAnimationTrigger()`

- Forwards animation events to the currently active state.

---

## State Machine Architecture

The character behavior system is built around three core components:

- `Entity`
- `StateMachine`
- `EntityState`

Together, they form a reusable finite state machine (FSM) architecture that separates shared character functionality, state management, and gameplay behavior.

### Component Overview

#### Entity

`Entity` serves as the owner of the state machine and provides shared functionality such as movement, collision detection, and character orientation.

#### StateMachine

`StateMachine` manages the currently active state and handles state transitions.

#### EntityState

`EntityState` represents a specific character behavior, such as idle, movement, attack, chase, or death.

#### Relationship

The relationship between these components is shown below:

```text
Entity - (owns) -> StateMachine - (controls) -> EntityState
```

- `Entity` owns a `StateMachine`.
- `StateMachine` manages the currently active `EntityState`.
- `EntityState` contains the actual gameplay behavior.

This separation allows shared functionality, state management, and state-specific behavior to remain independent and easy to maintain.

### Runtime Execution Flow

Unity's frame update begins at the `Entity` level through the `Update()` method. During each frame, the entity first updates its shared systems and then passes control to the currently active state through the state machine.

The following sequence occurs every frame:

```text
Entity.Update() -> StateMachine.UpdateActiveState() -> CurrentState.Update() -> Specific Behavior
```

As a result:

- `Entity` manages shared character systems.
- `StateMachine` manages which state is active.
- `EntityState` executes the actual behavior.

This clear separation of responsibilities makes the system easier to maintain, debug, and extend.

---

### Animation Integration

#### Animation Transitions

Each `EntityState` is associated with an animation parameter through:

```csharp
string animBoolName
```

When a state becomes active, the corresponding animation parameter is enabled:

```csharp
anim.SetBool(animBoolName, true)
```

When the state exits, the parameter is disabled:

```csharp
anim.SetBool(animBoolName, false)
```

This allows state transitions and animation transitions to remain synchronized.

```text
State Enter() -> Animation Parameter Set True -> Animation Plays
State Exit() ->  Animation Parameter Set False -> Animation Ends
```

As a result, each state is responsible for activating its own animation while the state machine controls when those states become active.

#### Animation Events

Animation events can communicate with the state machine through:

```csharp
CurrentStateAnimationTrigger()
```

which forwards the event to:

```csharp
stateMachine.currentState.AnimationTrigger()
```

This allows animation clips to notify the currently active state when an animation event occurs without directly referencing specific state implementations.

```text
Animation Event Triggered -> CurrentStateAnimationTrigger() -> currentState.AnimationTrigger() -> Specific Behavior
```

This design keeps animation logic decoupled from gameplay logic while still allowing states to react to animation timing.

---

## Character Controller

The player controller is implemented using a finite state machine (FSM) architecture.

### Input Handling

Player input is centralized through the `PlayerInputSet` component.

The input system is responsible for:

- Reading keyboard input
- Exposing movement input values
- Exposing action inputs such as jump, dash, and attack

States can access player input through:

```csharp
PlayerInputSet input;
```

This allows player behaviors to react to user input without directly querying Unity's input system.

```text
Keyboard Input -> PlayerInputSet -> FSM Architecture -> Character Behavior
```

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

## Combat & Health System

The Combat & Health System is built around a clear separation between attack execution and damage processing.

Combat interactions are initiated through the `EntityCombat` component, which is responsible for target detection and damage generation. Instead of directly modifying a target's health, attacks are delivered through the `IDamageable` interface, allowing different entity types to participate in combat using the same interaction model.

Damage processing is handled by the `EntityHealth` component. Upon receiving damage, the system applies defensive calculations, updates health values, determines knockback behavior, and handles death-related logic. Additional mechanics such as damage-over-time effects, healing, revival, and health modification are also managed within this component.

### Core Interfaces

The combat system uses interfaces to keep it flexible and extensible.

#### `IDamageable`

Defines an object that can receive damage from the combat system.

- TakeDamage(Damage damage, Transform damageDealer)

Responsibilities: 

- Receives and processes incoming damage.

#### `ICounterable`

Defines an object that can respond to counter mechanics.

- CanBeCountered()
- HandleCounter()

Responsibilities:

- Determines whether the entity is currently counterable.
- Executes counter logic when triggered.

### EntityCombat

`EntityCombat` is responsible for attack execution and damage delivery.

#### `PerformAttack()`

- Serves as the entry point of the attack system.
- Retrieves all valid targets within the attack range using `GetDetectedColliders()`.
- Iterates through each detected target and applies damage through `PerformAttackOnTarget()`.

#### `PerformAttackOnTarget(Transform target)`

- Handles combat interactions with a single target.
- Verifies that the target implements the `IDamageable` interface.
- Generates damage based on the damage dealer's current statistics.
- Delivers damage to the target through the `IDamageable` interface.
- Plays an hit visual effect after a successful attack.

#### `GetDetectedColliders(LayerMask whatToDetect)`

- Detects all valid targets within the configured attack range.
- Uses Unity's physics overlap queries to retrieve colliders.
- Filters detected objects through layer masks to ensure only intended targets are processed.
- Returns all valid colliders within the configured attack radius.

### PlayerCombat

`PlayerCombat` extends `EntityCombat` and adds player-specific combat mechanics while reusing the shared attack framework.

#### `CounterAttackPerformed()`

- Detects all counterable targets within the counter-attack range.
- Checks whether each target implements the `ICounterable` interface.
- Verifies that the target is currently in a counterable state.
- Triggers the target's counter-handling logic through the `ICounterable` interface.

### EntityHealth

`EntityHealth` implements `IDamageable` and is responsible for handling all health-related logic after damage has been received.

#### `TakeDamage(Damage damage, Transform damageDealer)`

- Serves as the entry point of the damage taken system.
- Checks whether the entity is already dead and ignores further damage if true.
- Calculates final damage using `CalculateFinalDamage()`.
- Reduces current health using `ReduceHP()`.
- Triggers knockback using `CalculateKnockback()`.
- Plays on damage visual effect after damage taken.

#### `CalculateFinalDamage(Damage damage)`

- Generates armor migitation based on the damage taker's current statistics.
- Converts raw damage into final effective damage value.

#### `ReduceHP(float damage)`

- Deducts health from the current HP value.
- Handles entity death by invoking `Die()` when HP reaches zero.
- Invokes health update events for UI synchronization.

#### `Die()`

- Marks the entity as dead.
- Triggers entity death behavior.

#### `CalculateKnockback(float damage, Transform damageDealer)`

- Determines knockback direction based on attacker position.
- Chooses between normal and heavy knockback based on damage percentage.
- Returns final knockback force applied to the entity.

---

## Stats System

The stats system is responsible for storing and calculating all combat-related attributes used by entities. Each character owns an `EntityStats` component that provides a centralized interface for retrieving offensive, defensive, and resource values.

### StatModifier

`StatModifier` represents a single modification applied to a stat.

Each modifier contains:

```csharp
float value
string source
```

#### Value

The numerical amount added to the stat.
Examples:

```text
+50 HP, +20 Damage, -40 Armor, ...
```

#### Source

Identifies where the modifier originated from.

Examples:

```text
Buff, Debuff, Skill, ...
```

This allows modifiers from the same source to be removed efficiently.

### Stat

`Stat` represents a single gameplay attribute.

Each stat consists of:

```csharp
float baseValue
List<StatModifier> modifiers
```

#### `GetValue()`

- Returns the current final value of the stat using `GetFinalValue()`.
- Uses lazy evaluation to avoid unnecessary recalculations.
- Recomputes only when modifiers or base value have changed.

#### `SetBaseValue(float baseValue)`

- Sets the base value of the stat.
- Marks the stat as dirty to trigger recalculation on next access.

#### `AddModifier(float value, string source)`

- Adds a new modifier to the stat.
- Each modifier is associated with a source identifier.
- Marks the stat for recalculation.

#### `RemoveModifier(string source)`

- Removes all modifiers that match the given source.
- Marks the stat for recalculation.

#### `GetFinalValue()`

- Internal calculation method.
- Iterates through all active modifiers and sums their values.
- Returns the computed final stat value.

The final stat value is calculated as:

```math
V_{final}=V_{base}+\sum_{i=1}^{n} M_i
```

### Stat Categories

#### Resource Stats

| Attribute | Default Value | Description |
|---|---|---|
| maxHP | 100 | Maximum Health |

#### Offensive Stats

| Attribute | Default Value | Description |
|---|---|---|
| damage | 50 | Base damage |
| critChance | 20% | Critical hit chance |
| critPower | 200% | Critical damage multiplier |

#### Defensive Stats

| Attribute | Default Value | Description |
|---|---|---|
| armor | 0 | Damage reduction factor |

#### Special Stats (In `PlayerStats` which implements `EntityStats` only)

| Attribute | Default Value | Description |
|---|---|---|
| skillEffectiveness | 100% | Multiplier applied to skill effects |

### Combat Calculations

All combat-related values are exposed through `EntityStats` getter methods. These methods serve as the single entry points for both offensive and defensive computations.

#### `GetMaxHP()`

- Returns the maximum health of the entity.

#### `GetDamage()`

- Returns the outgoing damage of the entity.

```math
outputDamage =
\begin{cases}
damage \times critPower, & \text{critical hit}\\
damage, & \text{normal hit}
\end{cases}
```

#### `GetArmorMitigation()`

- Returns the damage mitigation of the entity.

```math
damageMitigation = \frac{armor}{armor + 100}
```

#### `GetSkillEffect()` (In `PlayerStats` which implements `EntityStats` only)

- Returns the skill effect of the entity.

### AttributeUpgrade

`AttributeUpgrade` is the base class for all upgrade types. It defines the structure and behavior of an upgrade without enforcing specific implementation details.

#### Core Structure

```csharp
string name
PlayerStats stats
```

- `name` identifies the upgrade.
- `stats` provides access to the player's stat system.

#### `ApplyUpgrade()`

- Applies the upgrade effect by targeting specific stats inside the `EntityStats` system.

It first accesses the corresponding `Stat` instance from `PlayerStats`, then applies changes through the stat system.

```text
AttributeUpgrade -> PlayerStats -> Select Specific Stat -> Stat.AddModifier()
```

#### `GetDisplayEffects()`

- Returns UI-friendly data for displaying upgrade effects.

Return content:

```text
List<AttributeUpgradeDisplay>: [Attribute Upgrade 1, Attribute Upgrade 2, ..., ]
```

### AttributeUpgradeDisplay

`AttributeUpgradeDisplay` represents a single UI display entry for an upgrade effect.

#### Core Structure

```csharp
string attributeName
float value
bool isPercent
```

- `attributeName` represents the name of the affected attribute
- `value` is the numerical change value
- `isPercent` determines formatting style

#### `GetValueText()`

- Returns the formatted string used for UI display.

The value is formatted based on its type:

- If `isPercent = true`:

```text
+10%
```

- If `isPercent = false`:

```text
+10
```

### Upgrades Categories

- **Vitality**  
  - Increases survivability by enhancing health and defense.
  - Gives +20 Max HP and +40 Armor.

- **Strength**  
  - Focuses on raw physical power.
  - Gives +30 Damage.

- **Precision**  
  - Improves critical performance and consistency.
  - Gives +10% Crit Chance and +0.5 Crit Power.

- **Technique**  
  - Enhances overall skill efficiency and scaling.
  - Gives +0.4 Skill Effectiveness.

---

## Skill System

The player begins the game by selecting one of three core skills:

- Time Control

- Life Steal

- Phase

Each skill defines a distinct playstyle and has its own augment paths and scaling mechanics.

[Q] is used as the primary skill activation input.

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
| Stable Gain | Uncommon | Restores HP over time during the skill, but cannot heal after the skill ends |
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

---

## Enemy

All enemies use a finite state machine (FSM) architecture to control their behavior.

### General Battle behaviors

```text
Patrol:
Idle -> Move -> Idle -> Move -> Idle -> ...

Battle:
Detect player / Get attacked -> Battle
-> Attack / Retreat
-> Player not detected for a long period -> Idle
```

#### `Idle State`

- The enemy stays still for a short period.
- After the idle timer ends, it enters the `Move State`.

#### `Move State`

- The enemy patrols in its facing direction.
- While moving, it continuously checks:
  - Whether it reaches the edge of a platform.
  - Whether it hits a wall.
- If either condition is met, it turns around and enters the `Idle State`.

#### `Battle State`

- The enemy enters the `Battle State` when:
  - the player is detected.
  - the enemy is attacked by the player.
- While in battle, the enemy:
  - Continuously tracks the player's position.
  - Attacks the player when the attack conditions are met.
  - Retreats if the player gets too close.
- If the player is not detected for a long period, it returns to the `Idle State`.

### Specific Battle Behaviors

Once in battle, the enemy stops patrolling and executes its combat behavior. Different enemy types override this state to implement their own attack patterns while sharing the same state machine framework.

#### Skeleton

1. **Attack**
   
   - If the player is within the attack range, the skeleton first faces the player.
   - It then performs an attack.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**
   
   - If the player is outside the attack range, the skeleton moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.
    
#### Knight

1. **Attack**
   
   - If the player is within the attack range, the skeleton first faces the player.
   - It then performs an attack.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**
   
   - If the player is outside the attack range, the skeleton moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.
    
#### Evil Wizard

1. **Attack**

   - If the player is within the attack range, the Evil Wizard first faces the player.
   - It then releases a beam of fire in front of itself.
   - The fire performs multiple damage checks while active, allowing it to continuously damage the player.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**

   - If the player is outside the attack range, the Evil Wizard moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.
  
#### Slime

1. **Attack**
   
   - If the player is within the attack range, the skeleton first faces the player.
   - It then performs an attack.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**
   
   - If the player is outside the attack range, the skeleton moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.

3. **Split**

   - When the slime dies, it spawns multiple smaller slimes.

#### Archer Elf

1. **Attack**

   - If the player is within the attack range, the Archer Elf faces the player.
   - It then shoots an arrow at the player.
   - After attacking, it waits for the attack cooldown before shooting again.

2. **Retreat**

   - If the Archer Elf is not attacking, it moves away from the player to maintain a safe distance.
   - It continues retreating until one of the following conditions is met:
     - It reaches a wall.
     - It reaches the edge of a platform.
     - It has created enough distance from the player.

#### Mage

1. **Attack**
   
   - If the player is within the attack range, the skeleton first faces the player.
   - It then performs an attack.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**
   
   - If the player is outside the attack range, the skeleton moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.

3. **Retreat and Spell Cast**

   - At regular intervals, the mage checks the distance to the player.
   - If the player is too close, the mage quickly retreats.
   - It continues retreating until one of the following conditions is met:
     - It reaches a wall.
     - It reaches the edge of a platform.
     - It has retreated a sufficient distance from the player.
   - The mage then immediately casts a spell.
   - The spell launches three magic projectiles that travel in parabolic trajectories toward the player.

#### Skeleton King

**Phase 1 (Health > 50%)**

1. **Attack**
   
   - If the player is within the attack range, the skeleton first faces the player.
   - It then performs an attack.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**
   
   - If the player is outside the attack range, the skeleton moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.

**Phase 2 (Health ≤ 50%)**

1. **Summon Skeletons**

   - The Skeleton King stops moving.
   - At regular intervals, it summons three small skeletons.

#### Dark Knight

1. **Attack**

   - If the player is within the attack range, the Dark Knight performs one of two attacks.
   - The attack pattern follows a fixed cycle:
     - Perform a sword slash while releasing a sword wave.
     - Perform another sword slash while releasing a sword wave.
     - Dash toward the player.
   - During the dash, the Dark Knight continues moving until one of the following conditions is met:
     - It reaches the player.
     - It collides with a wall.
     - It reaches the maximum dash distance.
   - After the dash ends, it immediately performs an attack.
   - After each attack, it waits for the attack cooldown before executing the next attack in the cycle.

2. **Chase**

   - If the player is outside the attack range, the Dark Knight moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.

#### Necromancer

1. **Attack**

   - If the player is within the attack range, the Necromancer first faces the player.
   - It then releases a beam of fire in front of itself.
   - After attacking, it waits for the attack cooldown before attacking again.

2. **Chase**

   - If the player is outside the attack range, the Necromancer moves toward the player.
   - It continues moving until one of the following conditions is met:
     - The player enters the attack range.
     - A wall blocks its movement.

3. **Cast Homing Projectiles**

   - At regular intervals, the Necromancer casts a spell.
   - The spell launches three homing magic projectiles.
   - Each projectile continuously tracks the player's position until it hits the player or is destroyed.

#### Boss



The boss is confined to a fixed combat area throughout the battle.

- The boss cannot leave this designated area.
- All movement abilities, including walking, dashing, and teleporting, are restricted to the combat area.
- As a result, the boss does not need to avoid walls or platform edges during combat.

1. **Attack and Chase**

   - If the player is within the attack range, the boss performs an attack.
   - If the player is outside the attack range but within the dash range, the boss moves toward the player until the player enters the attack range, then performs anattack.
   - If the player is outside the dash range, the boss dashes toward the player.
   - The dash ends when one of the following conditions is met:
     - The boss reaches the player.
     - The maximum dash distance is reached.
   - After the dash ends, the boss immediately performs an attack.

2. **Special Attack**

   - At regular intervals, the boss performs a special attack.
   - One of the following attacks is selected at random:
     - **Teleport Strike**
       - Teleports behind the player.
       - Immediately performs a melee attack.
     - **Skeleton Summon**
       - Summons three skeletons.
       - The summoned skeletons fight independently using the normal skeleton battle behavior
     - **Magic Attack**
       - Teleports to a position far away from the player.
       - Then performs one of the following attacks at random:
         - Summons three lightning spells above the player's position.
         - Releases an energy wave toward the player.

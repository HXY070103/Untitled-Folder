## Character Controller

The player controller is implemented using a finite state machine (FSM) architecture.

### Movement

| Key | Action |
|---|---|
| [A] / [D] | Move left / right |
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

## State Machine

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

# PizzaPalooza Development Checklist

Use this as a working document during development and playtesting.

Status keys for tracker tables:
- `TODO` = not started
- `DOING` = in progress
- `DONE` = complete
- `BLOCKED` = waiting on something

## Quick Pass (Course Requirements)

Complete these four items first:

- [x] Start screen includes game name, team logo, and navigation buttons
- [ ] Three levels/maps exist and progression between maps is logical
- [ ] Game over screen includes both `Main Menu` and `Restart Level`
- [ ] Learning is assessed in each level via a challenge/boss mechanism

## Build Order (Recommended)

1. Core loop and controls
2. Start screen and navigation
3. Level 1 end-to-end
4. Level 2 end-to-end
5. Level 3 end-to-end
6. Game over and completion flow
7. Scoring, feedback, polish, and testing

## Phase 1: Core and UI

- [x] `CORE-01` Implement full loop: take order -> assemble -> bake -> deliver -> score
- [x] `CORE-02` Ensure first-person interactions feel tactile (grab/place/cook)
- [x] `CORE-03` Add increasing time pressure over shift
- [x] `CORE-04` Add customer patience system affecting score/tips
- [x] `UI-01` Show game title `Pizza-Palooza` and team identity `IdolMinds`
- [x] `UI-02` Add `Play`, `How to Play`, `Settings`, `Quit` buttons
- [x] `UI-03` Ensure navigation works via mouse and keyboard

## Phase 2: Maps and Progression

### Level 1 (Training Shift)
- [ ] `MAP-01` Build map (small kitchen, low complexity)
- [ ] `MAP-02` Implement basic recipes (cheese, pepperoni)
- [ ] `MAP-03` Teach sequence: dough -> sauce -> toppings -> oven -> serve

### Level 2 (Lunch Rush)
- [ ] `MAP-04` Build map (larger layout, tighter movement)
- [ ] `MAP-05` Add overlapping orders and stronger oven timing pressure
- [ ] `MAP-06` Add mild restocking requirements

### Level 3 (Dinner Chaos)
- [ ] `MAP-07` Build map (highest complexity)
- [ ] `MAP-08` Add specialty and combination orders with high frequency

### Progression Logic
- [ ] `MAP-09` Unlock maps using clear score/rating thresholds

## Phase 3: Fail, Assess, Score

### Game Over Flow
- [ ] `GO-01` Trigger fail state reliably (burns, misses, low patience, time)
- [ ] `GO-02` Add `Restart Level` button (required)
- [ ] `GO-03` Add `Main Menu` button (required)
- [ ] `GO-04` Show reason for failure and key stats

### Learning Assessment
- [ ] `ASSESS-01` Define one learning objective per level (required)
- [ ] `ASSESS-02` Add Level 1 challenge/boss (required)
- [ ] `ASSESS-03` Add Level 2 challenge/boss (required)
- [ ] `ASSESS-04` Add Level 3 challenge/boss (required)
- [ ] `ASSESS-05` Show pass/fail criteria before challenge
- [ ] `ASSESS-06` Provide post-challenge improvement feedback

### Scoring and Feedback
- [ ] `SCORE-01` Score includes speed, accuracy, cook quality, customer mood
- [ ] `SCORE-02` Show instant mistake feedback (wrong topping, late, burnt)
- [ ] `SCORE-03` Add end-of-level report card by category

## Phase 4: Flow, Polish, Test

### Session Flow
- [ ] `FLOW-01` Level complete screen with `Next`, `Retry`, `Main Menu`
- [ ] `FLOW-02` Final completion screen after Level 3
- [ ] `FLOW-03` Save progression state (minimum: current play session)

### Polish
- [ ] `POLISH-01` Add light comedic reactions/events
- [ ] `POLISH-02` Add audio cues (new order, oven done, angry customer)

### Testing
- [ ] `TEST-01` Start screen works from fresh launch
- [ ] `TEST-02` All 3 levels completable with no soft locks
- [ ] `TEST-03` Game over actions always route correctly
- [ ] `TEST-04` Assessment/boss triggers and evaluates in all levels
- [ ] `TEST-05` Playtest for fairness and pacing

## Level Learning Targets

| Level | Learning Objective | Assessment/Boss Check | Pass Condition |
|-------|--------------------|-----------------------|----------------|
| 1 - Training Shift | Correct pizza sequence and station familiarity | 5 timed basic orders | 80%+ order accuracy and 0 burnt pizzas |
| 2 - Lunch Rush | Multitasking and prioritization under pressure | Rush wave with overlapping orders | Keep customer rating above threshold and hit speed target |
| 3 - Dinner Chaos | Full workflow mastery in high complexity | Peak-hour survival with specialty combos | Meet minimum score, accuracy, and patience thresholds |

## Compact Progress Tracker

Use this if your team wants one table for standups.

| ID        | Task (Short)           | Owner | Status | Notes    |
|-----------|------------------------|-------|--------|----------|
| CORE-01   | Core loop complete     |       | DONE   | Automated setup/scripts added |
| UI-01     | Start screen complete  |       | DONE   | Menu + HUD builders available |
| MAP-01    | Level 1 complete       |       | TODO   |          |
| MAP-04    | Level 2 complete       |       | TODO   |          |
| MAP-07    | Level 3 complete       |       | TODO   |          |
| GO-02     | Restart button added   |       | TODO   | Required |
| GO-03     | Main menu button added |       | TODO   | Required |
| ASSESS-02 | Level 1 assessment     |       | TODO   | Required |
| ASSESS-03 | Level 2 assessment     |       | TODO   | Required |
| ASSESS-04 | Level 3 assessment     |       | TODO   | Required |
| TEST-02   | All levels verified    |       | TODO   |          |

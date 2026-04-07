# Game Requirement Document

## 1. Project Overview
- Name:Pizza Palooza
- Core concept: Simulate pizza creation.
- Player goal: Create pizza and satisfy customers
- Core gameplay loop: Wait for a customer, prepare the pizza, deliver the pizza. Repeat until stage ends

---

## 4. TODO (WHAT NEEDS TO BE DONE)
## 4.1 - Level 1
Customers:
- Implement customers joining and making a line to get the pizza (all T posing, no animations, movement made with Navmesh)
- Every model will have an sprite 2d white box above its head (always facing user). That sprite will have a mood emoji at left side, and satisfaction at right side
- The mood will be a sprite renderer image, that changes between 3 different emojis (calm when patience above 50%, bored when between 50% and 25%, angry when below 25%)
- When customer gets the pizza, it will have its emoji changed to happy and leave with the pizza above its head, but below its mood sprite. (child element)
- The customers will come from outside at left side (where their spawn is not visible), and leave at the same destination 
- Each customer will have its own patience meter, but a single script will control all of the customers with pool object recycling
- Up to (configurable, default 3) customers will be waiting in front of desk. When they are waiting, their order will be displayed in a window (flying canvas) at a position on their chest level
- There will be 3 empty game object that will set where the customers can wait
- For each customer waiting, there will be a window with their order. To deliver the pizza, press interact when aiming at the customer. If the order is correct, it will blink green, and red if not. After blinks green, customer leaves

UI:
- Make the top bar more visible. Black background, white letters, bigger font
- Remove side order list
- When you can interact with something, display with a textmeshpro label on the center of the screen the name of the interaction (like deliver pizza, get tomatoes, get cheese)

Gameplay
-Have each ingredient box with a texture representing what is it about
-Make dough be a placeable element that can be picked from one of the boxes, up to three doughs.
-Every time the user adds a element to the pizza, it updates the prefab
-You can put ingredients in the trash bin (create and implement, it can be a black box on the ground). Or even a mouse with navmesh and tpose catching it and running away.
-Make the stove have a timer. The prefab disappears when its inside, but when its ready you interact with the stove and get the prefab.
-To get the pizza from stove, the order is a FIFO (first in, last out). There will be a timer for each active cooking pizza in a transparent canvas above the stove (horizontal view for each entry)
-When you deliver the pizza, the model given to them will be just a closed pizza box on their heads, and the window of that order will be closed.
-The satisfaction percentage will be calculated on the formula "Satisfied customers/total customers"


## 5. Current bugs

---

## 5. Unclear / Needs Definition
- 
-

---

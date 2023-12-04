# Circum-Celeste

### WHAT
This is the source for the small-scope mobile game Circum Celeste. The core game was built and published over the course of 2 months from September to November 2021, with the (no longer supported) challenge levels being added in Spring 2022.


### WHY
For me, the purpose of this project was to take a project from start to finish as a solo developer as well as a means to learn more about the art and design side of game development.

While developing a PID controller for a professional application, I noticed that it was idly amusing to get the PID controlled object to orbit its target. I wanted to gamify this mechanic by using this to collect pickups, smash into things and carefully avoid hazards.

To keep the player on their toes, I also made it so that the orbiter damages the player if they collide meaning that the player must keep the orbiter orbiting in one direction at all times - I've had a lot of feedback that this is too tricky to start with. Although I was adamant about keeping it, I don't think I ever resolved this legitimate issue through the design.

Regardless, I am happy with the set of features this small-scope game has, the ability to watch replays of yours and the highscorers levels is pretty cool. I am also happy with a lot of the visual effects, interactions and "juice". It's fulfilled its purpose regardless of the fact that it was never a commercial success (which would have been surprising since I stripped the monetisation!).

### HOW
Supported by Unity version 2020.3.18f1. Use the "Play from Boot" button to start the game in the editor.

To create a level, create a "level layout" in the project.
The elements that a cell can be are as follows (in order of clicking):
BLOCK - these are walls that block the player (but not the orbiter)
PICKUP - these can be scattered for fun or are used in the "picked up all" escape criteria.
BLACK HOLE - these gravitate towards the player star and cause harm, you can hit them away with the orbiter.
WORM HOLE - this is the exit that appears once the escape criteria is met, get to this to complete the level.
PLAYER START - where the player starts.
PULSAR - these are hazards, your orbiter must carefully navigate around these.
beam enemy - unsupported.

To play the level you've created, create a "level flow" asset and add your layout to the layouts array. Then set this asset as the editor level progression in the LevelProvider in the game scene: ![image](https://github.com/jackmw94/Circum-Celeste/assets/22613988/64c04203-927c-43ce-8758-5214f0c645fc). Then click the play from boot button to try it out.

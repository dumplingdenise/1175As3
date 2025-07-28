Game Description:
This is a 2D top down shooter game where players can choose their character and weapons before starting the game to defeat the enemy.

Player Control:
WASD
Arrow Keys
Left Click to shoot 

Task Attempted:

Character System - Denise

Compulsory Task:
- Fields are initialized from CSV file
- Character class have a minimum of 2 fully implemented items (Green Alien & Yellow Alien)
- Player can pick a character to use during gameplay with different statistic (Shumin)
- Carries the selected character data to game via SelectedCharacterManager script (scene-to-scene data persistence)
- Selected character's sprite & stats applied to player in player script

Additional Task:
- Animation override based on character (idle/run swaps dynamically base on selected character in game)

scripts:
Characters
Player
Selected Character Manager

Dialogue System (Denise)
The dialogue system is being triggered at the tutorial scene where player interacts with the NPCs to guide them on the controls.

Compulsory Task:
- Implemented dialogue system with at least 2 different convo during tutorial
- Display the name of the speaker with a minimum of 3 lines each
- Pressing "E" to trigger the dialogue & Next Button to move on to the next line

Bonus Task:
- Sound effects to be played during dialogue lines
- Typewriter effect for spoken lines. Player must be able to skip to the end of the line with a click while
the effect is playing, and only move on to the next line if the player clicks after the effect has finished
playing. (Press Next Button to skip to the end of the line)

Additional Task:
- Displays of speaker image portrait

scripts:
Dialogue Manager
Dialogue
IInteractable
NPC
Tutorial Character Movement
Tutorial Player Interact
Tutorial Bullet
Tutorial Shoot

Weapon Class (compulsory) - KaiRu

scripts:
Weapon Loader
Player Weapon - Denise
Bullets
Weapon Selection

Enemy & Enemy Wave Class (compulsory) - Nomitha

scripts:
Wave Manager
Enemy Data
Enemy Controller
Data Manager
Enemy to Spawn

Dynamic Data System (compulsory) - ShuMin

scripts:
Dynamic Data

Save & Load (bonus) - ShuMin

scripts:
Statistics Display Manager

UI (compulsory) - ShuMin

scripts:
GameUIManager
Game Over Ui Manager
Menu Manager

Assests
https://www.kenney.nl/assets/alien-ufo-pack 
https://www.kenney.nl/assets/space-shooter-extension 

Game Description:
This is a 2D top down shooter game where players can choose their character and weapons before starting the game to defeat the enemy.

Player Control:
WASD
Arrow Keys
Left Click to shoot 

==========================================================================

Task Attempted:

Character System - Denise

Compulsory Task (Denise):
- Character class with stats
Fields are initialized from CSV file
- At least 2 fully implemented items (Green Alien & Yellow Alien)
- Player can pick a character to use during gameplay with different statistic (Shumin)
- Carries the selected character data to game via SelectedCharacterManager script (scene-to-scene data persistence)
- Selected character's sprite & stats applied to player in player script

Additional Task (Denise):
- Animation override based on character (idle/run/hurt swaps dynamically base on selected character in game)

scripts:
Characters.cs
Player.cs
SelectedCharacterManager.cs

==========================================================================

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
DialogueManager.cs
Dialogue.cs
IInteractable.cs
NPC.cs
TutorialCharMovement.cs
TutorialPlayerInteract.cs
TutorialBullet.cs
TutorialShoot.cs

==========================================================================

Weapon Class (compulsory) - KaiRu

scripts:
WeaponLoader.cs
PlayerWeapon.cs (Denise)
SelectedWeapon.cs (Denise)
Bullets.cs
WeaponSelection.cs

==========================================================================

Enemy & Enemy Wave Class (compulsory) - Nomitha

scripts:
WaveManager.cs
EnemyData.cs
EnemyController.cs
DataManager.cs
EnemytoSpawn.cs

==========================================================================

Dynamic Data System (ShuMin)

Compulsory task:
- Track at least 2 stats (enemiesDefeated, wavesCompleted, totalDistanceTraveled)
- Data persists across scene
- data displayed in-game (character's health & armor rating)
- Data saved to extern file (json file)
- Past log data preserved

Bonus task:
- Save/load system

scripts:
DynamicDataManager.cs
StatisticsDisplayManager.cs

==========================================================================

UI (ShuMin)

Compulsory task:
- Game Menu UI
- Game Over UI
- Game Pause UI
- Achievement UI

Bonus task:
- Character selection scalable UI

scripts:
GameUIManager.cs
GameOverUiManager.cs
MenuManager.cs
CharacterSelectionManager.cs

==========================================================================
List of Bugs :(

Bug Log: Enemies Not Spawning After Restart/Re-entering Game

Description:
When the game is restarted (either directly from a "Restart" button in-game 
or by returning to the main menu and starting a new game session), enemies fail to spawn. 
The WaveManager reports that its 'Enemy Prefab' is not assigned.

Symptoms:
- No enemies appear in the game world after initiating a new game session following a previous one.
- Debug console shows the error: "[WaveManager] Enemy Prefab is not assigned in the Inspector! Cannot spawn enemies."

Technical Details:
The 'enemyPrefab' field on the persistent 'WaveManager' script (located on the GameManager GameObject) 
appears to become null after a scene reload, despite being correctly assigned in the Inspector on initial game launch.

Location of Error:
WaveManager:SpawnEnemiesInWave (Assets/Scripts/Enemies/WaveManager.cs:254)

How to avoid (or workaround):
Avoidance/Workaround:
For persistent (DontDestroyOnLoad) singletons, explicitly store critical Inspector references 
in private variables during the initial 'Awake' or 'Start' call. Then, re-assign the public fields from 
these private variables within any reset/initialization method (like ResetWaveManager()) that runs when a new game session begins.

*Sorry prof i tried to fix this issue for the person doing this part but i cant, i tried my best :(

==========================================================================
Assests
https://www.kenney.nl/assets/alien-ufo-pack 
https://www.kenney.nl/assets/space-shooter-extension 

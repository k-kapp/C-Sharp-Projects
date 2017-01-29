#Sokoban

A Sokoban clone. The user must push all boxes onto the dots. This version that I wrote can take customized maps (puzzles) to play on. The user may design puzzles him/herself, after which they go into the "reserved" pool of puzzles. Only the "active" pool of puzzles are played. The reserved and active pools may be modified via the puzzle selector (from the main menu) shown in the fourth screenshot below.

A custom API was written to facilitate the creation of a GUI for the game. This included creating classes for buttons, forms, etc. This was done using the graphics functionality provided by the XNA framework (through Monogame).

<p align="center">
<img src="ScreenshotGame.png" width="550">
</p>

<p align="center">
<img src="ScreenshotGameMenu.png" width="550">
</p>

<p align="center">
<img src="ScreenshotDesigner.png" width="550">
</p>

<p align="center">
<img src="ScreenshotSelector.png" width="550">
</p>


##TODO

+ In puzzle editor, add functionality for:
 - specifying the grid size

+ Fix bug where the game throws a null reference exception if no puzzles were selected before play starts
 - Add a warning instead, then direct player to puzzle selector

+ Fix bug where mouse position is slightly out of line for buttons to activate

+ Add functionality for deletion of puzzles

+ Add functionality for active puzzles to stay active even after game was exited

+ Find new color scheme (this ugly yellow won't do!)

+ Cleanup code (make naming more consistent, relocate certain functions/methods to more sensible locations, etc.)

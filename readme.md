# AR Domino Game

This project is class project for UCSB CS291 class taught by Prof. Misha Sra.
Teammates: Huake He, Zihao Zhang, Boning Dong

## Introduction
Domino is a kind of tile-based game played with Domino tile pieces. Players need to place the tiles one by one with a small distance in between to form a long line, and eventually trigger the Domino tiles to start toppling by pushing the first tile. 

In our AR Domino project, we are trying to bring this Domino gaming experience into the AR world. In our memory, the most fun part of playing Domino is placing the tiles one by one and it's really satisfying to see that the tiles topple in a row. So in this project, we were trying hard to replicate this experience. 

Players can place virtual Domino tiles in the game, place other virtual game objects as platforms on which domino tiles can be placed, and trigger the domino
objects to topple. In addition to interacting with the virtual objects, players can also interact with real-world objects, such as placing virtual objects on top of the detected real-world objects.

## UI Elements
![GUI](https://github.com/boningdong/AR-Domino/Images/gui.png)
The above figure exhibits the user interface layout of our app. 
* In the center of the screen, a semi-transparent place indicator is presented to give users a concrete view on where the domino will be placed. 
* In the bottom-right corner, a rotation ring is used for adjusting the face direction of the domino.
* By tapping the place button, users can place the domino on the indicator mark. 
* In the bottom-left corner, there is an object selection scroll view, which allow users to select virtual objects including domino, cube box, and cuboid box for placement and interaction. 
* Users can tap the restart button to remove all of the placed virtual objects.
* Users can tap reset button to reset all of the toppled dominoes. 
* If users want to remove several but not all objects, they can tap and select the intended objects, and make the deletion.

## Development
If you want to continue your work based on our current progress, here are several things you should notice.
* DraggingAndDroppingMultiple is the scene you should look at.
* To add more objects that you can place, simply create prefabs and add them to the Inventory component on the GameState object in the scene.
* You can also give specific icons to display along with the object buttons.
* To make sure the objects you created are interactive, please make sure they have the correct scripts attached. (refer to other prefabs under the Prefabs folder)

# Zayker-Game-Engine
This, so far unnamed, engine has one main goal: Be as developer friendly as possible. This however, does not mean it's not meant for artists as well. 
What we are trying to achieve, is making the engine as open and editable as possible. 
Want to change something in the physics code? Sure, just edit it.
Need to render something in a very special way, that is not possible with this engine? No big deal, just get the OpenGl refernece from the engine and draw it that way.
Basicaly, the goal is to remove any "Ohh, the engine can't do that?", by making it easy to change any internal code you might want to change in your project. 

# Quick Documentation
This is just a short introduction on how the core of the engine works. A bigger documentation will be added using Githubs wiki feature. 

## Creating A Project
At the moment, the only way to create a project, is to copy the "Sandbox"-Project. (This will change soon).
One you copied the folder, run the engine and click "Project/Open" and enter the path to your project. 

## Working with the engine
Right now, the only thing the engines ui actually does, is module-manageent. After openeing a project, click on "Tools/Module Manager" to open the module manager.
There you can enable/disable which modules you want to include in your project. After making changes, hit "Project/Save" and "Project/Reimport Everything".
Now the modules are included in your project. To work with it, open the project with visual studio. 
In visual studio code, you will see a "Assets"-Folder. This should be the place where all your games code, models, textures, etc. goes.
The Game.cs file acts as your main entry point. The "Start" method is called once when the game starts and the update method is called once every frame.
To run/build your game, just use the Visual-Studio as in any other C# project.

## Editing the souce-code
Appart from the "Assets"-Folder, there is an "Engine"-Folder in your project. This contains all code provided by the engine. While I only recomend to edit
the content of the "Modules"-Folder, theoretly you can also change any other files, like the math library. 
Inside the "Modules"-Folder, you will find all included modules, as set in the engines Module-Manager. Making changes here only applies to this project, so
feel free to change it to your needs.
Be carefull though! When you hit "Reimport Everything" in the engine, all your changes will be overwritten!
I will implement some system to a) only reimport indivdual modules and b) to add a warning. For now, just be carefull!

## Modules
A module is a block of code and other data, containing engine features. Examples could be a "Physics"-Module, adding simulation and collision. 
Some modules depend on others and there is no system for dependencies yet, so be carefull and keep that in mind if there are errors.

### Renderer
The renderer currently only supports unlit .obj files and primitive shapes. This will be expanded in the future.

#### Worldspace
Forward: Positive Z-Axis (0, 0, 1)
Right: Positive X-Axis (1, 0, 0)
Up: Positive Y-Axis (0, 1, 0)

#### Screenspace
Currently the screenspace coordinates go from (-1, -1) (bottom left) to (1,1) (top right). This might change to (0,0) -> (1,1).
This means a screenspace object of the size (1,1) takes up a quater of the screen, which might be unintuitive. 

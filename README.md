# Zayker-Game-Engine
Main descibtion of the engine

# Goals
Create simle to use and modular engine, that still gives developers the option to access low-level code. 

# Recomended VS Extensions
- GLSL language integration

# Quick Documentation
This is just a short introduction on how the core of the engine works. A bigger documentation will be added using Githubs wiki feature. 

## Modules
A module is a block of code and other data, containing engine features. Examples could be a "Physics"-Module, adding simulation and collision. 

### Module.IsEnabled
Modules have a main loop. In the rendering module for example, this handles redrawing every frame. 
This update loop is only called on modules which are enabled. When enabeling a module, it will also be initialized.

### Including Modules In Build
Modules are not included in a project by default. This minimized build time, size and improves performance. To use a module in you project, include the module id in the project.meta. Later on, there will be a module editor, that provides a visual interface for including/excluding modules. 

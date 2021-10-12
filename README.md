# General Moore-Neighborhood 2D Cellular Automata Simulator
The capabilities of the program are shown off in a Youtube video here: 
The code is meant to be used in conjunction with Unity Game Engine, especially its "inspector". 

## 2D Cellular Automata
Cellular automata are a large family of systems, where individual discrete objects, known as cells, each follow a set of usually simple instructions. The observed behaviour of such a system set in "motion" can often be extremely complex and/or chaotic. 

While cellular automata are not bound by how exactly each cell gets their inputs to act on, nor what kind of instructions each cell executes, then the code at hand only deals with cellular automata that can be layed out as square pixels on a 2-dimensional surface. The code also only concerns one exact type of input scheme - the so called Moore Neighborhood. In addition the "size/diameter" of the Moore neighborhood is exactly 1. The code also restricts the kinds of instructions that can be given. Namely, one can only define the exact number of neighboring cells that need to be "alive" for a cell to be born or survive. All other cases assume that the cell will die, or remains dead if already so. 

The instructions to these specific types of cellular automata are often denoted as a string, such as B24/S35, meaning: if there are exactly 2 or 4 neighboring cells alive when the current cell is alive, then the current cell becomes alive. Or, if the current cell is already alive and exactly 3 or 5 neighboring cells are alive, then the current cell remains alive. In all other cases, the current cell would transition to dead state or remain in the dead state, if it was so to begin with. A valid rulestring for the code at hand is for example "B3S23" (this particular rulestring is also known as "Conway's Game of Life"). 

Given those restrictions, the code at hand is able to simulate any valid set of "instructions". The code uses Compute Shaders written in HLSL and executed on a computer's GPU to vastly speed up the process of simulating such cellular automata when compared to a CPU-execution based simulator. It is important to note, however, that more efficient algorithms exist for simulating 2D cellular automata, and as such, it is not fair to say that this GPU variant will always be faster than a CPU-based algorithm. 

## How to use... roughly
1. Import the code to a Unity Game Engine project
2. Attach Dispatcher.cs to the Main Camera in the scene
3. Drag and drop GOL.compute, Colorer.compute and Viewport.compute to the public ComputeShader fields of the Dispatcher script
3. Create an empty GameObject
4. Attach AnimationFlow.cs to the newly created object
5. Drag and drop the Main Camera GameObject to the public GameObject field of the AnimationFlow script
6. Import some initial texture(s) to your project - the textures need to be squares (eg. 512x512 pixels) and the side needs to be a power of 2. A "dead" cell is one that has 0 in its alpha channel of the pixel, and an "alive" cell is one that has a value in the alpha channel of the pixel
7. Drag and drop the wanted texture into the public Texture2D field of the Dispatcher script. 
8. Run the game while keeping the Unity's "inspector" open, in order to change any simulation parameters on runtime. 

## Animating
The AnimationFlow.cs script can be used to "write/define" a sequence of simulation parameter changes at exact moments in time. 

If anyone reading this wants to use this code for actually making an animation, and thus wants me to write an actual documentation with the capabilities and the way to use the AnimationFlow, then please scream at me in the Youtube comments section. 
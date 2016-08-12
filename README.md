# marching-squares

Executable demo of 2d deformation project with using marching squares algorithm

This demo uses the marching squares algorithm to transform an array of topographical data into a mesh that can be rendered and collided with.
It represents physical bodies using an internal marching squares grid, and an external convex hull. Smaller bodies interact with the concave inner geometries of larger bodies, and the convex outer hulls of bodies they are a similar size to.

Controls
WSAD - When close to planet - W/S - Move the player up or down. A/D - Move player left or right.
       When distant from planet - W/s - Move the player forwards and backwards. A/D - Rotate the player counterclockwise and clockwise.
Space - Dig.
Shift - Dig a large area. May be slow on portable devices.

IKJL - Controls celestial body the player is standing on. I/K - Apply an upwards force, a downwards force. J/L - rotate counterclockwise or clockwise. This allows the player to 'pilot' the object they are standing on, if object is small enough.



Build Instructions:
Download project folder.
Open the folder via Unity 5.3.3 or later.
The application can then be run inside the Unity IDE, or exported to the platform of choice via the Unity build menu.
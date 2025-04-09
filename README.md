# Mesh Generation Demo
This repository contains a Unity project with two scenes that demonstrate the capabilities of algorithmic 3D mesh generation. It includes source files from [OpenSimplex2](https://github.com/KdotJPG/OpenSimplex2) and [Noisy-Nodes](https://github.com/JimmyCushnie/Noisy-Nodes).

## Fractal noise
The procedural generation in this project is based on 2 different fractal noise implementations, one based on OpenSimplex2's noise functions and implemented in C# (`NoiseMap.cs`), and another one based on Noisy-Nodes' and implemented in HLSL (`PlanetGeneration.compute`). They both share these parameters: 
- **Amplitude:** intensity of the noise
- **Frequency:** determines the scaling of the noise
- **Octaves:** number of iterations of Perlin noise to be layered on top of each other
- **Persistence:** amplitude of an octave relative to the previous one
- **Lacunarity:** frequency of an octave relative to the previous one

By overlaying noise octaves of different amplitudes and frequencies, it is possible to achieve more natural-looking patterns.

## Planet Demo
The scene contains two planets generated procedurally through the `PlanetTerrain` script. This script generates subdivides an octahedron to generate a spherical mesh, which is then extruded and warped by calling the `PlanetGeneration` compute shader.

The planets react to the point light in the scene through a custom shader, and the parameters for terrain generation can be altered using the `PlanetTerrain` editor in the inspector tab.

### Editor options
- **Subdividions:** number of vertices to be placed on each edge of an octahedron to generate the sphere mesh.
- **Radius:** base radius of the sphere
- **Noise offset:** origin point of the 3D noise
- **Mountain noise:** 3D fractal noise that generates peaks and valleys on the planet's surface
- **Warping noise:** 3D fractal noise that distorts the sampling of the mountain noise, causing warping of the planet's features

## Terrain Demo
This `TerrainManager` object in this scene generates a simple 3D terrain. A script with the same name has been attached to it, and its parameters can be modified on the inspector tab.

The script instantiates an array of `GameObject`s with the `Chunk` script, which generates terrain based on a height map which is converted into a 3D scalar field and fed into the [marching cubes algorithm](https://paulbourke.net/geometry/polygonise/) to generate the final mesh. The parameters for the noise are held in the `TerrainManager` instance, which is statically accessed by each chunk.

### Editor options
- **Chunk Grid Size:** width of the square grid of chunks
- **Chunk Width:** width of each individual chunk
- **Chunk Height:** height of each chunk, _not_ of the terrain contained within its bounds
- **Seed:** seed for the noise
- **Terrain Noise:** 2D fractal noise that generates the height map
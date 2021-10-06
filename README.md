# primitive-mesh-creator
Primitive Mesh Creator is a tool for creating common primitive meshes directly in the Unity 3D engine. The tool offers an expanded set of common primitive shapes (Pyramid, Polyhedra, Triangle, etc) right alongside the default Primitive Objects from Unity (Cube, Sphere, etc.) It also includes a number of tools for creating custom shapes through a simple API or directly in the Unity Editor itself. The tool is also built to extensibly support new shapes with relative ease.

## Features
* Access an extended set of 3D Objects directly in the Unity Toolbar (**Game Object > 3D Object > Primitive Creator**)
* Adds an extended set of 3D Collider components directly in the Add Component menu (**Add Component > Primitive Creator**)
* Create a custom mesh from within the Scene view through the Primitive Creator Component (**Game Object > 3D Object > Primitive Creator Component**)
* Generate a custom mesh within your scripts through the PrimitiveCreatorUtility
* Build a custom mesh type by writing up a custom Mesh Creator script

## Motivations
This package strives to provide a simple extension of Unity’s default set of primitive meshes, without involving more extensive 3D modelling software (Blender, Maya) or a more feature-rich level creation package within Unity (ProBuilder, UModeler). In particular, the package strives to be:
* **Simple to Use**
  - Expanded offerings of default primitives (Polyhedra, Pyramid, etc) are immediately available from within the Editor (**Game Object > 3D Object > Primitive Creator**)
* **Customizable**
  - Create and save a custom mesh from a number of shapes to fit your particular needs, directly from within the Scene view (see **Primitive Creator Component**)
* **Extensible**
  - The package includes a streamlined approach to extending the included set of Mesh Creators, allowing for you to create your own custom procedural mesh creator
* **Standalone**
  - The package uses minimal dependencies or intricate integrations within Unity’s Editor, making it easy to import, make the mesh(es) you need, and remove it if so desired

## Limitations
* Meshes generated within a build can be stored in memory, but not written to file at the moment.
* Meshes bound by the Unit Sphere actually operate with a fixed radius of 0.5. This is to better adhere to Unity’s standard of 1m3 for a shape (1m x 1m x 1m). If you wish to bind a shape by a Unit Sphere with radius of 1, set the scale to (2,2,2) to accomplish the same function.
* The provided set of Colliders are extensions of MeshColliders. As such, these are less performant than Unity’s default PrimitiveColliders (Box, Capsule, Sphere) and should only be used when the standard PrimitiveColliders are not sufficient for your needs. However, they are the smallest possible vertex count that still provides for a viable mesh of that type.
* Primitive Creator Component utilizes the [SerializeReference], which was introduced in Unity 2019.3. Testing has not yet been done for earlier versions of Unity (see [here](https://docs.unity3d.com/2019.3/Documentation/ScriptReference/SerializeReference.html) for more).

## Getting Started
TODO: fill this out

## Usage
TODO: Fill this out

## Contributing
TODO: Fill this out

## License
TODO: Fill this out

# Primitive Mesh Creator
An extended set of primitive mesh shapes for Unity, with in-editor and scripting support.

Requires Unity 2019.3 or later.

## Shapes
| Shape | Type |
|---|---|
| Quad | 2D |
| Triangle | 2D |
| Polygon | 2D (n-gon) |
| Cube | 3D |
| Pyramid | 3D |
| Tetrahedron (d4) | Platonic solid |
| Hexahedron (d6) | Platonic solid |
| Octahedron (d8) | Platonic solid |
| Sphere (UV) | 3D |

## Toolbar
**GameObject > 3D Object > Primitive Creator**: creates a GameObject with mesh, renderer, and collider.

## Scripting API

```csharp
using PrimitiveCreator;
using static PrimitiveCreator.PrimitiveCreatorUtility;

// Default mesh
Mesh mesh = PrimitiveCreatorUtility.CreateMesh(MeshType.Tetrahedron);

// Custom vertex count
MeshCreator.MeshDetails details = PrimitiveCreatorUtility.GetMeshDetails(MeshType.SphereUV);
details.vertexCount = 200;
Mesh sphere = PrimitiveCreatorUtility.CreateMesh(MeshType.SphereUV, details);

// Full GameObject (mesh + renderer + collider)
GameObject obj = PrimitiveCreatorUtility.CreatePrimitive(MeshType.Pyramid);
```

## In-Editor Customization
Add a **Primitive Creator Component** via **GameObject > 3D Object > Primitive Creator Component** to adjust mesh type and vertex count in the Scene view and save the result as an asset.

## Extending
Subclass `MeshCreator` and implement `CreateMesh`, `CreateColliderMesh`, and `GetClosestViableVertexCount`. The utility will discover it automatically via reflection.

## License
MIT (see [LICENSE](../../LICENSE))

# Changelog
All notable changes to this package will be documented here. This project adheres to [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.0] - 2026-05-04
### Added
- Quad, Triangle, Polygon (2D shapes)
- Cube, Pyramid (3D general shapes)
- Tetrahedron (d4), Hexahedron (d6), Octahedron (d8) (Platonic solids)
- Sphere (UV)
- Toolbar integration: **GameObject > 3D Object > Primitive Creator**
- Collider components for each shape via **Add Component > Primitive Creator**
- `PrimitiveCreatorComponent` for in-scene mesh customization and asset saving
- `PrimitiveCreatorUtility` scripting API (`CreateMesh`, `CreateColliderMesh`, `CreatePrimitive`)
- `MeshCreator` abstract base class for adding custom mesh types
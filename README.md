### FlatRedBall NavMesh
C# implementation of 2D Nav-mesh, that uses axis-aligned rectangles, for [FlatRedBall game framework](http://flatredball.com/).

#### Usage
1. Use your favorite way to include FRBNavMesh (project or dll) into your FlatRedBall game project.
2. Create main nav-mesh object using `new FRBNavMesh.NavMesh<FRBNavMesh.PortalNode,FRBNavMesh.Link>(..)`
   - and giving it List of FlatRedBall `AxisAlignedRectangle`s that idealy share some edges. They will be used to define "wakable areas" for path-finding agents.
3. Use `YourFRBNavMeshInstance.FindPath(..)` to get shortest path from start to end Point
   - as both List of `Point`s and List of `FRBNavMesh.PortalNode`s

#### More
- You can use `FRBNavMesh.TiledExtensions.BuildNavMeshFromTiled<TLink,TNode>(..)` to build NavMesh from [Tiled editor](https://www.mapeditor.org/) .tmx file, loaded by FlatRedBall Tiled plugin.
  - You need to have a separate Object layer in your tmx map, with rectangle shapes in it.

- You can derive your own `PortalNode` and/or `Link` classes from `FRBNavMesh.PortalNodeBase<TLink,TNode>` and `FRBNavMesh.LinkBase<TLink,TNode>`. Your classes can than contain additional data used by your project.
  - Your derived Node and Link class(es) must be concrete (not generic) for you to be able to use them with generic `NavMesh<TNode,TLink>`. (NavMesh itself can be used as generic, supplying it your classes.)



---

Project doesn't (yet) implement any dynamic obstacle avoidance or flocking functionality.

Will support "floors" later.

---

This project uses modified version of A* implementation from [FlatRedBall](http://flatredball.com/).

This project is inspired by [Phaser-Navmesh](https://github.com/mikewesthad/phaser-navmesh) javascript rects-based nav-mesh project for Phaser engine.

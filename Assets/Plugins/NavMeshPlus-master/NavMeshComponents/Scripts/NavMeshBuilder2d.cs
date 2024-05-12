using NavMeshPlus.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace NavMeshPlus.Extensions
{
    class NavMeshBuilder2dState: IDisposable
    {
        public Dictionary<Sprite, Mesh> spriteMeshMap;
        public Dictionary<uint, Mesh> coliderMeshMap;
        public Action<UnityEngine.Object, NavMeshBuildSource> lookupCallback;
        public int defaultArea;
        public int layerMask;
        public int agentID;
        public bool overrideByGrid;
        public GameObject useMeshPrefab;
        public bool compressBounds;
        public Vector3 overrideVector;
        public NavMeshCollectGeometry CollectGeometry;
        public CollectObjects CollectObjects;
        public GameObject parent;
        public bool hideEditorLogs;
        
        protected IEnumerable<GameObject> _root;
        private bool _disposed;

        public IEnumerable<GameObject> Root => _root ?? GetRoot();

        
        public NavMeshBuilder2dState()
        {
            spriteMeshMap = new Dictionary<Sprite, Mesh>();
            coliderMeshMap = new Dictionary<uint, Mesh>();
            _root = null;
        }

        public Mesh GetMesh(Sprite sprite)
        {
            Mesh mesh;
            if (spriteMeshMap.ContainsKey(sprite))
            {
                mesh = spriteMeshMap[sprite];
            }
            else
            {
                mesh = new Mesh();
                NavMeshBuilder2d.sprite2mesh(sprite, mesh);
                spriteMeshMap.Add(sprite, mesh);
            }
            return mesh;
        }
        static GameObject p = new();
        /*
            InsideTriangle decides if a point P is Inside of the triangle
            defined by A, B, C.
        */
        static bool InsideTriangle(float Ax, float Ay,
                              float Bx, float By,
                              float Cx, float Cy,
                              float Px, float Py)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = Cx - Bx; ay = Cy - By;
            bx = Ax - Cx; by = Ay - Cy;
            cx = Bx - Ax; cy = By - Ay;
            apx = Px - Ax; apy = Py - Ay;
            bpx = Px - Bx; bpy = Py - By;
            cpx = Px - Cx; cpy = Py - Cy;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
        struct Vertex2D
        {
            public Vector3 point;
            public int index;
        }
        static bool IsLeftSide(Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4, bool dir = true)//判断点3指向点4的向量在点1指向点2的向量的哪侧
        {
            float x1 = p1.x;
            float y1 = p1.y;
            float x2 = p2.x;
            float y2 = p2.y;
            float x3 = p3.x;
            float y3 = p3.y;
            float x4 = p4.x;
            float y4 = p4.y;
            float ax = x2 - x1;
            float ay = y2 - y1;
            float bx = x4 - x3;
            float by = y4 - y3;
            //向量(ax,ay,0)叉乘向量(bx,by,0)得到的向量为(0,0,ab),由ab的正负和观察方向界定左右
            float ab = (ax * by - ay * bx);
            //
            bool? side = null;
            if (ab == 0)
            {
                side = true;//共线或者平行
            }
            else if (ab > 0)
            {
                if (dir)
                {
                    side = true;
                }
                else //dir=1
                {
                    side = false;
                }

            }
            else if (ab < 0)
            {
                if (dir)
                {
                    side = false;
                }
                else //dir=1
                {
                    side = true;
                }
            }
            return side.Value;
        }
        static int[] triangulate_earClip(Vector3[] points)
        {
            LinkedList<Vertex2D> remainVertexs = new();
            for (int i = 0; i < points.Length; i++)
            {
                Vertex2D vertex = new()
                {
                    point = points[i],
                    index = i,
                };
                remainVertexs.AddLast(vertex);
            }

            List<int> indices = new();
            LinkedListNode<Vertex2D> previousNode = remainVertexs.First;
            LinkedListNode<Vertex2D> node = previousNode.Next;
            LinkedListNode<Vertex2D> nextNode = node.Next;
            int c = 0;
            while (remainVertexs.Count > 3 && c < 1e5)
            {
                c++;
                //Debug.Log("remainVertexs = " + remainVertexs.Count);
                Vector3 p0 = previousNode.Value.point;
                Vector3 p1 = node.Value.point;
                Vector3 p2 = nextNode.Value.point;
                if (IsLeftSide(p0, p1,p0, p2))
                {
                    bool isEar = true;
                    for (LinkedListNode<Vertex2D> n = (nextNode.Next != remainVertexs.Last) ? nextNode.Next : remainVertexs.First;
                        n != previousNode;
                        n = (n.Next != remainVertexs.Last) ? n.Next : remainVertexs.First)
                    {
                        if (InsideTriangle(p0.x, p0.y, p1.x, p1.y, p2.x, p2.y, n.Value.point.x, n.Value.point.y))
                        {
                            isEar = false;
                            break;
                        }
                    }
                    if (isEar)
                    {
                        //增加三角形索引和移除当前的耳朵点
                        indices.Add(previousNode.Value.index);
                        indices.Add(node.Value.index);
                        indices.Add(nextNode.Value.index);

                        remainVertexs.Remove(node);

                        //更新当前处理的顶点
                        node = nextNode;
                        nextNode = (nextNode.Next != remainVertexs.Last) ? nextNode.Next : remainVertexs.First;
                        continue;
                    }
                }
                previousNode = (previousNode.Next != remainVertexs.Last) ? previousNode.Next : remainVertexs.First;
                node = (node.Next != remainVertexs.Last) ? node.Next : remainVertexs.First;
                nextNode = (nextNode.Next != remainVertexs.Last) ? nextNode.Next : remainVertexs.First;
            }
            previousNode = remainVertexs.First;
            node = previousNode.Next;
            nextNode = node.Next;

            indices.Add(previousNode.Value.index);
            indices.Add(node.Value.index);
            indices.Add(nextNode.Value.index);
            return indices.ToArray();
        }
        static Vector3[] V2_to_3_To_World(Vector2[] v2,Transform t)
        {
            Vector3[] v3 = new Vector3[v2.Length];
            p.transform.parent = t;
            for (int i = 0; i < v2.Length; i++)
            {
                p.transform.localPosition = v2[i];
                v3[i] = p.transform.position;
            }
            return v3;
        }
        public Mesh GetMesh(Collider2D collider)
        {
#if UNITY_2019_3_OR_NEWER
            Mesh mesh;
            uint hash = collider.GetShapeHash();
            if (coliderMeshMap.ContainsKey(hash))
            {
                mesh = coliderMeshMap[hash];
            }
            else
            {
                mesh = collider.CreateMesh(false, false);
                if (collider.GetComponent<EdgeCollider2D>())
                {
                    mesh.vertices = V2_to_3_To_World(collider.GetComponent<EdgeCollider2D>().points, collider.transform);
                    mesh.triangles = triangulate_earClip(mesh.vertices);
                }

                //for (int i = 0; i < mesh.vertices.Length; i++)
                //{
                //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    g.transform.position = mesh.vertices[i];
                //}
                coliderMeshMap.Add(hash, mesh);
            }
            return mesh;
#else
            throw new InvalidOperationException("PhysicsColliders supported in Unity 2019.3 and higher.");
#endif
        }
        public void SetRoot(IEnumerable<GameObject> root)
        {
            _root = root;
        }
        public IEnumerable<GameObject> GetRoot()
        {
            switch (CollectObjects)
            {
                case CollectObjects.Children: return new[] { parent };
                case CollectObjects.Volume:
                case CollectObjects.All:
                default:
                    {
                        var list = new List<GameObject>();
                        var roots = new List<GameObject>();
                        for (int i = 0; i < SceneManager.sceneCount; ++i)
                        {
                            var s = SceneManager.GetSceneAt(i);
                            if (!s.isLoaded) continue;
                            s.GetRootGameObjects(list);
                            roots.AddRange(list);
                        }
                        return roots;
                    }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                foreach (var item in spriteMeshMap)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(item.Value);
#else 
                    Object.Destroy(item.Value);
#endif
                }
                foreach (var item in coliderMeshMap)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(item.Value);
#else
                    Object.Destroy(item.Value);
#endif
                }
                spriteMeshMap.Clear();
                coliderMeshMap.Clear();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }

    class NavMeshBuilder2d
    {
        public static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder)
        {
            foreach (var it in builder.Root)
            {
                CollectSources(it, sources, builder);
            }
            //if (!builder.hideEditorLogs) Debug.Log("Sources " + sources.Count);
        }

        public static void CollectSources(GameObject root, List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder)
        {
            foreach (var modifier in root.GetComponentsInChildren<NavMeshModifier>())
            {
                if (((0x1 << modifier.gameObject.layer) & builder.layerMask) == 0)
                {
                    continue;
                }
                if (!modifier.AffectsAgentType(builder.agentID))
                {
                    continue;
                }
                int area = builder.defaultArea;
                //if it is walkable
                if (builder.defaultArea != 1 && !modifier.ignoreFromBuild)
                {
                    AddDefaultWalkableTilemap(sources, builder, modifier);
                }

                if (modifier.overrideArea)
                {
                    area = modifier.area;
                }
                if (!modifier.ignoreFromBuild)
                {
                    CollectSources(sources, builder, modifier, area);
                }
            }
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder, NavMeshModifier modifier, int area)
        {
            if (builder.CollectGeometry == NavMeshCollectGeometry.PhysicsColliders)
            {
                var collider = modifier.GetComponent<Collider2D>();
                if (collider != null)
                {
                    CollectSources(sources, collider, area, builder);
                }
            }
            else
            {
                var tilemap = modifier.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    CollectTileSources(sources, tilemap, area, builder);
                }
                var sprite = modifier.GetComponent<SpriteRenderer>();
                if (sprite != null)
                {
                    CollectSources(sources, sprite, area, builder);
                }
            }
        }

        private static void AddDefaultWalkableTilemap(List<NavMeshBuildSource> sources, NavMeshBuilder2dState builder, NavMeshModifier modifier)
        {
            var tilemap = modifier.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                if (builder.compressBounds)
                {
                    tilemap.CompressBounds();
                }

                if (!builder.hideEditorLogs) Debug.Log($"Walkable Bounds [{tilemap.name}]: {tilemap.localBounds}");
                var box = BoxBoundSource(NavMeshSurface.GetWorldBounds(tilemap.transform.localToWorldMatrix, tilemap.localBounds));
                box.area = builder.defaultArea;
                sources.Add(box);
            }
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, SpriteRenderer spriteRenderer, int area, NavMeshBuilder2dState builder)
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }
            Mesh mesh;
            mesh = builder.GetMesh(spriteRenderer.sprite);
            if (mesh == null)
            {
                if (!builder.hideEditorLogs) Debug.Log($"{spriteRenderer.name} mesh is null");
                return;
            }
            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.Mesh;
            src.component = spriteRenderer;
            src.area = area;
            src.transform = Matrix4x4.TRS(Vector3.Scale(spriteRenderer.transform.position, builder.overrideVector), spriteRenderer.transform.rotation, spriteRenderer.transform.lossyScale);
            src.sourceObject = mesh;
            sources.Add(src);

            builder.lookupCallback?.Invoke(spriteRenderer.gameObject, src);
        }

        public static void CollectSources(List<NavMeshBuildSource> sources, Collider2D collider, int area, NavMeshBuilder2dState builder)
        { 
            if (collider.usedByComposite)
            {
                collider = collider.GetComponent<CompositeCollider2D>();
            }

            Mesh mesh;
            mesh = builder.GetMesh(collider);
            if (mesh == null)
            {
                if (!builder.hideEditorLogs) Debug.Log($"{collider.name} mesh is null");
                return;
            }

            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.Mesh;
            src.area = area;
            src.component = collider;
            src.sourceObject = mesh;
            if (collider.attachedRigidbody)
            {
                src.transform = Matrix4x4.TRS(Vector3.Scale(collider.attachedRigidbody.transform.position, builder.overrideVector), collider.attachedRigidbody.transform.rotation, Vector3.one);
            }
            else
            {
                src.transform = Matrix4x4.identity;
            }

            sources.Add(src);

            builder.lookupCallback?.Invoke(collider.gameObject, src);
        }

        public static void CollectTileSources(List<NavMeshBuildSource> sources, Tilemap tilemap, int area, NavMeshBuilder2dState builder)
        {
            var bound = tilemap.cellBounds;

            var modifierTilemap = tilemap.GetComponent<NavMeshModifierTilemap>();

            if (modifierTilemap && !modifierTilemap.AffectsAgentType(builder.agentID))
            {
                return;
            }

            var vec3int = new Vector3Int(0, 0, 0);

            var size = new Vector3(tilemap.layoutGrid.cellSize.x, tilemap.layoutGrid.cellSize.y, 0);
            Mesh sharedMesh = null;
            Quaternion rot = default;

            if (builder.useMeshPrefab != null)
            {
                sharedMesh = builder.useMeshPrefab.GetComponent<MeshFilter>().sharedMesh;
                size = builder.useMeshPrefab.transform.localScale;
                rot = builder.useMeshPrefab.transform.rotation;
            }
            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                for (int j = bound.yMin; j < bound.yMax; j++)
                {
                    var src = new NavMeshBuildSource();
                    src.area = area;

                    vec3int.x = i;
                    vec3int.y = j;
                    if (!tilemap.HasTile(vec3int))
                    {
                        continue;
                    }

                    CollectTile(tilemap, builder, vec3int, size, sharedMesh, rot, ref src);
                    if (modifierTilemap && modifierTilemap.TryGetTileModifier(vec3int, tilemap, out NavMeshModifierTilemap.TileModifier tileModifier))
                    {
                        src.area = tileModifier.overrideArea ? tileModifier.area : area;
                    }    
                    sources.Add(src);

                    builder.lookupCallback?.Invoke(tilemap.GetInstantiatedObject(vec3int), src);
                }
            }
        }

        private static void CollectTile(Tilemap tilemap, NavMeshBuilder2dState builder, Vector3Int vec3int, Vector3 size, Mesh sharedMesh, Quaternion rot, ref NavMeshBuildSource src)
        {
            if (!builder.overrideByGrid && tilemap.GetColliderType(vec3int) == Tile.ColliderType.Sprite)
            {
                var sprite = tilemap.GetSprite(vec3int);
                if (sprite != null)
                {
                    Mesh mesh = builder.GetMesh(sprite);
                    src.component = tilemap;
                    src.transform = GetCellTransformMatrix(tilemap, builder.overrideVector, vec3int);
                    src.shape = NavMeshBuildSourceShape.Mesh;
                    src.sourceObject = mesh;
                }
            }
            else if (builder.useMeshPrefab != null || (builder.overrideByGrid && builder.useMeshPrefab != null))
            {
                src.transform = Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), builder.overrideVector), rot, size);
                src.shape = NavMeshBuildSourceShape.Mesh;
                src.sourceObject = sharedMesh;
            }
            else //default to box
            {
                src.transform = GetCellTransformMatrix(tilemap, builder.overrideVector, vec3int);
                src.shape = NavMeshBuildSourceShape.Box;
                src.size = size;
            }
        }

        public static Matrix4x4 GetCellTransformMatrix(Tilemap tilemap, Vector3 scale, Vector3Int vec3int)
        {
            return Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), scale) - tilemap.layoutGrid.cellGap, tilemap.transform.rotation, tilemap.transform.lossyScale) * tilemap.orientationMatrix * tilemap.GetTransformMatrix(vec3int);
        }

        internal static void sprite2mesh(Sprite sprite, Mesh mesh)
        {
            Vector3[] vert = new Vector3[sprite.vertices.Length];
            for (int i = 0; i < sprite.vertices.Length; i++)
            {
                vert[i] = new Vector3(sprite.vertices[i].x, sprite.vertices[i].y, 0);
            }
            mesh.vertices = vert;
            mesh.uv = sprite.uv;
            int[] tri = new int[sprite.triangles.Length];
            for (int i = 0; i < sprite.triangles.Length; i++)
            {
                tri[i] = sprite.triangles[i];
            }
            mesh.triangles = tri;
        }

        static private NavMeshBuildSource BoxBoundSource(Bounds localBounds)
        {
            var src = new NavMeshBuildSource();
            src.transform = Matrix4x4.Translate(localBounds.center);
            src.shape = NavMeshBuildSourceShape.Box;
            src.size = localBounds.size;
            src.area = 0;
            return src;
        }
    }
}

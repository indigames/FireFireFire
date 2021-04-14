using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class WrapMeshVertex
{
    public int index;
    public List<int> neighbors = new List<int>();
    public HashSet<int> neighborsSet = new HashSet<int>();
    public Vector3 position;
    public int mapx, mapy, mapz;
}

public class WrapMesh : MonoBehaviour
{
    public bool peelingMode;

    public GameObject targetContainer;
    public List<Material> burntMaterials;
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public List<WrapMeshVertex> wrapMeshVertices = null;

    [HideInInspector]
    public Mesh wrapMesh;
    [HideInInspector]
    public Mesh colliderMesh;

    // Start is called before the first frame update

    LayerMask layerMask;
    [System.NonSerialized]
    public MeshCollider wrapMeshTrigger;

    void Start()
    {
        SetupWrapMesh();

        GetComponent<MeshFilter>().mesh = wrapMesh;
        GetComponent<MeshCollider>().sharedMesh = wrapMesh;

        //targetMeshContainer.transform.SetParent(transform);
        MapVertices();
    }
    public void SetupWrapMesh()
    {
        gameObject.layer = LayerUtil.LAYER_WRAP_MESH;

        if (burntMaterials.Count > 0 && GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().material = burntMaterials[UnityEngine.Random.Range(0, burntMaterials.Count)];

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();

            var meshFilters = new List<MeshFilter>(targetContainer.GetComponentsInChildren<MeshFilter>(false));
            meshFilters.Remove(meshFilter); //do not include this mesh filter
            meshFilters.RemoveAll((m) =>
            {
                var renderer = m.GetComponent<MeshRenderer>();
                return renderer == null || renderer.enabled == false;
            });
            meshFilters.RemoveAll((m) => m.GetComponent<StageDecor>() != null);
            meshFilters.RemoveAll((m) => m.GetComponent<StageUnderlay>() != null);

            var combineCount = 0;

            foreach (var meshFilter in meshFilters)
                combineCount += meshFilter.mesh == null ? 0 : meshFilter.mesh.subMeshCount;

            CombineInstance[] combine = new CombineInstance[combineCount];
            int i = 0;

            List<Material> materials = new List<Material>();
            //List<int> submeshCounts = new List<int>();

            foreach (var meshFilter in meshFilters)
            {
                materials.AddRange(meshFilter.GetComponent<MeshRenderer>().materials);

                for (var submesh = 0; submesh < meshFilter.mesh.subMeshCount; submesh++)
                {
                    combine[i].mesh = meshFilter.sharedMesh;
                    if (meshFilter.sharedMesh.subMeshCount > 1) combine[i].subMeshIndex = submesh;
                    if (meshFilter.gameObject == transform.gameObject)
                        combine[i].transform = transform.parent.worldToLocalMatrix * Matrix4x4.Translate(-meshFilter.transform.localPosition) * meshFilter.transform.localToWorldMatrix;
                    else
                        combine[i].transform = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
                    i++;
                }
                if (peelingMode) meshFilter.gameObject.SetActive(false);
            }

            if (peelingMode) meshFilter.mesh.CombineMeshes(combine, false);
            else meshFilter.mesh.CombineMeshes(combine, true);

            var mesh = meshFilter.mesh;

            //REMAP MATERIALS AND SUBMESHES

            if (peelingMode)
            {
                var wrap_material = GetComponent<MeshRenderer>().material;
                for (var mi = 0; mi < materials.Count; mi++)
                {
                    var mat = Instantiate(wrap_material);
                    mat.mainTexture = materials[mi].mainTexture;
                    mat.color = materials[mi].color;
                    materials[mi] = mat;
                }
                GetComponent<MeshRenderer>().materials = materials.ToArray();
            }

            //mesh.subMeshCount = submeshCounts.Count;
            //for (var submesh = 0; submesh < submeshCounts.Count; submesh++)
            //{
            //    var submeshCount = submeshCounts[submesh];
            //    mesh.SetSubMesh(submesh, new UnityEngine.Rendering.SubMeshDescriptor(submeshIndex, submeshCount));
            //    submeshIndex += submeshCount;
            //}

            mesh.RecalculateNormals();
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var uvs = mesh.uv;
            var triangles = mesh.triangles;
            var colors = new Color[vertices.Length];

            //EXTRUDE MESH
            for (var vi = 0; vi < vertices.Length; vi++)
            {
                normals[vi] = normals[vi];
                vertices[vi] = vertices[vi] + normals[vi].normalized * 0.01f;

                if (peelingMode) colors[vi] = Color.white;
                else colors[vi] = Color.clear;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.colors = colors;

            this.wrapMesh = mesh;
            this.colliderMesh = Instantiate(mesh);
            {
                var colliderMeshVertices = this.colliderMesh.vertices;
                for (var j = 0; j < colliderMeshVertices.Length; j++)
                    colliderMeshVertices[j] *= 0.87f;
                this.colliderMesh.vertices = colliderMeshVertices;
            }

            //CREATE SIMPLIFIED MESH HERE
            //{
            //    var simplifier = new UnityMeshSimplifier.MeshSimplifier();
            //    simplifier.Initialize(mesh);
            //    var simplifiedVertices = simplifier.Vertices;
            //    for (var j = 0; j < simplifiedVertices.Length; j++)
            //        simplifiedVertices[j] *= 0.8f;
            //    simplifier.Vertices = simplifiedVertices;
            //    simplifier.SimplifyMesh(0.1f);

            //    this.colliderMesh = simplifier.ToMesh();
            //}


            var collider_extents = colliderMesh.bounds.extents;
            var collider_center = colliderMesh.bounds.center;
            var max_extent = Mathf.Max(collider_extents.x, collider_extents.y, collider_extents.z);

            //CREATE Trigger here
            var existing_trigger_colliders = new List<MeshCollider>(targetContainer.GetComponentsInChildren<MeshCollider>());
            var existing_trigger_collider = existing_trigger_colliders.Find((c) => c.gameObject != gameObject && c.isTrigger);

            if (existing_trigger_collider != null)
            {
                var scale = existing_trigger_collider.transform.localScale;
                existing_trigger_collider.transform.localScale = scale;
                existing_trigger_collider.transform.SetParent(transform);
                existing_trigger_collider.gameObject.layer = LayerUtil.LAYER_WRAP_TRIGGER;

                this.wrapMeshTrigger = existing_trigger_collider;                
            }
            else
            {
                var trigger_object = new GameObject("Trigger");
                trigger_object.transform.SetParent(transform);
                trigger_object.transform.localPosition = trigger_object.transform.localEulerAngles = Vector3.zero;
                trigger_object.layer = LayerUtil.LAYER_WRAP_TRIGGER;

                //var trigger_collider = trigger_object.AddComponent<SphereCollider>();
                //trigger_collider.isTrigger = true;
                //trigger_collider.radius = max_extent * 1.2f;
                //trigger_collider.transform.localPosition = collider_center;

                var trigger_collider = trigger_object.AddComponent<MeshCollider>();
                trigger_collider.convex = true;
                trigger_collider.isTrigger = true;
                trigger_collider.sharedMesh = this.colliderMesh;
                //trigger_collider.transform.localScale = new Vector3(1.7f, 1.7f, 1f);
                trigger_collider.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                this.wrapMeshTrigger = trigger_collider;
            }
        }

    }

    public List<WrapMeshVertex> MapVertices()
    {
        if (this.wrapMeshVertices != null && this.wrapMeshVertices.Count > 0) return this.wrapMeshVertices;

        var mesh = this.wrapMesh;

        List<WrapMeshVertex> mappedVertices = new List<WrapMeshVertex>();
        float minx, miny, minz, maxx, maxy, maxz;

        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var uvs = new Vector2[vertices.Length];

        minx = maxx = vertices[0].x;
        miny = maxy = vertices[0].y;
        minz = maxz = vertices[0].z;

        for (var i = 0; i < vertices.Length; i++)
        {
            var position = vertices[i];
            var vertex = new WrapMeshVertex()
            {
                index = i,
                position = position
            };
            mappedVertices.Add(vertex);

            minx = Mathf.Min(minx, position.x);
            miny = Mathf.Min(miny, position.y);
            minz = Mathf.Min(minz, position.z);

            maxx = Mathf.Max(maxx, position.x);
            maxy = Mathf.Max(maxy, position.y);
            maxz = Mathf.Max(maxz, position.z);
        }

        //CALCULATE THE SEGMENT COUNT HERE
        int SEGMENT_COUNT = Mathf.CeilToInt(Mathf.Pow(mesh.vertexCount, 1 / 3f));

        List<WrapMeshVertex>[,,] verticesMap = new List<WrapMeshVertex>[SEGMENT_COUNT, SEGMENT_COUNT, SEGMENT_COUNT];
        for (var x = 0; x < SEGMENT_COUNT; x++)
            for (var y = 0; y < SEGMENT_COUNT; y++)
                for (var z = 0; z < SEGMENT_COUNT; z++)
                    verticesMap[x, y, z] = new List<WrapMeshVertex>();

        //map to grid
        for (var i = 0; i < mappedVertices.Count; i++)
        {
            var vertex = mappedVertices[i];
            var pos = vertex.position;
            var ix = Mathf.RoundToInt(Mathf.InverseLerp(minx, maxx, pos.x) * (SEGMENT_COUNT - 1));
            var iy = Mathf.RoundToInt(Mathf.InverseLerp(miny, maxy, pos.y) * (SEGMENT_COUNT - 1));
            var iz = Mathf.RoundToInt(Mathf.InverseLerp(minz, maxz, pos.z) * (SEGMENT_COUNT - 1));

            vertex.mapx = ix;
            vertex.mapy = iy;
            vertex.mapz = iz;

            verticesMap[ix, iy, iz].Add(vertex);

            uvs[i] = new Vector2(Mathf.InverseLerp(minx, maxx, pos.x), Mathf.InverseLerp(miny, maxy, pos.y));
        }

        //linking neighbors
        for (var i = 0; i < mappedVertices.Count; i++)
        {
            var vertex = mappedVertices[i];
            for (var x = Mathf.Max(0, vertex.mapx - 1); x < Mathf.Min(SEGMENT_COUNT, vertex.mapx + 2); x++)
                for (var y = Mathf.Max(0, vertex.mapy - 1); y < Mathf.Min(SEGMENT_COUNT, vertex.mapy + 2); y++)
                    for (var z = Mathf.Max(0, vertex.mapz - 1); z < Mathf.Min(SEGMENT_COUNT, vertex.mapz + 2); z++)
                        foreach (var neighbor_vertex in verticesMap[x, y, z]) vertex.neighborsSet.Add(neighbor_vertex.index);
            vertex.neighborsSet.Remove(vertex.index);
        }

        //linking triangles
        for (var i = 0; i < triangles.Length; i += 3)
        {
            if (i > triangles.Length - 3) break;
            var i1 = triangles[i];
            var i2 = triangles[i + 1];
            var i3 = triangles[i + 2];

            mappedVertices[i1].neighborsSet.Add(i2);
            mappedVertices[i1].neighborsSet.Add(i3);

            mappedVertices[i2].neighborsSet.Add(i1);
            mappedVertices[i2].neighborsSet.Add(i3);

            mappedVertices[i3].neighborsSet.Add(i1);
            mappedVertices[i3].neighborsSet.Add(i2);
        }

        foreach (var vertex in mappedVertices)
            vertex.neighbors = new List<int>(vertex.neighborsSet);

        if (peelingMode == false) mesh.uv = uvs;

        this.wrapMeshVertices = mappedVertices;
        return mappedVertices;
    }

    public void DisableConvex()
    {
        var mc = GetComponent<MeshCollider>();
        mc.isTrigger = false;
        mc.convex = false;
    }

    public WrapMesh MakeInstanceFromModel(Transform target, Transform parent = null)
    {
        gameObject.name = target.name;
        if (parent == null) parent = transform.parent;

        //create new wrapmesh instance
        var result = Instantiate(this, parent);
        result.transform.position = target.position;

        //duplicate target model inside new wrap mesh
        target = Instantiate(target);
        target.gameObject.SetActive(true);
        target.SetParent(result.transform);

        //activate it and return
        result.gameObject.SetActive(true);
        return result;
    }

    public WrapMesh MakeInstanceFromModel(StageTarget target, Transform parent = null)
    {
        var result = MakeInstanceFromModel(target.transform);
        result.GetComponent<WrapMeshInteraction>().spreadSpeed = target.spreadSpeed;
        return result;
    }
}
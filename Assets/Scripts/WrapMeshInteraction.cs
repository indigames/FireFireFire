using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapMeshInteraction : MonoBehaviour
{
    class Vertex
    {
        public int index;
        public Vector3 position;
        public List<int> neighbors;
        public float[] spreadDurations;
        public float[] spreadDeltas;
        public bool spreaded;
        public bool snuffed;
    }

    private const float SNUFF_DURATION = 3f; //Default is 2f
    private const float SPREAD_SPEED = 3f; //Default is 1d
    private MeshFilter meshFilter;
    private WrapMesh wrapMesh;
    private List<Vertex> vertices = new List<Vertex>();
    private List<Color> colors;
    private List<int> triangles;

    public ParticleSystem firePs;
    [System.NonSerialized]
    public bool canSpreadTo = false;
    public bool spreadByDefault;
    private int snuffedVerticesCount = 0;
    private bool meshSnuffed = false;
    private bool meshIgnited = false;

    [Range(1, 10)]
    public float spreadSpeed = 1f;

    public bool MeshSnuffed => meshSnuffed;
    public bool MeshIgnited => meshIgnited;

    private IEnumerator Start()
    {
        this.meshFilter = GetComponent<MeshFilter>();
        this.wrapMesh = GetComponent<WrapMesh>();
        yield return true;
        SetupVertices();    
        canSpreadTo = spreadByDefault;
    }

    void SetupVertices()
    {
        const float SPREAD_RATE = 10f;
        var map_vertices = this.wrapMesh.MapVertices();

        colors = new List<Color>(map_vertices.Count);
        this.meshFilter.mesh.GetColors(colors);
        triangles = new List<int>(this.meshFilter.mesh.triangles);

        foreach (var vertex in map_vertices)
        {
            var new_vertex = new Vertex()
            {
                index = vertex.index,
                position = vertex.position,
                neighbors = new List<int>(vertex.neighbors),
                spreadDurations = new float[vertex.neighbors.Count],
                spreadDeltas = new float[vertex.neighbors.Count]
            };
            this.vertices.Add(new_vertex);
        }

        foreach (var vertex in vertices)
            for (var i = 0; i < vertex.neighbors.Count; i++)
                vertex.spreadDurations[i] = vertex.spreadDeltas[i] = (map_vertices[vertex.neighbors[i]].position - vertex.position).sqrMagnitude * SPREAD_RATE;
    }

    public void Restart()
    {
        for (var i = 0; i < colors.Count; i++) colors[i] = Color.clear;
        applyingColors = true;
        canSpreadTo = false;
        mapCollision.Clear();

        foreach (var vertex in vertices)
        {
            vertex.snuffed = false;
            vertex.spreaded = false;
            for (var i = 0; i < vertex.neighbors.Count; i++)
                vertex.spreadDeltas[i] = vertex.spreadDurations[i];
        }
    }

    bool applyingColors = false;
    float deltaTime = 0;
    private void Update()
    {
        if (this.canSpreadTo == false) return;
        deltaTime = Time.deltaTime;
        if (deltaTime > 0.1f) deltaTime = 0.1f;

        CheckIgniteMouse();
        CheckSpreadVertices();

        if (applyingColors)
        {
            applyingColors = false;
            this.meshFilter.mesh.SetColors(colors);
        }
    }

    void CheckIgniteMouse()
    {
        return;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hitInfo;
        //    if (GetComponent<MeshCollider>().Raycast(ray, out hitInfo, 9999)) ;
        //    else return;

        //    var index = hitInfo.triangleIndex;
        //    var mesh = meshFilter.mesh;
        //    var triangles = this.triangles;
        //    if (index < 0) return;

        //    var i1 = triangles[index * 3];
        //    var i2 = triangles[index * 3 + 1];
        //    var i3 = triangles[index * 3 + 2];
        //    SpreadToVertex(i1);
        //    SpreadToVertex(i2);
        //    SpreadToVertex(i3);
        //}
    }

    void CheckSpreadVertices()
    {
        foreach (var vertex in vertices) {
            if (vertex.spreaded && vertex.snuffed == false) CheckSpreadVertex(vertex);

            if (vertex.snuffed && this.colors[vertex.index].a > 0.5f + float.Epsilon * 2)
            {
                var c = this.colors[vertex.index];

                var alpha_step = Mathf.RoundToInt(c.a / 0.1f);
                c.a -= deltaTime * (0.5F / SNUFF_DURATION);
                if (alpha_step != Mathf.RoundToInt(c.a / 0.1f)) CheckSpreadToOther(vertex.index);

                if (c.a < 0.5f)
                {
                    c.a = 0.5f + float.Epsilon;
                    OnVertexSnuff(vertex.index);
                }
                this.colors[vertex.index] = c;
                applyingColors = true;
            }
        }
    }

    void OnVertexSnuff(int index)
    {
        snuffedVerticesCount += 1;
        CheckSpreadToOther(index);

        if (meshSnuffed == false && snuffedVerticesCount > meshFilter.mesh.vertexCount * 0.95f)
            meshSnuffed = true;

    }

    void CheckSpreadVertex(Vertex vertex)
    {
        bool completed = true;
        foreach (var neighbor in vertex.neighbors)
            if (vertices[neighbor].spreaded == false) completed = false;

        if (completed)
        {
            vertex.snuffed = true;
            return;
        }

        for (var i = 0; i < vertex.neighbors.Count; i++)
        {
            if (vertex.spreadDeltas[i] < 0) continue;
            vertex.spreadDeltas[i] -= deltaTime * SPREAD_SPEED * spreadSpeed;

            var ratio =  1 - vertex.spreadDeltas[i] / vertex.spreadDurations[i];
            if (ratio > 1) ratio = 1;
            if (ratio > this.colors[vertex.neighbors[i]].a) this.colors[vertex.neighbors[i]] = new Color(1, 1, 1, ratio);
            if (vertex.spreadDeltas[i] < 0) SpreadToVertex(vertex.neighbors[i]);
        }
    }

    void SpreadToVertex(int index)
    {
        if (this.meshIgnited == false) this.meshIgnited = true;
        var vertex = this.vertices[index];
        if (vertex.spreaded) return;
        vertex.spreaded = true;
        this.colors[index] = new Color(1, 1, 1, 1);
        applyingColors = true;

        var position = this.vertices[index].position;
        position = (Vector3) (transform.localToWorldMatrix * position) + transform.position;

        var size = Mathf.Pow(UnityEngine.Random.value, 3) * 10f;

        firePs.Emit(new ParticleSystem.EmitParams()
        {
            position = position + UnityEngine.Random.insideUnitSphere * 0.3f,
            startSize = UnityEngine.Random.Range(0.5f, 4f),
            startLifetime = SNUFF_DURATION
        }, 1) ;

        CheckSpreadToOther(position);
    }

    void CheckSpreadToOther(int index)
    {
        var position = this.vertices[index].position;
        position = (Vector3)(transform.localToWorldMatrix * position) + transform.position;
        CheckSpreadToOther(position);
    }

    //void CheckSpreadToOther(Vector3 position)
    //{
    //    RaycastHit[] hitInfos = Physics.SphereCastAll(position, 0.15f, Vector3.forward, 1f, notWallMask);
    //    if (hitInfos == null || hitInfos.Length <= 0) return;

    //    foreach (var hitInfo in hitInfos)
    //    {
    //        var wrapMeshInteraction = hitInfo.transform.GetComponent<WrapMeshInteraction>();
    //        if (wrapMeshInteraction == null || wrapMeshInteraction == this) return;

    //        var index = hitInfo.triangleIndex;
    //        var mesh = wrapMeshInteraction.meshFilter.mesh;
    //        var triangles = mesh.triangles; //SLOW
    //        if (index < 0) return;
    //        var colors = mesh.colors; //SLOW

    //        var i1 = triangles[index * 3];
    //        var i2 = triangles[index * 3 + 1];
    //        var i3 = triangles[index * 3 + 2];
    //        wrapMeshInteraction.SpreadToVertex(i1);
    //        wrapMeshInteraction.SpreadToVertex(i2);
    //        wrapMeshInteraction.SpreadToVertex(i3);
    //    }
    //}

    public void SpreadFromPoint(Vector3 position)
    {
        if (this.canSpreadTo == false) return;

        position.z = transform.position.z;
        var dist = transform.position - position;
        if (dist.sqrMagnitude <= float.Epsilon) dist = Vector3.forward;
        RaycastHit hitInfo;

        if (GetComponent<Collider>().Raycast(new Ray(position - dist, dist), out hitInfo, 20f) == false
            && GetComponent<Collider>().Raycast(new Ray(position - Vector3.forward * 10, Vector3.forward), out hitInfo, 20f) == false)
            return;

        var index = hitInfo.triangleIndex;
        var mesh = meshFilter.mesh;
        if (index < 0) return;

        var i1 = triangles[index * 3];
        var i2 = triangles[index * 3 + 1];
        var i3 = triangles[index * 3 + 2];
        SpreadToVertex(i1);
        SpreadToVertex(i2);
        SpreadToVertex(i3);
    }

    void CheckSpreadToOther(Vector3 position)
    {
        foreach (var keyval in mapCollision)
        {
            var wrapMeshInteraction = keyval.Key.gameObject.GetComponentInParent<WrapMeshInteraction>();
            if (wrapMeshInteraction == null) continue;

            var pos = keyval.Value;
            pos.z = position.z;
            if ((pos - position).sqrMagnitude > 0.5f) continue;

            //spread to other here
            wrapMeshInteraction.SpreadFromPoint(pos);
        }
    }

    Dictionary<GameObject, Vector3> mapCollision = new Dictionary<GameObject, Vector3>();

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<FirestarterArea>()?.WrapMeshTriggerEnter(wrapMesh.wrapMeshTrigger);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerUtil.LAYER_WRAP_TRIGGER)
        {
            var position = other.ClosestPoint(transform.position);
            position = other.transform.position + (position - other.transform.position) * 0.9f;
            mapCollision[other.gameObject] = position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<FirestarterArea>()?.WrapMeshTriggerExit(wrapMesh.wrapMeshTrigger);
        //if (mapCollision.ContainsKey(other.gameObject)) mapCollision.Remove(other.gameObject);
    }
}

public static class WrapMeshInteractionUtil
{
    public static int GetClosestVertexIndex(this RaycastHit aHit, int[] aTriangles)
    {
        var b = aHit.barycentricCoordinate;
        int index = aHit.triangleIndex * 3;
        if (aTriangles == null || index < 0 || index + 2 >= aTriangles.Length)
            return -1;
        if (b.x > b.y)
        {
            if (b.x > b.z)
                return aTriangles[index]; // x
            else
                return aTriangles[index + 2]; // z
        }
        else if (b.y > b.z)
            return aTriangles[index + 1]; // y
        else
            return aTriangles[index + 2]; // z
    }
}
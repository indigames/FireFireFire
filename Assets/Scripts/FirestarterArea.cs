using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoAutoLit());
    }

    public void Restart()
    {
        wrapColliders.Clear();
    }

    IEnumerator CoAutoLit()
    {
        while (true)
        {
            foreach (var wrapMesh in wrapColliders)
            {
                #pragma warning disable
                var collider = wrapMesh.GetComponent<MeshCollider>();
                var position = collider.ClosestPoint(transform.position) ;
                #pragma warning restore
                position += Vector3.up * 0.1f;
                collider.GetComponentInParent<WrapMeshInteraction>().SpreadFromPoint(position);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }


    HashSet<Collider> wrapColliders = new HashSet<Collider>();

    public void WrapMeshTriggerEnter(Collider other)
    {
        wrapColliders.Add(other);
    }

    public void WrapMeshTriggerExit(Collider other)
    {
        wrapColliders.Remove(other);
    }
}

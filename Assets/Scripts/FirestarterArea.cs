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

    IEnumerator CoAutoLit()
    {
        while (true)
        {
            foreach (var wrapMesh in wrapMeshes)
            {
                var position = wrapMesh.transform.position;
                position.y = transform.position.y - 1;
                wrapMesh.SpreadFromPoint(position);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }


    HashSet<WrapMeshInteraction> wrapMeshes = new HashSet<WrapMeshInteraction>();

    public void WrapMeshInteractionEnter(WrapMeshInteraction other)
    {
        wrapMeshes.Add(other);
    }

    public void WrapMeshInteractionExit(WrapMeshInteraction other)
    {
        wrapMeshes.Remove(other);
    }
}

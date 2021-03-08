using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUtil : MonoBehaviour
{
    public static int LAYER_WRAP_MESH
        , LAYER_WRAP_TRIGGER
        , MASK_WRAP_MESH;
    

    private void Awake()
    {
        LAYER_WRAP_MESH = LayerMask.NameToLayer("WrapMesh");
        LAYER_WRAP_TRIGGER = LayerMask.NameToLayer("WrapTrigger");
        MASK_WRAP_MESH = LayerMask.GetMask("WrapMesh");
    }
}

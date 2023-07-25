using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChildMeshFilter
{
    public List<MeshFilter> GetMeshFilters();

    public List<Material> GetMaterials();

    public List<MeshRenderer> GetMeshRenderers();
}

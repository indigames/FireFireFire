using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageBase : MonoBehaviour, IChildMeshFilter
{

    public StageItemInfo info;
    public List<MeshFilter> meshFilters;

    public List<MeshFilter> GetMeshFilters()
    {
        return meshFilters;
    }

    public List<Material> Materials;

    public List<Material> GetMaterials()
    {
        return Materials;
    }

    public List<MeshRenderer> MeshRenderers;

    public List<MeshRenderer> GetMeshRenderers()
    {
        return MeshRenderers;
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        if (!this.info)
        {
            StageItemInfo info = this.GetComponent<StageItemInfo>();
            if (!info)
            {
                this.info = this.gameObject.AddComponent<StageItemInfo>();
            }
            else
            {
                this.info = info;
            }
        }

        List<MeshFilter> currentMeshFilter = GetComponentsInChildren<MeshFilter>(false).ToList();
        currentMeshFilter.RemoveAll((m) =>
        {
            var renderer = m.GetComponent<MeshRenderer>();
            return renderer == null || renderer.enabled == false;
        });
        currentMeshFilter.RemoveAll((m) => m.GetComponent<StageDecor>() != null);
        currentMeshFilter.RemoveAll((m) => m.GetComponent<StageUnderlay>() != null);

        if (currentMeshFilter.Count != meshFilters.Count || !checkMeshfilters())
        {
            meshFilters = currentMeshFilter;
            Materials.Clear();
            MeshRenderers.Clear();
            foreach (var item in meshFilters)
            {
                MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
                Materials.AddRange(meshRenderer.sharedMaterials);
                MeshRenderers.Add(meshRenderer);
            }
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private bool checkMeshfilters()
    {
        foreach (var item in meshFilters)
        {
            if (item == null)
            {
                return false;
            }
        }

        return true;
    }

#endif
}
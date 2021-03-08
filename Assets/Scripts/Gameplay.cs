using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public Action<int> callbackRemainingMeshblock;
    public Action callbackVictory;
    public Action callbackDefeat;

    public MeshBlock baseMeshBlock;
    public MeshBlock emptyMeshBlock;
    public WrapMeshInteraction targetMesh;
    public Transform itemsContainer;

    List<MeshBlock> availableMeshBlocks = new List<MeshBlock>();
    int nextMeshBlockIndex;
    MeshBlock nextMeshBlock;
    bool waitingForLaunch = false;
    Vector3 nextLaunchPosition;
    bool victory = false;

    IEnumerator Start()
    {
        yield return true;
        baseMeshBlock.gameObject.SetActive(false);

        StartCoroutine(CoGameplay());
    }

    IEnumerator CoGameplay()
    {
        // collect child transforms from items container
        List<Transform> transforms = new List<Transform>();
        for (var i = 0; i < itemsContainer.childCount; i++) transforms.Add(itemsContainer.GetChild(i));

        // create new meshblocks here
        foreach (var transform in transforms)
        {
            var newMeshBlock = emptyMeshBlock.MakeInstanceFromModel(transform);
            newMeshBlock.GetRigidbody.isKinematic = true; //set to be kinematic by default
            newMeshBlock.template = false;
            newMeshBlock.gameObject.SetActive(true);
            availableMeshBlocks.Add(newMeshBlock);
        }

        yield return true;
        yield return true;

        foreach (var meshBlock in availableMeshBlocks) meshBlock.gameObject.SetActive(false);
        
        for (var i = 0; i < availableMeshBlocks.Count; i++)
        {
            if (i < availableMeshBlocks.Count - 1) availableMeshBlocks[i + 1].gameObject.SetActive(true);

            var nextMeshBlock = availableMeshBlocks[i];
            nextMeshBlock.gameObject.SetActive(true);

            callbackRemainingMeshblock?.Invoke(availableMeshBlocks.Count - i - 1);

            for (var dt = 0f; dt < 1; dt += Time.deltaTime / 0.25f)
            {
                nextMeshBlock.transform.position = Vector3.Lerp(new Vector3(0, 22, -2), new Vector3(0, 20, -4), dt);
                nextMeshBlock.transform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), new Vector3(1, 1, 1), dt);
                yield return true;
            }
            nextMeshBlock.transform.position = new Vector3(0, 18, -2);
            nextMeshBlock.transform.localScale = Vector3.one;

            this.nextMeshBlock = nextMeshBlock;
            waitingForLaunch = true;

            while (waitingForLaunch && victory == false) yield return true;
            if (victory) yield break;

            var fromLaunchPosition = nextMeshBlock.transform.position;
            for (var dt = 0f; dt < 1; dt += Time.deltaTime / 0.15f)
            {
                nextMeshBlock.transform.position = Vector3.Lerp(fromLaunchPosition, nextLaunchPosition, dt);
                yield return true;
            }
            nextMeshBlock.transform.position = nextLaunchPosition;
            nextMeshBlock.GetRigidbody.isKinematic = false;
            nextMeshBlock.ResetStatus();

            if (victory) yield break;
        }

        yield return new WaitForSeconds(3);

        bool allSnuffed = false;
        while (allSnuffed == false)
        {
            allSnuffed = true;
            foreach (var meshBlock in availableMeshBlocks) if (meshBlock.MeshIgnited && meshBlock.MeshSnuffed == false) allSnuffed = false;
            yield return true;
            yield return true;
        }

        //all is snuffed
        if (targetMesh.MeshIgnited == false) StartCoroutine(CoDefeat());
    }

    IEnumerator CoVictory()
    {
        yield return new WaitForSeconds(1);
        yield return true;

        callbackVictory?.Invoke();
    }

    IEnumerator CoDefeat()
    {
        yield return true;

        callbackDefeat?.Invoke();
    }

    void UpdateLaunch()
    {
        if (nextMeshBlock != null && waitingForLaunch && Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(ray, out hitInfo, 9999, LayerUtil.MASK_WRAP_MESH) && hitInfo.transform != null) return;

            var plane = new Plane(Vector3.forward, Vector3.zero);
            float distance;
            plane.Raycast(ray, out distance);

            var pos = ray.GetPoint(distance);
            pos.y = 16;

            //baseMeshBlock.MakeInstance(pos);
            nextLaunchPosition = pos;
            waitingForLaunch = false;
        }
    }

    void UpdateCheckForVictory()
    {
        if (targetMesh.MeshIgnited && victory == false)
        {
            victory = true;
            StopAllCoroutines();
            StartCoroutine(CoVictory());
        }
    }

    private void Update()
    {
        UpdateLaunch();
        UpdateCheckForVictory();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public Action callbackRestart;
    public Action<int> callbackRemainingMeshblock;
    public Action callbackVictory;
    public Action callbackDefeat;

    public StageCollection stageCollection;

    [Space]
    public Transform areaTarget;
    public Transform areaPreview;

    [Space]
    public MeshBlock baseMeshBlock;
    public WrapMesh baseWrapMesh;

    [Space]
    public Transform previewArea;
    public FirestarterArea fireStarterArea;
    public ParticleSystem fireParticles;

    public Camera previewCamera;

    WrapMeshInteraction targetMesh;
    List<MeshBlock> availableMeshBlocks = new List<MeshBlock>();
    int nextMeshBlockIndex;
    MeshBlock nextMeshBlock;
    bool newWaitingForLaunch = false;
    bool waitingForLaunch = false;
    Vector3 nextLaunchPosition;
    bool gameover = false;
    bool victory = false;

    Stage currentStage;


    IEnumerator Start()
    {
        Application.targetFrameRate = 60;
        previewCamera.backgroundColor = Color.clear;

        yield return true;
        baseMeshBlock.gameObject.SetActive(false);

        RestartGame(false);
    }

    public void RestartGame(bool nextStage)
    {
        StopAllCoroutines();

        if (nextStage)
            currentStage = stageCollection.NextStage;
        else
            currentStage = stageCollection.CurrentStage;

        callbackRestart?.Invoke();
        StartCoroutine(CoGameplay());
    }

    IEnumerator CoGameplay()
    {
        gameover = false;
        victory = false;
        if (fireParticles != null) fireParticles.Clear();
        foreach (var block in availableMeshBlocks) Destroy(block.gameObject);
        availableMeshBlocks.Clear();
        if (targetMesh != null) Destroy(targetMesh.gameObject);
        fireStarterArea.Restart();

        // collect child transforms from items container
        List<StageItem> stageItems = new List<StageItem>(currentStage.GetComponentsInChildren<StageItem>(true));
        StageTarget stageTarget = currentStage.GetComponentInChildren<StageTarget>(true);

        // create new meshblocks here
        foreach (var stageItem in stageItems)
        {
            var newMeshBlock = baseMeshBlock.MakeInstanceFromModel(stageItem);
            newMeshBlock.GetRigidbody.isKinematic = true; //set to be kinematic by default
            newMeshBlock.template = false;
            newMeshBlock.gameObject.SetActive(true);
            newMeshBlock.transform.position = areaPreview.position;
            availableMeshBlocks.Add(newMeshBlock);

            stageItem.gameObject.SetActive(false);
        }

        // create the target here
        targetMesh = baseWrapMesh.MakeInstanceFromModel(stageTarget).GetComponent<WrapMeshInteraction>();
        targetMesh.transform.position = areaTarget.position;
        stageTarget.gameObject.SetActive(false);

        yield return true;
        yield return true;

        foreach (var meshBlock in availableMeshBlocks) meshBlock.gameObject.SetActive(false);
        
        for (var i = 0; i < availableMeshBlocks.Count; i++)
        {
            if (i < availableMeshBlocks.Count - 1) availableMeshBlocks[i + 1].gameObject.SetActive(true);

            var nextMeshBlock = availableMeshBlocks[i];
            nextMeshBlock.MovementMode = true;
            nextMeshBlock.GetRigidbody.isKinematic = true;
            nextMeshBlock.gameObject.SetActive(true);

            callbackRemainingMeshblock?.Invoke(availableMeshBlocks.Count - i - 1);

            for (var dt = 0f; dt < 1; dt += Time.deltaTime / 0.25f)
            {
                nextMeshBlock.transform.position = Vector3.Lerp(new Vector3(0, 22, -2), new Vector3(0, 20, 0), dt);
                nextMeshBlock.transform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), new Vector3(1, 1, 1), dt);
                yield return true;
            }
            nextMeshBlock.transform.position = new Vector3(0, 20, 0);
            nextMeshBlock.transform.localScale = Vector3.one;

            this.nextMeshBlock = nextMeshBlock;
            nextMeshBlock.MovementMode = false;

            waitingForLaunch = true;
            newWaitingForLaunch = true;

            yield return true;
            newWaitingForLaunch = false;
            while (waitingForLaunch && victory == false) yield return true;
            if (victory) yield break;

            //var fromLaunchPosition = nextMeshBlock.transform.position;
            //for (var dt = 0f; dt < 1; dt += Time.deltaTime / 0.15f)
            //{
            //    nextMeshBlock.transform.position = Vector3.Lerp(fromLaunchPosition, nextLaunchPosition, dt);
            //    yield return true;
            //}
            //nextMeshBlock.transform.position = nextLaunchPosition;
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
        gameover = true;
        yield return new WaitForSeconds(1);
        yield return true;

        callbackVictory?.Invoke();
    }

    IEnumerator CoDefeat()
    {
        gameover = true;
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

    void UpdateDrag()
    {
        if (nextMeshBlock == null || waitingForLaunch == false) return;

        if (newWaitingForLaunch)
        {
            nextMeshBlock.MovementMode = true;
            nextMeshBlock.GetRigidbody.isKinematic = true;
            return;
        }

        if (nextMeshBlock != null && waitingForLaunch && Input.GetMouseButton(0))
        {
            nextMeshBlock.MovementMode = true;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(ray, out hitInfo, 9999, LayerUtil.MASK_WRAP_MESH) && hitInfo.transform != null) return;

            var plane = new Plane(Vector3.forward, new Vector3(0, 0, 1));
            float distance;
            plane.Raycast(ray, out distance);

            var pos = ray.GetPoint(distance);

            nextMeshBlock.GetRigidbody.velocity = (pos - nextMeshBlock.transform.position).normalized * 25;
        }

        if (nextMeshBlock != null && waitingForLaunch && Input.GetMouseButtonUp(0))
        {
            nextMeshBlock.GetRigidbody.velocity = Vector3.zero;
            nextMeshBlock.MovementMode = false;
            waitingForLaunch = false;
        }
    }

    void UpdateCheckForVictory()
    {
        if (targetMesh != null && targetMesh.MeshIgnited && victory == false)
        {
            victory = true;
            StopAllCoroutines();
            StartCoroutine(CoVictory());
        }
    }

    private void Update()
    {
        //UpdateLaunch();
        UpdateDrag();
        //UpdateDragTetris();
        UpdateCheckForVictory();
    }

    Vector3 recentNextMeshBlockPosition;
    int nextMeshBlockPauseFrames = 0;
    void UpdateDragTetris()
    {
        if (nextMeshBlock == null || waitingForLaunch == false) return;

        if (newWaitingForLaunch)
        {
            nextMeshBlock.MovementMode = true;
            nextMeshBlock.GetRigidbody.velocity = Vector3.down * 6;
            recentNextMeshBlockPosition = nextMeshBlock.transform.position;
            nextMeshBlockPauseFrames = 0;
            return;
        }

        nextMeshBlock.GetRigidbody.velocity = Vector3.down * 6;


        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.forward, new Vector3(0, 0, 1));
            float distance;
            plane.Raycast(ray, out distance);
            var pos = ray.GetPoint(distance);

            if (pos.x + 0.2f < recentNextMeshBlockPosition.x) nextMeshBlock.GetRigidbody.velocity += Vector3.left * 5;
            if (pos.x - 0.2f > recentNextMeshBlockPosition.x) nextMeshBlock.GetRigidbody.velocity += Vector3.right * 5;
        }

        // CHECK FOR STOPPING CONDITION
        var ydiff = nextMeshBlock.transform.position.y - recentNextMeshBlockPosition.y;
        recentNextMeshBlockPosition = nextMeshBlock.transform.position;
        var ydiff_moving = ydiff < -0.001f;

        if (ydiff_moving == false && nextMeshBlock.SoftCollided) {
            nextMeshBlock.MovementMode = false;
            waitingForLaunch = false;
            return;
        }


    }
}

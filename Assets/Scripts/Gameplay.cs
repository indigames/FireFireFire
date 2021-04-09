using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gameplay : MonoBehaviour
{
    static float SNUFF_FIRESTARTER_DURATION = 3;

    public Action callbackRestart;
    public Action<int> callbackRemainingMeshblock;
    public Action callbackVictory;
    public Action callbackDefeat;

    public StageCollection stageCollection;

    [Space]
    public Transform areaTarget;
    public Transform areaPreview;
    public AudioPool audioSpawn;

    [Space]
    public MeshBlock baseMeshBlock;
    public WrapMesh baseWrapMesh;

    [Space]
    public Transform previewArea;
    public FirestarterArea fireStarterArea;
    public ParticleSystem confettiParticle;
    public ParticleSystem crumbleParticle;

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

    GameObject attachmentInstance = null;

    Stage currentStage;

    public MeshBlock DraggingMeshBlock => dragging && gameover == false ? nextMeshBlock : null;

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
        snuffingFirestarter = false;
        gameover = false;
        victory = false;
        confettiParticle.Clear();
        crumbleParticle.Clear();
        foreach (var block in availableMeshBlocks) Destroy(block.gameObject);
        availableMeshBlocks.Clear();
        if (targetMesh != null) Destroy(targetMesh.gameObject);
        fireStarterArea.Restart();
        if (attachmentInstance != null)
        {
            Destroy(attachmentInstance);
            attachmentInstance = null;
        }

        // collect child transforms from items container
        List<StageItem> stageItems = new List<StageItem>(currentStage.GetComponentsInChildren<StageItem>(true));
        StageTarget stageTarget = currentStage.GetComponentInChildren<StageTarget>(true);

        // create new meshblocks here
        var mass = baseMeshBlock.GetRigidbody.mass;
        foreach (var stageItem in stageItems)
        {
            var newMeshBlock = baseMeshBlock.MakeInstanceFromModel(stageItem);
            newMeshBlock.GetRigidbody.isKinematic = true; //set to be kinematic by default
            newMeshBlock.template = false;
            newMeshBlock.gameObject.SetActive(true);
            newMeshBlock.transform.position = areaPreview.position;
            newMeshBlock.GetRigidbody.mass = mass;
            mass *= 0.7f;
            availableMeshBlocks.Add(newMeshBlock);
        }

        // create the target here
        targetMesh = baseWrapMesh.MakeInstanceFromModel(stageTarget).GetComponent<WrapMeshInteraction>();
        targetMesh.override_snuff_duration = 0.7f;
        targetMesh.spreadSpeed *= 2;
        targetMesh.transform.position = areaTarget.position + Vector3.up * currentStage.verticalOffset;
        foreach (var collider in targetMesh.GetComponentsInChildren<Collider>())
            if (collider.isTrigger == false && collider.gameObject != targetMesh.gameObject) Destroy(collider.gameObject);

        var attachmentType = AttachmentUtil.GetAttachmentTemplate(stageTarget.attachment);
        if (attachmentType != null)
        {
            this.attachmentInstance = Instantiate(attachmentType, targetMesh.transform);
            this.attachmentInstance.gameObject.SetActive(false);
        }

        yield return true;
        yield return true;

        if (attachmentInstance != null)
        {
            this.attachmentInstance.transform.position = targetMesh.GetComponent<MeshRenderer>().bounds.center + Vector3.forward;
            this.attachmentInstance.gameObject.SetActive(true);
        }

        crumbleParticle.Clear();

        foreach (var meshBlock in availableMeshBlocks)
        {
            meshBlock.transform.position = areaPreview.position - meshBlock.GetBoundOffset();
            meshBlock.gameObject.SetActive(false);
        }

        StartCoroutine(CoCheckDefeat());
        yield return new WaitForSeconds(1.25f);

        for (var i = 0; i < availableMeshBlocks.Count; i++)
        {
            //if (i < availableMeshBlocks.Count - 1) availableMeshBlocks[i + 1].gameObject.SetActive(true);44

            var nextMeshBlock = availableMeshBlocks[i];
            nextMeshBlock.MovementMode = true;
            nextMeshBlock.GetRigidbody.isKinematic = true;
            nextMeshBlock.gameObject.SetActive(true);

            callbackRemainingMeshblock?.Invoke(availableMeshBlocks.Count - i);

            //for (var dt = 0f; dt < 1; dt += Time.deltaTime / 0.25f)
            //{
            //    nextMeshBlock.transform.position = Vector3.Lerp(new Vector3(0, 22, -2), new Vector3(0, 20, 0), dt);
            //    nextMeshBlock.transform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), new Vector3(1, 1, 1), dt);
            //    yield return true;
            //}
            //nextMeshBlock.transform.position = new Vector3(0, 20, 0);
            //nextMeshBlock.transform.localScale = Vector3.one;

            this.nextMeshBlock = nextMeshBlock;
            nextMeshBlock.MovementMode = false;

            waitingForLaunch = true;
            newWaitingForLaunch = true;

            yield return true;
            newWaitingForLaunch = false;
            while (waitingForLaunch && victory == false) yield return true;
            yield return new WaitForSeconds(0.1f);
            nextMeshBlock.MovementMode = false;
            nextMeshBlock.transform.localScale = Vector3.one;

            callbackRemainingMeshblock?.Invoke(availableMeshBlocks.Count - i - 1);

            StartCoroutine(CoSnuffFirestarter());
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

    IEnumerator CoCheckDefeat()
    {
        while (true)
        {
            if (fireStarterArea.FireEnabled)
            {
                yield return true;
                continue;
            }

            if (victory) yield break;

            bool burning = false;
            foreach (var meshBlock in availableMeshBlocks) if (meshBlock.MeshIgnited && meshBlock.MeshSnuffed == false) burning = true;

            //all is snuffed
            if (burning == false) break;
            yield return new WaitForSeconds(0.1f);
        }

        StopAllCoroutines();
        StartCoroutine(CoDefeat());
    }

    bool snuffingFirestarter;
    IEnumerator CoSnuffFirestarter()
    {
        if (snuffingFirestarter) yield break;
        snuffingFirestarter = true;
        yield return new WaitForSeconds(SNUFF_FIRESTARTER_DURATION);
        fireStarterArea.DisableFire();
    }

    IEnumerator CoVictory()
    {
        if (gameover) yield break;

        gameover = true;
        fireStarterArea.PlayVictory();

        //WAIT FOR THE BURN TO FINISHES
        while (targetMesh.MeshSnuffRatio < 0.6f) yield return true;

        confettiParticle.transform.position = targetMesh.transform.position + Vector3.back * 2;
        confettiParticle.Stop();
        yield return true;
        confettiParticle.Play();
        yield return new WaitForSeconds(1.2f);
        targetMesh.StartCrumble();

        yield return new WaitForSeconds(1.5f);
        callbackVictory?.Invoke();
    }

    IEnumerator CoDefeat()
    {
        gameover = true;
        fireStarterArea.PlayDefeat();
        yield return true;

        callbackDefeat?.Invoke();
    }

    Vector2 InputPosition {
        get {
            if (Input.touchCount > 0) return Input.touches[0].position;
            else return Input.mousePosition;
        }
    }

    void UpdateLaunch()
    {
        if (nextMeshBlock != null && waitingForLaunch && Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(InputPosition);
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

    Coroutine coZoomIn = null;
    bool dragging = false;
    void UpdateDrag()
    {
        if (gameover) return;
        if (nextMeshBlock == null || waitingForLaunch == false) return;

        if (newWaitingForLaunch)
        {
            nextMeshBlock.MovementMode = true;
            nextMeshBlock.GetRigidbody.isKinematic = true;
            dragging = false;
            return;
        }

        var ray = Camera.main.ScreenPointToRay(InputPosition);
        var plane = new Plane(Vector3.forward, new Vector3(0, 0, 1));
        float distance;
        plane.Raycast(ray, out distance);
        var pos = ray.GetPoint(distance);

        if (nextMeshBlock != null && waitingForLaunch && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                pos = GetSafeSpawnPosition(pos);
                var nextPos = nextMeshBlock.transform.position;
                nextPos.x = pos.x;
                nextPos.y = pos.y;
                nextMeshBlock.transform.position = nextPos;
                nextMeshBlock.MovementMode = true;
                coZoomIn = StartCoroutine(CoPopupTransform(nextMeshBlock.transform));
                dragging = true;

                audioSpawn.Play();
            }
        }

        if (dragging && nextMeshBlock != null && waitingForLaunch && Input.GetMouseButton(0))
            nextMeshBlock.GetRigidbody.velocity = (pos - nextMeshBlock.transform.position).normalized * 25;

        if (dragging && nextMeshBlock != null && waitingForLaunch && Input.GetMouseButtonUp(0))
        {
            if (coZoomIn != null)
            {
                StopCoroutine(coZoomIn);
                coZoomIn = null;
            }
            nextMeshBlock.transform.localScale = Vector3.one;
            nextMeshBlock.GetRigidbody.velocity = Vector3.zero;
            waitingForLaunch = false;
            dragging = false;
        }
    }

    //prevent spawning overlap
    bool CheckPositionOverlap(Vector3 position)
    {
        var hits = Physics.SphereCastAll(position, 1f, Vector3.forward);
        HashSet<MeshBlock> blocks = new HashSet<MeshBlock>();
        foreach (var hit in hits)
            blocks.Add(hit.collider.GetComponentInParent<MeshBlock>());

        if (blocks.Count > 1) return true;
        return false;
    }

    Vector3 GetSafeSpawnPosition(Vector3 position)
    {
        if (position.y < 0.5f) position.y = 0.5f;
        while (CheckPositionOverlap(position))
            position += Vector3.up * 0.25f;
        return position;
    }

    float EaseOutElastic(float t)
    {
        float b = 0;
        float c = 1;
        float d = 1;
        float s = 1.70158f;
        float p = 0;
        float a = c;
        if (t == 0) return b;
        if ((t /= d) == 1) return b + c;
        if (p == 0) p = d * 0.3f;
        if (a < Mathf.Abs(c))
        {
            a = c;
            s = p / 4;
        }
        else s = p / (2 * Mathf.PI) * Mathf.Asin(c / a);
        return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b;
    }

    IEnumerator CoPopupTransform(Transform tr)
    {
        tr.localScale = Vector3.one * 0.5f;
        for (var i = 0f; i < 1f; i += Time.deltaTime / 0.3f)
        {
            yield return true;
            tr.localScale = Vector3.one * (0.5f + 0.5f * EaseOutElastic(Mathf.Pow(i, 1.5f)));
        }
        tr.localScale = Vector3.one;
        coZoomIn = null;
    }

    Vector3 CurrentTouchPosition => GetCurrentTouchPosition();
    Vector3 GetCurrentTouchPosition()
    {
        nextMeshBlock.MovementMode = true;
        var ray = Camera.main.ScreenPointToRay(InputPosition);
        //RaycastHit hitInfo;
        //if (Physics.Raycast(ray, out hitInfo, 9999, LayerUtil.MASK_WRAP_MESH) && hitInfo.transform != null) return;

        var plane = new Plane(Vector3.forward, new Vector3(0, 0, 1));
        float distance;
        plane.Raycast(ray, out distance);

        var pos = ray.GetPoint(distance);
        return pos;
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
        UpdateCheckForVictory();
        UpdateVisualTarget();
    }

    void UpdateVisualTarget()
    {
        return;
        if (targetMesh == null || targetMesh.Crumbling) return;
        if (targetMesh.MeshSnuffRatio < 0.8f) return;
        targetMesh.StartCrumble();
    }

    Vector3 recentNextMeshBlockPosition;
}

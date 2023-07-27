using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(Rigidbody))]
public class MeshBlock : MonoBehaviour
{
    const float COLLISION_TRESHOLD = 0.5f;
    const float AUTO_SLEEP_DURATION = 5f;
    const float BURN_DELAY_DURATION = 0.5f;

    [Header("Root")]
    public MeshCollider meshCollider;
    public Rigidbody rigidBody;
    public WrapMesh wrapMesh;
    public WrapMeshInteraction wrapMeshInteraction;
    [Space]


    [Header("Events")]
    public IntEventChannel OnScoreAddedEvent;

    [Header("Setting")]

    public bool template = true;
    [Range(0, 1)]
    public float drag_rate = 0.25f;
    public bool movementMode = false;

    public StageItem stageItem;

    private VoidEventChannel OnObjectBurnedEvent;

    private float duration = AUTO_SLEEP_DURATION;
    private float burndelay = BURN_DELAY_DURATION;
    private float collision_delay = COLLISION_TRESHOLD;

    private float baseMass;
    private float baseDrag;
    private float baseAngularDrag;
    private bool collided = false;

    // Use this for initialization
    void Start()
    {
        baseMass = rigidBody.mass;
        baseDrag = rigidBody.drag;
        baseAngularDrag = rigidBody.angularDrag;

        if (HasDefaultCollider() == false) meshCollider.sharedMesh = wrapMesh.colliderMesh;

        if (template) gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (!OnObjectBurnedEvent)
        {

            OnObjectBurnedEvent = new();
            wrapMeshInteraction.OnObjectBurnedAction = OnObjectBurnedEvent;
        }

        OnObjectBurnedEvent.OnEventRaised += OnObjectBurnedReceived;
    }

    void OnDisable()
    {
        OnObjectBurnedEvent.OnEventRaised -= OnObjectBurnedReceived;
    }

    bool HasDefaultCollider()
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            if (collider.gameObject == gameObject) continue;
            if (collider.GetComponent<WrapMesh>()) continue;
            if (collider.isTrigger) continue;
            return true;
        }
        return false;
    }

    public bool MeshSnuffed => wrapMeshInteraction.MeshSnuffed;
    public bool MeshIgnited => wrapMeshInteraction.MeshIgnited;

    public void Restart()
    {
        burndelay = BURN_DELAY_DURATION;
        collision_delay = COLLISION_TRESHOLD;
        MovementMode = false;

        collided = false;
        rigidBody.drag = baseDrag;
        rigidBody.angularDrag = baseAngularDrag;

        wrapMeshInteraction.Restart();
        transform.localScale = Vector3.one;
    }

    public Vector3 GetBoundOffset()
    {
        var bound = wrapMesh.meshRenderer.bounds;
        return bound.center - transform.position;
    }

    public bool MovementMode
    {
        set => SetMovementMode(value);
        get => movementMode;
    }

    public void SetMovementMode(bool value)
    {
        movementMode = value;
        ResetStatus();
        if (value)
        {
            rigidBody.isKinematic = false;
            rigidBody.mass = 0.0001f;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rigidBody.mass = baseMass;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        }
    }

    public void ResetStatus()
    {
        duration = AUTO_SLEEP_DURATION;
        burndelay = BURN_DELAY_DURATION;
        collision_delay = COLLISION_TRESHOLD;
        wrapMeshInteraction.canSpreadTo = false;
    }

    private void Update()
    {
        if (movementMode) return;

        if (collided && burndelay > 0)
        {
            burndelay -= Time.deltaTime;
            if (burndelay < 0) wrapMeshInteraction.canSpreadTo = true;
        }

        if (duration > 0)
        {
            if (collided)
            {
                rigidBody.drag += 15f * Time.deltaTime * drag_rate;
                rigidBody.angularDrag += 15f * Time.deltaTime * drag_rate;
            }

            duration -= Time.deltaTime;
            if (duration < 0)
            {
                rigidBody.isKinematic = true;
                wrapMeshInteraction.canSpreadTo = true;

            }
        }
        //if (rigidBody != null && rigidBody.IsSleeping())
        //{
        //    rigidBody.isKinematic = true;
        //    wrapMeshInteraction.canSpreadTo = true;
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (movementMode) return;
        collided = true;
        //if (collision_delay > 0)
        //{
        //    collision_delay -= Time.fixedDeltaTime;
        //    Debug.Log(collision_delay);
        //    if (collision_delay < 0) collided = true;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.layer == LayerUtil.LAYER_WRAP_TRIGGER)
        //{
        //    Debug.Log(other);
        //}
    }

    public MeshBlock MakeInstance(Vector3 position, Transform parent = null)
    {
        if (parent == null) parent = transform.parent;
        var result = Instantiate(this, parent);


        if (result.wrapMesh != null) result.wrapMesh.originalTransform = transform;

        result.transform.position = position;
        result.template = false;
        result.gameObject.SetActive(true);
        return result;
    }

    public MeshBlock MakeInstanceFromModel(StageItem item, Transform parent = null)
    {
        Transform target = item.transform;

        gameObject.name = target.name;
        if (parent == null) parent = transform.parent;

        var result = Instantiate(this, parent);
        result.transform.position = target.position;
        result.template = false;
        result.gameObject.SetActive(true);
        result.wrapMeshInteraction.spreadSpeed = item.spreadSpeed;

        if (result.wrapMesh != null) result.wrapMesh.originalTransform = transform;

        StageItem stageItem = Instantiate(item);
        stageItem.gameObject.gameObject.SetActive(true);
        stageItem.transform.SetParent(result.transform);

        result.stageItem = stageItem;
        result.wrapMesh.SetUp(stageItem);
        result.wrapMeshInteraction.SetUp();

        return result;
    }

    private void OnObjectBurnedReceived()
    {
        OnScoreAddedEvent.RaiseEvent(stageItem.info.GetScore());
    }

    void StartShrinkage()
    {
        StartCoroutine(CoShrinkage());
    }

    IEnumerator CoShrinkage()
    {
        yield return new WaitForSeconds(1);
        for (var i = 0f; i < 1f; i += Time.deltaTime / 3)
        {
            transform.localScale = Vector3.one * (1 - 0.1f * i);
            yield return true;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("StickyObstacle"))
        {
            rigidBody.isKinematic = true;
        }
    }
}
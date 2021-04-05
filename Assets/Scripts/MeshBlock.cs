using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(Rigidbody))]
public class MeshBlock : MonoBehaviour
{
    const float COLLISION_TRESHOLD = 0.5f;
    const float AUTO_SLEEP_DURATION = 5f;
    const float BURN_DELAY_DURATION = 0.5f;

    MeshCollider meshCollider;
    Rigidbody rigidBody;
    public WrapMesh wrapMesh;
    public WrapMeshInteraction wrapMeshInteraction;

    public bool template = true;
    [Range(0, 1)]
    public float drag_rate = 0.25f;
    float duration = AUTO_SLEEP_DURATION;
    float burndelay = BURN_DELAY_DURATION;
    float collision_delay = COLLISION_TRESHOLD;

    public Rigidbody GetRigidbody => GetComponent<Rigidbody>();
    float baseMass;
    float baseDrag;
    float baseAngularDrag;
    public bool movementMode = false;
    bool collided = false;

    // Use this for initialization
    IEnumerator Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        rigidBody = GetComponent<Rigidbody>();

        yield return true;

        baseMass = rigidBody.mass;
        baseDrag = rigidBody.drag;
        baseAngularDrag = rigidBody.angularDrag;

        if (HasDefaultCollider() == false) meshCollider.sharedMesh = wrapMesh.colliderMesh;
        foreach (var decor in meshCollider.GetComponentsInChildren<StageDecor>()) decor.gameObject.SetActive(false);

        if (template) gameObject.SetActive(false);
        GetComponentInChildren<WrapMeshInteraction>().onMeshIgnited += StartShrinkage;
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
        var bound = wrapMesh.GetComponent<MeshRenderer>().bounds;
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
        } else
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
        result.transform.position = position;
        result.template = false;
        result.gameObject.SetActive(true);
        return result;
    }

    public MeshBlock MakeInstanceFromModel(Transform target, Transform parent = null) {
        gameObject.name = target.name;
        if (parent == null) parent = transform.parent;

        var result = Instantiate(this, parent);
        result.transform.position = target.position;

        target = Instantiate(target);
        target.gameObject.SetActive(true);
        target.SetParent(result.transform);

        result.template = false;
        result.gameObject.SetActive(true);
        return result;
    }

    public MeshBlock MakeInstanceFromModel(StageItem item, Transform parent = null)
    {
        var result = MakeInstanceFromModel(item.transform, parent);
        result.GetComponentInChildren<WrapMeshInteraction>().spreadSpeed = item.spreadSpeed;
        return result;
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
}
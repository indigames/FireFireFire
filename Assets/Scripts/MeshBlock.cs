using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(Rigidbody))]
public class MeshBlock : MonoBehaviour
{
    MeshCollider meshCollider;
    Rigidbody rigidBody;
    public WrapMesh wrapMesh;
    public WrapMeshInteraction wrapMeshInteraction;

    public bool template = true;
    float duration = 2.5f;

    public Rigidbody GetRigidbody => GetComponent<Rigidbody>();
    float baseMass;
    bool movementMode = false;

    // Use this for initialization
    IEnumerator Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        rigidBody = GetComponent<Rigidbody>();

        yield return true;

        baseMass = rigidBody.mass;
        meshCollider.sharedMesh = wrapMesh.colliderMesh;

        if (template) gameObject.SetActive(false);
    }

    public bool MeshSnuffed => wrapMeshInteraction.MeshSnuffed;
    public bool MeshIgnited => wrapMeshInteraction.MeshIgnited;

    public void Restart()
    {
        MovementMode = false;
        wrapMeshInteraction.Restart();
    }

    public bool MovementMode
    {
        set
        {
            SetMovementMode(value);
        }
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
        duration = 2.5f;
        wrapMeshInteraction.canSpreadTo = false;
    }

    private void Update()
    {
        //if (movementMode == false)
        //{
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration < 0)
            {
                rigidBody.isKinematic = true;
                wrapMeshInteraction.canSpreadTo = true;
            }
        }
        if (rigidBody != null && rigidBody.IsSleeping())
        {
            rigidBody.isKinematic = true;
            wrapMeshInteraction.canSpreadTo = true;
        }
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
}
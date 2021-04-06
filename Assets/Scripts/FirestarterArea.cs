using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterArea : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem fireParticle;
    bool fireEnabled;

    public bool FireEnabled => fireEnabled;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        StartCoroutine(CoAutoLit());
    }

    public void Restart()
    {
        wrapColliders.Clear();
        animator.Play("Play", 0, 0);
        fireEnabled = true;
    }

    public void DisableFire()
    {
        fireEnabled = false;
        fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    IEnumerator CoAutoLit()
    {
        while (true)
        {
            if (fireEnabled)
                foreach (var wrapMesh in wrapColliders)
                {
                    #pragma warning disable
                    var collider = wrapMesh.GetComponent<MeshCollider>();
                    if (collider == null) continue;
                    var position = collider.ClosestPoint(transform.position);
#pragma warning restore
                    position += Vector3.up * 0.1f;
                    collider.GetComponentInParent<WrapMeshInteraction>().SpreadFromPoint(position);
                }
            yield return new WaitForSeconds(0.2f);
        }
    }


    HashSet<Collider> wrapColliders = new HashSet<Collider>();

    public void WrapMeshTriggerEnter(Collider other)
    {
        wrapColliders.Add(other);
    }

    public void WrapMeshTriggerExit(Collider other)
    {
        wrapColliders.Remove(other);
    }

    public void PlayVictory()
    {
        animator.Play("Victory");
    }

    public void PlayDefeat()
    {
        animator.Play("Defeat");
    }
}

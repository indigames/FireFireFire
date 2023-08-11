using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterArea : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem fireParticle;
    bool fireEnabled;

    public bool FireEnabled => fireEnabled;
    public float DisableAfter;

    [SerializeField] private BoolEventChannel _onPauseGamePlayChange;

    private void OnDisable()
    {
        StopAllCoroutines();
        _onPauseGamePlayChange.OnEventRaised -= OnPauseGamePlayReceived;
    }

    private void OnEnable()
    {
        StartCoroutine(CoAutoLit());
        _onPauseGamePlayChange.OnEventRaised += OnPauseGamePlayReceived;
    }

    private void OnPauseGamePlayReceived(bool value)
    {
        isPauseGamePlay = value;
    }

    public void Restart()
    {
        wrapColliders.Clear();
        animator.Play("Default", 0, 0);
        fireEnabled = true;
    }

    public void EnableFire()
    {
        animator.Play("Play", 0, 0);
        fireEnabled = true;
        _FireDurationLeft = Time.time + DisableAfter;
    }

    public void DisableFire()
    {
        fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private float _FireDurationLeft;
    private bool isPauseGamePlay;
    IEnumerator CoAutoLit()
    {
        while (true)
        {
            if (fireEnabled)
            {

                if (Time.time > _FireDurationLeft && !isPauseGamePlay)
                {
                    fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    yield return null;
                    continue;
                }
                _FireDurationLeft -= Time.deltaTime;

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

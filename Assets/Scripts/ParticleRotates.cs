using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotates : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    Matrix4x4 worldToLocalMat;
    Vector3 position;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        worldToLocalMat = transform.worldToLocalMatrix;
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            var count = ps.GetParticles(particles);
            for (var i = 0; i < count; i++)
            {
                particles[i].position -= position;
                particles[i].position = transform.localToWorldMatrix * (worldToLocalMat * particles[i].position);
                particles[i].position += transform.position;
            }
            ps.SetParticles(particles, count);
            transform.hasChanged = false;

            worldToLocalMat = transform.worldToLocalMatrix;
            position = transform.position;
        }
    }
}

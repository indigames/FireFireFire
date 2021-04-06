using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    public float randomOffset = 0.4f;
    public List<AudioSource> sources;
    int currentIndex;
    private void Awake()
    {
        sources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
    }

    public void Play()
    {
        var source = sources[currentIndex];
        source.pitch = 1 + (UnityEngine.Random.value - 1) * randomOffset;
        source.Play();
        currentIndex = (currentIndex + 1) % sources.Count;
    }

}

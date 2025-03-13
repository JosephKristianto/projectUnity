using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAudioSourceReader : MonoBehaviour
{
    // Start is called before the first frame update

    AudioSource audioSource;
    public float time;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        time = audioSource.time;
    }
}

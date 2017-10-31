using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeAudioPitch : MonoBehaviour
{
    private AudioSource m_AudioSource;
    public float MinPitch = 0.5f;
    public float MaxPitch = 2.0f;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (m_AudioSource == null)
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        if (m_AudioSource != null)
        {
            m_AudioSource.pitch = Random.Range(MinPitch, MaxPitch);
        }
    }
}

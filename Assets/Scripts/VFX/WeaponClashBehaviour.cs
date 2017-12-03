using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClashBehaviour : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private ParticleSystem m_ParticleSystem;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_ParticleSystem = GetComponent<ParticleSystem>();    
    }

    public void PlayClashEffect()
    {
        m_AudioSource.Play();
        m_ParticleSystem.Play();
    }

    public void PlayBleedEffect()
    {

    }

    public void PlayBlockEffect()
    {

    }

    public void PlayMissEffect()
    {

    }
}

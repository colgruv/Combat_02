using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioParticleEffect : Effect
{
    private AudioSource m_AudioSource;
    private ParticleSystem m_ParticleSystem;

    public override void Play()
    {
        base.Play();

        // If an AudioSource exists on this object, play it.
        if (!m_AudioSource)
            m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource)
            m_AudioSource.Play();

        // If a ParticleSystem exists on this object, play it.
        if (!m_ParticleSystem)
            m_ParticleSystem = GetComponent<ParticleSystem>();
        if (m_ParticleSystem)
            m_ParticleSystem.Play();
    }

    void Update()
    {
        // If both AudioSource and ParticleSystem finish, finish the effect.
        if (!m_AudioSource.isPlaying && !m_ParticleSystem.isPlaying)
            onComplete();    
    }
}

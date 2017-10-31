using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGravitonBehaviour : MonoBehaviour
{
    public float Gravity;

    private ParticleSystem m_System;
    private ParticleSystem.Particle[] m_Particles;

	// Use this for initialization
	void Start ()
    {
        m_System = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        initializeParticlesIfNeeded();

        int numParticlesAlive = m_System.GetParticles(m_Particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            m_Particles[i].velocity = (m_Particles[i].position / Gravity) * -1f;
        }

        m_System.SetParticles(m_Particles, numParticlesAlive);
	}

    private void initializeParticlesIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }
}

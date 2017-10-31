using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWeaponColliderBehaviour : MonoBehaviour
{
    private Collider m_WeaponCollider;
    private TrailRenderer m_WeaponTrail;

    /// <summary>
    /// When this script is disabled, disable the relevant weapon collider.
    /// </summary>
    void OnDisable()
    {
        ensureWeaponComponentsAvailable();

        // If weapon collider is still null, this is a remote networked entity -- return
        if (m_WeaponCollider == null)
            return;

        m_WeaponCollider.enabled = false;
        m_WeaponTrail.enabled = false;
    }

    /// <summary>
    /// When this script is enabled (at the start of an attack), enable the relevant weapon collider.
    /// </summary>
    void OnEnable()
    {
        ensureWeaponComponentsAvailable();

        // If weapon collider is still null, this is a remote networked entity -- return
        if (m_WeaponCollider == null)
            return;

        m_WeaponCollider.enabled = true;
        m_WeaponTrail.enabled = true;
    }

    /// <summary>
    /// Make sure there is a reference to the instances of the relevant objects.
    /// </summary>
    private void ensureWeaponComponentsAvailable()
    {
        if (m_WeaponCollider == null)
            m_WeaponCollider = transform.parent.GetComponentInChildren<AttackSource>().GetComponent<Collider>();
        if (m_WeaponTrail == null)
            m_WeaponTrail = transform.parent.GetComponentInChildren<TrailRenderer>();
    }
}

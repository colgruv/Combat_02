using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : AttackObject
{
    // UI Helpers
    public Transform HitIndicator;
    public HealthBar HealthBar;

    // Hit Points
    public float MaxGuard = 200f;
    private float m_CurrentGuard;
    public float MaxHealth = 50f;
    private float m_CurrentHealth;

    // Guard recharge
    public float GuardRechargeDelay = 3f;
    public float GuardRechargeSpeed = 10f;
    private float m_TimeSinceLastHit;

    // Flags
    private bool m_IsAlive;
    public bool IsAlive { get { return m_IsAlive; } }

    // Other Character Components
    private CharacterAnimationHandler m_AnimationHandler;
    private Collider m_Collider;

	// Use this for initialization
	void Start ()
    {
        m_AnimationHandler = transform.parent.GetComponentInChildren<CharacterAnimationHandler>();
        m_Collider = GetComponent<Collider>();

        m_TimeSinceLastHit = 0f;

        m_CurrentGuard = MaxGuard;
        m_CurrentHealth = MaxHealth;

        m_IsAlive = true;

        findOwner(transform);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_IsAlive)
            return;

        if (m_CurrentHealth <= 0f)
        {
            Die();
            return;
        }

        rechargeGuard();

        if (HealthBar)
            HealthBar.DisplayVitals((float)m_CurrentHealth / MaxHealth, (float)m_CurrentGuard / MaxGuard);
	}

    private void rechargeGuard()
    {
        m_TimeSinceLastHit += Time.deltaTime;

        if (m_TimeSinceLastHit > GuardRechargeDelay)
        {
            m_CurrentGuard += GuardRechargeSpeed * Time.deltaTime;
            m_CurrentGuard = Mathf.Min(m_CurrentGuard, MaxGuard);
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        // Only process triggers of type AttackSource
        if (!_other.GetComponent<AttackSource>())
            return;

        // Never process an AttackSource that shares an owner with this AttackTarget
        // OBSOLETE: Potentially not needed if we properly disable AttackTarget on the local player
        //if (_other.GetComponent<AttackSource>().Owner == m_Owner)
            //return;

        if (resolveHit(_other.GetComponent<AttackSource>()))
        {
            triggerHitIndicator(_other);
        }
    }

    /// <summary>
    /// [Deprecated] If a wound occurs, trigger the local HitIndicator object
    /// </summary>
    /// <param name="_other">AttackSource collider, for orientation purposes.</param>
    private void triggerHitIndicator(Collider _other)
    {
        if (!HitIndicator)
            return;

        HitIndicator.forward = _other.transform.right * -1f;
        HitIndicator.GetComponent<ParticleSystem>().Play();
    }

    private bool resolveHit(AttackSource _attack)
    {
        m_TimeSinceLastHit = 0f;

        float hitPool = m_CurrentGuard + _attack.Accuracy;
        float hitRoll = Random.Range(0, hitPool);
        if (hitRoll > m_CurrentGuard)
        {
            // Hit! Subtract from Health
            m_CurrentHealth -= Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);
            return true;
        }

        // Miss! Subtract from Guard
        m_CurrentGuard -= Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);

        // TODO: Determine whether the miss was a result of Block, Dodge, Parry, Armor, or Magic.
        _attack.GetComponentInChildren<WeaponClashBehaviour>().PlayClash();

        return false;
    }

    /// <summary>
    /// Modify any combat mechanic-related components to reflect a dead character.
    /// </summary>
    public void Die()
    {
        if (HealthBar)
            HealthBar.gameObject.SetActive(false);

        m_Collider.enabled = false;
        m_IsAlive = false;
        m_AnimationHandler.OnDeath();
    }

    /// <summary>
    /// Undo the changes made in die() and set character's Health to 1 by default
    /// </summary>
    public void Revive()
    {
        m_Collider.enabled = true;

        HealthBar.gameObject.SetActive(true);
        m_IsAlive = true;

        m_AnimationHandler.OnRevive();

        // Set health to 1 to avoid the character immediately dying again
        m_CurrentHealth = 1;
    }
}

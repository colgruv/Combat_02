using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : AttackObject
{
    // UI Helpers
    public Transform HitIndicator;
    public HealthBar WorldHealthBar;
    public static HealthBar ScreenHealthBar;
    private HealthBar m_CurrentHealthBar;
    public HealthBar CurrentHealthBar { set { m_CurrentHealthBar = value; } }

    // Hit Points
    //public float MaxGuard = 200f;
    //private float m_CurrentGuard;
    //public float MaxHealth = 50f;
    //private float m_CurrentHealth;

    // Guard recharge (ToDo: Move to NetworkCharacterData)
    public float GuardRechargeDelay = 3f;
    public float GuardRechargeSpeed = 10f; 
    private float m_TimeSinceLastHit;

    // Flags
    private bool m_IsAlive;
    public bool IsAlive { get { return m_IsAlive; } }

    // Other Character Components
    private CharacterAnimationHandler m_AnimationHandler;
    private Collider m_Collider;
    private NetworkCharacterData m_CharacterData;

	// Use this for initialization
	void Start ()
    {
        m_AnimationHandler = transform.parent.GetComponentInChildren<CharacterAnimationHandler>();
        m_Collider = GetComponent<Collider>();
        m_CharacterData = transform.parent.GetComponent<NetworkCharacterData>();

        m_TimeSinceLastHit = 0f;

        //m_CurrentGuard = MaxGuard;
        //m_CurrentHealth = MaxHealth;

        m_IsAlive = true;

        findOwner(transform);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // (For now) Don't go past this point if we are dead
        if (!m_IsAlive)
            return;

        // Check to see if we should die
        if (m_CharacterData.GaugeAttributes.Health <= 0f)
        {
            Die();
            return;
        }

        // Perform per-frame gauge recharge
        rechargeGuard();

        // Update gauge display
        if (!m_CurrentHealthBar)
        {
            if (WorldHealthBar.gameObject.activeSelf)
                m_CurrentHealthBar = WorldHealthBar;
            else
                m_CurrentHealthBar = ScreenHealthBar;
        }
        m_CurrentHealthBar.DisplayVitals(
            m_CharacterData.GaugeAttributes.Health / m_CharacterData.GaugeAttributes.MaxHealth, 
            m_CharacterData.GaugeAttributes.Guard / m_CharacterData.GaugeAttributes.MaxGuard);
	}

    private void rechargeGuard()
    {
        m_TimeSinceLastHit += Time.deltaTime;
        GaugeAttributes gAtt = m_CharacterData.GaugeAttributes;

        if (m_TimeSinceLastHit > GuardRechargeDelay)
        {
            gAtt.Guard += GuardRechargeSpeed * Time.deltaTime;
            gAtt.Guard = Mathf.Min(gAtt.Guard, gAtt.MaxGuard);
        }

        m_CharacterData.GaugeAttributes = gAtt;
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
        GaugeAttributes gAtt = m_CharacterData.GaugeAttributes;
        float hitPool = gAtt.Guard + _attack.Accuracy;
        float hitRoll = Random.Range(0, hitPool);

        if (hitRoll > gAtt.Guard)
        {
            // Hit! Subtract from Health
            gAtt.Health -= Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);
            m_CharacterData.GaugeAttributes = gAtt;
            return true;
        }

        // Miss! Subtract from Guard
        gAtt.Guard -= Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);
        m_CharacterData.GaugeAttributes = gAtt;

        // TODO: Determine whether the miss was a result of Block, Dodge, Parry, Armor, or Absorb.
        _attack.GetComponentInChildren<WeaponClashBehaviour>().PlayClash();

        return false;
    }

    /// <summary>
    /// Modify any combat mechanic-related components to reflect a dead character.
    /// </summary>
    public void Die()
    {
        if (m_CurrentHealthBar)
            m_CurrentHealthBar.gameObject.SetActive(false);

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
        if (m_CurrentHealthBar)
            m_CurrentHealthBar.gameObject.SetActive(true);
        m_IsAlive = true;
        m_AnimationHandler.OnRevive();

        // Set health to 1 to avoid the character immediately dying again
        GaugeAttributes gAtt = m_CharacterData.GaugeAttributes;
        gAtt.Health = 1;
        m_CharacterData.GaugeAttributes = gAtt;
    }
}

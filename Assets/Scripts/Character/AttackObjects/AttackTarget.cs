using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : AttackObject
{
    // UI Helpers
    public AudioParticleEffect HitIndicator_Wound;
    public AudioParticleEffect HitIndicator_Parry;

    // Other Character Components
    private NetworkCharacterData m_CharacterData;

	// Use this for initialization
	void Start ()
    {
        m_CharacterData = transform.parent.GetComponent<NetworkCharacterData>();

        findOwner(transform);
    }
	
	// Update is called once per frame
	void Update ()
    {
        

        
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
            triggerHitIndicator(HitIndicator_Wound);
        }
        else
        {
            triggerHitIndicator(HitIndicator_Parry);
        }
    }

    /// <summary>
    /// Immediately play hit effects on AttackTargets belonging to remote players
    /// </summary>
    /// <param name="_effect"></param>
    private void triggerHitIndicator(AudioParticleEffect _effect)
    {
        _effect.Play();
    }

    private bool resolveHit(AttackSource _attack)
    {
        Debug.Log(m_CharacterData.Health);

        float hitPool = m_CharacterData.Guard + _attack.Accuracy;
        float hitRoll = Random.Range(0, hitPool);

        CombatEvent cEvent = new CombatEvent();
        cEvent.attackerNetworkID = _attack.Owner.netId;
        cEvent.defenderNetworkID = Owner.netId;

        if (hitRoll > m_CharacterData.Guard)
        {
            // Hit! Send Wound combatEvent type to Attacker's data
            cEvent.type = CombatEvent.CombatEventType.WOUND;
            cEvent.magnitude = Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);
            _attack.Owner.ClientAttackerReceiveCombatEvent(cEvent);
            return true;
        }

        // Miss! Send Parry combatEvent type to Attacker's data
        // TODO: Determine whether the miss was a result of Block, Dodge, Parry, Armor, or Absorb.
        cEvent.type = CombatEvent.CombatEventType.PARRY;
        cEvent.magnitude = Random.Range(_attack.MinimumDamage, _attack.MaximumDamage);
        _attack.Owner.ClientAttackerReceiveCombatEvent(cEvent);

        // TODO: Determine whether the miss was a result of Block, Dodge, Parry, Armor, or Absorb.
        _attack.GetComponentInChildren<WeaponClashBehaviour>().PlayClashEffect();

        return false;
    }

    /// <summary>
    /// TODO: Display VFX to indicate the reception of certain CombatEvents
    /// </summary>
    /// <param name="_cEvent"></param>
    public void ProcessLocalCombatEvent(CombatEvent _cEvent)
    {
        switch(_cEvent.type)
        {
            case CombatEvent.CombatEventType.PARRY:
                break;
            case CombatEvent.CombatEventType.BLOCK:
                break;
            case CombatEvent.CombatEventType.DODGE:
                break;
            case CombatEvent.CombatEventType.WOUND:
                break;
        }
    }   
}

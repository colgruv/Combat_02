using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct CombatEvent
{
    public enum CombatEventType
    {
        DODGE,
        PARRY,
        BLOCK,
        ABSORB,
        WOUND,
        EFFECT
    }

    public CombatEventType type;
    // ToDo: Add CombatEffect property here for more details
    public float magnitude;
    public float duration;

    public float timeStamp;
}

public struct CoreAttributes
{
    public float strength;
    public float constitution;
    public float dexterity;
    public float intellect;
    public float willpower;
}

public struct GaugeAttributes
{
    public float health;
    public float maxHealth;
    public float tempHealth;

    public float guard;
    public float maxGuard;
    public float tempGuard;

    public float mana;
    public float maxMana;
    public float tempMana;

    public float stamina;
    public float maxStamina;
    public float tempStamina;
}

[NetworkSettings(channel = 2, sendInterval = 0.1f)]
public class NetworkCharacterData : NetworkBehaviour
{
    // Don't know that this is necessary?
    private int m_CharacterID;
    public int CharacterID { get { return m_CharacterID; } }

    private CoreAttributes m_CoreAttributes;
    private GaugeAttributes m_GaugeAttributes;

    // Ordered list of CombatEvents that are processed via CmdProcessCombatEvent
    private List<CombatEvent> m_CombatLog;

    // Health Bars for reflecting combat resources: World is for remote players; Screen is for the local player
    private HealthBar m_WorldHealthBar;
    private HealthBar m_ScreenHealthBar;

	// Use this for initialization
	void Start ()
    { 

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void initializeAttributes()
    {
        m_CoreAttributes = new CoreAttributes();
        m_GaugeAttributes = new GaugeAttributes();
    }

    [Command]
    public void CmdProcessCombatImpact(CombatEvent _impact)
    {

    }
}

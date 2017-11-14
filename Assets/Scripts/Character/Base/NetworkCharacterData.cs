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
    [SyncVar]
    public float Health;
    public float MaxHealth;
    public float TempHealth;

    public float Guard;
    public float MaxGuard;
    public float TempGuard;

    public float Mana;
    public float MaxMana;
    public float TempMana;

    public float Stamina;
    public float MaxStamina;
    public float TempStamina;
}

[NetworkSettings(channel = 2, sendInterval = 0.1f)]
public class NetworkCharacterData : NetworkBehaviour
{
    // Don't know that this is necessary?
    private int m_CharacterID;
    public int CharacterID { get { return m_CharacterID; } }

    [SyncVar]
    private string m_CharacterName;
    public string CharacterName { get { return m_CharacterName; } }

    private CoreAttributes m_CoreAttributes;
    private GaugeAttributes m_GaugeAttributes;
    public GaugeAttributes GaugeAttributes { get { return m_GaugeAttributes; } set { m_GaugeAttributes = value; } }

    // Ordered list of CombatEvents that are processed via CmdProcessCombatEvent
    private List<CombatEvent> m_CombatLog;

    // Health Bars for reflecting combat resources: World is for remote players; Screen is for the local player
    private HealthBar m_WorldHealthBar;
    private HealthBar m_ScreenHealthBar;

	// Use this for initialization
	void Start ()
    {
        initializeMetadata();
        initializeAttributes();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void initializeMetadata()
    {
        if (isLocalPlayer)
            m_CharacterName = PlayerPrefs.GetString("CharacterName");
    }

    private void initializeAttributes()
    {
        m_CoreAttributes = new CoreAttributes();
        m_GaugeAttributes = new GaugeAttributes();

        // [Temp] Default attribute initialization:
        m_GaugeAttributes.MaxHealth = 50f;
        m_GaugeAttributes.Health = m_GaugeAttributes.MaxHealth;
        m_GaugeAttributes.MaxGuard = 200f;
        m_GaugeAttributes.Guard = m_GaugeAttributes.MaxGuard;
    }

    [Command]
    public void CmdProcessCombatImpact(CombatEvent _impact)
    {

    }
}

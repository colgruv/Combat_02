using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct CombatEvent
{
    public NetworkInstanceId attackerNetworkID;
    public NetworkInstanceId defenderNetworkID;

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

public class CombatLogSync : SyncListStruct<CombatEvent>
{
}

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class NetworkCharacterData : NetworkBehaviour
{
    // Child component references
    private CharacterAnimationHandler m_AnimationHandler;
    private Collider m_TargetCollider;
    private AttackTarget m_AttackTarget;

    // Combat attributes
    [SyncVar]
    private float m_Health;
    public float Health { get { return m_Health; } }
    [SyncVar]
    private float m_MaxHealth;
    public float MaxHealth { get { return m_MaxHealth; } }
    [SyncVar]
    private float m_Guard;
    public float Guard { get { return m_Guard; } }
    [SyncVar]
    private float m_MaxGuard;
    public float MaxGuard { get { return m_MaxGuard; } }

    // Track the most recent impact to display feedback on local player.
    [SyncVar]
    private CombatEvent.CombatEventType m_LastEventReceived;
    [SyncVar]
    private int m_LastEventReceivedMagnitude;

    // Guard recharge (ToDo: Move to NetworkCharacterData)
    public float GuardRechargeDelay = 3f;
    public float GuardRechargeSpeed = 10f;
    private float m_TimeSinceLastHit;

    // Don't know that this is necessary?
    private int m_CharacterID;
    public int CharacterID { get { return m_CharacterID; } }

    [SyncVar]
    private string m_CharacterName;
    public string CharacterName { get { return m_CharacterName; } }

    // HealthBar helpers
    public HealthBar WorldHealthBar;
    public static HealthBar ScreenHealthBar;
    private HealthBar m_CurrentHealthBar;
    public HealthBar CurrentHealthBar { set { m_CurrentHealthBar = value; } }

    // Flags
    [SyncVar]
    private bool m_IsAlive;
    public bool IsAlive { get { return m_IsAlive; } }

    //private CoreAttributes m_CoreAttributes;

    // Server combat log stores incoming CombatEvents; ClientCombatLog keeps track of which events have been processed on the Client
    private CombatLogSync m_ServerCombatLog;
    private List<CombatEvent> m_ClientCombatLog;
    public float CombatEventLifetime = 120f;

	// Use this for initialization
	void Start ()
    {
        m_TargetCollider = GetComponentInChildren<AttackTarget>().GetComponent<Collider>();
        m_AnimationHandler = GetComponentInChildren<CharacterAnimationHandler>();

        playerNetworkSetup();
	}

    void Update()
    {
        // (For now) Don't go past this point if we are dead
        if (!m_IsAlive)
            return;

        // GameData updates
        if (isServer)
        {
            rechargeGuard();
            checkForDeath();
        }

        // Perform maintenance on combat logs
        cleanCombatLog();
        if (!isServer)
            syncClientCombatLog();

        // Local display updates
        updateHealthBars();
    }

    private void playerNetworkSetup()
    {
        // Only enable the first-person controller on the local player
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = isLocalPlayer;

        // Only enable the input-based animation controller on the local player
        GetComponentInChildren<CharacterAnimationHandler>().IsPlayerCharacter = isLocalPlayer;

        // Disable all anchored camera objects on remote players
        GetComponentInChildren<CameraPerspectiveController>().enabled = isLocalPlayer;
        Camera[] cameras = GetComponentsInChildren<Camera>();
        foreach (Camera camera in cameras)
        {
            camera.enabled = isLocalPlayer;

            if (camera.gameObject.GetComponent<AudioListener>())
                camera.enabled = false;
        }

        // Disable all child audio listeners on remote players
        AudioListener[] listeners = GetComponentsInChildren<AudioListener>();
        foreach (AudioListener listener in listeners)
        {
            listener.enabled = isLocalPlayer;
        }

        // Disable AttackTarget on local player ("Favor The Attacker")
        m_AttackTarget = GetComponentInChildren<AttackTarget>();
        m_AttackTarget.GetComponent<Collider>().enabled = !isLocalPlayer;

        // Only enable Character Overlay (child containing the Billboard component) on remote player
        GetComponentInChildren<Billboard>().gameObject.SetActive(!isLocalPlayer);

        // Only enable AttackSource on the local player ("Favor The Attacker")
        AttackSource[] attackSources = GetComponentsInChildren<AttackSource>();
        if (!isLocalPlayer)
        {
            foreach (AttackSource source in attackSources)
                Component.Destroy(source.GetComponent<Collider>());
        }

        // Send character name (stored in local PlayerPrefs) to the Server-Side entity so other players can see it
        if (isLocalPlayer)
            CmdSetServerCharacterName(PlayerPrefs.GetString("CharacterName"));

        // Initialize both Server and Client CombatLogs
        if (isServer)
            m_ServerCombatLog = new CombatLogSync();
        else
            m_ClientCombatLog = new List<CombatEvent>();

        // Initialize attributes on the server; they should sync to the client
        initializeAttributes(isServer);
    }

    [Command]
    private void CmdSetServerCharacterName(string _characterName)
    {
        m_CharacterName = _characterName;
    }

    private void initializeAttributes(bool _server)
    {
        // Everything the player client needs should be synced from the server -- no initialization needed (in theory?)
        if (!_server)
            return;

        // [Temp] Default attribute initialization:
        m_MaxHealth = 50f;
        m_Health = m_MaxHealth;
        m_MaxGuard = 200f;
        m_Guard = m_MaxGuard;

        m_TimeSinceLastHit = 0f;
        m_IsAlive = true;
    }

    #region Per-Frame Checks

    private void rechargeGuard()
    {
        m_TimeSinceLastHit += Time.deltaTime;

        if (m_TimeSinceLastHit > GuardRechargeDelay)
        {
            m_Guard += GuardRechargeSpeed * Time.deltaTime;
            m_Guard = Mathf.Min(m_Guard, m_MaxGuard);
        }
    }

    private void updateHealthBars()
    {
        Debug.Log("Update health bars: " + m_Health + ", " + m_Guard);

        // Update gauge display
        if (!m_CurrentHealthBar)
        {
            if (WorldHealthBar.gameObject.activeInHierarchy)
                m_CurrentHealthBar = WorldHealthBar;
            else
                m_CurrentHealthBar = ScreenHealthBar;
        }

        Debug.Log(m_CurrentHealthBar.name);

        m_CurrentHealthBar.DisplayVitals(
            m_Health / m_MaxHealth,
            Guard / MaxGuard);
    }

    private void checkForDeath()
    {
        // Check to see if we should die
        if (m_Health <= 0f)
        {
            Die();
            return;
        }
    }
    #endregion

    #region CombatLog
    /// <summary>
    /// Remove old CombatEvents from the CombatLog so that it doesn't grow indefinitely.
    /// </summary>
    private void cleanCombatLog()
    {
        if (m_ServerCombatLog != null)
        {
            foreach (CombatEvent cEvent in m_ServerCombatLog)
            {
                if ((Time.time - cEvent.timeStamp) > CombatEventLifetime)
                {
                    m_ServerCombatLog.Remove(cEvent);
                }
            }
        }

        if (m_ClientCombatLog != null)
        {
            foreach (CombatEvent cEvent in m_ClientCombatLog)
            {
                if ((Time.time - cEvent.timeStamp) > CombatEventLifetime)
                {
                    m_ClientCombatLog.Remove(cEvent);
                }
            }
        }
    }

    /// <summary>
    /// Transfer new CombatEvents from the ServerCombatLog to the ClientCombatLog and reflect them on the AttackTarget.
    /// </summary>
    private void syncClientCombatLog()
    {
        // Starts after the end of the ClientCombatLog and iterates through the end of the ServerCombatLog.
        // If the two are the same length, this loop should exit immediately.
        int count = m_ClientCombatLog.Count;
        for (int i = count; i < m_ServerCombatLog.Count; i++)
        {
            // Add the CombatEvent to the ClientCombatLog
            m_ClientCombatLog.Add(m_ServerCombatLog[i]);

            // Process the CombatEvent on the AttackTarget
            m_AttackTarget.ProcessLocalCombatEvent(m_ServerCombatLog[i]);
        }
    }
    #endregion

    #region CombatEventProcesses

    /// <summary>
    /// Receive combat event from the Client-Side Remote Target
    /// </summary>
    /// <param name="_cEvent">Packet containing info relevant to the combat interaction.</param>
    public void ClientAttackerReceiveCombatEvent(CombatEvent _cEvent)
    {
        Debug.Log("ClientAttackerReceiveCombatEvent");

        CmdServerAttackerReceiveCombatEvent(_cEvent);
    }

    /// <summary>
    /// Send combat event to this Attacker's Server-Side entity (and log it)
    /// </summary>
    /// <param name="_cEvent">Packet containing info relevant to the combat interaction.</param>
    [Command]
    private void CmdServerAttackerReceiveCombatEvent(CombatEvent _cEvent)
    {
        Debug.Log("CmdServerAttackerReceiveCombatEvent");

        // Log the combat event for the attacker
        m_ServerCombatLog.Add(_cEvent);

        // Find the Server-Side Defender and send the combat event to it.
        NetworkCharacterData defender = NetworkServer.FindLocalObject(_cEvent.defenderNetworkID).GetComponent<NetworkCharacterData>();
        if (!defender)
        {
            Debug.LogError("[Server Error] Defender entity not found on the server.");
            return;
        }
        defender.ServerDefenderReceiveCombatEvent(_cEvent);
    }

    /// <summary>
    /// Send combat event from the Attacker's Server-Side entity to the Defender's Server-Side entity,
    /// then log the CombatEvent and process its effects on the Defender's attributes.
    /// </summary>
    /// <param name="_cEvent">Packet containing info relevant to the combat interaction.</param>
    public void ServerDefenderReceiveCombatEvent(CombatEvent _cEvent)
    {
        Debug.Log("ServerDefenderReceiveCombatEvent");

        // Log the combat event for the defender
        m_ServerCombatLog.Add(_cEvent);

        // Any attributes modified here should be synchronized automatically down to the Client-Side entity
        switch (_cEvent.type)
        {
            case CombatEvent.CombatEventType.WOUND:
                m_Health -= _cEvent.magnitude;
                break;
            case CombatEvent.CombatEventType.PARRY:
                m_Guard -= _cEvent.magnitude;
                break;
            default:
                break;
        }

        m_TimeSinceLastHit = 0f;
    }

    #endregion

    #region Status Modifiers

    /// <summary>
    /// Modify any combat mechanic-related components to reflect a dead character.
    /// </summary>
    public void Die()
    {
        if (m_CurrentHealthBar)
            m_CurrentHealthBar.gameObject.SetActive(false);

        m_TargetCollider.enabled = false;
        m_IsAlive = false;
        m_AnimationHandler.OnDeath();
    }

    /// <summary>
    /// Undo the changes made in die() and set character's Health to 1 by default
    /// </summary>
    public void Revive()
    {
        m_TargetCollider.enabled = true;
        if (m_CurrentHealthBar)
            m_CurrentHealthBar.gameObject.SetActive(true);
        m_IsAlive = true;
        m_AnimationHandler.OnRevive();

        // Set health to 1 to avoid the character immediately dying again
        m_Health = 1;
    }

    #endregion
}

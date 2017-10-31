using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.2f)]
public class PlayerNetworkAnimator : NetworkBehaviour
{
    // Animator component parameters
    private Animator m_ChildAnimator;
    [SyncVar]
    public int m_RunDirection;
    [SyncVar]
    public bool m_Jumping;
    [SyncVar]
    public bool m_Attacking;
    [SyncVar]
    public bool m_Running;
    [SyncVar]
    public bool m_Blocking;
    [SyncVar]
    public bool m_Dead;

    // Character transform parameters
    public Transform m_CharacterHeading;
    [SyncVar]
    public Quaternion m_HeadRotation;

    void Start()
    {
        m_ChildAnimator = GetComponentInChildren<Animator>();
        m_ChildAnimator.GetComponent<CharacterAnimationHandler>().IsPlayerCharacter = isLocalPlayer;

        m_CharacterHeading = GetComponentInChildren<AudioListener>().transform;
    }

    void Update()
    {
        if (!m_ChildAnimator)
            return;

        if (isLocalPlayer)
        {
            // Set this component's parameters based on the child Animator component
            if (isServer)
            {
                m_RunDirection = m_ChildAnimator.GetInteger("RunDirection");
                m_Jumping = m_ChildAnimator.GetBool("Jumping");
                m_Attacking = m_ChildAnimator.GetBool("Attacking");
                m_Running = m_ChildAnimator.GetBool("Running");
                m_Blocking = m_ChildAnimator.GetBool("Blocking");
                m_Dead = m_ChildAnimator.GetBool("Dead");
                m_HeadRotation = m_CharacterHeading.transform.localRotation;
            }
            else
            {
                // Normally, for clients, a Command must be sent to the server
                CmdSetAnimState(m_ChildAnimator.GetInteger("RunDirection"),
                    m_ChildAnimator.GetBool("Jumping"),
                    m_ChildAnimator.GetBool("Attacking"),
                    m_ChildAnimator.GetBool("Running"),
                    m_ChildAnimator.GetBool("Blocking"),
                    m_ChildAnimator.GetBool("Dead"),
                    m_CharacterHeading.transform.localRotation);
            }
        }
        else
        {
            // Set the child Animator component's parameters based on this component
            m_ChildAnimator.SetInteger("RunDirection", m_RunDirection);
            m_ChildAnimator.SetBool("Jumping", m_Jumping);
            m_ChildAnimator.SetBool("Attacking", m_Attacking);
            m_ChildAnimator.SetBool("Running", m_Running);
            m_ChildAnimator.SetBool("Blocking", m_Blocking);
            m_ChildAnimator.SetBool("Dead", m_Dead);

            // Smoothly rotate the head toward the network synchronized orientation
            m_CharacterHeading.localRotation = Quaternion.Lerp(m_CharacterHeading.localRotation, m_HeadRotation, Time.deltaTime);
        }
    }

    

    [Command]
    private void CmdSetAnimState(int _runDirection, bool _jumping, bool _attacking, bool _running, bool _blocking, bool _dead, Quaternion _headRotation)
    {
        m_RunDirection = _runDirection;
        m_Jumping = _jumping;
        m_Attacking = _attacking;
        m_Running = _running;
        m_Blocking = _blocking;
        m_Dead = _dead;
        m_HeadRotation = _headRotation;
    }
}

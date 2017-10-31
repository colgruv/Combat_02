using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionDirector : MonoBehaviour
{
    // Debug interface for allocating character actions
    public CharacterAction[] DebugActions;

    // Reference to the character Animation Handler
    private CharacterAnimationHandler m_AnimationHandler;
    public CharacterAnimationHandler AnimationHandler { get { return m_AnimationHandler; } }

    // Reference to the character's equipped weapon
    private AttackSource m_EquippedWeapon;
    public AttackSource EquippedWeapon { get { return m_EquippedWeapon; } }

    // Selection of actions the character has equipped
    private List<CharacterAction> m_EquippedActions;

    // Actions the character has learned (includes all equipped actions)
    private List<CharacterAction> m_LearnedActions;

    // Reference to ActionBarHandler for equppping actions
    private ActionBarHandler m_ActionBar;

	// Use this for initialization
	void Start ()
    {
        m_EquippedActions = new List<CharacterAction>();
        m_LearnedActions = new List<CharacterAction>();
        m_ActionBar = FindObjectOfType<ActionBarHandler>();
        m_AnimationHandler = transform.parent.GetComponentInChildren<CharacterAnimationHandler>();
        m_EquippedWeapon = m_AnimationHandler.GetComponentInChildren<AttackSource>();

	    foreach(CharacterAction act in DebugActions)
        {
            m_LearnedActions.Add(act);
            m_EquippedActions.Add(act);
        }	
	}

    private void autoEquipAction(CharacterAction _action)
    {
        
    }
}

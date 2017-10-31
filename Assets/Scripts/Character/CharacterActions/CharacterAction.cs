using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : MonoBehaviour
{
    // Identification
    public string Name = "Default";
    public Sprite Icon;

    // Reource costs
    public int ManaCost = 0;
    public int StaminaCost = 0;

    // Object references
    protected static CharacterActionDirector s_Director;

    // Cooldown statistics
    public static float s_GlobalCooldown = 2.8f;
    public bool UsesGlobalCooldown = true;
    public float UniqueCooldown = 0f;
    protected float m_RemainingCooldown;
    public float RemainingCooldown { get { return m_RemainingCooldown; } }
    public float TotalCooldown { get { return (UsesGlobalCooldown ? s_GlobalCooldown : 0f) + UniqueCooldown; } }

    // Duration statistics
    public float Duration;      // 0f if effect is instantaneous
    protected float m_RemainingDuration;
    public float RemainingDuration { get { return m_RemainingDuration; } }
    public float PrepTime;      // 0f if action is immediate
    protected float m_RemainingPrepTime;
    public float RemainingPrepTime { get { return m_RemainingPrepTime; } }

    protected bool m_IsActive;
    public bool IsActive { get { return m_IsActive; } }

	// Use this for initialization
	void Start ()
    {
        m_IsActive = false;

        if (!s_Director)
            s_Director = transform.parent.GetComponent<CharacterActionDirector>();
	}

    protected IEnumerator ProcessCooldown()
    {
        m_RemainingCooldown = (UsesGlobalCooldown ? s_GlobalCooldown : 0f) + UniqueCooldown;

        while (m_RemainingCooldown > 0f)
        {
            m_RemainingCooldown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public virtual IEnumerator ProcessAction()
    {
        // Begin preparation
        m_RemainingPrepTime = PrepTime;
        s_Director.AnimationHandler.WalkOverride = true;

        // Suspend for prep time
        while (m_RemainingPrepTime > 0f)
        {
            m_RemainingPrepTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Begin duration
        m_RemainingDuration = Duration;

        // Perform startup
        //s_Director.EquippedWeapon.GetComponent<Collider>().enabled = true;
        int defaultAccuracy = s_Director.EquippedWeapon.Accuracy;
        s_Director.EquippedWeapon.Accuracy = defaultAccuracy * 2;
        s_Director.AnimationHandler.GetComponent<Animator>().Play("Soldier_01", 2);

        // Suspend for duration
        while (m_RemainingDuration > 0f)
        {
            m_RemainingDuration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //s_Director.EquippedWeapon.GetComponent<Collider>().enabled = false;
        s_Director.AnimationHandler.WalkOverride = false;
        DeactivateAction();
    }

    /// <summary>
    /// Activates the current CharacterAction
    /// </summary>
    /// <returns>Whether the CharacterAction is available to be activated right now.</returns>
    public virtual bool ActivateAction()
    {
        if (m_RemainingPrepTime > 0f)
            return false;

        if (m_RemainingCooldown > 0f)
            return false;

        m_IsActive = true;
        StartCoroutine(ProcessAction());
        return true;
    }

    /// <summary>
    /// Deactivates the current CharacterAction
    /// </summary>
    /// <returns>FALSE if the CharacterAction was interrupted. TRUE if it timed out or otherwise deactivated normally.</returns>
    public virtual bool DeactivateAction()
    {
        m_IsActive = false;
        StartCoroutine(ProcessCooldown());
        return true;
    }
}

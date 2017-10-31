using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterAnimationHandler : MonoBehaviour
{
    public enum Cardinal
    {
        NONE,
        NORTH,
        NORTHEAST,
        EAST,
        SOUTHEAST,
        SOUTH,
        SOUTHWEST,
        WEST,
        NORTHWEST
    };

    public bool IsPlayerCharacter;

    private Animator m_Animator;
    private FirstPersonController m_FPController;
    private LookTargetBehaviour m_LookTarget;
    private Cardinal m_CurrentDirection;

    private float m_DefaultWalkSpeed;
    private float m_DefaultRunSpeed;
    private float m_DefaultJumpSpeed;

    private float m_LastYDirection;

    // Other components can force the CharacterAnimationHandler to tell the character to walk
    private bool m_WalkOverride = false;
    public bool WalkOverride { set { m_WalkOverride = value; } }

	// Use this for initialization
	void Start ()
    {
        m_Animator = GetComponent<Animator>();

        if (IsPlayerCharacter)
        {
            m_FPController = transform.parent.GetComponent<FirstPersonController>();

            m_DefaultWalkSpeed = m_FPController.WalkSpeed;
            m_DefaultRunSpeed = m_FPController.RunSpeed;
            m_DefaultJumpSpeed = m_FPController.JumpSpeed;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        calculateBaseRotaton();

        if (IsPlayerCharacter)
        {
            calculateCardinalDirectionFromButtons();

            if (!calculateJumping())
            {
                calculateAttacking();
            }
        }
	}

    private void calculateCardinalDirectionFromAxes()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.NONE;
        else if (horizontal == 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTH;
        else if (horizontal > 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTHEAST;
        else if (horizontal > 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.EAST;
        else if (horizontal > 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTHEAST;
        else if (horizontal == 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTH;
        else if (horizontal < 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTHWEST;
        else if (horizontal < 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.WEST;
        else if (horizontal < 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTHWEST;

        if (m_Animator == null)
            m_Animator = GetComponent<Animator>();
        if (m_Animator != null)
            m_Animator.SetInteger("RunDirection", (int)m_CurrentDirection);
    }

    private void calculateBaseRotaton()
    {
        // If standing idle, lock the root transform orientation
        if (m_Animator.GetInteger("RunDirection") == 0)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = m_LastYDirection;
            transform.eulerAngles = eulerAngles;
        }
        else
            transform.localEulerAngles = Vector3.zero;
        m_LastYDirection = transform.eulerAngles.y;
    }

    private void calculateCardinalDirectionFromButtons()
    {
        float horizontal = 0;
        horizontal -= (Input.GetKey(KeyCode.A)) ? 1 : 0;
        horizontal += (Input.GetKey(KeyCode.D)) ? 1 : 0;

        float vertical = Input.GetAxis("Vertical");
        vertical -= (Input.GetKey(KeyCode.S)) ? 1 : 0;
        vertical += (Input.GetKey(KeyCode.W)) ? 1 : 0;

        if (horizontal == 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.NONE;
        else if (horizontal == 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTH;
        else if (horizontal > 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTHEAST;
        else if (horizontal > 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.EAST;
        else if (horizontal > 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTHEAST;
        else if (horizontal == 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTH;
        else if (horizontal < 0f && vertical < 0f)
            m_CurrentDirection = Cardinal.SOUTHWEST;
        else if (horizontal < 0f && vertical == 0f)
            m_CurrentDirection = Cardinal.WEST;
        else if (horizontal < 0f && vertical > 0f)
            m_CurrentDirection = Cardinal.NORTHWEST;

        if (m_Animator == null)
            m_Animator = GetComponent<Animator>();
        if (m_Animator != null)
            m_Animator.SetInteger("RunDirection", (int)m_CurrentDirection);
    }

    private bool calculateJumping()
    {
        bool jumping = false;
        
        if (m_FPController == null)
            m_FPController = transform.parent.GetComponent<FirstPersonController>();
        if (m_FPController != null)
        {
            // Poll jumping status from FP Controller
            jumping = m_FPController.IsJumping;

            // Update Animator booleans
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();
            if (m_Animator != null)
                m_Animator.SetBool("Jumping", jumping);
        }

        return jumping;
    }

    private bool calculateAttacking()
    {
        // Poll mouse input
        bool blocking = Input.GetMouseButton(1);
        bool attacking = Input.GetMouseButton(0);
        
        // Update Animator booleans
        if (m_Animator == null)
            m_Animator = GetComponent<Animator>();
        if (m_Animator != null)
        {
            m_Animator.SetBool("Blocking", blocking);
            m_Animator.SetBool("Attacking", attacking);
            m_Animator.SetBool("Running", !(attacking || blocking || m_WalkOverride));
        }

        // Slow movement if blocking or attacking
        m_FPController.WalkSpeed = (attacking || blocking || m_WalkOverride) ? m_DefaultWalkSpeed / 4f : m_DefaultWalkSpeed;
        m_FPController.RunSpeed = (attacking || blocking || m_WalkOverride) ? m_DefaultRunSpeed / 4f : m_DefaultRunSpeed;
        m_FPController.JumpSpeed = (attacking || blocking || m_WalkOverride) ? m_DefaultJumpSpeed / 4f : m_DefaultJumpSpeed;

        // Return true if blocking or attacking
        return (attacking || blocking);
    }

    /// <summary>
    /// Modify any animation-related components to reflect a dead character.
    /// </summary>
    public void OnDeath()
    {
        // Disable look target behaviour so the character's death animation looks natural
        if (m_LookTarget == null)
            m_LookTarget = GetComponent<LookTargetBehaviour>();
        m_LookTarget.enabled = false;

        // Disable collider so the character can be walked over
        transform.parent.GetComponent<Collider>().enabled = false;

        // Get animator and play death animation state
        if (m_Animator == null)
            m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Dead", true);
    }

    /// <summary>
    /// Undo the changes made in OnDeath
    /// </summary>
    public void OnRevive()
    {
        if (m_LookTarget == null)
            m_LookTarget = GetComponent<LookTargetBehaviour>();
        m_LookTarget.enabled = true;

        // Setting the Dead boolean false should play an animation in which the player stands back up
        if (m_Animator == null)
            m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Dead", false);
    }
}

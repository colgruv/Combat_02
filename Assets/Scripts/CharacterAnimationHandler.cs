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

    private Animator mAnimator;
    private FirstPersonController mFPController;
    private Cardinal mCurrentDirection;

    private float mDefaultWalkSpeed;
    private float mDefaultRunSpeed;
    private float mDefaultJumpSpeed;

    private float mLastYDirection;

	// Use this for initialization
	void Start () {
        mFPController = transform.parent.GetComponent<FirstPersonController>();
        mDefaultWalkSpeed = mFPController.WalkSpeed;
        mDefaultRunSpeed = mFPController.RunSpeed;
        mDefaultJumpSpeed = mFPController.JumpSpeed;
	}
	
	// Update is called once per frame
	void Update ()
    {
        calculateCardinalDirectionFromButtons();

        if (!calculateJumping())
        {
            calculateAttacking();
        }
	}

    private void calculateCardinalDirectionFromAxes()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0f && vertical == 0f)
            mCurrentDirection = Cardinal.NONE;
        else if (horizontal == 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTH;
        else if (horizontal > 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTHEAST;
        else if (horizontal > 0f && vertical == 0f)
            mCurrentDirection = Cardinal.EAST;
        else if (horizontal > 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTHEAST;
        else if (horizontal == 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTH;
        else if (horizontal < 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTHWEST;
        else if (horizontal < 0f && vertical == 0f)
            mCurrentDirection = Cardinal.WEST;
        else if (horizontal < 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTHWEST;

        if (mAnimator == null)
            mAnimator = GetComponent<Animator>();
        if (mAnimator != null)
            mAnimator.SetInteger("RunDirection", (int)mCurrentDirection);
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
            mCurrentDirection = Cardinal.NONE;
        else if (horizontal == 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTH;
        else if (horizontal > 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTHEAST;
        else if (horizontal > 0f && vertical == 0f)
            mCurrentDirection = Cardinal.EAST;
        else if (horizontal > 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTHEAST;
        else if (horizontal == 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTH;
        else if (horizontal < 0f && vertical < 0f)
            mCurrentDirection = Cardinal.SOUTHWEST;
        else if (horizontal < 0f && vertical == 0f)
            mCurrentDirection = Cardinal.WEST;
        else if (horizontal < 0f && vertical > 0f)
            mCurrentDirection = Cardinal.NORTHWEST;

        if (mAnimator == null)
            mAnimator = GetComponent<Animator>();
        if (mAnimator != null)
            mAnimator.SetInteger("RunDirection", (int)mCurrentDirection);

        // If standing idle, lock the root transform orientation
        if (mCurrentDirection == Cardinal.NONE)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = mLastYDirection;
            transform.eulerAngles = eulerAngles;
        }
        else
            transform.localEulerAngles = Vector3.zero;
        mLastYDirection = transform.eulerAngles.y;
    }

    private bool calculateJumping()
    {
        bool jumping = mFPController.IsJumping;

        if (mFPController == null)
            mFPController = transform.parent.GetComponent<FirstPersonController>();
        if (mFPController != null)
        {
            if (mAnimator == null)
                mAnimator = GetComponent<Animator>();
            if (mAnimator != null)
                mAnimator.SetBool("Jumping", jumping);
        }

        return jumping;
    }

    private bool calculateAttacking()
    {
        bool attacking = Input.GetMouseButton(0);

        if (mAnimator == null)
            mAnimator = GetComponent<Animator>();
        if (mAnimator != null)
        {
            mAnimator.SetBool("Attacking", attacking);
            mAnimator.SetBool("Running", !attacking);
        }

        mFPController.WalkSpeed = attacking ? mDefaultWalkSpeed/4f : mDefaultWalkSpeed;
        mFPController.RunSpeed = attacking ? mDefaultRunSpeed/4f : mDefaultRunSpeed;
        mFPController.JumpSpeed = attacking ? mDefaultJumpSpeed/4f : mDefaultJumpSpeed;

        return attacking;
    }
}

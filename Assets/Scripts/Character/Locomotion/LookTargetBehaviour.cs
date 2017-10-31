using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetBehaviour : MonoBehaviour
{
    public Transform LookAtTarget;
    private Animator mAnimator;

    public float mLookWeight = 1f;
    public float mBodyWeight = 0.25f;
    public float mHeadWeight = 0.9f;
    public float mEyesWeight = 1f;
    public float mClampWeight = 1f;

	// Use this for initialization
	void Start ()
    {
        mAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnAnimatorIK()
    {
        mAnimator.SetLookAtPosition(LookAtTarget.position);
        mAnimator.SetLookAtWeight(mLookWeight, mBodyWeight, mHeadWeight, mEyesWeight, mClampWeight);
    }
}

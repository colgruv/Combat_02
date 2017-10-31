using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailBehaviour : MonoBehaviour
{
    private TrailRenderer mTrailRenderer;

    public float MinSpeed = 100.0f;
    public float MaxSpeed = 155.0f;

    private Vector3 mLastPosition;

	// Use this for initialization
	void Start () {
        mTrailRenderer = GetComponent<TrailRenderer>();
        mLastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 currentPosition = transform.position;

        float speed = (currentPosition - mLastPosition).magnitude / Time.deltaTime;

        Color trailColor = mTrailRenderer.startColor;
        float alpha = ((speed - MinSpeed) / (MaxSpeed - MinSpeed));
        Debug.Log(alpha);
        alpha = Mathf.Min(alpha, 1f);
        alpha = Mathf.Max(alpha, 0f);
        trailColor.a = alpha;
        mTrailRenderer.startColor = trailColor;
        mTrailRenderer.endColor = trailColor;

        mLastPosition = currentPosition;
	}
}

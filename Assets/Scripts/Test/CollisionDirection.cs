using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDirection : MonoBehaviour
{
    public Transform DirectionIdentifier;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnCollisionEnter(Collision _collision)
    {
        Debug.Log("Collision Enter");
        DirectionIdentifier.position = _collision.contacts[0].point;
        DirectionIdentifier.forward = _collision.impulse * -1f;
        DirectionIdentifier.GetComponent<ParticleSystem>().Play();
    }
}

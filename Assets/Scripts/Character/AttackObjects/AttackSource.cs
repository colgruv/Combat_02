using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSource : AttackObject
{
    public int Accuracy = 20;

    public int MinimumDamage = 10;
    public int MaximumDamage = 20;

	// Use this for initialization
	void Start ()
    {
        findOwner(transform);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

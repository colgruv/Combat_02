using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    protected virtual void Start()
    {
        // Effect GameObjects are disabled by default until they are played.
        gameObject.SetActive(false);
    }	

    public virtual void Play()
    {
        gameObject.SetActive(true);
    }

    protected virtual void onComplete()
    {
        gameObject.SetActive(false);
    } 
}

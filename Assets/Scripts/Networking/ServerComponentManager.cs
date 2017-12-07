using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerComponentManager : NetworkBehaviour
{
	// Use this for initialization
	void Start ()
    {
		if (!isServer)
        {
            GetComponent<AudioListener>().enabled = false; 
        }
	}
}

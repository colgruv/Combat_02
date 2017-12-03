using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkConnectionSetup : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        NetworkManager networkManager = GetComponent<NetworkManager>();

	    if (networkManager)
        {
            if (PlayerPrefs.GetInt("IsServer") > 0)
            {
                networkManager.StartServer();
            }
            else
            {
                networkManager.StartClient();
            }
        }	
	}
}

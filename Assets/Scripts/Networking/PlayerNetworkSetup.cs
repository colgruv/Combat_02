using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
    {
        // First-person controller
        //GetComponent<CharacterController>().enabled = isLocalPlayer;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = isLocalPlayer;

        // Animation controller
        GetComponentInChildren<CharacterAnimationHandler>().IsPlayerCharacter = isLocalPlayer;

        // All camera objects
        Camera[] cameras = GetComponentsInChildren<Camera>();
        foreach (Camera camera in cameras)
        {
            camera.enabled = isLocalPlayer;

            if (camera.gameObject.GetComponent<AudioListener>())
                camera.enabled = false;
        }

        // All audio listeners
        AudioListener[] listeners = GetComponentsInChildren<AudioListener>();
        foreach(AudioListener listener in listeners)
        {
            listener.enabled = isLocalPlayer;
        }

        // Attack Target: Disable collider on AttackTarget and WorldHealthBar if local player
        AttackTarget attackTarget = GetComponentInChildren<AttackTarget>();
        attackTarget.GetComponent<Collider>().enabled = !isLocalPlayer;
        attackTarget.WorldHealthBar.transform.parent.gameObject.SetActive(!isLocalPlayer); // Also manages other overlay objects
        //if (!AttackTarget.ScreenHealthBar)
        //    AttackTarget.ScreenHealthBar = GameObject.FindGameObjectWithTag("Screen").GetComponentInChildren<HealthBar>();
        //AttackTarget.ScreenHealthBar.gameObject.SetActive(isLocalPlayer);

        // Attack Sources: Only enable AttackSources on local player
        AttackSource[] attackSources = GetComponentsInChildren<AttackSource>();
        if (!isLocalPlayer)
        {
            foreach (AttackSource source in attackSources)
                Component.Destroy(source.GetComponent<Collider>());
        }
	}
}
